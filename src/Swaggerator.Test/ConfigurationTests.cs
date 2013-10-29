using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Linq;

namespace Swaggerator.Test
{
	[TestClass]
	public class ConfigurationTests
	{
		[TestMethod]
		public void CanGetSettingsFromConfig()
		{
			Discoverator discoverator = new Discoverator("swagger");

			Assert.IsNotNull(discoverator.TagSettings);
			Assert.IsTrue(discoverator.TagSettings.Keys.Count == 2);
			Assert.IsFalse(discoverator.TagSettings["Foo"].Visibile);
			Assert.IsTrue(discoverator.TagSettings["Bar"].Visibile);
		}

		[TestMethod]
		public void CanReadAppConfig()
		{
			var swaggersettings = (Swaggerator.Configuration.SwaggerSection)ConfigurationManager.GetSection("swagger");

			Assert.IsNotNull(swaggersettings);
			Assert.IsTrue(swaggersettings.Tags.Count == 2);
			Assert.IsTrue(swaggersettings.Tags.OfType<Swaggerator.Configuration.TagElement>().Count(t => t.Name.Equals("Foo")) == 1);
			Assert.IsTrue(swaggersettings.Tags.OfType<Swaggerator.Configuration.TagElement>().Count(t => t.Name.Equals("Bar")) == 1);
		}
	}
}
