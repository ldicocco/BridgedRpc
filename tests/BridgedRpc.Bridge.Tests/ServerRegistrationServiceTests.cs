using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using BridgedRpc.Bridge;

namespace BridgedRpc.Bridge.Tests
{
	public class ServerRegistrationServiceTests
	{
		[Fact]
		public void RegisterServer()
		{
			var registrationService = new ServerRegistrationService();

			var serverName = "testServer";
			var connectionId = Guid.NewGuid().ToString();
			var connectionIdOther = Guid.NewGuid().ToString();

			Assert.Equal(0, registrationService.Count);
			Assert.False(registrationService.IsRegistered(serverName));
			Assert.Null(registrationService.GetConnectionId(serverName));

			var added = registrationService.Register(serverName, connectionId);
			Assert.True(added);
			added = registrationService.Register(serverName, connectionIdOther);
			Assert.False(added, "Added twice");

			Assert.Equal(1, registrationService.Count);
			Assert.Equal(connectionId, registrationService.GetConnectionId(serverName));
		}

		[Fact]
		public void RegisterServers()
		{
			var list = new List<string>();
			var registrationService = new ServerRegistrationService();

			int count = 1000;
			for (int i = 0; i < count; i++)
			{
				var name = "server" + i;
				var connectionId = Guid.NewGuid().ToString();
				registrationService.Register(name, connectionId);
				list.Add(name);
			}
			Assert.Equal(count, registrationService.Count);

			list.ForEach(name => registrationService.Unregister(name));
			Assert.Equal(0, registrationService.Count);
		}


		[Fact]
		public void RemoveConnection()
		{
			var registrationService = new ServerRegistrationService();
			var sameConnectionId = Guid.NewGuid().ToString();

			int sameConnectionCount = 0;
			int count = 1000;
			for (int i = 0; i < count; i++)
			{
				var name = "server" + i;
				var connectionId = Guid.NewGuid().ToString();
				if (i % 3 == 0)
				{
					connectionId = sameConnectionId;
					sameConnectionCount++;
				}
				registrationService.Register(name, connectionId);
			}

			registrationService.RemoveConnection(sameConnectionId);

			Assert.Equal(count - sameConnectionCount, registrationService.Count);
		}

	}
}
