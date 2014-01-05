﻿/*
 * Copyright (c) 2014 Digimarc Corporation
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
 * CompositeType.cs : Sample class demonstrating a complex model with tagged properties and multiple property types.
 */

using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SampleService
{
	[DataContract]
	public class CompositeType
	{
		public CompositeType()
		{
			BoolValue = true;
			ArrayValue = new List<string>() { "Foo", "Bar", "Baz" };
		}

		[DataMember]
		[Description("Whatever you do don't set this to")]
		public bool BoolValue { get; set; }

		[Hidden]
		[DataMember]
		public string StringValue { get; set; }

		[Tag("InternalUse")]
		[DataMember(EmitDefaultValue = false)]
		public SecretObject Secret { get; set; }

		[DataMember]
		public List<string> ArrayValue { get; set; }

		[DataMember]
		public EnumType? EnumValue { get; set; }

		[DataMember]
		public short ShortValue { get; set; }

		[DataMember]
		[MemberProperties(TypeSizeNote = "10", Description = "Description text")]
		public string StringValueWithLengthRestriction { get; set; }
	}

	public enum EnumType
	{
		Alpha,
		Beta,
		Gamma
	}

	[DataContract]
	public class SecretObject
	{
		[DataMember]
		public string SecretData { get; set; }
	}
}