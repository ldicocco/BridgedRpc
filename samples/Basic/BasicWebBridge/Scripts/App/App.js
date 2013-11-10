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

$(function () {
	$('#sendRequest').on('click', function () {
		var serverProxy = new BridgedRpc.ServerProxy('server01');
		serverProxy.sendRequest("add", parseInt($('#addP0').val(), 10), parseInt($('#addP1').val(), 10))
			.done(function (res) { $('#response').text(res.Result); });
	});
});