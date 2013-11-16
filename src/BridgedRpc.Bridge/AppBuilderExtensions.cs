using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace BridgedRpc.Bridge
{
	public static class AppBuilderExtensions
	{
		public static IAppBuilder MapBridgedRpc(this IAppBuilder app, string connectionPath = "/BridgedRpc", string helperPath = "rpc/RpcBridge")
		{
			app.MapSignalR<BridgedRpcConnection>(connectionPath);
			BridgerService.Instance.Initialize(helperPath);
			return app;
		}
	}
}
