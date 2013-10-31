using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Swaggerator.Test
{
	[TestClass]
	public class MapperTests
	{
		[TestMethod]
		public void CanMapCollectionTypes()
		{
			var typeMap = new Stack<Type>();
			Assert.AreEqual("array", Helpers.MapSwaggerType(typeof(List<string>), typeMap));
			Assert.AreEqual("array", Helpers.MapSwaggerType(typeof(int[]), typeMap));

			Assert.AreEqual(0, typeMap.Count);
		}

		[TestMethod]
		public void CanMapOperation()
		{
			var mapper = new Mapper(new List<string> { "SecretThings" });

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			Assert.AreEqual(1, operations.Count());
			Assert.AreEqual("/method/test", operations.First().Item1);
			var operation = operations.First().Item2;

			Assert.AreEqual(3, operation.parameters.Count);

			var uno = operation.parameters.First(p => p.name.Equals("uno"));
			var dos = operation.parameters.First(p => p.name.Equals("dos"));
			var tres = operation.parameters.First(p => p.name.Equals("tRes"));

			Assert.AreEqual("query", uno.paramType);
			Assert.AreEqual("query", dos.paramType);
			Assert.AreEqual("query", tres.paramType);
		}

		[TestMethod]
		public void CanMapResponseCodes()
		{
			var mapper = new Mapper(null);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			var operation = operations.First(o => o.Item1.Equals("/keepitsecret")).Item2;

			Assert.AreEqual(1, operation.errorResponses.Count());
			Assert.AreEqual("Just because.", operation.errorResponses[0].message);
			Assert.AreEqual(500, operation.errorResponses[0].code);
		}

		[TestMethod]
		public void CanMapNotesAndSummary()
		{
			var mapper = new Mapper(null);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());
			var operation = operations.First(o => o.Item1.Equals("/method/test")).Item2;

			Assert.AreEqual("Short format", operation.summary);
			Assert.AreEqual("Long format", operation.notes);
		}

		interface IMapTest
		{
			[Swaggerator.Attributes.OperationSummary("Short format"), Swaggerator.Attributes.OperationNotes("Long format")]
			[System.ServiceModel.Web.WebGet(UriTemplate = "/method/test?uno={uno}&dos={dos}&tRes={thRee}")]
			int Method(string uno, string dos, string thRee);

			[Swaggerator.Attributes.Tag("SecretThings")]
			[Swaggerator.Attributes.ResponseCode(500, "Just because.")]
			[System.ServiceModel.Web.WebGet(UriTemplate = "/keepitsecret")]
			int SecretMethod();
		}

		class MapTest : IMapTest
		{
			public int Method(string uno, string dos, string tres) { throw new NotImplementedException(); }

			public int SecretMethod() { throw new NotImplementedException(); }
		}
	}
}
