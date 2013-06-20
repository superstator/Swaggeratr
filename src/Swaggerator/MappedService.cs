using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator
{
	internal class MappedService
	{
		public string Path { get; set; }
		public Type Implementation { get; set; }
	}
}
