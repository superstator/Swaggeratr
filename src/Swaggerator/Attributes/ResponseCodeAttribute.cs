using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseCodeAttribute : Attribute
    {
        public ResponseCodeAttribute(int code, string reason = "")
        {
            Code = code;
            Description = reason;
        }

        public int Code { get; set; }

        public string Description { get; set; }
    }
}
