using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Swaggerator.Models
{
	[DataContract]
	internal class Operation
	{
		public Operation()
		{
			parameters = new List<Parameter>();
			errorResponses = new List<ResponseCode>();
		}

		[DataMember]
		public string httpMethod { get; set; }
		[DataMember]
		public string nickname { get; set; }
		[DataMember]
		public string type { get; set; }
		[DataMember]
		public string items { get; set; }
		[DataMember]
		public List<Parameter> parameters { get; set; }
		[DataMember]
		public string summary { get; set; }
		[DataMember]
		public string notes { get; set; }
		[DataMember]
		public List<ResponseCode> errorResponses { get; set; }
	}
}