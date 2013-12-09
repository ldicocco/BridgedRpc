using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.SignalR.Client;

using BridgedRpc.Server;

namespace RfServer
{
	class Program
	{
		static void Main(string[] args)
		{
//			string baseUrl = "http://localhost:49826";
			string baseUrl = "http://ldicocco.azurewebsites.net/";
			var rpcServer = new RpcServer("server01", baseUrl);
			rpcServer.Connection.Received += (data) => Console.WriteLine("ECHO: " + data);
			rpcServer.OnRpc("getFileSystemEntries", (string root, string path) =>
			{
				if (!Roots.Instance.ContainsRoot(root) || path.Contains("../"))
				{
					return null;
				}

				var rootPath = Roots.Instance[root];
				var di = new DirectoryInfo(rootPath + path);
				var dirs = di.EnumerateFileSystemInfos().Select(i => new { i.Name, Path = i.FullName.Replace(rootPath, ""), IsDirectory = i.Attributes.HasFlag(FileAttributes.Directory) }).ToArray();
				return dirs.OrderByDescending(i => i.IsDirectory).ThenBy(i => i.Name);
			});
			rpcServer.OnRpc("getFile", (string path, string root) =>
			{
				if (!Roots.Instance.ContainsRoot(root) || path.Contains("../"))
				{
					return null;
				}

				var rootPath = Roots.Instance[root];
				return File.OpenRead(rootPath + path);
			});
			rpcServer.Start().Wait();
			rpcServer.Register().Wait();

			Console.WriteLine("Ready");
			Console.ReadLine();
			rpcServer.Unregister().Wait();
		}
	}
}
