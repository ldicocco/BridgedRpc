using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgedRpc.Infrastructure
{
	public class ServerResponse
	{
		public object Result { get; set; }

		public ServerResponse(object result)
		{
			Result = result;
		}
	}
}
