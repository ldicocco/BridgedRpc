using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BridgedRpc.Client;

namespace BasicNetClient
{
	class Program
	{
		static void Main(string[] args)
		{
			var proxy = new RpcServerProxy("server01", "http://localhost:58355");

			Console.WriteLine("Ready");
			Console.ReadLine();
		}
	}
}
