using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class APIErrorResponsesSerializer : IAPIErrorResponsesListSerializer
    {
        public string SerializeErrorResponses(List<Models.APIs.APIResponseCode> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();

                foreach (var r in list)
                {
                    writer.WriteStartObject();

                    writer.WriteOptionalProperty("code", r.code);
                    writer.WriteOptionalProperty("message", r.message);

                    writer.WriteEnd();
                }

                writer.WriteEnd();
            }
            return sb.ToString();
        }
    }
}
