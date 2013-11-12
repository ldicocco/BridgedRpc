using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace BridgedRpc.Bridge
{
	public class BridgedRpcConnection : PersistentConnection
	{
		protected override Task OnConnected(IRequest request, string connectionId)
		{
//			return Task.FromResult(0);
			//			return Connection.Send(connectionId, "Welcome!");
			return base.OnConnected(request, connectionId);
		}

		protected override Task OnDisconnected(IRequest request, string connectionId)
		{
			BridgerService.Instance.RemoveConnection(connectionId);
			return base.OnDisconnected(request, connectionId);
		}

		protected override Task OnReceived(IRequest request, string connectionId, string data)
		{
//			var msg = JsonConvert.DeserializeObject<dynamic>(data);
			var msg = data.Split('|');
			switch (msg[0] as string)
			{
				case "R":
					return HandleRegisterServer(request, connectionId, msg[1]);
				case "U":
					return HandleUnregisterServer(request, connectionId, msg[1]);
				case "?":
					return HandleQueryServer(request, connectionId, msg[1]);
			}
			return Connection.Broadcast(data);
		}

		private Task HandleRegisterServer(IRequest request, string connectionId, string name)
		{
			return BridgerService.Instance.RegisterServer(name, connectionId);
		}

		private Task HandleUnregisterServer(IRequest request, string connectionId, string name)
		{
			return BridgerService.Instance.UnregisterServer(name);
		}

		private Task HandleQueryServer(IRequest request, string connectionId, string name)
		{
			return BridgerService.Instance.QueryServer(name, connectionId);
		}
	}
}