using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Newtonsoft.Json;
using Swaggerator.Models;

namespace Swaggerator
{
	[ServiceContract]
	public interface ISwaggerized
	{
		[OperationContract]
		[WebGet(UriTemplate = "swagger/api-docs.json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		ServiceList GetServices();

		[OperationContract]
		[WebGet(UriTemplate = "swagger/api.json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		Stream GetServiceDetails();
	}

	public class Swaggerized : ISwaggerized
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

			ServiceHost host = OperationContext.Current.Host as ServiceHost;

			string filePath = HttpContext.Current.Request.CurrentExecutionFilePath;

			
			IEnumerable<RouteBase> routes = RouteTable.Routes;
			List<MappedService> services = SwaggerMappers.FindServices().ToList();

			//foreach (string s in SwaggerMappers.FindServices())
			//{
			//	Service res = new Service();
			//	res.path = s;

			//	list.apis.Add(res);
			//}

			return list;
		}

		public Stream GetServiceDetails()
		{
			string serviceName = "rest";

			Api api = new Api()
			{
				apiVersion = API_VERSION,
				swaggerVersion = SWAGGER_VERSION
			};

			SwaggerMappers maps = new SwaggerMappers();

			api.basePath = string.Format("{0}://{1}/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, serviceName);
			api.resourcePath = serviceName;

			//Type serviceType = SwaggerMappers.FindServiceType(serviceName);


			//api.apis.AddRange(maps.FindMethods(serviceType));

			//api.models.AddRange(FindModels(serviceType));

			string reply = JsonConvert.SerializeObject(api);

			string replyModels = maps.InsertModelSchemas(reply, api);

			MemoryStream ms = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(replyModels));
			return ms;
		}
	}
}
