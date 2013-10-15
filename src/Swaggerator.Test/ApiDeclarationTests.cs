using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Swaggerator.Test
{
	[TestClass]
	public class ApiDeclarationTests
	{
		AppDomain _TestDomain;

		[TestInitialize]
		public void Init()
		{
			AppDomainSetup testDomainSetup = new AppDomainSetup()
			{
				ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
			};

			_TestDomain = AppDomain.CreateDomain("SampleServiceDomain",
				null,
				testDomainSetup);
			_TestDomain.Load("SampleService");
		}

		[TestCleanup]
		public void Cleanup()
		{
			AppDomain.Unload(_TestDomain);
			_TestDomain = null;
		}

		[TestMethod]
		public void CanGetSampleDeclaration()
		{
			Discoverator discoverator = new Discoverator();

			Stream stream = discoverator.GetServiceDetails(_TestDomain, new Uri("http://mockhost"), "v1/rest");
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			Assert.IsFalse(string.IsNullOrEmpty(str));

			var obj = JObject.Parse(str);
			Assert.AreEqual("1.2", obj["swaggerVersion"]);
			Assert.AreEqual("1.0.0.0", obj["apiVersion"]);
			Assert.AreEqual("http://mockhost", obj["basePath"]);
			Assert.AreEqual("/v1/rest", obj["resourcePath"]);
			Assert.IsTrue(obj["apis"].HasValues);

			var api = obj["apis"][0];
			Assert.AreEqual("/v1/rest/Data/{value}", api["path"]);
		}
	}
}
