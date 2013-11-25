using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.AspNet.SignalR.Client;

namespace BridgedRpc.Client
{
	public class RpcServerProxy
	{
		public string ServerName { get; private set; }
		public Uri BaseUri { get; private set; }
		public Connection Connection { get; private set; }
		private HttpClient HttpClient { get; set; }
		private string _connectionPath;
		private string _rpcPath;

		public RpcServerProxy(string serverName, string baseUri, string connectionPath = "/BridgedRpc", string rpcPath = "/rpc/RpcBridge")
		{
			BaseUri = new Uri(baseUri);
			_connectionPath = connectionPath;
			_rpcPath = rpcPath;
			Connection = new Connection(BaseUri.Append(connectionPath).ToString());
			ServerName = serverName;
		}

	}
}
