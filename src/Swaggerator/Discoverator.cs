using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Swaggerator
{
    public class Discoverator : IDiscoverator
    {
        private const string SWAGGER_VERSION = "1.1";

        public Models.ServiceList GetServices()
        {
            Models.ServiceList serviceList = new Models.ServiceList();
            serviceList.basePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/api-docs.json";
            serviceList.swaggerVersion = SWAGGER_VERSION;
            serviceList.apiVersion = "No Swaggerized assemblies.";

            Assembly[] allAssm = AppDomain.CurrentDomain.GetAssemblies();

            bool foundAssembly = false;
            foreach (Assembly assm in allAssm)
            {
                IEnumerable<Models.Service> services = GetDiscoveratedServices(assm);
                if (services.Count() > 0)
                {
                    if (foundAssembly) { serviceList.apiVersion = "Multiple Assemblies"; }
                    else
                    {
                        foundAssembly = true;
                        serviceList.apiVersion = assm.GetName().Version.ToString();
                    }
                    serviceList.apis.AddRange(services);
                }
            }

            return serviceList;
        }

        private IEnumerable<Models.Service> GetDiscoveratedServices(Assembly assembly)
        {
            foreach (TypeInfo ti in assembly.DefinedTypes)
            {
                DiscoveratedAttribute da = ti.GetCustomAttribute<DiscoveratedAttribute>();
                if (da != null)
                {
                    DescriptionAttribute descAttr = ti.GetCustomAttribute<DescriptionAttribute>();
                    Models.Service service = new Models.Service();
                    service.path = da.LocalPath;
                    service.description = (da == null) ? (descAttr == null) ? "" : descAttr.Description : da.Description;
                    yield return service;
                }
            }

        }
    }
}
