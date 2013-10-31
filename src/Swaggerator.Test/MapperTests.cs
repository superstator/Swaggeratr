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

			var uno = operation.parameters.Where(p => p.name.Equals("uno")).First();
			var dos = operation.parameters.Where(p => p.name.Equals("dos")).First();
			var tres = operation.parameters.Where(p => p.name.Equals("tRes")).First();

			Assert.AreEqual("query", uno.paramType);
			Assert.AreEqual("query", dos.paramType);
			Assert.AreEqual("query", tres.paramType);
		}

		interface IMapTest
		{
			[System.ServiceModel.Web.WebGet(UriTemplate = "/method/test?uno={uno}&dos={dos}&tRes={thRee}")]
			int Method(string uno, string dos, string thRee);

			[Swaggerator.Attributes.Tag("SecretThings")]
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
