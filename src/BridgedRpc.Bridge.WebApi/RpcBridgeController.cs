﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.IO;

using BridgedRpc.Infrastructure;
using BridgedRpc.Bridge;
using BridgedRpc.Bridge.SlicedFile;
using System.Net.Http.Headers;

namespace BridgedRpc.Bridge.WebApi
{
	public class RpcBridgeController : ApiController
	{
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
			if (res.Result is byte[])
			{
				var ba = res.Result as byte[];
				response.Content = new ByteArrayContent(ba);
				response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
				response.Content.Headers.ContentDisposition.FileName = System.IO.Path.GetFileName(request.Parameters[0] as string);
				response.Content.Headers.ContentLength = ba.Length;
			}
			else if (res.Result is SlicedFileStream)
			{
				var stream = res.Result as SlicedFileStream;
				response.Content = new StreamContent(stream);
				response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
				response.Content.Headers.ContentDisposition.FileName = System.IO.Path.GetFileName(request.Parameters[0] as string);
				response.Content.Headers.ContentLength = stream.Length;
			}
			else
			{
				response.Content = new ObjectContent<ServerResponse>(res, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
			}

			return response;
		}
		private static byte[] ToByteArray(Stream stream)
		{
			stream.Position = 0;
			byte[] buffer = new byte[stream.Length];
			for (int totalBytesCopied = 0; totalBytesCopied < stream.Length; )
				totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
			return buffer;
		}

		// GET api/<controller>
		public IEnumerable<object> GetParameters(Guid id)
		{
			return BridgerService.Instance.GetPendingRequestParameters(id);
		}

		public async Task<string> SetResult(Guid id, HttpRequestMessage reqMsg)
		{
			var response = await reqMsg.Content.ReadAsAsync<ServerResponse>();
			BridgerService.Instance.SetPendingRequestResult(id, response);
			return "OK";
		}

		public async Task<string> SetBlobResult(Guid id, HttpRequestMessage reqMsg)
		{
			var stream = await reqMsg.Content.ReadAsStreamAsync();
			var response = new ServerResponse();
			response.Success = true;
			response.Result = ToByteArray(stream);
			BridgerService.Instance.SetPendingRequestResult(id, response);
			return "OK";
		}

		public async Task<string> SetSlicedResult(Guid id, int index, int total, HttpRequestMessage reqMsg)
		{
			var pr = BridgerService.Instance.GetPendingRequest(id);
			if (pr.Result == null)
			{
				var rootPath = AppDomain.CurrentDomain.BaseDirectory;
				pr.Result = new SlicedFileStream(total, Path.Combine(rootPath, "Tmp"));
			}
			var stream = await reqMsg.Content.ReadAsStreamAsync();
			SlicedFileStream sfs = (SlicedFileStream)pr.Result;
			await sfs.SetSliceAsync(index, stream);
			if (sfs.IsReady())
			{
				var response = new ServerResponse();
				response.Success = true;
				response.Result = sfs;
				BridgerService.Instance.SetPendingRequestResult(id, response);
			}
			return "OK";
		}

	}
}