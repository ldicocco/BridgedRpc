using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgedRpc.Bridge
{
	public class ServerRegistrationService
	{
		private static object _syncRoot = new Object();
//		private static readonly Lazy<ServerRegistrationService> _instance = new Lazy<ServerRegistrationService>(() => new ServerRegistrationService(), true);
		private readonly ConcurrentDictionary<string, ServerEntry> _registeredServers;

		public ServerRegistrationService()
		{
			_registeredServers = new ConcurrentDictionary<string, ServerEntry>();
		}

/*		public static ServerRegistrationService Instance
		{
			get
			{
				return _instance.Value;
			}
		}*/

		public int Count
		{
			get
			{
				return _registeredServers.Count;
			}
		}

		public bool IsRegistered(string name)
		{
			return _registeredServers.ContainsKey(name);
		}

		public string GetConnectionId(string name)
		{
			ServerEntry se = null;
			if (_registeredServers.TryGetValue(name, out se))
			{
				return se.ConnectionId;
			}
			else
			{
				return null;
			}
		}

		public bool Register(string name, string connectionId)
		{
			var se = new ServerEntry(name, connectionId);
			var newSE = _registeredServers.AddOrUpdate(name, se, (key, oldValue) => oldValue);
			return newSE == se;
		}

		public bool Unregister(string name)
		{
			ServerEntry se = null;
			_registeredServers.TryRemove(name, out se);
			return se != null;
		}

		public void RemoveConnection(string connectionId)
		{
			ServerEntry se = null;
			_registeredServers.Values.Where(i => i.ConnectionId == connectionId).Select(i => i.Name).ToList()
				.ForEach(name => _registeredServers.TryRemove(name, out se));
		}

		public ServerProxy GetServerProxy(string name)
		{
			ServerEntry se = null;
			if (_registeredServers.TryGetValue(name, out se))
			{
				return se.ServerProxy;
			}
			else
			{
				return null;
			}
		}
	}
}
