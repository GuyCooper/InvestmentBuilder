﻿'use strict'

function NotifyService() {
    
    //notify service acts as a broker service between the controllers. it contains
    //several lists of listeners that need to be called when a particular view is
    //selected.
    var PortfolioListeners = []; //list of listeners that are invoked when the user clicks on the Portfolio view
    var AddTradeListeners = []; //list of listeners that are invoked when the user clicks on the AddTrade view
    var CashFlowListeners = []; //list of listeners that are invoked when the user clicks on the CashFlow view
    var ReportsListeners = []; //list of listeners that are invoked when the user clicks on the Reports view
    var RedemptionListeners = []; //list of listeners that are invoked when the user clicks on the Redemptions view
    var BuildStatusListeners = []; //this of listeners that should be invoked when the build status changes
    var AccountListeners = []; //this is a list of listeners that should be invoked when the account is changed
    var ConnectionListeners = []; //this list contains a list of handlers that should be called once connection to the middleware is complete
    var DisconnectionListeners = []; //this list contains a list of handlers that should be called when the session is ended

    //listeners for the current view
    var listeners = null;

    var busyStateChangedListener = null;
    //flag to determine if system is busy with request / connecting etc..
    var isBusy = false;
    //store the sessionid of the connection to allow secure file requests to the server
    var sessionid = null;

    this.RegisterBusyStateChangedListener = function (listener) {
        busyStateChangedListener = listener;
    }
    this.UpdateBusyState = function (busy, applyScope) {
        isBusy = busy;
        if (busyStateChangedListener != null) {
            busyStateChangedListener(busy, applyScope);
        }
    }
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

    this.RegisterRedemptionListener = function (listener) {
        RedemptionListeners.push(listener);
    };

    this.RegisterConnectionListener = function (listener) {
        ConnectionListeners.push(listener);
    };

    this.RegisterDisconnectionListener = function (listener) {
        DisconnectionListeners.push(listener);
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

    this.InvokeRedemptions = function () {
        listeners = RedemptionListeners;
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

    this.OnConnected = function (connectionID, username) {
        sessionid = connectionID;
        invokeCallbacks(ConnectionListeners, username);
    };

    this.InvokeDisconnectionListeners = function () {
        invokeCallbacks(DisconnectionListeners);
    };

    this.SetBusyState = function (busy) {
        IsBusy = busy;
    };

    this.GetBusyState = function () {
        return IsBusy;
    };

    this.GetSessionID = function () {
        return sessionid;
    };
}
