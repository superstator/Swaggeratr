using Newtonsoft.Json;
using Swaggerator.Core.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Serializers
{
    public class ServiceListSerializer : IServiceListSerializer
    {
        public string SerializeServiceList(ServiceList serviceList)
        {
            return JsonConvert.SerializeObject(serviceList);
        }
    }
}
