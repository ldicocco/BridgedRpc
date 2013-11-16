(function () {

	//angular module
	var myApp = angular.module('App', ['bridgedRpcTreeview', 'ldcBridgedRpc']);

	myApp.controller('MainCtrl', function ($scope, bridgedRpc) {
		var _self = this;
//		$scope.server01 = bridgedRpc.getServerProxy('server01');

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
						$scope.roleList1 = data;
					})
				);
		};

		$scope.onExpand = function (node) {
//			alert(node.Name);
			$scope.server01.sendRequest("getFileSystemEntries", "Main", node.Path)
				.done(
					applyFunc(function (data) {

						node.children = data;
					})
				);
		};

//		rlangis.start().done(function () { $scope.server01.checkStatus(); });
	});

})();

