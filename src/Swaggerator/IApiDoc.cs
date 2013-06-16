using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Swaggerator
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IApiDoc
	{

		[OperationContract]
		[WebGet(UriTemplate = "/api-docs.json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		Models.ServiceList GetServices();

		[OperationContract]
		[WebGet(UriTemplate = "/api-docs.json?svc={serviceName}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		Stream GetServiceDetails(string serviceName);
	}



}
