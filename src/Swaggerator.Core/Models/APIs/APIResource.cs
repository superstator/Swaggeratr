using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core.Models.APIs
{
    public class APIResource
    {
        public APIResource()
        {
            operations = new List<APIOperation>();
        }


        public string path { get; set; }
        public string description { get; set; }
        public List<APIOperation> operations { get; set; }
    }
}
