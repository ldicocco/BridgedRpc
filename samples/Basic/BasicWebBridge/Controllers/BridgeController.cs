using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

using BridgedRpc.Infrastructure;
using BridgedRpc.Bridge;

namespace BasicWebBridge.Controllers
{
	public class BridgeController : ApiController
	{
		public async Task<ServerResponse> SendRequest([FromBody]ServerRequest request)
		{
			var uri = Request.RequestUri;
			return await BridgerService.Instance.SendRequest(request.Server, request.Method, request.Parameters);
		}

		// GET api/<controller>
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/<controller>/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<controller>
		public void Post([FromBody]string value)
		{
		}

		// PUT api/<controller>/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/<controller>/5
		public void Delete(int id)
		{
		}
	}
}