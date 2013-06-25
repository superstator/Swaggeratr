using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace Swaggerator
{
    public class Discoverator : IDiscoverator
    {
        public Models.ServiceList GetServices()
        {
            Models.ServiceList serviceList = new Models.ServiceList();
            serviceList.basePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/api-docs.json";
            serviceList.swaggerVersion = Globals.SWAGGER_VERSION;
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
                SwaggeratedAttribute da = ti.GetCustomAttribute<SwaggeratedAttribute>();
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

        private Type FindServiceTypeByPath(string servicePath)
        {
            Assembly[] allAssm = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in allAssm)
            {
                foreach (TypeInfo ti in assembly.DefinedTypes)
                {
                    SwaggeratedAttribute da = ti.GetCustomAttribute<SwaggeratedAttribute>();
                    if (da != null && da.LocalPath == servicePath)
                    {
                        return ti.AsType();
                    }
                }
            }
            return null;
        }

        public Stream GetServiceDetails(string servicePath)
        {
            Type serviceType = FindServiceTypeByPath(string.Format("/{0}", servicePath));

            Stack<Type> typeStack = new Stack<Type>();

            string api = Serializers.WriteApi(servicePath, serviceType, typeStack);

            MemoryStream ms = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(api));
            return ms;
        }        
    }
}
