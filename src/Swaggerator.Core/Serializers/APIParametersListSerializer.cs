using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class APIParametersListSerializer:IAPIParametersListSerializer
    {
        public string SerializeParameters(List<Models.APIs.APIParameter> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();

                foreach (var p in list)
                {
                    writer.WriteStartObject();

                    writer.WriteOptionalList("allowableValues", p.allowableValues);
                    writer.WriteOptionalProperty("allowMultiple",p.allowMultiple);
                    writer.WriteOptionalProperty("description",p.description);
                    writer.WriteOptionalProperty("name",p.name);
                    writer.WriteOptionalProperty("paramType",p.paramType);
                    writer.WriteOptionalProperty("required",p.required);
                    writer.WriteOptionalProperty("type",p.type);

                    writer.WriteEnd();
                }

                writer.WriteEnd();
            }
            return sb.ToString();
        }
    }
}
