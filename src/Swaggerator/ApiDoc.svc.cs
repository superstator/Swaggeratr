using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Swaggerator.Models;

namespace Swaggerator
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ApiDoc : IApiDoc
	{
		string API_VERSION = "Who Knows(tm)";
		const string SWAGGER_VERSION = "1.1";

		public ServiceList GetServices()
		{
			ServiceList list = new ServiceList()
			{
				apiVersion = API_VERSION,
				swaggerVersion = SWAGGER_VERSION
			};

			list.basePath = HttpContext.Current.Request.Url.OriginalString + "?svc=";

			IEnumerable<RouteBase> routes = RouteTable.Routes;
			foreach (string s in SwaggerMappers.FindServices())
			{
				Service res = new Service();
				res.path = s;

				list.apis.Add(res);
			}

			return list;
		}

		public Stream GetServiceDetails(string serviceName)
		{
			Api api = new Api()
			{
				apiVersion = API_VERSION,
				swaggerVersion = SWAGGER_VERSION
			};

			SwaggerMappers maps = new SwaggerMappers();

			api.basePath = string.Format("{0}://{1}/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, serviceName);
			api.resourcePath = serviceName;

			Type serviceType = SwaggerMappers.FindServiceType(serviceName);


			api.apis.AddRange(maps.FindMethods(serviceType));

			//api.models.AddRange(FindModels(serviceType));

			string reply = JsonConvert.SerializeObject(api);

			string replyModels = maps.InsertModelSchemas(reply, api);

			MemoryStream ms = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(replyModels));
			return ms;
		}



		/// <summary>
		/// Create Swaggerator.Models.Model objects for each type in the _MappedTypes collection
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		//private IEnumerable<Model> FindModels(Type serviceType)
		//{
		//	List<Model> models = new List<Model>();
		//	JsonSchemaGenerator gen = new JsonSchemaGenerator();

		//	foreach (string typeName in _MappedTypes.Keys)
		//	{
		//		Model model = new Model();
		//		model.Type = "<<" + typeName + ">>";
		//		model.Test = gen.Generate(_MappedTypes[typeName]);
		//		//model.Test.Title = typeName;
		//		//model.id = typeName;
		//		//model.properties = new List<Model>();
		//		//model.properties.Add(new Model() { type = "string" });
		//		models.Add(model);
		//	}

		//	return models;
		//}


		private class SwaggerMappers
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

			public static IEnumerable<string> FindServices()
			{
				return from h in GetRouteServiceTable().Keys.OfType<string>()
						 select h.Substring(2);
			}

			public static Type FindServiceType(string serviceName)
			{
				Hashtable ht = GetRouteServiceTable();

				Type t = ht["~/" + serviceName].GetType();
				PropertyInfo p = t.GetProperty("ServiceType");
				string serviceTypeName = p.GetValue(ht["~/" + serviceName]) as string;
				Type serviceType = Type.GetType(serviceTypeName);
				return serviceType;
			}

			private static Hashtable GetRouteServiceTable()
			{
				//take the first ServiceRoute in the RouteTable
				ServiceRoute rd = RouteTable.Routes[0] as ServiceRoute;
				//get the underlying ServiceRouteHandler type, which is normally unavailable, then get the private static fields of that type
				Type t = rd.RouteHandler.GetType();
				FieldInfo[] fields = t.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				//get the routeServiceTable - this is the only private static field on ServiceRouteHandler
				return fields[0].GetValue(rd) as Hashtable;
			}

			private Tuple<string, Operation> FindWebMethod(MethodInfo m)
			{
				WebGetAttribute wg = m.GetCustomAttribute<WebGetAttribute>();
				WebInvokeAttribute wi = m.GetCustomAttribute<WebInvokeAttribute>();
				if (wg == null && wi == null) { return null; }

				string httpMethod = (wi == null) ? "GET" : wi.Method;
				string uriTemplate = (wi == null) ? wg.UriTemplate : wi.UriTemplate;
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
}
