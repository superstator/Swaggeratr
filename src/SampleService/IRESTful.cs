using Swaggerator.Attributes;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using System.Collections.Generic;

namespace SampleService
{
	[ServiceContract]
	public interface IRESTful
	{
		[OperationContract]
		[WebGet(UriTemplate = "/Data/{value}")]
		string GetData(string value);

		[OperationSummary("Does stuff.")]
		[OperationNotes("I mean, it does some really interesting stuff. Stuff like you wouldn't believe.")]
		[ResponseCode(400, "This will be overridden by the implementation")]
		[OperationContract]
		[WebInvoke(UriTemplate = "/data", Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		CompositeType GetDataUsingDataContract(CompositeType composite);

		[OperationContract]
		[WebInvoke(
			UriTemplate="/Data/{value}?val={anothervalue}",
			Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		string PutData(string value, string anothervalue);

		[OperationContract]
		[WebGet(UriTemplate = "/List")]
		CompositeType[] GetList();
	}


	
}
