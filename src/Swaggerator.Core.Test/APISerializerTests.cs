using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swaggerator.Core.Serializers;
using Moq;
using Swaggerator.Core.Models.APIs;

namespace Swaggerator.Core.Test
{
    [TestClass]
    public class APISerializerTests
    {
        [TestMethod]
        public void CanSerializeApi()
        {
            var mockResourceSerializer = new Mock<IAPIResourceListSerializer>();
            mockResourceSerializer.Setup(m => m.SerializeAPIResources(It.IsAny<List<APIResource>>())).Returns(" {apis} ");

            var mockTypesSerializer = new Mock<IAPITypeListSerializer>();
            mockTypesSerializer.Setup(m => m.SerializeAPITypes(It.IsAny<List<APIType>>())).Returns(" {models} ");

            var ser = new APISerializer(mockResourceSerializer.Object, mockTypesSerializer.Object);

            var api = new Swaggerator.Core.Models.APIs.API
            {
                apis = null,
                apiVersion = "2",
                basePath = "http://localhost",
                models = null,
                resourcePath = "/api",
                swaggerVersion = "1.2"
            };

            var json = ser.SerializeAPI(api);

            Assert.AreEqual("{\"apiVersion\":\"2\",\"swaggerVersion\":\"1.2\",\"basePath\":\"http://localhost\",\"resourcePath\":\"/api\",\"apis\": {apis} ,\"models\": {models} }", json);
        }

        [TestMethod]
        public void CanSerializeTypeList()
        {
            var mockPropertiesSerializer = new Mock<IAPITypePropertiesSerializer>();
            mockPropertiesSerializer.Setup(m => m.SerializeAPITypeProperties(It.IsAny<List<APITypeProperty>>())).Returns(" {props} ");

            var list = new List<APIType>();
            list.Add(new APIType { id = "MyType", });
            list.Add(new APIType { id = "YourType", });

            var ser = new APITypeListSerializer(mockPropertiesSerializer.Object);
            var json = ser.SerializeAPITypes(list);

            Assert.AreEqual("{\"MyType\":{\"id\":\"MyType\",\"properties\": {props} },\"YourType\":{\"id\":\"YourType\",\"properties\": {props} }}", json);
        }

        [TestMethod]
        public void CanSerializeTypeProperties()
        {
            var list = new List<APITypeProperty>();
            list.Add(new APITypeProperty { id = "PropA", description = "Property A", required = true, type = "array", itemsType = "string" });
            list.Add(new APITypeProperty { id = "PropJoe", required = true, type = "string" });

            var ser = new APITypePropertiesSerializer();
            var json = ser.SerializeAPITypeProperties(list);

            Assert.AreEqual("{\"PropA\":{\"description\":\"Property A\",\"type\":\"array\",\"required\":true,\"items\":{\"$ref\":\"string\"}},\"PropJoe\":{\"type\":\"string\",\"required\":true}}", json);
        }

        [TestMethod]
        public void CanSerializeResources()
        {
            var mockOperationsSerializer = new Mock<IAPIOperationsListSerializer>();
            mockOperationsSerializer.Setup(m => m.SerializeOperations(It.IsAny<List<APIOperation>>())).Returns(" {ops} ");

            var list = new List<APIResource>();
            list.Add(new APIResource { path = "/r1", description = "something" });
            list.Add(new APIResource { path = "/r2" });

            var ser = new APIResourceListSerializer(mockOperationsSerializer.Object);

            var json = ser.SerializeAPIResources(list);

            Assert.AreEqual("[{\"path\":\"/r1\",\"description\":\"something\",\"operations\": {ops} },{\"path\":\"/r2\",\"operations\": {ops} }]", json);
        }

        [TestMethod]
        public void CanSerializeOperations()
        {
            var mockErrorSerializer = new Mock<IAPIErrorResponsesListSerializer>();
            mockErrorSerializer.Setup(m => m.SerializeErrorResponses(It.IsAny<List<APIResponseCode>>())).Returns(" {resp} ");

            var mockParametersSerializer = new Mock<IAPIParametersListSerializer>();
            mockParametersSerializer.Setup(m => m.SerializeParameters(It.IsAny<List<APIParameter>>())).Returns(" {params} ");

            var list = new List<APIOperation>();
            list.Add(new APIOperation
            {
                accepts = null,
                httpMethod = "POST",
                itemsType = "string",
                nickname = "MyMethod",
                notes = "test",
                summary = "It does stuff",
                type = "array"
            });

            list.Add(new APIOperation
            {
                accepts = { "application/json", "application/xml" },
                httpMethod = "POST",
                nickname = "MyMethod",
                notes = "test2",
                type = "string"
            });

            var ser = new APIOperationsListSerializer(mockErrorSerializer.Object, mockParametersSerializer.Object);
            var json = ser.SerializeOperations(list);

            Assert.AreEqual("[{\"httpMethod\":\"POST\",\"nickname\":\"MyMethod\",\"type\":\"array\",\"items\":{\"$ref\":\"string\"},\"summary\":\"It does stuff\",\"notes\":\"test\",\"parameters\": {params} ,\"errorResponses\": {resp} },{\"httpMethod\":\"POST\",\"nickname\":\"MyMethod\",\"type\":\"string\",\"notes\":\"test2\",\"parameters\": {params} ,\"errorResponses\": {resp} ,\"accepts\":[\"application/json\",\"application/xml\"]}]", json);
        }

        [TestMethod]
        public void CanSerializeResponses()
        {
            var list = new List<APIResponseCode>();
            list.Add(new APIResponseCode { code = 200, message = "Success" });
            list.Add(new APIResponseCode { code = 500, message = "Crap" });

            var ser = new APIErrorResponsesSerializer();
            var json = ser.SerializeErrorResponses(list);

            Assert.AreEqual("[{\"code\":200,\"message\":\"Success\"},{\"code\":500,\"message\":\"Crap\"}]", json);
        }

        [TestMethod]
        public void CanSerializeParameters()
        {
            var list = new List<APIParameter>();
            list.Add(new APIParameter
            {
                allowableValues = { "a", "b" },
                allowMultiple = false,
                description = "A or B",
                name = "aorb",
                paramType = "body",
                required = false,
                type = "string"
            });
            list.Add(new APIParameter
            {
                name = "key",
                type = "string",
                paramType = "query",
            });

            var ser = new APIParametersListSerializer();
            var json = ser.SerializeParameters(list);

            Assert.AreEqual("[{\"allowableValues\":[\"a\",\"b\"],\"allowMultiple\":false,\"description\":\"A or B\",\"name\":\"aorb\",\"paramType\":\"body\",\"required\":false,\"type\":\"string\"},{\"allowMultiple\":false,\"name\":\"key\",\"paramType\":\"query\",\"required\":false,\"type\":\"string\"}]", json);
        }
    }
}
