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
			Discoverator discoverator = new Discoverator();

			Assert.IsNotNull(discoverator.HiddenTags);
			Assert.IsTrue(discoverator.HiddenTags.Count() == 1);
			Assert.IsTrue(discoverator.HiddenTags.Contains("Foo"));
			Assert.IsFalse(discoverator.HiddenTags.Contains("Bar"));
		}

		[TestMethod]
		public void CanReadAppConfig()
		{
			var swaggersettings = (Configuration.SwaggerSection)ConfigurationManager.GetSection("swagger");

			Assert.IsNotNull(swaggersettings);
			Assert.IsTrue(swaggersettings.Tags.Count == 2);
			Assert.IsTrue(swaggersettings.Tags.OfType<Configuration.TagElement>().Count(t => t.Name.Equals("Foo")) == 1);
			Assert.IsTrue(swaggersettings.Tags.OfType<Configuration.TagElement>().Count(t => t.Name.Equals("Bar")) == 1);
		}
	}
}
