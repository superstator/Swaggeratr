using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Configuration
{
	public class SwaggerSection : ConfigurationSection
	{
		[ConfigurationProperty("tags", IsRequired = true)]
		public TagCollection Tags
		{
			get { return (TagCollection)this["tags"]; }
			set { this["tags"] = value; }
		}
	}
}
