using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

using BridgedRpc.Infrastructure;

namespace BridgedRpc.Bridge
{
	public class BridgerService
	{
		// Singleton instance
		private readonly static Lazy<BridgerService> _instance = new Lazy<BridgerService>(
			() => new BridgerService(GlobalHost.ConnectionManager.GetConnectionContext<BridgedRpcConnection>())
		);
		private readonly ServerRegistrationService _serverRegistration;
		private readonly ConcurrentDictionary<Guid, PendingRequest> _pendingRequests;

		private IPersistentConnectionContext _context;
		private bool _initialized = false;
		public string HelperPath { get; private set; }

		private BridgerService(IPersistentConnectionContext context)
		{
			_context = context;
			_serverRegistration = new ServerRegistrationService();
			_pendingRequests = new ConcurrentDictionary<Guid, PendingRequest>();
		}

		internal void Initialize(string helperPath)
		{
			HelperPath = helperPath;
			_initialized = true;
		}

		public static BridgerService Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		public Task RegisterServer(string name, string connectionId)
		{
			_serverRegistration.Register(name, connectionId);
			return _context.Connection.Broadcast(String.Join("|", "R", name), connectionId);
		}

		public Task UnregisterServer(string name)
		{
			string connectionId = _serverRegistration.Unregister(name);
			if (!String.IsNullOrEmpty(connectionId))
			{
				return _context.Connection.Broadcast(String.Join("|", "U", name), connectionId);
			}
			else
			{
				return Task.FromResult(0);
			}
		}

		public void RemoveConnection(string connectionId)
		{
			_serverRegistration.RemoveConnection(connectionId, (name) => _context.Connection.Broadcast(String.Join("|", "U", name), connectionId));
		}

		public Task QueryServer(string name, string connectionId)
		{
			if (_serverRegistration.IsRegistered(name))
			{
				return _context.Connection.Send(connectionId, (String.Join("|", "R", name)));
			}
			else
			{
				return Task.FromResult(0);
			}
		}

		public async Task<ServerResponse> SendRequest(string serverName, string methodName, IList<object> parameters)
		{
			var serverEntry = _serverRegistration.GetServerEntry(serverName);
			if (serverEntry == null)
			{
				return await Task.FromResult((ServerResponse)null);
			}
			var pr = new PendingRequest(serverName, methodName, parameters);
			pr = _pendingRequests.AddOrUpdate(pr.Id, pr, (key, oldValue) => oldValue);
			await _context.Connection.Send(serverEntry.ConnectionId, String.Join("|", "G", methodName, HelperPath, pr.Id.ToString()));
			return await pr.Tcs.Task;
		}

		public PendingRequest GetPendingRequest(Guid id)
		{
			PendingRequest pr = null;
			_pendingRequests.TryGetValue(id, out pr);
			return pr;
		}

		public IList<object> GetPendingRequestParameters(Guid id)
		{
			PendingRequest pr = null;
			_pendingRequests.TryGetValue(id, out pr);
			return pr.Parameters;
		}

		public void SetPendingRequestResult(Guid id, ServerResponse response)
		{
			PendingRequest pr = null;
			if (_pendingRequests.TryGetValue(id, out pr))
			{
				pr.Tcs.SetResult(response);
			}
		}
	}
}
