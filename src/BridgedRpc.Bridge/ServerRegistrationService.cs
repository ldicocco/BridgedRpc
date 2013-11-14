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
		private readonly ConcurrentDictionary<string, ServerEntry> _registeredServers;

		public ServerRegistrationService()
		{
			_registeredServers = new ConcurrentDictionary<string, ServerEntry>();
		}

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

		internal ServerEntry GetServerEntry(string name)
		{
			ServerEntry se = null;
			_registeredServers.TryGetValue(name, out se);
			return se;
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

		public string Unregister(string name)
		{
			ServerEntry se = null;
			_registeredServers.TryRemove(name, out se);
			if (se != null)
			{
				return se.ConnectionId;
			}
			else
			{
				return null;
			}
		}

		public void RemoveConnection(string connectionId, Action<string> postAction)
		{
			ServerEntry se = null;
			_registeredServers.Values.Where(i => i.ConnectionId == connectionId).Select(i => i.Name).ToList()
				.ForEach(name => { _registeredServers.TryRemove(name, out se); if (postAction != null) postAction(name); });
		}

	}
}
