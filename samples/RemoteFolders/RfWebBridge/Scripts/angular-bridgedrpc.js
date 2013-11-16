angular.module('ldcBridgedRpc', [])
	.factory("bridgedRpc", function ($rootScope) {
		/*		var connection = $.connection();
				var rlangisHubProxy = connection.createRlangisHubProxy();
				function ServerProxy(rlangisHubProxy, name) {
					var _self = this;
					this.hubProxy = rlangisHubProxy;
					this.name = name;
					this.isConnected = false;
		
					this.hubProxy.onRlangisConnected(function (name, connectionId) {
						$rootScope.$apply(function () { if (name === _self.name) { _self.isConnected = true; } });
					});
		
					this.hubProxy.onRlangisDisconnected(function (name, connectionId) {
						$rootScope.$apply(function () { if (name === _self.name) { _self.isConnected = false; } });
					});
		
					this.checkStatus = function () {
						this.hubProxy.invoke('_isActive', this.name).done(function (res) { $rootScope.$apply(function () { _self.isConnected = res; }); });
					};
		
					this.sendRequest = function () {
						var args = Array.prototype.slice.call(arguments);
						args.splice(0, 0, this.name);
						return this.hubProxy.sendRequest.apply(this.hubProxy, args);
					};
				}
				rlangisHubProxy.onRlangisConnected(function (name, connectionId) { $rootScope.$emit("serverStatus", name, true); });
				rlangisHubProxy.onRlangisDisconnected(function (name, connectionId) { $rootScope.$emit("serverStatus", name, false); });
				var onRlangisNameConnected = function (onConnected) {
					var func = function (name, connectionId) { $rootScope.$apply(onConnected.apply(rlangisHubProxy, arguments)); };
					rlangisHubProxy.onRlangisConnected(func);
				};
				var onRlangisNameDisconnected = function (onDisconnected) {
					var func = function (name, connectionId) { $rootScope.$apply(onDisconnected.apply(rlangisHubProxy, arguments)); };
					rlangisHubProxy.onRlangisDisconnected(func);
				};
				var getServerProxy = function (name) { return new ServerProxy(rlangisHubProxy, name); };
				var start = function () { return connection.start(); };*/
		return {
			/*			getServerProxy: getServerProxy,
						start: start,
						onRlangisNameConnected: onRlangisNameConnected,
						onRlangisNameDisconnected: onRlangisNameDisconnected*/
		};
	});
