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
			var res = await BridgerService.Instance.SendRequest(request.Server, request.Method, request.Parameters);
			return res;
		}

		public async Task<HttpResponseMessage> Download(HttpRequestMessage reqMsg)
		{
			var req = reqMsg.Content.ReadAsFormDataAsync().Result;
			var r = req["request"];
			var request = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerRequest>(r);
			var res = await BridgerService.Instance.SendRequest(request.Server, request.Method, request.Parameters);
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = new ByteArrayContent(System.Convert.FromBase64String((string)res.Result));
			response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			response.Content.Headers.ContentDisposition.FileName = request.Parameters[0] as string;

			return response;
		}

	}
}