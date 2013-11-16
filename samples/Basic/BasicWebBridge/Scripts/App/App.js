
$(function () {
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
});