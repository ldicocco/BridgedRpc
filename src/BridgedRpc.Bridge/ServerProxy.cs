using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgedRpc.Bridge
{
	public class ServerProxy
	{
		public string Name { get; set; }
		public string ConnectionId { get; set; }
		private readonly ConcurrentDictionary<Guid, PendingRequest> _pendingRequests;

		public ServerProxy(string name, string connectionId)
		{
			Name = name;
			ConnectionId = connectionId;
			_pendingRequests = new ConcurrentDictionary<Guid, PendingRequest>();
		}

		public PendingRequest GetPendingRequest(Guid id)
		{
			PendingRequest pr = null;
			_pendingRequests.TryGetValue(id, out pr);
			return pr;
		}

		public IList<object> GetParameters(Guid id)
		{
			PendingRequest pr = null;
			_pendingRequests.TryGetValue(id, out pr);
			return pr.Parameters;
		}

		public Guid StartRequest(string method, params object[] parameters)
		{
			var pr = new PendingRequest(Name, method, parameters);
			pr = _pendingRequests.AddOrUpdate(pr.Id, pr, (key, oldValue) => oldValue);
			return pr.Id;
		}

		public IList<object> SendParametersToServer(Guid id)
		{
			PendingRequest pr = null;
			_pendingRequests.TryGetValue(id, out pr);
			return pr.Parameters;
		}

		public void GotResponseFromServer(Guid id, object result)
		{
		}
	}
}
