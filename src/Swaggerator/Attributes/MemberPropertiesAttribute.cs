/*
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
 * Attributes.cs : Attribute used to specify properties of a DataMember.
 */


using System;

namespace Swaggerator.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class MemberPropertiesAttribute : Attribute
	{
		/// <summary>
		/// Attribute used to specify properties of a DataMeber
		/// </summary>
		/// <param name="typeSizeNote">Summary for data type size/range, usally in numbers.</param>
		/// <param name="description">Description for a DataMember</param>
		public MemberPropertiesAttribute(string typeSizeNote = null, string description = null)
		{
			TypeSizeNote = typeSizeNote;
			Description = description;
		}

		public string TypeSizeNote { get; set; }

		public string Description { get; set; }
	}
}
