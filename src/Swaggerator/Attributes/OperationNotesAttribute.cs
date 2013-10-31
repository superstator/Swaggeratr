using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Attributes
{
	public class OperationNotesAttribute : Attribute
	{
		public OperationNotesAttribute(string notes)
		{
			Notes = notes;
		}

		public string Notes { get; set; }
	}
}
