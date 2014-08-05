using System;
namespace Swaggerator.Core
{
    public interface ISerializer
    {
        string SerializeAPI(Swaggerator.Core.Models.APIs.API api);
        string SerializeServiceList(Swaggerator.Core.Models.Services.ServiceList serviceList);
        string SerializeAPIDetails(Swaggerator.Core.Models.APIs.API api);
    }
}
