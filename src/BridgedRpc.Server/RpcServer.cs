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
			var ub = new UriBuilder(_baseUri);
			ub.Path += _connectionPath;
			_connection = new Connection(ub.ToString());
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
			var ub = new UriBuilder(_baseUri);
			ub.Path = path;
			ub.Path += "/" + id;
			Console.WriteLine(ub.ToString());
			if (!_methodsTable.ContainsKey(method))
			{
				var sr0 = new ServerResponse(null);
				var sc0 = new ObjectContent<ServerResponse>(sr0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
				var res0 = await _httpClient.PutAsync(ub.ToString(), sc0);
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
				await SetBlobResult(path, id, sr.Result as byte[]);
			}
			else if (sr.Result is FileStream)
			{
				await SetFileStreamResult(path, id, sr.Result as FileStream);
			}
			else
			{
				await SetResult(path, id, sr);
			}
		}

		private async Task<IList<object>> GetParameters(string path, string id)
		{
			var ub = new UriBuilder(_baseUri);
			ub.Path = path;
			ub.Path += "/GetParameters/" + id;
			Console.WriteLine(ub.ToString());
			var response = await _httpClient.GetAsync(ub.ToString());
			var content = await response.Content.ReadAsStringAsync();
			var parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<object>>(content);
			Console.WriteLine("PARAMETERS");
			Console.WriteLine(content);
			return parameters;
		}

		private async Task SetResult(string path, string id, ServerResponse sr)
		{
			var ub = new UriBuilder(_baseUri);
			ub.Path = path;
			ub.Path += "/SetResult/" + id;
			var res = await _httpClient.PostAsJsonAsync(ub.ToString(), sr);
			Console.WriteLine(res);
			Console.WriteLine(res.Content.ReadAsStringAsync().Result);
		}

		private async Task SetBlobResult(string path, string id, byte[] blob)
		{
			var ub = new UriBuilder(_baseUri);
			ub.Path = path;
			ub.Path += "/SetBlobResult/" + id;
			var res = await _httpClient.PostAsJsonAsync(ub.ToString(), blob);
			Console.WriteLine("SetBlobResult");
			Console.WriteLine(res);
		}

		private async Task SetFileStreamResult(string path, string id, FileStream stream)
		{
			Console.WriteLine("SetBlobResult - Length = {0}", stream.Length);
			var ub = new UriBuilder(_baseUri);
			ub.Path = path;
			ub.Path += "/SetBlobResult/" + id;
			var content = new StreamContent(stream);
			var res = await _httpClient.PostAsync(ub.ToString(), content);
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
