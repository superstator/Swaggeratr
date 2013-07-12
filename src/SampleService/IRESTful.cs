using Swaggerator.Attributes;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;

namespace SampleService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IRESTful
	{
		[OperationContract]
		[WebGet(UriTemplate = "/Data/{value}")]
		string GetData(string value);

		[OperationSummary("Does stuff.")]
		[ResponseCode(400, "This will be overridden by the implementation")]
		[OperationContract]
		[WebInvoke(UriTemplate = "/data", Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		CompositeType GetDataUsingDataContract(CompositeType composite);

		[OperationContract]
		[WebInvoke(
			UriTemplate="/Data/{value}?val={anothervalue}",
			Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		string PutData(string value, string anothervalue);
	}


	[DataContract]
	public class CompositeType
	{
		bool _BoolValue = true;
		string _StringValue = "Hello ";

		[DataMember]
		[Description("Whatever you do don't set this to")]
		public bool BoolValue
		{
			get { return _BoolValue; }
			set { _BoolValue = value; }
		}

		[DataMember]
		public string StringValue
		{
			get { return _StringValue; }
			set { _StringValue = value; }
		}

		[Hidden]
		[DataMember(EmitDefaultValue = false)]
		public SecretContainer Secret { get; set; }
	}

	[Hidden]
	[DataContract]
	public class SecretContainer
	{
		[DataMember]
		public string SecretData { get; set; }
	}
}
