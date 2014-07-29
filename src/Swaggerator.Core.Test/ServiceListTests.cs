using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Swaggerator.Core.Test
{
    [TestClass]
    public class ServiceListTests
    {
        [TestMethod]
        public void CanGenerateServiceList()
        {
            var serializer = new Serializer();
            var serviceList = new Swaggerator.Core.Models.Services.ServiceList
            {
                apiVersion = "2",
                swaggerVersion = "1.2"
            };
            serviceList.apis.Add(new Models.Services.Service
            {
                description = "desc",
                path = "one"
            });
            serviceList.apis.Add(new Models.Services.Service
            {
                path = "two"
            });
            var json = serializer.SerializeServiceList(serviceList);

            Assert.AreEqual("{\"apiVersion\":\"2\",\"swaggerVersion\":\"1.2\",\"apis\":[{\"path\":\"one\",\"description\":\"desc\"},{\"path\":\"two\"}]}", json);
        }
    }
}
