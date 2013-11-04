using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgedRpc.Bridge
{
	public class PendingRequest
	{
		public Guid Id { get; set; }
		public string ServerName { get; set; }
		public TaskCompletionSource<object> Tcs { get; set; }
		public DateTime TimeStarted { get; set; }
		public PendingRequestStatus Status { get; set; }
		public string Method { get; set; }
		public IList<object> Parameters { get; set; }
		public object Result { get; set; }

		public PendingRequest(string serverName, string method, params object[] parameters)
		{
			Id = Guid.NewGuid();
			Tcs = new TaskCompletionSource<object>();
			ServerName = serverName;
			TimeStarted = DateTime.Now;
			Status = PendingRequestStatus.Started;
			Method = method;
			Parameters = new List<object>(parameters);
		}
	}
}
