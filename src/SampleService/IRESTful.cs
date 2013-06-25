using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ComponentModel;

namespace SampleService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRESTful
    {
        [OperationContract]
        [WebGet(UriTemplate = "/GetData/{value}")]
        string GetData(string value);

			[OperationSummary("Does stuff.")]
		  [ResponseCode(400,"This will be overridden by the implementation")]
        [OperationContract]
        [WebInvoke(UriTemplate = "/data", Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //CompositeType GetDataUsingDataContract(string composite);
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }


    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
		 [Description("Whatever you do don't set this to")]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
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
