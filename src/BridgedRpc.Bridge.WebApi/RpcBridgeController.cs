using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using BridgedRpc.Bridge;
using BridgedRpc.Infrastructure;

namespace BridgedRpc.Bridge.WebApi
{
	public class RpcBridgeController : ApiController
	{
		[ActionName("SendRequest")]
		public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage reqMsg)
		{
			ServerRequest request = null;
			try
			{
				request = await reqMsg.Content.ReadAsAsync<ServerRequest>();
			}
			catch (System.Net.Http.UnsupportedMediaTypeException)
			{
			}

			if (request == null)
			{
				var req = await reqMsg.Content.ReadAsFormDataAsync();
				var r = req["request"];
				request = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerRequest>(r);
			}
			var res = await BridgerService.Instance.SendRequest(request.Server, request.Method, request.Parameters);
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			if (request.IsFile)
			{
				response.Content = new ByteArrayContent(System.Convert.FromBase64String((string)res.Result));
				response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
				response.Content.Headers.ContentDisposition.FileName = System.IO.Path.GetFileName(request.Parameters[0] as string);
			}
			else
			{
				response.Content = new ObjectContent<ServerResponse>(res, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
			}

			return response;
		}

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