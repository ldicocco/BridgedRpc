using Microsoft.Owin;
using Owin;

using BridgedRpc.Bridge;

[assembly: OwinStartupAttribute(typeof(BasicWebBridge.Startup))]
namespace BasicWebBridge
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapBridgedRpc("/BridgedRpc", "/rpc/RpcBridge");
		}
	}
}
