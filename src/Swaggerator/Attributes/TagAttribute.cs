using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Attributes
{
	/// <summary>
	/// Identifies a class/method/etc for swagger so that visibility can be controlled via configuration.
	/// </summary>
	public class TagAttribute : Attribute
	{
		public TagAttribute(string name)
		{
			TagName = name;
		}

		public string TagName { get; set; }
	}
}
