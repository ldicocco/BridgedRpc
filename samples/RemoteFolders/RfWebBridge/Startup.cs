using Microsoft.Owin;
using Owin;

using BridgedRpc.Bridge;

[assembly: OwinStartupAttribute(typeof(RfWebBridge.Startup))]
namespace RfWebBridge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			app.MapBridgedRpc("/BridgedRpc", "/rpc/RpcBridge");
			ConfigureAuth(app);
        }
    }
}
