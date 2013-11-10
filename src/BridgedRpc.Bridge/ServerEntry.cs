using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgedRpc.Bridge
{
	internal class ServerEntry
	{
		public string Name { get; set; }
		public string ConnectionId { get; set; }

		public ServerEntry(string name, string connectionId)
		{
			Name = name;
			ConnectionId = connectionId;
		}
	}
}
