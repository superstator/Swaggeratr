using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Swaggerator.Test
{
	[TestClass]
	public class ResourceListingTests
	{
		AppDomain _EmptyDomain;
		AppDomain _TestDomain;

		[TestInitialize]
		public void Init()
		{
			_EmptyDomain = AppDomain.CreateDomain("EmptyDomain");
			//_EmptyDomain = AppDomain.CurrentDomain;

			//this doesn't seem to work as expected - if this is run concurrently with the EmptyResourceList test using CurrentDomain, things break
			AppDomainSetup testDomainSetup = new AppDomainSetup();
			testDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			_TestDomain = AppDomain.CreateDomain("SampleServiceDomain",
				null,
				testDomainSetup);
			_TestDomain.Load("SampleService");
		}

		[TestCleanup]
		public void Cleanup()
		{
			AppDomain.Unload(_EmptyDomain);
			AppDomain.Unload(_TestDomain);
			_TestDomain = null;
			_EmptyDomain = null;
		}

		[TestMethod]
		public void CanGetEmptyResourceList()
		{
			Discoverator discoverator = new Discoverator();

			Stream stream = discoverator.GetServices(_EmptyDomain);
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			Assert.IsFalse(string.IsNullOrEmpty(str));

			var obj = JObject.Parse(str);
			Assert.AreEqual("1.2", obj["swaggerVersion"]);
			Assert.AreEqual("No Swaggerized assemblies.", obj["apiVersion"]);
			Assert.IsNull(obj["basePath"]);
			Assert.IsFalse(obj["apis"].HasValues);
		}

		[TestMethod]
		public void CanGetSampleResourceList()
		{
			Discoverator discoverator = new Discoverator();


			Stream stream = discoverator.GetServices(_TestDomain);
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			Assert.IsFalse(string.IsNullOrEmpty(str));

			var obj = JObject.Parse(str);
			Assert.AreEqual("1.2", obj["swaggerVersion"]);
			Assert.AreEqual("1.0.0.0", obj["apiVersion"]);
			Assert.IsTrue(obj["apis"].HasValues);

			var api1 = obj["apis"].Children().FirstOrDefault(o => o["path"].Value<string>().Equals("/rest"));
			Assert.IsNotNull(api1);
			Assert.AreEqual("A RESTful WCF Service", api1["description"]);

			var api2 = obj["apis"].Children().FirstOrDefault(o => o["path"].Value<string>().Equals("/SecondaryService.svc"));
			Assert.IsNotNull(api2);
			Assert.AreEqual("Another endpoint", api2["description"]);
		}
	}
}
