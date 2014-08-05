using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class APITypePropertiesSerializer : IAPITypePropertiesSerializer
    {
        public string SerializeAPITypeProperties(List<Models.APIs.APITypeProperty> list)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            //var hasParent = type.BaseType != typeof (Object);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                foreach (var t in list)
                {
                    writer.WritePropertyName(t.id);

                    writer.WriteStartObject();

                    if (!string.IsNullOrEmpty(t.description))
                    {
                        writer.WritePropertyName("description");
                        writer.WriteValue(t.description);
                    }

                    writer.WritePropertyName("type");
                    writer.WriteValue(t.type);

                    writer.WritePropertyName("required");
                    writer.WriteValue(t.required);

                    if (!string.IsNullOrEmpty(t.itemsType))
                    {
                        writer.WritePropertyName("items");
                        
                        writer.WriteStartObject();
                        
                        writer.WritePropertyName("$ref");
                        writer.WriteValue(t.itemsType);
                        
                        writer.WriteEnd();
                    }

                    writer.WriteOptionalList("enum", t.enumValues);

                    writer.WriteEnd();
                }

                writer.WriteEnd();
            }
            return sb.ToString();
        }
    }
}
