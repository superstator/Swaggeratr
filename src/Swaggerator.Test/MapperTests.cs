/*
 * Copyright (c) 2013 Digimarc Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * MapperTests.cs : Tests of model creation via mapper methods.
 */


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
			Assert.AreEqual(true, uno.required);
			Assert.IsTrue(string.IsNullOrEmpty(uno.description));

			Assert.AreEqual("query", dos.paramType);
			Assert.AreEqual(false, dos.required);
			Assert.AreEqual("integer", dos.type);

			Assert.AreEqual("query", tres.paramType);
			Assert.AreEqual(false, tres.required);
			Assert.AreEqual("The third option.", tres.description);
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
		public void CanMapContentTypes()
		{
			var mapper = new Mapper(null);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			var operation = operations.First(o => o.Item1.Equals("/keepitsecret")).Item2;
			Assert.AreEqual(1, operation.produces.Count);
			Assert.AreEqual("application/xml", operation.produces[0]);

			var operation2 = operations.First(o => o.Item1.Equals("/method/test")).Item2;
			Assert.AreEqual(2, operation2.produces.Count);
			Assert.IsTrue(operation2.produces.Contains("application/xml"));
			Assert.IsTrue(operation2.produces.Contains("application/json"));
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
			int Method(
				[Swaggerator.Attributes.ParameterSettings(IsRequired = true)]string uno,
				[Swaggerator.Attributes.ParameterSettings(IsRequired = true)]string dos,
				[Swaggerator.Attributes.ParameterSettings(Description = "The third option.")]string thRee);

			[Swaggerator.Attributes.Tag("SecretThings")]
			[Swaggerator.Attributes.ResponseCode(500, "Just because.")]
			[Swaggerator.Attributes.Produces(ContentType = "application/xml")]
			[System.ServiceModel.Web.WebGet(UriTemplate = "/keepitsecret")]
			int SecretMethod();
		}

		class MapTest : IMapTest
		{
			public int Method(string uno, [Swaggerator.Attributes.ParameterSettings(IsRequired = false, UnderlyingType = typeof(int))]string dos, string tres) { throw new NotImplementedException(); }

			public int SecretMethod() { throw new NotImplementedException(); }
		}
	}
}
