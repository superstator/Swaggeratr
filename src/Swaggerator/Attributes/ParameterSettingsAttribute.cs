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
 * ParameterSettingsAttribute.cs : Attribute to override default properties of a method parameter.
 */

using System;

namespace Swaggerator.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class ParameterSettingsAttribute : Attribute
	{
		/// <summary>
		/// Overrides default behavior for a method parameter.
		/// </summary>
		/// <param name="isRequired">Is the parameter required or optional for this method. Defaults to false (not required).</param>
		/// <param name="underlyingType">What is the expected type for the parameter (int, bool, string, etc.)</param>
		/// <param name="description">Descriptive text.</param>
		/// <param name="hidden">Should be parameter be hidden for this method? Defaults to false.</param>
		/// <param name="typeSizeNote">Note about parameter data type, such as max length. It will appear as part of data type string.</param>
		public ParameterSettingsAttribute(bool isRequired = false, Type underlyingType = null, string description = null, bool hidden = false, string typeSizeNote = null)
		{
			IsRequired = isRequired;
			UnderlyingType = underlyingType;
			Description = description;
			Hidden = hidden;
			TypeSizeNote = typeSizeNote;
		}

		public bool IsRequired { get; set; }
		public Type UnderlyingType { get; set; }
		public string Description { get; set; }
		public bool Hidden { get; set; }
		public string TypeSizeNote { get; set; }
	}
}
