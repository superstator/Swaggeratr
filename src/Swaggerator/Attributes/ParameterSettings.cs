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
 * ParameterSettings.cs : Attribute to override default properties of a method parameter.
 */

using System;

namespace Swaggerator.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class ParameterSettings : Attribute
	{
		/// <summary>
		/// Overrides default behavior for a method parameter.
		/// </summary>
		/// <param name="isRequired">Is the parameter required or optional for this method. Defaults to false (not required).</param>
		/// <param name="underlyingType">What is the expected type for the parameter (int, bool, string, etc.)</param>
		/// <param name="description">Descriptive text.</param>
		public ParameterSettings(bool isRequired = false, Type underlyingType = null, string description = null)
		{
			IsRequired = isRequired;
			UnderlyingType = underlyingType;
			Description = description;
		}

		public bool IsRequired { get; set; }
		public Type UnderlyingType { get; set; }
		public string Description { get; set; }
	}
}
