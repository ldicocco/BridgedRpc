using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

using BridgedRpc.Server;

namespace BasicServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var rpcServer = new RpcServer("server01", "http://localhost:58355");
			rpcServer.Connection.Received += (data) => Console.WriteLine(data);
			rpcServer.OnRpc("add", (long a, long b) => a + b);
			rpcServer.Start().Wait();
			rpcServer.Register().Wait();
//			rpcServer.Connection.Send(String.Join("|", "U", "server01")).Wait();

			Console.ReadLine();
		}
	}
}
