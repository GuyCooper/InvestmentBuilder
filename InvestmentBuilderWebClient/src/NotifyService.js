'use strict'

function NotifyService(MiddlewareService) {
    
    //notify service acts as a broker service between the controllers. it contains
    //several lists of listeners that need to be called when a particular view is
    //selected.
    var PortfolioListeners = [];
    var AddTradeListeners = [];
    var CashFlowListeners = [];
    var ReportsListeners = [];
    var BuildStatusListeners = []; //this of listeners that should be invoked when the build status changes
    var AccountListeners = []; //this is a list of listeners that should be invoked when the account is changed
    var ConnectionListeners = []; //this list contains a list of handlers that should be called once connection to the middleware is complete

    //listeners for the current view
    var listeners = null;

    //register callback methods 
    this.RegisterAccountListener = function (listener) {
        AccountListeners.push(listener);
    };

    this.RegisterPortfolioListener = function (listener) {
        PortfolioListeners.push(listener);
    };

    this.RegisterAddTradeListener = function (listener) {
        AddTradeListeners.push(listener);
    };

    this.RegisterCashFlowListener = function (listener) {
        CashFlowListeners.push(listener);
    };

    this.RegisterReportsListener = function (listener) {
        ReportsListeners.push(listener);
    };

    this.RegisterConnectionListener = function (listener) {
        ConnectionListeners.push(listener);
    };

    this.RegisterBuildStatusListener = function (listener) {
        BuildStatusListeners.push(listener);
    };

    //helper method for invoking an array of callbacks
    var invokeCallbacks = function (callbacks, parameter) {
        if (callbacks != null) {
            for (var i = 0; i < callbacks.length; i++) {
                callbacks[i](parameter);
            }
        }
    };

    //helper method for invoking the current listeners
    var invokeListeners = function () {
        invokeCallbacks(listeners);
    }

    //following methods called to invoke the listeners for a particjular view
    this.InvokePortfolio = function () {
        listeners = PortfolioListeners;
        invokeListeners();
    };

    this.InvokeAddTrade = function () {
        listeners = AddTradeListeners;
        invokeListeners();
    };

    this.InvokeCashFlow = function () {
        listeners = CashFlowListeners;
        invokeListeners();
    };

    this.InvokeReports = function () {
        listeners = ReportsListeners;
        invokeListeners();
    };

    //call this method if the account is changed. Calls the exisitng view listener and all the 
    //account listeners
    this.InvokeAccountChange = function () {
        invokeCallbacks(AccountListeners);
        invokeListeners();
    }

    //this method is called when the build status changes and notifies all the buildstatus listeners of the new build state
    this.InvokeBuildStatusChange = function (status) {
        invokeCallbacks(BuildStatusListeners, status);
    }

    //now connect to the middleware server. once conncted inform any connection listeners that connection is complete
    MiddlewareService.Connect("ws://localhost:8080", "guy@guycooper.plus.com", "rangers").then(function () {
        console.log("connection to middleware succeded!");
        invokeCallbacks(ConnectionListeners);
    },
    function (error) {
        console.log("connection to middleware failed" + error);
    });
}
