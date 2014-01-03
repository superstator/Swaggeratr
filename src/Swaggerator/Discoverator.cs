/*
 * Copyright (c) 2014 Digimarc Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Discoverator.cs : Core Swaggerator service. Provides primary public methods for service discovery and description.
 */


using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.ServiceModel.Activation;

namespace Swaggerator
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class Discoverator : IDiscoverator
	{
		/// <summary>
		/// Gets a new instance of the core Discoverator service
		/// </summary>
		public Discoverator()
		{
			string sectionName = "swagger";
			var config = (Configuration.SwaggerSection)(System.Configuration.ConfigurationManager.GetSection(sectionName) ?? new Configuration.SwaggerSection());
			HiddenTags = config.Tags.OfType<Configuration.TagElement>().Where(t => t.Visibile.Equals(false)).Select(t => t.Name);
			_Serializer = new Serializer(HiddenTags);
		}

		internal Discoverator(IEnumerable<string> hiddenTags)
		{
			HiddenTags = hiddenTags;
			_Serializer = new Serializer(HiddenTags);
		}

		internal readonly IEnumerable<string> HiddenTags;
		private readonly Serializer _Serializer;

		public Stream GetServices()
		{
			return GetServices(AppDomain.CurrentDomain);
		}

		public Stream GetServices(AppDomain searchDomain)
		{
			Models.ServiceList serviceList = new Models.ServiceList
			{
				swaggerVersion = Globals.SWAGGER_VERSION,
				apiVersion = "No Swaggerized assemblies."
			};

			Assembly[] searchAssemblies = searchDomain.GetAssemblies();

			bool foundAssembly = false;
			foreach (Assembly assm in searchAssemblies)
			{
				IEnumerable<Models.Service> services = GetDiscoveratedServices(assm);
				if (services.Any())
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

			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(serviceList)));

			return stream;
		}

		private static IEnumerable<Models.Service> GetDiscoveratedServices(Assembly assembly)
		{
			IEnumerable<TypeInfo> types;
			try
			{
				types = assembly.DefinedTypes;
			}
			catch (ReflectionTypeLoadException)
			{
				//couldn't load this assembly - probably a non-issue
				yield break;
			}

			foreach (TypeInfo ti in types)
			{
				SwaggeratedAttribute da = ti.GetCustomAttribute<SwaggeratedAttribute>();
				if (da != null)
				{
					DescriptionAttribute descAttr = ti.GetCustomAttribute<DescriptionAttribute>();
					Models.Service service = new Models.Service
					{
						path = da.LocalPath,
						description = (descAttr == null) ? da.Description : descAttr.Description
					};
					yield return service;
				}
			}
		}

		private static Type FindServiceTypeByPath(string servicePath)
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
			return GetServiceDetails(AppDomain.CurrentDomain, HttpContext.Current.Request.Url, servicePath);
		}

		public Stream GetServiceDetails(AppDomain domain, Uri baseUri, string servicePath)
		{
			Type serviceType = FindServiceTypeByPath(string.Format("/{0}", servicePath));

			Stack<Type> typeStack = new Stack<Type>();

			string api = _Serializer.WriteApi(baseUri, string.Format("/{0}", servicePath), serviceType, typeStack);

			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(api));
			return ms;
		}
	}
}
