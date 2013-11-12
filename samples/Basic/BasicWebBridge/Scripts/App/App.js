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

	var ServerProxy = function (name) {
		this.name = name;
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
	$.rpcServer = function (name) {
		return new ServerProxy(name);
	};
}(window.jQuery, window));

$(function () {
	$('#sendRequest').on('click', function () {
		var serverProxy = new BridgedRpc.ServerProxy('server01');
		serverProxy.sendRequest("add", parseInt($('#addP0').val(), 10), parseInt($('#addP1').val(), 10))
			.done(function (res) { $('#response').text(res.Result); });
	});

	$(document).on("click", "#getFile", function (e) {
		var request = {
			server: 'server01',
			method: 'getFile',
			parameters: [$('#fileName').val()]
		}
		$.fileDownload('/api/bridge/Download', {
			//			preparingMessageHtml: "We are preparing your report, please wait...",
			//			failMessageHtml: "There was a problem generating your report, please try again.",
			httpMethod: "POST",
			data: 'request=' + JSON.stringify(request)
		});
		e.preventDefault(); //otherwise a normal form submit would occur
	});
});