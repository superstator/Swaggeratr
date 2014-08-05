using Swaggerator.Core.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public interface IServiceListSerializer
    {
        string SerializeServiceList(ServiceList serviceList);
    }
}
