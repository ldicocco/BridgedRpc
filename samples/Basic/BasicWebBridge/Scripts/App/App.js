
$(function () {
	var connection = $.connection('/BridgedRpc');
	var serverProxy = $.rpcServer('server01', connection);
	$('#sendRequest').on('click', function () {
		serverProxy.sendRequest("add", parseInt($('#addP0').val(), 10), parseInt($('#addP1').val(), 10))
			.done(function (res) { $('#response').text(res.Result); });
	});

	$(document).on("click", "#getFile", function (e) {
		serverProxy.requestFile('getFile', $('#fileName').val());
/*		var request = {
			server: 'server01',
			method: 'getFile',
			parameters: [$('#fileName').val()]
		};
		$.fileDownload('/api/bridge/Download', {
			//			preparingMessageHtml: "We are preparing your report, please wait...",
			//			failMessageHtml: "There was a problem generating your report, please try again.",
			httpMethod: "POST",
			data: 'request=' + JSON.stringify(request)
		});*/
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