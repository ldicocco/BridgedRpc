using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BridgedRpc.Bridge.SlicedFile
{
	public class SliceData
	{
		public string TempFileName {get; set; }
		public Stream Stream {get; set; }
	}
}
