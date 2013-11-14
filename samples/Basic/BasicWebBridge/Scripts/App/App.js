var BridgedRpc = {};
BridgedRpc.ServerProxy = function (name) {
	this.name = name;
};
BridgedRpc.ServerProxy.prototype.sendRequest = function () {
	var args = $.makeArray(arguments);
	return $.ajax({
		type: 'POST',
		url: '/api/bridge/SendRequest',
		contentType: 'application/json',
		data: JSON.stringify({ server: 'server01', method: args[0], parameters: args.slice(1) })
	});

};

(function ($, window) {
	"use strict";

	var ServerProxy = function (name, connection) {
		var _self = this;
		this.name = name;
		if (connection && connection.received) {
			this.connection = connection;
			this.connection.received(function (msg) {
				var fields = msg.split('|');
				if (fields.length > 1) {
					switch (fields[0]) {
						case 'R':
							if (fields[1] === _self.name && _self.onRegistered) {
								_self.onRegistered();
							}
							break;
						case 'U':
							if (fields[1] === _self.name && _self.onUnregistered) {
								_self.onUnregistered();
							}
							break;
					}
				}

			});
		}
	};
	ServerProxy.prototype.sendRequest = function () {
		var args = $.makeArray(arguments);
		return $.ajax({
			type: 'POST',
			url: '/api/bridge/SendRequest',
			contentType: 'application/json',
			data: JSON.stringify({ server: 'server01', method: args[0], parameters: args.slice(1) })
		});
	};
	ServerProxy.prototype.onRegistered = function (onRegistered) {
		this.onRegistered = onRegistered;
	};
	ServerProxy.prototype.onUnregistered = function (onUnregistered) {
		this.onUnregistered = onUnregistered;
	};
	ServerProxy.prototype.queryRegistered = function () {
		if (this.connection) {
			this.connection.send('?|' + this.name);
		}
	};
	$.rpcServer = function (name, connection) {
		return new ServerProxy(name, connection);
	};
}(window.jQuery, window));

$(function () {
	var connection = $.connection('/BridgedRpc');
	var serverProxy = $.rpcServer('server01', connection);
	$('#sendRequest').on('click', function () {
		serverProxy.sendRequest("add", parseInt($('#addP0').val(), 10), parseInt($('#addP1').val(), 10))
			.done(function (res) { $('#response').text(res.Result); });
	});

	$(document).on("click", "#getFile", function (e) {
		var request = {
			server: 'server01',
			method: 'getFile',
			parameters: [$('#fileName').val()]
		};
		$.fileDownload('/api/bridge/Download', {
			//			preparingMessageHtml: "We are preparing your report, please wait...",
			//			failMessageHtml: "There was a problem generating your report, please try again.",
			httpMethod: "POST",
			data: 'request=' + JSON.stringify(request)
		});
		e.preventDefault(); //otherwise a normal form submit would occur
	});

	$('#queryRegistered').on('click', function () {
		serverProxy.queryRegistered();
	});

	serverProxy.onRegistered(function () { $('#sendRequest, #getFile').prop('disabled', false); });
	serverProxy.onUnregistered(function () { $('#sendRequest, #getFile').prop('disabled', true); });
	connection.start()
		.done(function () { })
		.fail(function () { alert('CONNECTION FAILED'); });
});