using Swaggerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator
{
    [ServiceContract]
    interface IDiscoverator
    {
        [OperationContract]
        [WebGet(UriTemplate = "api-docs.json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ServiceList GetServices();
    }
}
