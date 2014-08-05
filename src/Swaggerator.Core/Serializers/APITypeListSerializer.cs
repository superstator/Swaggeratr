using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class APITypeListSerializer : IAPITypeListSerializer
    {
        public APITypeListSerializer() : this(new APITypePropertiesSerializer()) { }

        public APITypeListSerializer(IAPITypePropertiesSerializer propertiesSerializer)
        {
            _propertiesSerializer = propertiesSerializer;
        }

        private IAPITypePropertiesSerializer _propertiesSerializer;

        public string SerializeAPITypes(List<Models.APIs.APIType> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                foreach (var t in list)
                {
                    writer.WritePropertyName(t.id);
                    writer.WriteStartObject();

                    writer.WritePropertyName("id");
                    writer.WriteValue(t.id);

                    writer.WritePropertyName("properties");
                    writer.WriteRawValue(_propertiesSerializer.SerializeAPITypeProperties(t.properties));
                    writer.WriteEnd();
                }

                writer.WriteEnd();
            }

            return sb.ToString();
        }
    }
}
