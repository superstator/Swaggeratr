using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public interface IAPIOperationsListSerializer
    {
        string SerializeOperations(List<Models.APIs.APIOperation> list);
    }
}
