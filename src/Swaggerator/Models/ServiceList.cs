using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Swaggerator.Models
{
	[DataContract]
	internal class ServiceList
	{
		public ServiceList()
		{
			apis = new List<Service>();
		}

		[DataMember]
		public string apiVersion { get; set; }
		[DataMember]
		public string swaggerVersion { get; set; }
		[DataMember]
		public List<Service> apis { get; set; }
	}	
}