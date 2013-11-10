using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgedRpc.Infrastructure
{
	public class ServerRequest
	{
		public string Server { get; set; }
		public string Method { get; set; }
		public IList<object> Parameters { get; set; }

		public ServerRequest(string server, string method, params object[] parameters)
		{
			Server = server;
			Method = method;
			Parameters = new List<object>(parameters);
		}
	}
}
