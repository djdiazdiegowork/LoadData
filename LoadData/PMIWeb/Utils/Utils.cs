using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.PMI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using PMIWeb.Models;

namespace PMIWeb.Utils
{
    /// <summary>
    /// Clase que se va a utilizar para contener el conjunto
    /// de metodo utiles en todo el sistema
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Clase que se va a utiliza para construir un tree view
        /// a partir de un ruta
        /// </summary>
        /// <param name="directoryFieldValue"></param>
        /// <returns></returns>
        public static TreeViewModel GetTreeViewModelFromPath(IEnumerable<string> directoryFieldValue)
        {
            Action<TreeViewModel, IEnumerable<string>> ensureExists = null;
            ensureExists = (ftm, ts) =>
            {
                if (ts.Any())
                {
                    var title = ts.First();
                    var child = ftm.nodes.Where(x => x.text == title).SingleOrDefault();
                    if (child == null)
                    {
                        child = new TreeViewModel()
                        {
                            text = title,
                            nodes = new List<TreeViewModel>(),
                        };
                        ftm.nodes.Add(child);
                    }
                    ensureExists(child, ts.Skip(1));
                }
            };

            var root = new TreeViewModel() { text = "\\", nodes = new List<TreeViewModel>() };

            foreach (var path in directoryFieldValue)
            {
                var parts = path.Split('\\');
                ensureExists(root, parts);
            }

            return root;
        }
        
        /// <summary>
        /// Metodo que se va a encargar de generar los atributos select.
        /// Para esta version los atributos select van a ser constantes.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AttributeSelect> GetAttributeSelects()
        {
            var result = new List<AttributeSelect>
            {
                new AttributeSelect("PRGMID", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("PRGMNAME", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("PRGMTYPEID", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("VIDEOFILENAME", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("DIRECTORY", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("DURATIONMS", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("CREATOR", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("CREATETIME", AttributeType.Instance, AttributeDataType.TypeDate),
                new AttributeSelect("EDITOR", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("EDITTIME", AttributeType.Instance, AttributeDataType.TypeDate),
                new AttributeSelect("SUBTYPEID", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("BITRATE", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("FILESIZE", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("LOCKED", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("CHECKOR", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("CHECKTIME", AttributeType.Instance, AttributeDataType.TypeDate),
                new AttributeSelect("CHECKUP", AttributeType.Instance, AttributeDataType.TypeDouble),
            };
            return result;
        }

        /// <summary>
        /// Metodo que se va a encargar de generar los atributos filter.
        /// En dependencia de los tipos de campos por los cuales se esta
        /// realizando el query.
        /// </summary>
        /// <param name="formModel"></param>
        /// <returns></returns>
        public static IEnumerable<AttributeFilter> GetAttributeFilters(FormModel formModel)
        {
            var result = new List<AttributeFilter>();

            if (!string.IsNullOrEmpty(formModel.GeneralSearch))
            {
                formModel.GeneralSearch = $"%{formModel.GeneralSearch}%";

                //// Rellenado los campos select
                //// Tipos select
                //// PRGMTYPENAME
                var attribFilterPRGMTYPENAME_GS =
                    GetAttributeFilterSelect(formModel.GeneralSearch, "PRGMTYPETB_PRGMTYPENAME");
                if (attribFilterPRGMTYPENAME_GS != null)
                {
                    result.Add(attribFilterPRGMTYPENAME_GS);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.OrOperator;
                }

                //// SUBTYPENAME
                var attribFilterSUBTYPENAME_GS =
                    GetAttributeFilterSelect(formModel.GeneralSearch, "PRGMSUBTYPETB_SUBTYPENAME");
                if (attribFilterSUBTYPENAME_GS != null)
                {
                    result.Add(attribFilterSUBTYPENAME_GS);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.OrOperator;
                }

                //// PRGMNAME
                var attribFilterPRGMNAME_GS = GetAttributeFilterSelect(formModel.GeneralSearch, "PRGMNAME");
                if (attribFilterPRGMNAME_GS != null)
                {
                    result.Add(attribFilterPRGMNAME_GS);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.OrOperator;
                }

                //// CREATOR
                var attribFilterCREATOR_GS = GetAttributeFilterSelect(formModel.GeneralSearch, "CREATOR");
                if (attribFilterCREATOR_GS != null)
                {
                    result.Add(attribFilterCREATOR_GS);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.OrOperator;
                }

                //// EDITOR
                var attribFilterEDITOR_GS = GetAttributeFilterSelect(formModel.GeneralSearch, "EDITOR");
                if (attribFilterEDITOR_GS != null)
                {
                    result.Add(attribFilterEDITOR_GS);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.OrOperator;
                }

                //// CHECKOR
                var attribFilterCHECKOR_GS = GetAttributeFilterSelect(formModel.GeneralSearch, "CHECKOR");
                if (attribFilterCHECKOR_GS != null)
                {
                    result.Add(attribFilterCHECKOR_GS);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.OrOperator;
                }

            }

            else
            {




                //// Tipos numericos
                //// Para el PRGMID
                var attribFilterPRGMID =
                    GetAttributeFilterNumeric(formModel.PRGMID_COMP, formModel.PRGMID_VAL_1, formModel.PRGMID_VAL_2,
                        "PRGMID");
                if (attribFilterPRGMID != null)
                {
                    result.AddRange(attribFilterPRGMID);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// Tipos timer
                //// DURATIONMS
                if (formModel.DURATIONMS_VAL_1 != "0")
                {
                    var attribFilterDURATIONMS = GetAttributeFilterNumeric(formModel.DURATIONMS_COMP,
                        decimal.Parse(formModel.DURATIONMS_VAL_1) * 1000,
                        !string.IsNullOrEmpty(formModel.DURATIONMS_VAL_2)
                            ? decimal.Parse(formModel.DURATIONMS_VAL_2) * 1000
                            : 0, "DURATIONMS");
                    if (attribFilterDURATIONMS != null)
                    {
                        result.AddRange(attribFilterDURATIONMS);
                        result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                    }
                }

                //// Tipos numericos con magnitud
                //// BITRATE
                if (formModel.BITRATE_VAL_1 != -1)
                {
                    var value1BITRATEReal = Convert(formModel.BITRATE_VAL_1, formModel.BITRATE_TYPE);
                    var value2BITRATEReal = Convert(formModel.BITRATE_VAL_2, formModel.BITRATE_TYPE);
                    var attribFilterBITRATE =
                        GetAttributeFilterNumeric(formModel.BITRATE_COMP, value1BITRATEReal, value2BITRATEReal,
                            "BITRATE");
                    result.AddRange(attribFilterBITRATE);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// FILESIZE
                if (formModel.FILESIZE_VAL_1 != -1)
                {
                    var value1FILESIZEReal = Convert(formModel.FILESIZE_VAL_1, formModel.FILESIZE_TYPE);
                    var value2FILESIZEReal = Convert(formModel.FILESIZE_VAL_2, formModel.FILESIZE_TYPE);
                    var attribFilterFILESIZE =
                        GetAttributeFilterNumeric(formModel.FILESIZE_COMP, value1FILESIZEReal, value2FILESIZEReal,
                            "FILESIZE");

                    result.AddRange(attribFilterFILESIZE);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// Tipos booleanos
                //// LOCKED
                var attribFilterLOCKED = GetAttributeFilterBoolean(formModel.LOCKED_VAL, "LOCKED");
                if (attribFilterLOCKED != null)
                {
                    result.Add(attribFilterLOCKED);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// CHECKUP
                var attribFilterCHECKUP = GetAttributeFilterBoolean(formModel.CHECKUP_VAL, "CHECKUP");
                if (attribFilterCHECKUP != null)
                {
                    result.Add(attribFilterCHECKUP);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// Tipos select
                //// PRGMTYPENAME
                var attribFilterPRGMTYPENAME =
                    GetAttributeFilterSelect(formModel.PRGMTYPENAME_VAL, "PRGMTYPETB_PRGMTYPENAME");
                if (attribFilterPRGMTYPENAME != null)
                {
                    result.Add(attribFilterPRGMTYPENAME);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// SUBTYPENAME
                var attribFilterSUBTYPENAME =
                    GetAttributeFilterSelect(formModel.SUBTYPENAME_VAL, "PRGMSUBTYPETB_SUBTYPENAME");
                if (attribFilterSUBTYPENAME != null)
                {
                    result.Add(attribFilterSUBTYPENAME);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// PRGMNAME
                var attribFilterPRGMNAME = GetAttributeFilterSelect(formModel.PRGMNAME_VAL, "PRGMNAME");
                if (attribFilterPRGMNAME != null)
                {
                    result.Add(attribFilterPRGMNAME);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// CREATOR
                var attribFilterCREATOR = GetAttributeFilterSelect(formModel.CREATOR_VAL, "CREATOR");
                if (attribFilterCREATOR != null)
                {
                    result.Add(attribFilterCREATOR);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// EDITOR
                var attribFilterEDITOR = GetAttributeFilterSelect(formModel.EDITOR_VAL, "EDITOR");
                if (attribFilterEDITOR != null)
                {
                    result.Add(attribFilterEDITOR);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// CHECKOR
                var attribFilterCHECKOR = GetAttributeFilterSelect(formModel.CHECKOR_VAL, "CHECKOR");
                if (attribFilterCHECKOR != null)
                {
                    result.Add(attribFilterCHECKOR);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// Tipos time
                //// CREATETIME
                var attribsTimeCREATETIME = GetAttributesFilterTime(formModel.CREATETIME_COMP,
                    formModel.CREATETIME_VAL_1,
                    formModel.CREATETIME_VAL_2, "CREATETIME");
                if (attribsTimeCREATETIME != null)
                {
                    result.AddRange(attribsTimeCREATETIME);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// EDITTIME
                var attribsTimeEDITTIME = GetAttributesFilterTime(formModel.EDITTIME_COMP, formModel.EDITTIME_VAL_1,
                    formModel.EDITTIME_VAL_2, "EDITTIME");
                if (attribsTimeEDITTIME != null)
                {
                    result.AddRange(attribsTimeEDITTIME);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// CHECKTIME
                var attribsTimeCHECKTIME = GetAttributesFilterTime(formModel.CHECKTIME_COMP, formModel.CHECKTIME_VAL_1,
                    formModel.CHECKTIME_VAL_2, "CHECKTIME");
                if (attribsTimeCHECKTIME != null)
                {
                    result.AddRange(attribsTimeCHECKTIME);
                    result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.AndOperator;
                }

                //// Tipos Tree View
                //// DIRECTORY
                var attribFilterDIRECTORY = GetAttributeFilterTreeView(formModel.DIRECTORY_VAL, "DIRECTORY");
                if (attribFilterDIRECTORY != null)
                    result.Add(attribFilterDIRECTORY);
            }

            //// Corrigiendo el listado de queries que van a ser enviados a oracle text.
            ArrangeQueries(result);

            return result;
        }

        /// <summary>
        /// Metodo que se encarga de generar un atributo filter para los campos
        /// de tipo numericos.
        /// </summary>
        /// <param name="compType">el tipo de compracion</param>
        /// <param name="value1">el primer valor</param>
        /// <param name="value2">el segundo valor</param>
        /// <param name="fieldName">el nombre del campo</param>
        /// <returns></returns>
        private static IEnumerable<AttributeFilter> GetAttributeFilterNumeric(string compType, decimal value1, decimal value2, string fieldName)
        {
            if (IsValidNumeric(value1))
            {
                var result = new List<AttributeFilter>();
                var operatorForValues = GetOperatorForValuesNumeric(compType);
                if (operatorForValues.Count > 1)
                {
                    var attribFilterBigger = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDouble,
                        new List<string>() {value1.ToString(CultureInfo.InvariantCulture)}, operatorForValues[0],
                        OperatorForLinkAtribute.AndOperator,
                        ParenthesisThisAtribute.OpenParenthesis);
                    var attribFilterMinor = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDouble,
                        new List<string>() {value2.ToString(CultureInfo.InvariantCulture)}, operatorForValues[1],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.CloseParenthesis);
                    result.Add(attribFilterBigger);
                    result.Add(attribFilterMinor);
                }
                else
                {
                    var attribFilter = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDouble,
                        new List<string>() {value1.ToString(CultureInfo.InvariantCulture)}, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilter);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Metodo que se va a utilizar para generar un atributo filter
        /// para los campos booleanos.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static AttributeFilter GetAttributeFilterBoolean(string value, string fieldName)
        {
            var realValue = 1;
            switch (value)
            {
                case "no":
                    realValue = 0;
                    break;
                case "si":
                    realValue = 1;
                    break;
                default:
                    realValue = -1;
                    break;
            }

            //// En caso de que el valor de la variable "realValue" sea -1
            //// indica que el atributo no se va a tener en cuenta.
            if (realValue != -1)
            {
                var attribFilter = new AttributeFilter(fieldName, AttributeType.Instance,
                    AttributeDataType.TypeDouble,
                    new List<string>() { realValue.ToString() }, OperatorForValues.Equal,
                    OperatorForLinkAtribute.None,
                    ParenthesisThisAtribute.None);
                return attribFilter;
            }

            return null;
        }

        /// <summary>
        /// Metodo que se encarga de generar un atributo filter
        /// para los campos de tipo select
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static AttributeFilter GetAttributeFilterSelect(string value, string fieldName)
        {
            if (IsValidSelect(value))
            {
                //value = value.TrimStart(' ', '\'');
                //value = value.TrimEnd(' ', '\'');

                value = value.Replace("'", "''").Replace(@"\",
                    @"\\");

                var attribFilter = new AttributeFilter(fieldName, AttributeType.Instance,
                    AttributeDataType.TypeString,
                    new List<string>() { value }, OperatorForValues.Equal,
                    OperatorForLinkAtribute.None,
                    ParenthesisThisAtribute.None);
                return attribFilter;
            }
            return null;
        }

        /// <summary>
        /// Metodo que se encarga de generar el atributo filter para los
        /// campos de tipo de treeview.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static AttributeFilter GetAttributeFilterTreeView(string value, string fieldName)
        {
            if (!IsValidTreeview(value)) return null;

            //value = value.TrimStart('\'');
            //value = value.TrimEnd('\'');

            value = value.Replace("'", "''").Replace(@"\",
                @"\\");

            var attribFilter = new AttributeFilter(fieldName, AttributeType.Instance,
                AttributeDataType.TypeString,
                new List<string>() {$"{value}%"}, OperatorForValues.Equal,
                OperatorForLinkAtribute.None,
                ParenthesisThisAtribute.None);
            return attribFilter;
        }

        /// <summary>
        /// Metodo que se encarga de generar los atributos filter
        /// para los campos de tipo tiempo.
        /// </summary>
        /// <param name="compType"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="fielName"></param>
        /// <returns></returns>
        private static IEnumerable<AttributeFilter> GetAttributesFilterTime(string compType, string value1,
            string value2, string fieldName)
        {
            if (!IsValidTime(value1)) return null;

            var result = new List<AttributeFilter>();

            var compTypeNum = int.Parse(compType);
            var operatorForValues = new List<OperatorForValues>();
            if (compTypeNum >= 0 && compTypeNum <= 5 && compTypeNum != 1)
                operatorForValues.Add(OperatorForValues.Between);
            else if(compTypeNum == 1)
            {
                operatorForValues.Add(OperatorForValues.Equal);
            }
            else
            {
                operatorForValues.Add(OperatorForValues.BiggerEqual);
                operatorForValues.Add(OperatorForValues.MinorEqual);
            }

            switch (compType)
            {
                //// Periodicidad
                case "0":
                    var attribFilterBetween = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() {value1, value2}, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterBetween);
                    break;
                //// Igual
                case "1":
                    var attribFilterEqual = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() { value1 }, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterEqual);
                    break;
                //// Mes Anno
                case "2":
                    var values = GetValuesForMonthYearTime(value1);
                    var attribFilterBetween2 = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() { values[0], values[1] }, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterBetween2);
                    break;
                //// Anno
                case "3":
                    var values1 = GetValuesForYearTime(value1);
                    var attribFilterBetween3 = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() { values1[0], values1[1] }, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterBetween3);
                    break;
                //// Trimestre
                case "4":
                    var values2 = GetValuesForQuarterTime(value1);
                    var attribFilterBetween4 = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() { values2[0], values2[1] }, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterBetween4);
                    break;
                //// Verano (Julio y Agosto)
                case "5":
                    var values3 = GetValuesForSummer(value1);
                    var attribFilterBetween5 = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() { values3[0], values3[1] }, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterBetween5);
                    break;
                //// Mayor que
                case "6":
                    var attribFilterBigger = new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() { value1 }, operatorForValues[0],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterBigger);
                    break;
                //// Menor que
                case "7":
                    var attribFilterMinor2= new AttributeFilter(fieldName, AttributeType.Instance,
                        AttributeDataType.TypeDate,
                        new List<string>() { value1 }, operatorForValues[1],
                        OperatorForLinkAtribute.None,
                        ParenthesisThisAtribute.None);
                    result.Add(attribFilterMinor2);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Metodo que se va a encargar de devolver las cotas inferior
        /// y superior que representan las cotas en tiempo de un mes
        /// en un determinado anno.
        /// </summary>
        /// <param name="value">formato MM/YYYY</param>
        /// <returns></returns>
        private static IList<string> GetValuesForMonthYearTime(string value)
        {
            var valueSplit = value.Split('/');

            var month = int.Parse(valueSplit[0]);
            var year = int.Parse(valueSplit[1]);

            var datetimeStart = new DateTime(year, month, 01);
            var datetimeFinish = datetimeStart.AddMonths(1);

            var startString = datetimeStart.ToString("yyyy-MM-dd HH:mm:ss");
            var finishString = datetimeFinish.ToString("yyyy-MM-dd HH:mm:ss");

            return new List<string> {startString, finishString};
        }

        /// <summary>
        /// Metodo que se encarga de devolver el rango de fechas para un determinado
        /// anno.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IList<string> GetValuesForYearTime(string value)
        {
            var year = int.Parse(value);
            var datetimeStart = new DateTime(year, 01, 01);
            var datetimeFinish = datetimeStart.AddYears(1);

            var startString = datetimeStart.ToString("yyyy-MM-dd HH:mm:ss");
            var finishString = datetimeFinish.ToString("yyyy-MM-dd HH:mm:ss");

            return new List<string> { startString, finishString };
        }

        /// <summary>
        /// Metodo que se encarga de generar las fechas para un trimestre
        /// </summary>
        /// <param name="value">formato Enero-Marzo 2019</param>
        /// <returns></returns>
        private static IList<string> GetValuesForQuarterTime(string value)
        {
            var monthsYearSplit = value.Split(' ');
            var monthsSplit = monthsYearSplit[0].Split('-');
            var monthStart = monthsSplit[0];
            var monthFinish = monthsSplit[1];
            var year = monthsYearSplit[1];

            var monthStartNum = GetNumberMonth(monthStart);
            var monthFinishNum = GetNumberMonth(monthFinish);
            var yearNum = int.Parse(year);

            var datetimeStart = new DateTime(yearNum, monthStartNum, 01);
            var datetimeFinish = new DateTime(yearNum, monthFinishNum, 01).AddMonths(1);

            var startString = datetimeStart.ToString("yyyy-MM-dd HH:mm:ss");
            var finishString = datetimeFinish.ToString("yyyy-MM-dd HH:mm:ss");

            return new List<string> {startString, finishString};
        }

        /// <summary>
        /// Metodo que se va a encargar de devolver los valores
        /// de fecha para el periodo de verano
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IList<string> GetValuesForSummer(string value)
        {
            var valueSplit = value.Split(' ');
            var year = int.Parse(valueSplit[valueSplit.Length - 1]);

            var datetimeJuly = new DateTime(year, 07, 01);
            var datetimeAug = new DateTime(year, 08, 01).AddMonths(1);

            var startString = datetimeJuly.ToString("yyyy-MM-dd HH:mm:ss");
            var finishString = datetimeAug.ToString("yyyy-MM-dd HH:mm:ss");

            return new List<string> { startString, finishString };
        }

        /// <summary>
        /// Metodo que se encarga de devolver el numero que representa un
        /// mes dado por su nombre.
        /// </summary>
        /// <param name="monthString"></param>
        /// <returns></returns>
        private static int GetNumberMonth(string monthString)
        {
            var monthToMatch = monthString.ToLower();
            var months = new List<string>
            {
                "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "noviembre",
                "diciembre"
            };
            for (int i = 0; i < months.Count; i++)
            {
                if (monthToMatch == months[i]) return i + 1;
            }

            return -1;
        }
        

        /// <summary>
        /// Metodo que se va a encargar de devolver el tipo de operador
        /// de comparacion que se esta requiriendo.
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        private static List<OperatorForValues> GetOperatorForValuesNumeric(string compType, bool isTime = false)
        {
            var result = new List<OperatorForValues>();
            switch (compType)
            {
                case "0":
                    result.Add(OperatorForValues.Equal);
                    break;
                //// Rango
                case "1":
                    if (!isTime)
                    {
                        result.Add(OperatorForValues.BiggerEqual);
                        result.Add(OperatorForValues.MinorEqual);
                    }
                    result.Add(OperatorForValues.Between);
                    break;
                case "2":
                    result.Add(OperatorForValues.MinorEqual);
                    break;
                case "3":
                    result.Add(OperatorForValues.BiggerEqual);
                    break;
                default: break;
            }
            return result;
        }

        /// <summary>
        /// Metodo que se va a encargar de devolver el o los atributos
        /// filters asociados a un tipo de campo time.
        /// </summary>
        /// <param name="comptype"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="fielName"></param>
        /// <returns></returns>
        private static IEnumerable<AttributeFilter> GetAttributeFilterTime(string comptype, string value1, string value2, string fielName)
        {
            var value1Parse = ParseFromTimeToDecimal(value1);
            var value2Parse = string.IsNullOrEmpty(value2) ? 0 : ParseFromTimeToDecimal(value2);
            return GetAttributeFilterNumeric(comptype, (decimal) value1Parse, (decimal) value2Parse, fielName);
        }

        /// <summary>
        /// Metodo que se va a encargar de parsear un string
        /// en formato de timerpicker a su valor en milisegundos
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double ParseFromTimeToDecimal(string value)
        {
            var format = value.Length > 7 ? "hh:mm tt" : "h:mm tt";
            var dateTime = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
            var result = dateTime.TimeOfDay;
            return result.TotalMilliseconds;
        }

        /// <summary>
        /// Metodo que se utiliza para convertir un valor dado en una medida
        /// para su medidad mas basica.
        /// </summary>
        /// <param name="value">el valor que va a ser convertido, viene como un decimal
        /// en forma de string</param>
        /// <param name="magnitudeFrom">a partir de cual magnitud se va a convertir</param>
        /// <returns></returns>
        private static decimal Convert(decimal value, string magnitudeFrom)
        {
            return value * ConvertMagnitudeFromToBasic(magnitudeFrom);
        }

        /// <summary>
        /// Metodo que se encarga de convertir una magnitud en su escala
        /// mas basica
        /// </summary>
        /// <param name="magnitudeFrom">origen</param>
        /// <returns></returns>
        private static decimal ConvertMagnitudeFromToBasic(string magnitudeFrom)
        {
            var multiplier = 1d;
            switch (magnitudeFrom)
            {
                //// Para el BITRATE
                case "MB/S":
                    multiplier = Math.Pow(1024, 2);
                    break;
                case "KB/S":
                    multiplier = 1024;
                    break;
                //// Para el FILESIZE
                case "GB":
                    multiplier = Math.Pow(1024, 3);
                    break;
                case "MB":
                    multiplier = Math.Pow(1024, 2);
                    break;
                case "KB":
                    multiplier = 1024;
                    break;
                default: break;
            }
            //// En caso de que sean las propias medidas basicas
            //// se estara multiplicando por 1 para lograr el mismo
            //// valor  

            return (decimal)multiplier;
        }

        /// <summary>
        /// Metodo que se encarga de validar si un select tiene un valor
        /// seleccionable.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsValidSelect(string value)
        {
            return !value.ToLower().StartsWith("seleccione un");
        }

        /// <summary>
        /// Metodo que se va a utilizar para validar los campos de
        /// tipo numerico.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsValidNumeric(decimal value)
        {
            return value != -1;
        }

        /// <summary>
        /// Metodo que se va a encargar de validar un campo de tipo
        /// time
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsValidTime(string value)
        {
            return value != null;
        }

        /// <summary>
        /// Metodo que se va a encargar de validar un campo de tipo
        /// treeview
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsValidTreeview(string value)
        {
            return !string.IsNullOrEmpty(value) && !value.StartsWith("undefined");
        }

        /// <summary>
        /// Metodo que se va a encargar de arreglar el ultimo
        /// atributo de link del listado de los atributos filters
        /// que se van a utilizar.
        /// </summary>
        /// <param name="result"></param>
        private static void ArrangeQueries(List<AttributeFilter> result)
        {
            if (result != null && result.Any())
            {
                result.Last().OperatorForLinkAtribute = OperatorForLinkAtribute.None;
            }
        }

        /// <summary>
        /// Metodo que se va a utilizar para realizar la exportacion de los resultados
        /// en docx o en xlsx
        /// </summary>
        /// <param name="format">el formato que representa el tipo de documento
        /// que se quiere generar, los posibles valores son:
        /// 1- xlsx: Excel
        /// 2- docx: Word
        /// </param>
        /// <param name="data">El conjunto de datos que se van a utilizar para generar
        /// el documento</param>
        /// <returns></returns>
        public static string GetUrlDocumentFile(string format, string path, IEnumerable<PROGRAMTB> data)
        {
            var uploadFolder = path;
            string sFileName = $"{Guid.NewGuid().ToString()}.{format}";
            using (var fs = new FileStream(Path.Combine(uploadFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
               switch (format)
                {
                    case "xlsx":
                        var excel = CreateAExcel(data);
                        excel.Write(fs);
                        break;
                    case "docx":
                        var word = CreateAWord(data);
                        word.Write(fs);
                        break;
                }
                return Path.Combine(uploadFolder, sFileName);
            }
        }

        /// <summary>
        /// Metodo que va a salvar temporalmente un documento que va a ser
        /// utilizado para la generacion de un reporte.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="data"></param>
        /// <param name="temp"></param>
        public static string SaveDocumentTemp(string format, IEnumerable<PROGRAMTB> data, Dictionary<string, byte[]> temp)
        {
            string fileName = $"{Guid.NewGuid().ToString()}.{format}";
            using (var memoryStream = new NpoiMemoryStream())
            {
                memoryStream.AllowClose = false;
                switch (format)
                {
                    case "xlsx":
                        var excel = CreateAExcel(data);
                        excel.Write(memoryStream);
                        break;
                    case "docx":
                        var word = CreateAWord(data);
                        word.Write(memoryStream);
                        break;
                }

                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.AllowClose = true;
                temp[fileName] = memoryStream.ToArray();
                return fileName;
            }
        }

        /// <summary>
        /// Metodo que se va a encargar de crear un excel en memoria
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IWorkbook CreateAExcel(IEnumerable<PROGRAMTB> data)
        {
            IWorkbook workbook;
            workbook = new XSSFWorkbook();
            ISheet excelSheet = workbook.CreateSheet("Principal");

            IRow row = excelSheet.CreateRow(0);
            var headers = GetPropertiesNamePROGRAMTB();
            for (int i = 0; i < headers.Count; i++)
            {
                row.CreateCell(i).SetCellValue(headers[i]);
            }

            var index = 1;
            foreach (var programtb in data)
            {
                row = excelSheet.CreateRow(index++);
                row.CreateCell(0).SetCellValue(programtb.PRGMID);
                row.CreateCell(1).SetCellValue(programtb.PRGMNAME);
                row.CreateCell(2).SetCellValue(programtb.PRGMTYPEID.ToString());
                row.CreateCell(3)
                    .SetCellValue(programtb.PRGMTYPETB != null ? programtb.PRGMTYPETB.PRGMTYPENAME : "-");
                row.CreateCell(4).SetCellValue(programtb.VIDEOFILENAME);
                row.CreateCell(5).SetCellValue(programtb.DIRECTORY);
                row.CreateCell(6).SetCellValue(programtb.DURATIONMS.ToString());
                row.CreateCell(7).SetCellValue(programtb.CREATOR);
                row.CreateCell(8).SetCellValue(programtb.CREATETIME.ToString());
                row.CreateCell(9).SetCellValue(programtb.EDITOR);
                row.CreateCell(10).SetCellValue(programtb.EDITTIME.ToString());
                row.CreateCell(11).SetCellValue(programtb.SUBTYPEID.ToString());
                row.CreateCell(12).SetCellValue(programtb.PRGMSUBTYPETB != null
                    ? programtb.PRGMSUBTYPETB.SUBTYPENAME
                    : "-");
                row.CreateCell(13).SetCellValue(programtb.BITRATE.ToString());
                row.CreateCell(14).SetCellValue(programtb.FILESIZE.ToString());
                row.CreateCell(15).SetCellValue(programtb.LOCKED.ToString());
                row.CreateCell(16).SetCellValue(programtb.CHECKOR);
                row.CreateCell(17).SetCellValue(programtb.CHECKTIME.ToString());
                row.CreateCell(18).SetCellValue(programtb.CHECKUP.ToString());
            }
            
            return workbook;
        }

        /// <summary>
        /// Metodo que se va a encargar de realizar un word en memoria
        /// </summary>
        /// <returns></returns>
        private static XWPFDocument CreateAWord(IEnumerable<PROGRAMTB> data)
        {
            var doc = new XWPFDocument();
            foreach (var programtb in data)
            {
                var textFormat =
                    $"PID:{programtb.PRGMID} PRGMNAME:{programtb.PRGMNAME} PRGMTYPEID:{programtb.PRGMTYPEID.ToString()} PRGMTYPETB:{(programtb.PRGMTYPETB != null ? programtb.PRGMTYPETB.PRGMTYPENAME : "-")} VIDEOFILENAME:{programtb.VIDEOFILENAME} DIRECTORY:{programtb.DIRECTORY} DURATIONMS:{programtb.DURATIONMS.ToString()} CREATOR:{programtb.CREATOR} CREATETIME:{programtb.CREATETIME.ToString()} EDITOR:{programtb.EDITOR} EDITTIME:{programtb.EDITTIME.ToString()} SUBTYPEID:{programtb.SUBTYPEID.ToString()} PRGMSUBTYPETB:{(programtb.PRGMSUBTYPETB != null ? programtb.PRGMSUBTYPETB.SUBTYPENAME : "-")} BITRATE:{programtb.BITRATE.ToString()} FILESIZE:{programtb.FILESIZE.ToString()} LOCKED:{programtb.LOCKED.ToString()} CHECKOR:{programtb.CHECKOR} CHECKTIME:{programtb.CHECKTIME.ToString()} CHECKUP:{programtb.CHECKUP.ToString()}";

                var paragraph = doc.CreateParagraph();
                var run = paragraph.CreateRun();
                run.SetText(textFormat);
            }

            return doc;
        }

        /// <summary>
        /// Metodo que se encarga de obtener el listado de los nombres
        /// de las propiedades de la clase PROGRAMTB.
        /// </summary>
        /// <returns></returns>
        private static IList<string> GetPropertiesNamePROGRAMTB()
        {
            var propertyList = new List<string>
            {
                "PID", "PRGMNAME", "PRGMTYPEID", "PRGMTYPETB", "VIDEOFILENAME", "DIRECTORY", "DURATIONMS", "CREATOR",
                "CREATETIME", "EDITOR", "EDITTIME", "SUBTYPEID", "PRGMSUBTYPETB", "BITRATE", "FILESIZE", "LOCKED",
                "CHECKOR", "CHECKTIME", "CHECKUP"
            };
            return propertyList;
        }

        /// <summary>
        /// Metodo que se va a utilizar para testear los metodos sin datos
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PROGRAMTB> GetDataTest()
        {
            var prg1 = new PROGRAMTB
            {
                PRGMID = 1,
                PRGMTYPEID = 2,
                PRGMNAME = "algun nombre",
                PRGMTYPETB = new PRGMTYPETB
                {
                    PRGMTYPEID = 2,
                    PRGMTYPENAME = "algun nombre de tipo de programa",
                },
                VIDEOFILENAME = "algun video filename",
                DIRECTORY = "algun directory name",
                BITRATE = 2,
                CHECKOR = "algun checkor",
                CHECKTIME = DateTime.Now,
                CHECKUP = 1,
                CREATETIME = DateTime.Now,
                CREATOR = "algun creado",
                DURATIONMS = 1,
                EDITOR = "algun editor",
                EDITTIME = DateTime.Now,
                FILESIZE = 1,
                LOCKED = 1,
                PRGMSUBTYPETB = new PRGMSUBTYPETB
                {
                    SUBTYPENAME = "algun subtype name",
                    SUBTYPEID = 3,
                },
                SUBTYPEID = 3
            };
            return Enumerable.Range(0, 100).Select(i => prg1);
        }

        /// <summary>
        /// Metodo que se encarga de salvar un file en una carpeta
        /// </summary>
        /// <param name="iFile"></param>
        /// <param name="webRootFolder"></param>
        /// <returns></returns>
        public static async Task<(bool, string, string)> SaveImage(IFormFile iFile, string webRootFolder, string oldImageName=null)
        {
            if (iFile != null)
            {
                var imageCustomName = iFile.FileName;
                var extension = Path.GetExtension(imageCustomName);
                var imageRealName = $"{Guid.NewGuid().ToString()}{extension}";

                var file = Path.Combine(webRootFolder, "Uploads", imageRealName);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    try
                    {
                        await iFile.CopyToAsync(fileStream);
                    }
                    catch (Exception)
                    {
                        return (false, string.Empty, string.Empty);
                    }
                }
                
                var resultDelete = DeleteFile(webRootFolder, oldImageName);
                if(!resultDelete) return (false, string.Empty, string.Empty);
                return (true, imageCustomName, imageRealName);
            }

            return (false, string.Empty, string.Empty);
        }

        /// <summary>
        /// Metodo que se va a utilizar para eliminar un fichero de la carpeta Upload
        /// </summary>
        /// <param name="webRootFolder">carpeta root</param>
        /// <param name="filePath">el nombre del archivo</param>
        /// <returns></returns>
        public static bool DeleteFile(string webRootFolder, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var fileNameOldImage = Path.Combine(webRootFolder, "Uploads", filePath);
                try
                {
                    File.Delete(fileNameOldImage);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Metodo que se encarga de verificar si existe fisicamente un
        /// archivo en la carpeta Uploads
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ExistsFile(string webRootFolder, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var filePathFull = Path.Combine(webRootFolder, "Uploads", filePath);
                return File.Exists(filePathFull);
            }

            return false;
        }
    }
}
