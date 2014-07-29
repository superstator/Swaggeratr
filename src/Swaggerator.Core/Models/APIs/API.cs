using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Models.APIs
{
    public class API
    {
        public string apiVersion { get; set; }
        public string swaggerVersion { get; set; }
        public string basePath { get; set; }
        public string resourcePath { get; set; }
        public List<APIResource> apis { get; set; }
        public List<APIType> models { get; set; }
    }
}
