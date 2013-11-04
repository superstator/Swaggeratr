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
 * SerializerTests.cs : Tests for serializer output of methods/models/properties
 */


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using System.Linq;

namespace Swaggerator.Test
{
	[TestClass]
	public class SerializerTests
	{
		[TestMethod]
		public void CanWriteCompositeType()
		{
			Serializer serializer = new Serializer(null);
			string model = serializer.WriteType(typeof(SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(model));

			var obj = JObject.Parse(model);

			Assert.AreEqual("SampleService.CompositeType", obj["id"]);

			var props = obj["properties"] as JObject;
			Assert.IsNotNull(props);
			Assert.IsTrue(props.HasValues);
			Assert.AreEqual(4, props.Count);

			Assert.AreEqual(true, props["BoolValue"]["required"]);
			Assert.AreEqual("array", props["ArrayValue"]["type"]);
			Assert.AreEqual("string", props["EnumValue"]["type"]);
			Assert.AreEqual(false, props["EnumValue"]["required"]);
		}

		[TestMethod]
		public void CanWriteTypeStack()
		{
			Serializer serializer = new Serializer(new [] { "InternalUse" });
			Stack<Type> typeStack = new Stack<Type>();
			typeStack.Push(typeof(SampleService.CompositeType));

			string models = serializer.WriteModels(typeStack);

			var obj = JObject.Parse(HttpUtility.UrlDecode(models));

			Assert.AreEqual(1, obj.Count);
			Assert.IsNotNull(obj["SampleService.CompositeType"]);
		}

		[TestMethod]
		public void CanWriteContainerProperty()
		{
			Serializer serializer = new Serializer(null);

			string model = serializer.WriteType(typeof(SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(model));

			var obj = JObject.Parse(HttpUtility.UrlDecode(model));

			Assert.AreEqual("SampleService.CompositeType", obj["id"].ToString());

			var container = obj["properties"]["ArrayValue"];
			Assert.IsNotNull(container);
			Assert.AreEqual("array", container["type"]);
			Assert.AreEqual("string", container["items"]["$ref"]);

			var enumProperty = obj["properties"]["EnumValue"];
			Assert.IsNotNull(enumProperty);
			Assert.AreEqual("string", enumProperty["type"]);

			var enumValues = enumProperty["enum"] as JArray;
			Assert.AreEqual(3, enumValues.Count);
			Assert.IsTrue(enumValues.Any(v => v.ToString().Equals("Alpha")));
		}

		[TestMethod]
		public void CanHideHiddenTypes()
		{
			//gets the Secret property when it's tag isn't configured
			var serializerAll = new Serializer(null);

			string modelAll = serializerAll.WriteType(typeof(SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(modelAll));

			var objAll = JObject.Parse(modelAll);
			Assert.IsNotNull(objAll["properties"]["ArrayValue"]);
			Assert.IsNotNull(objAll["properties"]["Secret"]);

			//hides it when it is
			var serializer = new Serializer(new [] { "InternalUse" });

			string model = serializer.WriteType(typeof(SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(model));

			var obj = JObject.Parse(model);
			Assert.IsNotNull(obj["properties"]["ArrayValue"]);
			Assert.IsNull(obj["properties"]["Secret"]);
		}

	}
}
