using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Configuration
{
    public class TagElement: ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("visible", DefaultValue = true, IsRequired = true)]
        public bool Visibile
        {
            get { return (bool)this["visible"]; }
            set { this["visible"] = value; }
        }
    }
}
