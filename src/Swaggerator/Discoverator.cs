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

		private Type FindServiceTypeByPath(string servicePath)
		{
			Assembly[] allAssm = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in allAssm)
			{
				foreach (TypeInfo ti in assembly.DefinedTypes)
				{
					DiscoveratedAttribute da = ti.GetCustomAttribute<DiscoveratedAttribute>();
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
			
			Models.Api api = new Models.Api()
			{
				apiVersion = serviceType.Assembly.GetName().Version.ToString(),
				swaggerVersion = SWAGGER_VERSION
			};

			SwaggerMappers maps = new SwaggerMappers();

			api.basePath = string.Format("{0}://{1}/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, servicePath);
			api.resourcePath = string.Format(servicePath);


			api.apis.AddRange(maps.FindMethods(serviceType));

			//api.models.AddRange(FindModels(serviceType));

			string reply = JsonConvert.SerializeObject(api);

			string replyModels = maps.InsertModelSchemas(reply, api);

			MemoryStream ms = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(replyModels));
			return ms;			
		}
	}
}
