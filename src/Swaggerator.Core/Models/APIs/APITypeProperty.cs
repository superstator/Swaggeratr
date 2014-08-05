using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Models.APIs
{
    public class APITypeProperty
    {
        public string id { get; set; }
        public string description { get; set; }
        public bool required { get; set; }
        public string type { get; set; }
        public string itemsType { get; set; }
        public List<string> enumValues { get; set; }
    }
}
