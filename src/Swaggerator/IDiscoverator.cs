using Swaggerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Swaggerator
{
    [ServiceContract]
    interface IDiscoverator
    {
        [OperationContract]
        [WebGet(UriTemplate = "api-docs.json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        Stream GetServices();

        [OperationContract]
        [WebGet(UriTemplate = "api-docs.json/{*service}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        Stream GetServiceDetails(string service);
    }
}
