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
			rpcServer.Connection.Received += (data) => Console.WriteLine("ECHO: " + data);
			rpcServer.OnRpc("add", (long a, long b) => a + b);
			rpcServer.OnRpc("getFile", (string name) => {
				return System.IO.File.ReadAllBytes(System.IO.Path.GetFileName(name));
			});
			rpcServer.Start().Wait();
			rpcServer.Register().Wait();

			Console.WriteLine("Ready");
			Console.ReadLine();
			rpcServer.Unregister().Wait();
		}
	}
}
