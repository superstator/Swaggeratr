using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public interface IAPIErrorResponsesListSerializer
    {
        string SerializeErrorResponses(List<Models.APIs.APIResponseCode> list);
    }
}
