using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace Swaggerator.Test
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void CanReadAppConfig()
        {
            var settings = ConfigurationManager.AppSettings;
        }
    }
}
