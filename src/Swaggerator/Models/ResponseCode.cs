using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Models
{
    [DataContract]
    public class ResponseCode
    {
        [DataMember]
        public int code { get; set; }

        [DataMember]
        public string reason { get; set; }
    }
}
