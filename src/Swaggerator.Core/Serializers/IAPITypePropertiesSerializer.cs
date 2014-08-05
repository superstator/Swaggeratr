using Swaggerator.Core.Models.APIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public interface IAPITypePropertiesSerializer
    {
        string SerializeAPITypeProperties(List<APITypeProperty> list);
    }
}
