using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Swaggerator.Models
{
	[DataContract]
	public class Service
	{
		[DataMember]
		public string path { get; set; }
		[DataMember(EmitDefaultValue = false)]
		public string description { get; set; }
	}
}