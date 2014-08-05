using System;
namespace Swaggerator.WCF.Reflector
{
    interface IServiceReflector
    {
        Swaggerator.Core.Models.Services.ServiceList GetServices();
        Swaggerator.Core.Models.Services.ServiceList GetServices(AppDomain searchDomain);
        Type FindServiceTypeByPath(string servicePath);
        Swaggerator.Core.Models.APIs.API GetAPIDetails(Uri basePath, string servicePath);
    }
}
