using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Swaggerator.Models
{
	[DataContract]
	internal class Parameter
	{
		//public Parameter()
		//{
		//	allowableValues = new List<string>();
		//}

		[DataMember]
		public string paramType { get; set; }
		[DataMember]
		public string description { get; set; }
		[DataMember]
		public string name { get; set; }
		[DataMember]
		public string type { get; set; }
		[DataMember]
		public bool required { get; set; }
		[DataMember(EmitDefaultValue = false)]
		public List<string> allowableValues { get; set; }
		[DataMember]
		public bool allowMultiple { get; set; }
	}
}