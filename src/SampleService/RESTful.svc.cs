using Swaggerator.Attributes;
using System;
using System.ComponentModel;

namespace SampleService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	[Swaggerated("/v1/rest", "A RESTful WCF Service")]
	public class RESTful : IRESTful
	{
		public string GetData(string value)
		{
			return string.Format("You entered: {0}", value);
		}

		[Description("A detailed explanation of the fabulous things this method can do for you.")]
		[ResponseCode(400)]
		[ResponseCode(401, "Something weird happened")]
		public CompositeType GetDataUsingDataContract(CompositeType composite)
		{
			if (composite == null)
			{
				throw new ArgumentNullException("composite");
			}
			if (composite.BoolValue)
			{
				composite.StringValue += "Suffix";
				composite.Secret = new SecretContainer { SecretData = "Boo!" };
			}
			return composite;
		}

		public CompositeType GetDataUsingDataContract(string composite)
		{
			return new CompositeType
			{
				BoolValue = false,
				StringValue = "foobar"
			};
		}


		public string PutData(string value, string anothervalue)
		{
			return value + anothervalue;
		}

		public string[] GetList()
		{
			return new string[3]
			{
				"Foo",
				"Bar",
				"Baz"
			};
		}
	}
}
