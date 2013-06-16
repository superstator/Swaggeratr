using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Swaggerator.Models
{
	[DataContract]
	public class Api
	{
		public Api()
		{
			apis = new List<Method>();
			models = "<<modelsplaceholder>>";
		}

		[DataMember]
		public string apiVersion { get; set; }
		[DataMember]
		public string swaggerVersion { get; set; }
		[DataMember]
		public string basePath { get; set; }
		[DataMember]
		public string resourcePath { get; set; }
		[DataMember]
		public List<Method> apis { get; set; }
		[DataMember]
		public string models { get; set; }
	}

	

	
}