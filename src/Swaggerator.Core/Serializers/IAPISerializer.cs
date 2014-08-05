using Swaggerator.Core.Models.APIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public interface IAPISerializer
    {
        string SerializeAPI(API api);
    }
}
