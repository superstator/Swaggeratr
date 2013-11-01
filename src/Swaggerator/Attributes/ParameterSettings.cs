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
