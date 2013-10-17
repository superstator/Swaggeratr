using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
	}
}
