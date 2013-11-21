BridgedRpc
==========

A .NET library to make RPC requests beyond a firewall

What can it be used for?
-----------

Sometimes it is necessary to get data that are accessible from a machine that is located behind a firewall, possibly from a database.
SignalR can be used to signal a remote process that some data to process are available, which operation is to be done to data, and where to send the result.
Using something like Web API, it is not difficult to implement this, but it requires a lot of boilerplate code.
This library aim at reducing this boilerplate code.

This is still a very early version, which still has many limitations.

How it works.
--------
A Web  application is the Bridge. Clients make HTTP requests to the Bridge, that send the request to the server behind the firewall, gets the result and send it back to the Client.

Sample code.
----

In the Bridge, just reference the BridgedRpc.Bridge.WebApi dll, and configure the Startup Owin file:

	public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			ConfigureAuth(app);
			app.MapBridgedRpc("/BridgedRpc", "/rpc/RpcBridge");
        }
    }

Using Web API with default configuration, the route has to be configured for RPC like that:

	config.Routes.MapHttpRoute(
		name: "DefaultApi",
		routeTemplate: "rpc/{controller}/{action}/{id}",
		defaults: new { id = RouteParameter.Optional }
	);


In the Server, create an RpcServer class, with the Server name, the Bridge Url, and the methods implemented:

	var rpcServer = new RpcServer("server01", "http://localhost:58355");
	rpcServer.Connection.Received += (data) => Console.WriteLine("ECHO: " + data);
	rpcServer.OnRpc("add", (long a, long b) => a + b);
	rpcServer.OnRpc("getFile", (string name) => {
		return System.IO.File.OpenRead(System.IO.Path.GetFileName(name));
	});
	rpcServer.Start().Wait();
	rpcServer.Register().Wait();

In the JavaScript client, you can optionally use a SignalR PermanentConnection.
It is used to be notified of server connection and disconnection, but it is not necessary for sending requests.

	var connection = $.connection('/BridgedRpc');
	var serverProxy = $.rpcServer('server01', connection);
	$('#sendRequest').on('click', function () {
		serverProxy.sendRequest("add", parseInt($('#addP0').val(), 10), parseInt($('#addP1').val(), 10))
			.done(function (res) { $('#response').text(res.Result); });
	});

	$(document).on("click", "#getFile", function (e) {
		serverProxy.requestFile('getFile', $('#fileName').val());
	});

	$('#queryRegistered').on('click', function () {
		serverProxy.queryConnected();
	});

	serverProxy.onConnected(function () { $('#sendRequest, #getFile').prop('disabled', false); });
	serverProxy.onDisconnected(function () { $('#sendRequest, #getFile').prop('disabled', true); });
	connection.start()
		.done(function () { })
		.fail(function () { alert('CONNECTION FAILED'); });


