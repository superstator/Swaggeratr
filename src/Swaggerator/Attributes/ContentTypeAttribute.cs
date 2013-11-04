using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Attributes
{
    public class ContentTypeAttribute : Attribute
    {
        public System.ServiceModel.Web.WebMessageFormat Produces { get; set; }
    }
}
