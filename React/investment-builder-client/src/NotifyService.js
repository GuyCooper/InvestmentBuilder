
const NotifyService = function () {
    
    //notify service acts as a broker service between the controllers. it contains
    //several lists of listeners that need to be called when a particular view is
    //selected.
    let PortfolioListeners = []; //list of listeners that are invoked when the user clicks on the Portfolio view
    let AddTradeListeners = []; //list of listeners that are invoked when the user clicks on the AddTrade view
    let CashFlowListeners = []; //list of listeners that are invoked when the user clicks on the CashFlow view
    let ReportsListeners = []; //list of listeners that are invoked when the user clicks on the Reports view
    let RedemptionListeners = []; //list of listeners that are invoked when the user clicks on the Redemptions view
    let BuildStatusListeners = []; //this of listeners that should be invoked when the build status changes
    let AccountListeners = []; //this is a list of listeners that should be invoked when the account is changed
    let ConnectionListeners = []; //this list contains a list of handlers that should be called once connection to the middleware is complete
    let DisconnectionListeners = []; //this list contains a list of handlers that should be called when the session is ended

    //listeners for the current view
    let listeners = null;

    let busyStateChangedListener = null;
    //flag to determine if system is busy with request / connecting etc..
    let isBusy = false;
    //store the sessionid of the connection to allow secure file requests to the server
    let sessionid = null;

    this.RegisterBusyStateChangedListener = function (listener) {
        busyStateChangedListener = listener;
    }

    this.UnRegisterBusyStateChangedListener = function() {
        busyStateChangedListener = null;
    }

    this.UpdateBusyState = function (busy) {
        isBusy = busy;
        if (busyStateChangedListener != null) {
            busyStateChangedListener(busy);
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

    this.UnRegisterAccountListener = function (listener) {
        unRegisterCallback(AccountListeners,listener);
    };

    this.UnRegisterPortfolioListener = function (listener) {
        unRegisterCallback(PortfolioListeners,listener);
    };

    this.UnRegisterAddTradeListener = function (listener) {
        unRegisterCallback(AddTradeListeners,listener);
    };

    this.UnRegisterCashFlowListener = function (listener) {
        unRegisterCallback(CashFlowListeners,listener);
    };

    this.UnRegisterReportsListener = function (listener) {
        unRegisterCallback(ReportsListeners,listener);
    };

    this.UnRegisterRedemptionListener = function (listener) {
        unRegisterCallback(RedemptionListeners,listener);
    };

    this.UnRegisterConnectionListener = function (listener) {
        unRegisterCallback(ConnectionListeners,listener);
    };

    this.UnRegisterDisconnectionListener = function (listener) {
        unRegisterCallback(DisconnectionListeners,listener);
    };

    this.UnRegisterBuildStatusListener = function (listener) {
        unRegisterCallback(BuildStatusListeners,listener);
    };

    //helper method for invoking an array of callbacks
    const invokeCallbacks = function (callbacks, parameter) {
        if (callbacks != null) {
            for (let i = 0; i < callbacks.length; i++) {
                callbacks[i](parameter);
            }
        }
    };

    const unRegisterCallback = function(callbacks, callback ) {
        let idx = callbacks.indexOf( callback );
        if(idx > -1) {
            callbacks.splice( idx, 1);
        }
    };

    //helper method for invoking the current listeners
    const invokeListeners = function () {
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
        console.log('InvokeAccountChange called');
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
        isBusy = busy;
    };

    this.GetBusyState = function () {
        return isBusy;
    };

    this.GetSessionID = function () {
        return sessionid;
    };
};


let notifyService = new NotifyService();

export default notifyService;