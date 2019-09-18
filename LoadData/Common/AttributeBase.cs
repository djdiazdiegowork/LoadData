using System;
using System.Collections.Generic;
using Common.JSON;
using Newtonsoft.Json;

namespace Common
{
    [JsonConverter(typeof(AttributeBaseJsonConverter))]
    public class AttributeBase
    {
        public string Name { get; private set; }
        public AttributeType Type { get; private set; }
        public AttributeDataType DataType { get; private set; }
        public ColumnType ColumnType { get; set; }
        

        public AttributeBase(string name, AttributeType attributeType, AttributeDataType dataType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Un atributo debe tener nombre");
            }
            
            Name = name;
            Type = attributeType;
            DataType = dataType;
        }

        protected string GetValueFromFormat(string value, AttributeDataType attributeDataType)
        {
            var result = value;
            switch (attributeDataType)
            {
                case AttributeDataType.TypeDate:
                    result = $"''{value}''";
                    break;
                //// Para este caso se va a realizar una busqueda exacta
                //// por lo que hay que ponerle comillas simples. 
                case AttributeDataType.TypeString:
                    result = $"''{value}''"; 
                    break;
                default: break;
            }

            return result;
        }
        /// <summary>
        /// Metodo que se va a utilizar para generar el string
        /// query de los atributos. Este metodo debe ser reescrito
        /// en las clases derivadas.
        /// </summary>
        /// <returns></returns>
        public virtual string ToStringQuery()
        {
            return string.Empty;
        }
    }
    
    public class AttributeSelect : AttributeBase
    {
        public AttributeSelect(string name, AttributeType attributeType, AttributeDataType dataType)
            : base(name, attributeType, dataType)
        {
        }

        public override string ToStringQuery()
        {
            return Name;
        }
    }

    public class AttributeFilter : AttributeBase
    {
        public List<string> Values { get; private set; }
        public OperatorForValues OperatorForValues { get; private set; }
        public OperatorForLinkAtribute OperatorForLinkAtribute { get; set; }
        public ParenthesisThisAtribute ParenthesisThisAtribute { get; private set; }

        public AttributeFilter(string name, AttributeType attributeType, AttributeDataType dataType,
            List<string> values, OperatorForValues operatorForValues, OperatorForLinkAtribute operatorForLinkAtribute,
            ParenthesisThisAtribute parenthesisThisAtribute)
            : base(name, attributeType, dataType)
        {
            if (values == null || values.Count < 0)
            {
                throw new Exception("Un atributo de tipo filtro debe tener valores");
            }

            // si este atributo tiene valores de fecha o numero, deben cumplirse ciertas condiciones
            if (operatorForValues == OperatorForValues.Between && values.Count != 2)
            {
                throw new Exception("Un atributo de tipo filtro con el operador" + operatorForValues +
                                    " debe tener 2 valores");
            }

            if (operatorForValues == OperatorForValues.Bigger || operatorForValues == OperatorForValues.BiggerEqual ||
                operatorForValues == OperatorForValues.Minor || operatorForValues == OperatorForValues.MinorEqual)
            {
                if (values.Count != 1)
                {
                    throw new Exception("Un atributo de tipo filtro con el operador" + operatorForValues +
                                        " debe tener 1 valor");
                }
            }

            Values = values;
            OperatorForValues = operatorForValues;
            OperatorForLinkAtribute = operatorForLinkAtribute;
            ParenthesisThisAtribute = parenthesisThisAtribute;
        }

        /// <summary>
        /// Metodo que se utiliza para obtener la representacion
        /// del string query del tipo de atributo.
        /// </summary>
        /// <returns></returns>
        public override string ToStringQuery()
        {
            var result = string.Empty;
            var headerForSDATA = DataType == AttributeDataType.TypeDate || DataType == AttributeDataType.TypeDouble
                ? "SDATA"
                : string.Empty;
            var eventHeader = "EVENTO_";
            var entityHeader = "";
            ////Aqui se pone el nombre del atributo con el formato de EVENTO_ o
            //// para la instancia. 
            var nameHeader = ColumnType == ColumnType.Dummy
                ? (Type == AttributeType.Event ? $"{eventHeader}" : entityHeader)
                : string.Empty;
            var nameMain = $"{nameHeader}{Name}";

            //// Aqui se definen los tokens que se van a
            //// generar.
            var leftOperand = nameMain;
            var rightOperand = GetValueFromFormat(Values[0], DataType);
            var binaryOperator = OperatorForValues.OperatorForValuesCustomToString();

            //// Forma de un operador binario con sus operandos
            var atom = string.Empty;
            //// En caso de que se trate del operador equal y el tipo de dato sea string
            //// se formatea esta forma. 
            if (DataType == AttributeDataType.TypeString && OperatorForValues == OperatorForValues.Equal)
            {
                //// Salvando temporalmente el valor del operador izquierdo.
                var temp = leftOperand;

                leftOperand = rightOperand;
                rightOperand = temp;
                binaryOperator = "within";
            }

            //// Verificando los valores multiples
            if (Values.Count > 1)
            {
                //// Caso del BETWEEN
                if (OperatorForValues == OperatorForValues.Between)
                {
                    result =
                        $"{headerForSDATA} ({nameMain} {OperatorForValues.OperatorForValuesCustomToString()} {GetValueFromFormat(Values[0], DataType)} AND {GetValueFromFormat(Values[1], DataType)})";
                }
                //// Es una lista de valores
                else
                {
                    var operatorForLinkAttributeInList = OperatorForValues == OperatorForValues.Equal
                        ? OperatorForLinkAtribute.OrOperator
                        : OperatorForLinkAtribute.AndOperator;
                    atom = GenerateAtom(leftOperand, binaryOperator, rightOperand);
                    result =
                        $"{headerForSDATA} ({atom}) ";
                    for (int i = 1; i < Values.Count; i++)
                    {
                        //// Verificando el caso de que se trate de un operador equal y el tipo de dato sea string
                        if (DataType == AttributeDataType.TypeString && OperatorForValues == OperatorForValues.Equal)
                            leftOperand = GetValueFromFormat(Values[i], DataType);
                        atom = GenerateAtom(leftOperand, binaryOperator, rightOperand);
                        result +=
                            $"{operatorForLinkAttributeInList.OperatorForLinkAtributeCustomToString()} {headerForSDATA} ({atom}) ";
                    }

                    result = $"({result})";
                }
            }

            //// Solo tiene un valor
            else
            {
                atom = GenerateAtom(leftOperand, binaryOperator, rightOperand);
                result =
                    $"{headerForSDATA} ({atom})";
            }

            var tempResult = ParenthesisThisAtribute == ParenthesisThisAtribute.OpenParenthesis
                ? $"{ParenthesisThisAtribute.ParenthesisThisAtributeCustomToString()} {result}"
                : $"{result} {ParenthesisThisAtribute.ParenthesisThisAtributeCustomToString()}";
            return $"{tempResult} {OperatorForLinkAtribute.OperatorForLinkAtributeCustomToString()}";
        }

        /// <summary>
        /// Metodo que se va a utilizar para generar un atomo formateado.
        /// </summary>
        /// <param name="leftOperand"></param>
        /// <param name="binaryOperator"></param>
        /// <param name="rightOperand"></param>
        /// <returns></returns>
        private string GenerateAtom(string leftOperand, string binaryOperator, string rightOperand)
        {
            return $"{leftOperand} {binaryOperator} {rightOperand}";
        }
    }

    public enum AttributeType
    {
        Instance,
        Event
    }
    
    public enum AttributeDataType
    {
        TypeDate,
        TypeDouble,
        TypeString,
        TypeTextGeneral
    }

    public enum OperatorForValues
    {
        Equal,
        Different,
        Bigger,
        Minor,
        BiggerEqual,
        MinorEqual,
        Between
    }

    public enum OperatorForLinkAtribute
    {
        AndOperator,
        OrOperator,
        None,
    }
    public enum ParenthesisThisAtribute
    {
        OpenParenthesis,
        CloseParenthesis,
        None
    }


}