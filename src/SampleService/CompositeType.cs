using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SampleService
{
	[DataContract]
	public class CompositeType
	{
		public CompositeType()
		{
			BoolValue = true;
			ArrayValue = new List<string>() { "Foo", "Bar", "Baz" };
		}

		[DataMember]
		[Description("Whatever you do don't set this to")]
		public bool BoolValue { get; set; }

		[Hidden]
		[DataMember]
		public string StringValue { get; set; }

		[Tag("InternalUse")]
		[DataMember(EmitDefaultValue = false)]
		public SecretObject Secret { get; set; }

		[DataMember]
		public List<string> ArrayValue { get; set; }

		[DataMember]
		public EnumType? EnumValue { get; set; }
	}

	public enum EnumType
	{
		Alpha,
		Beta,
		Gamma
	}

	[DataContract]
	public class SecretObject
	{
		[DataMember]
		public string SecretData { get; set; }
	}
}