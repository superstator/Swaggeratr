using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Swaggerator.Models
{
	[DataContract]
	internal class Method
	{
		public Method()
		{
			operations = new List<Operation>();
		}

		[DataMember]
		public string path { get; set; }
		[DataMember]
		public string description { get; set; }
		[DataMember]
		public List<Operation> operations { get; set; }
	}
}