using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Swaggerator.Models;

namespace Swaggerator
{
	internal class SwaggerMappers
	{
		public SwaggerMappers()
		{
			_MappedTypes = new List<Type>();
		}

		public string InsertModelSchemas(string reply, Api api)
		{
			JsonSchemaGenerator gen = new JsonSchemaGenerator();

			StringBuilder sb = new StringBuilder();
			sb.Append("{");

			for (int i = 0; i < _MappedTypes.Count; i++)
			{
				sb.AppendFormat("\"{0}\":", _MappedTypes[i].FullName);

				JsonSchema schema = gen.Generate(_MappedTypes[i]);
				schema.Id = _MappedTypes[i].FullName;
				StringWriter writer = new StringWriter();
				JsonTextWriter jsonTextWriter = new JsonTextWriter(writer);
				schema.WriteTo(jsonTextWriter);
				string model = writer.ToString();

				sb.Append(model);
				if (i != _MappedTypes.Count - 1) { sb.Append(","); }
			}

			sb.Append("}");

			string models = sb.ToString().Replace("[\"string\",\"null\"]", "\"string\"");
			models = models.Replace("[\"object\",\"null\"]", "\"object\"");
			models = models.Replace("[\"array\",\"null\"]", "\"string\"");


			return reply.Replace("\"<<modelsplaceholder>>\"", models);
		}

		public IEnumerable<Method> FindMethods(Type serviceType)
		{
			List<Method> methods = new List<Method>();
			//name, op, return
			List<Tuple<string, Operation>> ops = new List<Tuple<string, Operation>>();

			Type[] interfaces = serviceType.GetInterfaces();
			foreach (Type i in interfaces)
			{
				Attribute dc = i.GetCustomAttribute(typeof(ServiceContractAttribute));
				if (dc != null)
				{
					MethodInfo[] mi = i.GetMethods();
					foreach (MethodInfo m in mi)
					{
						Tuple<string, Operation> op = FindWebMethod(m);
						if (op != null) { ops.Add(op); }
					}
				}
			}

			foreach (Tuple<string, Operation> t in ops)
			{
				Method method = (from m in methods
									  where m.path.Equals(t.Item1)
									  select m).FirstOrDefault();
				if (method == null)
				{
					method = new Method() { path = t.Item1 };
					methods.Add(method);
				}
				method.operations.Add(t.Item2);
			}

			return methods;
		}

		public static IEnumerable<MappedService> FindServices()
		{
			foreach (var x in FindStandardServiceEndpoints()) { yield return x; }
			foreach (var x in FindRoutedServiceEndpoints()) { yield return x; }
		}

		//public static IEnumerable<string> FindServices()
		//{
		//	return from h in GetRouteServiceTable().Keys.OfType<string>()
		//			 select h.Substring(2);
		//}

		//find standard configured endpoints
		public static IEnumerable<MappedService> FindStandardServiceEndpoints()
		{
			ServiceModelSectionGroup config = ServiceModelSectionGroup.GetSectionGroup(System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath));
			ServiceHost host = OperationContext.Current.Host as ServiceHost;
			foreach (ServiceElement svc in config.Services.Services)
			{
				foreach (ServiceEndpointElement ep in svc.Endpoints)
				{
					yield return new MappedService()
					{
						Implementation = Type.GetType(svc.Name),
						Path = "/" + ep.Address.OriginalString
					};
				}
			}
		}

		//find any endpoints that are routed via RouteServiceTable
		public static IEnumerable<MappedService> FindRoutedServiceEndpoints()
		{
			Hashtable ht = GetRouteServiceTable();
			foreach (var x in ht.Values)
			{
				Type t = x.GetType();
				PropertyInfo typeProp = t.GetProperty("ServiceType");
				PropertyInfo pathProp = t.GetProperty("VirtualPath");
				string path = pathProp.GetValue(x) as string;
				yield return new MappedService()
				{
					Implementation = Type.GetType(typeProp.GetValue(x) as string),
					Path = path.Substring(path.IndexOf('/'))
				};
			}
		}

		private static Hashtable GetRouteServiceTable()
		{
			if (RouteTable.Routes.Count > 0)
			{
				//take the first ServiceRoute in the RouteTable
				ServiceRoute rd = RouteTable.Routes[0] as ServiceRoute;
				//get the underlying ServiceRouteHandler type, which is normally unavailable, then get the private static fields of that type
				Type t = rd.RouteHandler.GetType();
				FieldInfo[] fields = t.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				//get the routeServiceTable - this is the only private static field on ServiceRouteHandler
				return fields[0].GetValue(rd) as Hashtable;
			}
			else { return new Hashtable(); }
		}

		private Tuple<string, Operation> FindWebMethod(MethodInfo m)
		{
			WebGetAttribute wg = m.GetCustomAttribute<WebGetAttribute>();
			WebInvokeAttribute wi = m.GetCustomAttribute<WebInvokeAttribute>();
			if (wg == null && wi == null) { return null; }

			string httpMethod = (wi == null) ? "GET" : wi.Method;
			string uriTemplate = (wi == null) ? wg.UriTemplate ?? "" : wi.UriTemplate ?? "";
			Regex regex = new Regex(@"^.+\.");
			string description = GetCustomAttributeValue<string, DescriptionAttribute>(m, "Description") ?? "";
			Tuple<string, Operation> tuple = new Tuple<string, Operation>(uriTemplate, new Operation()
			{
				httpMethod = httpMethod,
				nickname = m.Name + httpMethod,
				responseClass = MapSwaggerType(m.ReturnType),
				summary = (description.Length > 0) ? description.Substring(0, description.IndexOf(".")) : "",
				notes = description
			});



			ParameterInfo[] parameters = m.GetParameters();
			foreach (ParameterInfo parameter in parameters)
			{
				Parameter parm = new Parameter();
				parm.name = parameter.Name;
				parm.allowMultiple = false;
				parm.required = true;
				parm.dataType = MapSwaggerType(parameter.ParameterType);
				if (uriTemplate.Contains("{" + parameter.Name + "}")) // need better test for query, etc.
				{
					parm.paramType = "path";
				}
				else
				{
					parm.paramType = "body";
				}
				tuple.Item2.parameters.Add(parm);
			}

			return tuple;
		}

		private List<Type> _MappedTypes;

		private string MapSwaggerType(Type type)
		{
			if (type == typeof(byte)) { return "byte"; }
			if (type == typeof(bool)) { return "boolean"; }
			if (type == typeof(int)) { return "int"; }
			if (type == typeof(long)) { return "long"; }
			if (type == typeof(float)) { return "float"; }
			if (type == typeof(double)) { return "double"; }
			if (type == typeof(string)) { return "string"; }
			if (type == typeof(DateTime)) { return "Date"; }
			else
			{
				if (!_MappedTypes.Contains(type)) { _MappedTypes.Add(type); }
				return type.FullName;
			}
		}

		public static T1 GetCustomAttributeValue<T1, T2>(MethodInfo method, string propertyName)
			where T1 : class
			where T2 : Attribute
		{
			T2 attr = method.GetCustomAttribute<T2>();
			if (attr == null) { return null; }

			PropertyInfo prop = typeof(T2).GetProperty(propertyName);
			if (prop == null || prop.PropertyType != typeof(T1)) { return null; }

			return prop.GetValue(attr) as T1;
		}
	}
}
