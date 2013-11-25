using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.AspNet.SignalR.Client;

using BridgedRpc.Infrastructure;

namespace BridgedRpc.Server
{
	public class RpcServer
	{
		private string _name;
		private Uri _baseUri;
		private string _connectionPath;
		private Connection _connection;
		private HttpClient _httpClient;
		readonly Dictionary<string, Func<IList<object>, object>> _methodsTable = new Dictionary<string, Func<IList<object>, object>>();

		public RpcServer(string name, string baseUri, string connectionPath = "/BridgedRpc")
		{
			_name = name;
			_baseUri = new Uri(baseUri);
			_connectionPath = connectionPath;
			_connection = new Connection(_baseUri.Append(_connectionPath).ToString());
			_connection.Received += OnConnectionReceived;
			_httpClient = new HttpClient();
		}

		public Connection Connection
		{
			get
			{
				return _connection;
			}
		}

		public Task Start()
		{
			return _connection.Start();
		}

		public Task Register()
		{
			return _connection.Send(String.Join("|", "R", _name));
		}

		public Task Unregister()
		{
			return _connection.Send(String.Join("|", "U", _name));
		}

		protected async void OnConnectionReceived(string data)
		{
			try
			{
				var msg = data.Split('|');
				if (msg.Length == 4 && msg[0] == "G")
				{
					await HandleRequest(msg[1], msg[2], msg[3]);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private async Task HandleRequest(string method, string path, string id)
		{
			Console.WriteLine("HandleGetParameters {0} {1} {2}", method, path, id);
			if (!_methodsTable.ContainsKey(method))
			{
				var sr0 = new ServerResponse(null);
				var sc0 = new ObjectContent<ServerResponse>(sr0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
				var res0 = await _httpClient.PutAsync(_baseUri.Append(path, id).ToString(), sc0);
			}
			var parameters = await GetParameters(path, id);
			var sr = new ServerResponse();
			try
			{
				var result = _methodsTable[method](parameters);
				sr.Result = result;
				sr.Success = true;
				Console.WriteLine("Result " + sr.Result);
			}
			catch (Exception ex)
			{
				sr.Success = false;
				sr.ExceptionDescription = ex.Message;
				Console.WriteLine("Exception " + ex.Message);
			}

			if (sr.Result is byte[])
			{
//				await SetBlobResult(path, id, sr.Result as byte[]);
				var stream = new MemoryStream(sr.Result as byte[]);
				await SetSlicedFileStreamResult(path, id, stream);
			}
			else if (sr.Result is FileStream)
			{
				//				await SetFileStreamResult(path, id, sr.Result as FileStream);
				await SetSlicedFileStreamResult(path, id, sr.Result as FileStream);
			}
			else
			{
				await SetResult(path, id, sr);
			}
		}

		private async Task<IList<object>> GetParameters(string path, string id)
		{
			var response = await _httpClient.GetAsync(_baseUri.Append(path, "GetParameters", id).ToString());
			var content = await response.Content.ReadAsStringAsync();
			var parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<object>>(content);
			Console.WriteLine("PARAMETERS");
			Console.WriteLine(content);
			return parameters;
		}

		private async Task SetResult(string path, string id, ServerResponse sr)
		{
			var res = await _httpClient.PostAsJsonAsync(_baseUri.Append(path, "SetResult", id).ToString(), sr);
			Console.WriteLine(res);
			Console.WriteLine(res.Content.ReadAsStringAsync().Result);
		}
/*
		private async Task SetBlobResult(string path, string id, byte[] blob)
		{
			var res = await _httpClient.PostAsJsonAsync(_baseUri.Append(path, "SetBlobResult", id).ToString(), blob);
			Console.WriteLine("SetBlobResult");
			Console.WriteLine(res);
		}
*/
		private async Task SetSlicedFileStreamResult(string path, string id, Stream stream)
		{
			int sliceSize = 500000;
			int totalSlices = (int)Math.Ceiling(stream.Length / (double)sliceSize);
			for (int slice = 0; slice < totalSlices; slice++)
			{
				byte[] buffer = new byte[sliceSize];
				int bytesRead = await stream.ReadAsync(buffer, 0, sliceSize);
				MemoryStream ms = new MemoryStream(buffer, 0, bytesRead);
				var content = new StreamContent(ms);
				var queryString = String.Format("?index={0}&total={1}", slice, totalSlices);
				var url = _baseUri.Append(path, "SetSlicedResult", id, queryString).ToString();
				Console.WriteLine(url);
				var res = await _httpClient.PostAsync(url, content);
				Console.WriteLine(res);
			}
		}

		private async Task SetFileStreamResult(string path, string id, FileStream stream)
		{
			var content = new StreamContent(stream);
			var res = await _httpClient.PostAsync(_baseUri.Append(path, "SetBlobResult", id).ToString(), content);
			Console.WriteLine(res);
		}

		private static TResult JConvert<TResult>(object val)
		{
			if (val is Newtonsoft.Json.Linq.JObject)
			{
				var jo = val as Newtonsoft.Json.Linq.JObject;
				return jo.ToObject<TResult>();
			}
			if (val is Newtonsoft.Json.Linq.JArray)
			{
				var jo = val as Newtonsoft.Json.Linq.JArray;
				return jo.ToObject<TResult>();
			}
			return (TResult)val;
		}

		public void OnRpc(string methodName, Func<object> func)
		{
			//Console.WriteLine("Encapsulated Func<object> " + methodName);
			_methodsTable[methodName] = (list) =>
			{
				return func();
			};
		}

		public void OnRpc<T0>(string methodName, Func<T0, object> func)
		{
			//Console.WriteLine("Encapsulated Func<T0,object> " + methodName);
			_methodsTable[methodName] = (list) =>
			{
				T0 p0 = JConvert<T0>(list[0]);
				return func(p0);
			};
		}

		public void OnRpc<T0, T1>(string methodName, Func<T0, T1, object> func)
		{
			//Console.WriteLine("Encapsulated Func<T0, T1,object> " + methodName);
			_methodsTable[methodName] = (list) =>
			{
				T0 p0 = JConvert<T0>(list[0]);
				T1 p1 = JConvert<T1>(list[1]);
				return func(p0, p1);
			};
		}

		public void OnRpc<T0, T1, T2>(string methodName, Func<T0, T1, T2, object> func)
		{
			//Console.WriteLine("Encapsulated Func<T0, T1, T2,object> " + methodName);
			_methodsTable[methodName] = (list) =>
			{
				T0 p0 = JConvert<T0>(list[0]);
				T1 p1 = JConvert<T1>(list[1]);
				T2 p2 = JConvert<T2>(list[2]);
				return func(p0, p1, p2);
			};
		}

		public void OnRpc<T0, T1, T2, T3>(string methodName, Func<T0, T1, T2, T3, object> func)
		{
			//Console.WriteLine("Encapsulated Func<T0, T1, T2, t3,object> " + methodName);
			_methodsTable[methodName] = (list) =>
			{
				T0 p0 = JConvert<T0>(list[0]);
				T1 p1 = JConvert<T1>(list[1]);
				T2 p2 = JConvert<T2>(list[2]);
				T3 p3 = JConvert<T3>(list[3]);
				return func(p0, p1, p2, p3);
			};
		}
	}
}
