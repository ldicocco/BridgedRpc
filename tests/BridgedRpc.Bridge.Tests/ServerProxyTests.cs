using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using BridgedRpc.Bridge;

namespace BridgedRpc.Bridge.Tests
{
	public class ServerProxyTests
	{
		[Fact]
		public void StartRequest()
		{
			var serverName = "testServer";
			var connectionId = Guid.NewGuid().ToString();
			var connectionIdOther = Guid.NewGuid().ToString();
			var methodName = "method01";
			var param0 = 56;
			var param1 = new {test = "test", tail = 67.8};
			var serverProxy = new ServerProxy(serverName, connectionId);

			var id = serverProxy.StartRequest(methodName, param0, param1);
			var pr = serverProxy.GetPendingRequest(id);

			Assert.Equal(methodName, pr.Method);
			Assert.Equal(2, pr.Parameters.Count);
			Assert.Equal(param0, pr.Parameters[0]);
			Assert.Equal(param1, pr.Parameters[1]);
		}
	}
}
