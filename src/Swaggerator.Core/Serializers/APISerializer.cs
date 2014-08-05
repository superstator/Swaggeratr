using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class APISerializer : IAPISerializer
    {
        public APISerializer() : this(new APIResourceListSerializer(), new APITypeListSerializer()) { }

        public APISerializer(IAPIResourceListSerializer resourceSerializer, IAPITypeListSerializer modelSerializer)
        {
            _APIModelListSerializer = modelSerializer;
            _APIResourceListSerializer = resourceSerializer;
        }

        private IAPIResourceListSerializer _APIResourceListSerializer;
        private IAPITypeListSerializer _APIModelListSerializer;

        public string SerializeAPI(Models.APIs.API api)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("apiVersion");
                writer.WriteValue(api.apiVersion);

                writer.WritePropertyName("swaggerVersion");
                writer.WriteValue(api.swaggerVersion);

                writer.WritePropertyName("basePath");
                writer.WriteValue(api.basePath);

                writer.WritePropertyName("resourcePath");
                writer.WriteValue(api.resourcePath);

                writer.WritePropertyName("apis");
                writer.WriteRawValue(_APIResourceListSerializer.SerializeAPIResources(api.apis));

                writer.WritePropertyName("models");
                writer.WriteRawValue(_APIModelListSerializer.SerializeAPITypes(api.models));
            }

            return sb.ToString();
        }
    }
}
