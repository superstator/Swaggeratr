using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class APIResourceListSerializer : IAPIResourceListSerializer
    {
        public APIResourceListSerializer() : this(new APIOperationsListSerializer()) { }

        public APIResourceListSerializer(IAPIOperationsListSerializer operationsSerializer)
        {
            _OperationsListSerializer = operationsSerializer;
        }

        private IAPIOperationsListSerializer _OperationsListSerializer;

        public string SerializeAPIResources(List<Models.APIs.APIResource> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();

                foreach (var r in list)
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("path");
                    writer.WriteValue(r.path);

                    if (!string.IsNullOrEmpty(r.description))
                    {
                        writer.WritePropertyName("description");
                        writer.WriteValue(r.description);
                    }

                    writer.WritePropertyName("operations");
                    writer.WriteRawValue(_OperationsListSerializer.SerializeOperations(r.operations));

                    writer.WriteEnd();
                }

                writer.WriteEnd();
            }
            return sb.ToString();
        }
    }
}
