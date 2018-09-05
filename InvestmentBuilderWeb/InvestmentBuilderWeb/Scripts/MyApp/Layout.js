'use strict'

function NotifyService() {

    var PortfolioListeners = [];
    var AddTradeListeners = [];
    var CashFlowListeners = [];

    //var BuildReportListener = null;
    //var ViewReportListeners = [];

    this.RegisterPortfolioListener = function (listener) {
        PortfolioListeners.push(listener);
        this.InvokePortfolio(); //as this is the default view ensure it gets loaded
    };

    this.RegisterAddTradeListener = function (listener) {
        AddTradeListeners.push(listener);
    };

    this.RegisterCashFlowListener = function (listener) {
        CashFlowListeners.push(listener);
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