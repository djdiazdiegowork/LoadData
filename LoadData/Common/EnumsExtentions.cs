using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// Metodos extensores para los enums
    /// </summary>
    public static class EnumsExtentions
    {
        public static string OperatorForValuesCustomToString(this OperatorForValues op)
        {
            switch (op)
            {
                case OperatorForValues.Between:
                    return "BETWEEN";
                case OperatorForValues.Bigger:
                    return ">";
                case OperatorForValues.BiggerEqual:
                    return ">=";
                case OperatorForValues.Different:
                    return "!=";
                case OperatorForValues.Equal:
                    return "=";
                case OperatorForValues.Minor:
                    return "<";
                case OperatorForValues.MinorEqual:
                    return "<=";
                default: return string.Empty;
            }
        }
        public static string OperatorForLinkAtributeCustomToString(this OperatorForLinkAtribute op)
        {
            switch (op)
            {
                case OperatorForLinkAtribute.AndOperator:
                    return "AND";
                case OperatorForLinkAtribute.OrOperator:
                    return "OR";
                default: return string.Empty;
            }
        }
        public static string ParenthesisThisAtributeCustomToString(this ParenthesisThisAtribute p)
        {
            switch (p)
            {
                case ParenthesisThisAtribute.OpenParenthesis:
                    return "(";
                case ParenthesisThisAtribute.CloseParenthesis:
                    return ")";
                default: return string.Empty;
            }
        }
    }
}
