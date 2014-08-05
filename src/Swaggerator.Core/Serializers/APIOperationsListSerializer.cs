using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class APIOperationsListSerializer : IAPIOperationsListSerializer
    {
        public APIOperationsListSerializer() : this(new APIErrorResponsesSerializer(), new APIParametersListSerializer()) { }

        public APIOperationsListSerializer(IAPIErrorResponsesListSerializer errorsSerializer, IAPIParametersListSerializer parametersSerializer)
        {
            _ParametersSerializer = parametersSerializer;
            _ErrorsSerializer = errorsSerializer;
        }

        IAPIParametersListSerializer _ParametersSerializer;
        IAPIErrorResponsesListSerializer _ErrorsSerializer;


        public string SerializeOperations(List<Models.APIs.APIOperation> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();

                foreach (var o in list)
                {
                    writer.WriteStartObject();

                    writer.WriteOptionalProperty("httpMethod", o.httpMethod);
                    writer.WriteOptionalProperty("nickname", o.nickname);
                    writer.WriteOptionalProperty("type", o.type);
                    writer.WriteOptionalRef("items", o.itemsType);
                    writer.WriteOptionalProperty("summary", o.summary);
                    writer.WriteOptionalProperty("notes", o.notes);

                    writer.WritePropertyName("parameters");
                    writer.WriteRawValue(_ParametersSerializer.SerializeParameters(o.parameters));
                    
                    writer.WritePropertyName("errorResponses");
                    writer.WriteRawValue(_ErrorsSerializer.SerializeErrorResponses(o.errorResponses));
                                        
                    writer.WriteOptionalList("accepts", o.accepts);                    
                    writer.WriteOptionalList("produces", o.produces);

                    writer.WriteEnd();
                }

                writer.WriteEnd();
            }
            return sb.ToString();
        }
    }
}
