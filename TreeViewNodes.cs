using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindFiles
{
	public class TreeViewNodes
	{
		public TreeViewNodes()
		{
			Children = new List<TreeViewNodes>();
		}

		public string Text { get; set; }
		public IList<TreeViewNodes> Children { get; set; }
	}
}
