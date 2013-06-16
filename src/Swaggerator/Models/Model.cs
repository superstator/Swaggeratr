using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Swaggerator.Models
{
	[DataContract]
	public class Model
	{
		[JsonProperty]
		public string Type { get; set; }
		public JsonSchema Test { get; set; }
		//[DataMember(EmitDefaultValue=false)]
		//[Description("Type name for the complex type.")]
		//public string id { get; set; }

		//[DataMember(EmitDefaultValue = false)]
		//public List<Model> properties { get; set; }

		//public string type { get; set; }
		//public string description { get; set; }
		//public bool required { get; set; }
	}
}