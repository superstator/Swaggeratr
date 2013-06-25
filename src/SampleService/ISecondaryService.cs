using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SampleService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISecondaryService" in both code and config file together.
	[ServiceContract]
	public interface ISecondaryService
	{
		[WebGet(UriTemplate = "/work", ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		void DoWork();
	}
}
