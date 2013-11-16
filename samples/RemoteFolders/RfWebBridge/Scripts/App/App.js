(function () {

	//angular module
	var myApp = angular.module('App', ['bridgedRpcTreeview']);

	myApp.controller('MainCtrl', function ($scope) {
		var _self = this;
		//		$scope.server01 = bridgedRpc.getServerProxy('server01');
		$scope.isConnected = false;
		var connection = $.connection('/BridgedRpc');
		$scope.server01 = $.rpcServer('server01', connection);
		//		$scope.server01 = bridgedRpc.getRpcServer('server01', connection);

		var applyFunc = function (func) {
			var args = Array.prototype.slice.call(arguments);
			return function () {
				var args = Array.prototype.slice.call(arguments);
				$scope.$apply(function () {
					func.apply(_self, args);
				});
			};
		};

		//test tree model 1
		$scope.roleList1 = [];
/*		$scope.roleList1 = [
			{ Name: "Dir1", IsDirectory:true, Path: "\Dir1" },
			{ Name: "Dir1", IsDirectory:true, Path: "\Dir1" },
			{ Name: "Dir1", IsDirectory:true, Path: "\Dir1" },
			{ Name: "File1", IsDirectory: false, Path: "\File1" },
			{ Name: "File1", IsDirectory: false, Path: "\File1" },
			{ Name: "File1", IsDirectory: false, Path: "\File1" },
			{ Name: "File1", IsDirectory: false, Path: "\File1" },
			{ Name: "File100", IsDirectory: false, Path: "\File100" }
		];*/

		$scope.$watch('tree01.currentNode', function (newObj, oldObj) {
			if ($scope.tree01 && angular.isObject($scope.tree01.currentNode)) {
				console.log('Node Selected!!');
				console.log($scope.tree01.currentNode);
			}
		}, false);

		$scope.addNodes = function () {
		};

		$scope.getRoot = function () {
			$scope.server01.sendRequest("getFileSystemEntries", "Main", "/")
				.done(
					applyFunc(function (data) {
						$scope.roleList1 = data.Result;
					})
				);
		};

		$scope.onExpand = function (node) {
//						alert(node.Name);
			$scope.server01.sendRequest("getFileSystemEntries", "Main", node.Path)
				.done(
					applyFunc(function (data) {

						node.children = data.Result;
					})
				);
		};

		$scope.onDblClick = function (node) {
//			alert("onDblClick" + node.Name);
			$scope.server01.requestFile("getFile", node.Path, "Main");
		};

		$scope.server01.onConnected(function () { $scope.$apply(function () { $scope.isConnected = true; }) });

		connection.start().done(function () { $scope.server01.queryConnected(); });
		//		rlangis.start().done(function () { $scope.server01.checkStatus(); });
	});

})();

