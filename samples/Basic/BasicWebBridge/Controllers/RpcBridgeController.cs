using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using BridgedRpc.Bridge;
using BridgedRpc.Infrastructure;

namespace BasicWebBridge.Controllers
{
	public class RpcBridgeController : ApiController
	{
		// GET api/<controller>
		public IEnumerable<object> Get(Guid id)
		{
			return BridgerService.Instance.GetPendingRequestParameters(id);
		}

		// PUT api/<controller>/5
		public string Put(Guid id, [FromBody]ServerResponse response)
		{
			BridgerService.Instance.SetPendingRequestResult(id, response);
			return "OK";
		}

	}
}