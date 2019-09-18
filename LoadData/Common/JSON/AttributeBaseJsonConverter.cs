using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace Common.JSON
{
    public class AttributeBaseJsonConverter : JsonCreationConverter<AttributeBase>
    {
        protected override AttributeBase Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException("jObject");

            var name = jObject["Name"].ToObject<string>();
            var attributeType = jObject["Type"].ToObject<AttributeType>();
            var dataType = jObject["DataType"].ToObject<AttributeDataType>();

            if (jObject["Values"] != null)
            {
                var values = jObject["Values"].ToObject<List<string>>();
                var operatorForValues = jObject["OperatorForValues"].ToObject<OperatorForValues>();
                var operatorForLinkAtribute = jObject["OperatorForLinkAtribute"].ToObject<OperatorForLinkAtribute>();
                var parenthesisThisAtribute = jObject["ParenthesisThisAtribute"].ToObject<ParenthesisThisAtribute>();

                return new AttributeFilter(name, attributeType, dataType, values, operatorForValues,
                    operatorForLinkAtribute, parenthesisThisAtribute);
            }
            else
            {
                return new AttributeSelect(name, attributeType, dataType);
            }
        }
    }
}
