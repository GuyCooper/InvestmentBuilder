'use strict'

function NotifyService(MiddlewareService) {

    var PortfolioListeners = [];
    var AddTradeListeners = [];
    var CashFlowListeners = [];

    //this list contains a list of handlers that should be called once connection to the middleware
    //is complete
    var ConnectionListeners = [];
    //var BuildReportListener = null;
    //var ViewReportListeners = [];

    this.RegisterPortfolioListener = function (listener) {
        PortfolioListeners.push(listener);
        //this.InvokePortfolio(); //as this is the default view ensure it gets loaded
    };

    this.RegisterAddTradeListener = function (listener) {
        AddTradeListeners.push(listener);
    };

    this.RegisterCashFlowListener = function (listener) {
        CashFlowListeners.push(listener);
    };

    this.RegisterConnectionListener = function (listener) {
        ConnectionListeners.push(listener);
    };
    //NotifyService.RegisterViewReportListener = function (listener) {
    //    ViewReportListeners.push(listener);
    //};

    var invokeListeners = function (listeners) {
        for (var i = 0; i < listeners.length; i++) {
            listeners[i]();
        }
    }

    this.InvokePortfolio = function () {
        invokeListeners(PortfolioListeners);
    };

    this.InvokeAddTrade = function () {
        invokeListeners(AddTradeListeners);
    };

    this.InvokeCashFlow = function () {
        invokeListeners(CashFlowListeners);
    };

    //now connect to the middleware server. once conncted inform any listeners that connection is complete
    MiddlewareService.Connect("ws://localhost:8080", "guy@guycooper.plus.com", "rangers").then(function () {
        console.log("connection to middleware succeded!");
        for (var i = 0; i < ConnectionListeners.length; i++) {
            ConnectionListeners[i]();
        }
    },
    function (error) {
        console.log("connection to middleware failed" + error);
    });
    //NotifyService.InvokeViewReport = function () {
    //    invokeListeners(ViewReportListeners);
    //};
}

function Layout($http, $scope, $log, NotifyService) {
    $scope.onPortfolio = function () {
        NotifyService.InvokePortfolio();
        $scope.canBuild = false;
    };

    $scope.onAddTrade = function () {
        NotifyService.InvokeAddTrade();
        $scope.canBuild = false;
    };

    $scope.onCashFlow = function () {
        NotifyService.InvokeCashFlow();
        $scope.canBuild = true;
    };
     
    //$scope.onViewReport = function () {
    //    NotifyService.InvokeViewReport();
    //};

    $scope.BuildReport = function () {
      //TODO  
    }
}