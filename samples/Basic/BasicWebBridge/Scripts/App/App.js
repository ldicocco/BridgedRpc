$(function () {
	$('#sendRequest').on('click', function () {
		$.ajax({
			type: 'POST',
			url: '/api/bridge/SendRequest',
			contentType:  'application/json',
			data: JSON.stringify({ server: 'server01', method: 'add', parameters: [34, 8]})
//			data: {server: 'server01', method: 'test', parameters: [42, "PROVA"]}
		})
		.done(function (res) { $('#response').text(res.Result); });
	});
});