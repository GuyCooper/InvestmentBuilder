import middleware from "./Middleware.js";

const  MiddlewareService = function() {
    let subscriptionList = [];
    let pendingRequestList = [];

    let server_ = '';
    let username_ = '';
    let password_ = '';

    let onConnected = function(message) {
        console.log('connected to middleware. ' + message);
    };

    let onError = function (message) {
        console.log('Error from middleware. ' + message);
    };

    let isNullOrUndefined = function(object) {
        return object === null || object === undefined;
    }

    let onMessage = function (requestId, payload, binaryPayload) {
        for (var i = 0; i < pendingRequestList.length; i++) {
            var request = pendingRequestList[i];
            if (request.pendingId === requestId) {
                //pendingRequestList.splice(i, 1);
                if (isNullOrUndefined(request.callback) === false) {
                    request.callback(payload, binaryPayload);
                }
                break;
            }
        }
    };

    this.Connect = function (server, username, password) {
        subscriptionList = [];
        pendingRequestList = [];
        server_ = server;
        username_ = username;
        password_ = password;
        return new Promise(function (resolve, reject) {
            middleware.Connect(server_, username_, password_, resolve, reject, onMessage);
        });
    };

    this.Disconnect = function () {
        middleware.Disconnect();
    };

    var sendRequestToChannel = function (channel, message, handler) {
        middleware.SendRequest(channel, message).then((id) => {
            pendingRequestList.push({"pendingId": id,"callback": handler });
        }, (error) => { console.log("unable to send request to channel " + channel + ". error: " +  error); });
    };

    var doCommand = function (request, response, message, handler) {
        //first check we are subscribing to the response channel

        if (subscriptionList.findIndex(function (val) {
            return val === response;
        }) === -1) {
            middleware.SubscribeChannel(response).then(() => {
                subscriptionList.push(response);
                sendRequestToChannel(request, message, handler);
            });
        }
        else {
            sendRequestToChannel(request, message, handler);
        }
    };

    this.LoadPortfolio = function (handler) {
        doCommand("GET_PORTFOLIO_REQUEST", "GET_PORTFOLIO_RESPONSE", null, handler);
    };

    this.UpdateTrade = function (trade, handler) {
        doCommand("UPDATE_TRADE_REQUEST", "UPDATE_TRADE_RESPONSE", trade, handler);
    };

    this.SellTrade = function (name, handler) {
        var dto = { TradeName: name };
        doCommand("SELL_TRADE_REQUEST", "SELL_TRADE_RESPONSE", dto, handler)
    };

    this.GetAccountsForUser = function (handler) {
        doCommand("GET_ACCOUNT_NAMES_REQUEST", "GET_ACCOUNT_NAMES_RESPONSE", null, handler);
    };

    this.UpdateCurrentAccount = function (account, handler) {
        var dto = { AccountName: account };
        doCommand("UPDATE_CURRENT_ACCOUNT_REQUEST", "UPDATE_CURRENT_ACCOUNT_RESPONSE", dto, handler);
    };

    this.CheckBuildStatus = function (handler) {
        doCommand("CHECK_BUILD_STATUS_REQUEST", "CHECK_BUILD_STATUS_RESPONSE", null, handler);
    };

    this.GetCashFlowContents = function (dateFrom, handler) {
        var dto = { DateFrom: dateFrom };
        doCommand("GET_CASH_FLOW_REQUEST", "GET_CASH_FLOW_RESPONSE", dto, handler);
    };

    this.BuildReport = function (handler) {
        doCommand("BUILD_REPORT_REQUEST", "BUILD_REPORT_RESPONSE", null, handler);
    };

    this.AddReceiptTransaction = function (transaction, handler) {
        doCommand("ADD_RECEIPT_TRANSACTION_REQUEST", "ADD_RECEIPT_TRANSACTION_RESPONSE", transaction, handler);
    };

    this.AddPaymentTransaction = function (transaction, handler) {
        doCommand("ADD_PAYMENT_TRANSACTION_REQUEST", "ADD_PAYMENT_TRANSACTION_RESPONSE", transaction, handler);
    };

    this.RemoveTransaction = function (transaction, handler) {
        doCommand("REMOVE_TRANSACTION_REQUEST", "REMOVE_TRANSACTION_RESPONSE", transaction, handler);
    };

    this.GetTransactionParameters = function (type, handler) {
        doCommand("GET_TRANSACTION_PARAMETERS_REQUEST", "GET_TRANSACTION_PARAMETERS_RESPONSE", type, handler);
    };

    this.GetInvestmentSummary = function (handler) {
        doCommand("GET_INVESTMENT_SUMMARY_REQUEST", "GET_INVESTMENT_SUMMARY_RESPONSE", null, handler);
    };

    this.LoadRecentReports = function (request, handler) {
        doCommand("GET_RECENT_REPORTS_REQUEST", "GET_RECENT_REPORTS_RESPONSE", request, handler);
    };

    this.UpdateAccountDetails = function (account, handler) {
        doCommand("UPDATE_ACCOUNT_DETAILS_REQUEST", "UPDATE_ACCOUNT_DETAILS_RESPONSE", account, handler);
    };

    this.CreateAccount = function (account, handler) {
        doCommand("CREATE_ACCOUNT_REQUEST", "CREATE_ACCOUNT_RESPONSE", account, handler);
    };

    this.GetAccountDetails = function (accountName, handler) {
        var dto = { AccountName: accountName };
        doCommand("GET_ACCOUNT_DETAILS_REQUEST", "GET_ACCOUNT_DETAILS_RESPONSE", dto, handler);
    };

    this.GetAccountMembers = function (handler) {
        doCommand("GET_ACCOUNT_MEMBERS_REQUEST", "GET_ACCOUNT_MEMBERS_RESPONSE", null, handler);
    };

    this.GetCurrencies = function (handler) {
        doCommand("GET_CURRENCIES_REQUEST", "GET_CURRENCIES_RESPONSE", null, handler);
    };

    this.GetBrokers = function (handler) {
        doCommand("GET_BROKERS_REQUEST", "GET_BROKERS_RESPONSE", null, handler);
    };

    this.GetLastTransaction = function (handler) {
        doCommand("GET_LAST_TRANSACTION_REQUEST", "GET_LAST_TRANSACTION_RESPONSE", null, handler);
    }

    this.UndoLastTransaction = function (handler) {
        doCommand("UNDO_LAST_TRANSACTION_REQUEST", "UNDO_LAST_TRANSACTION_RESPONSE", null, handler);
    }

    this.GetRedemptions = function (handler) {
        doCommand("GET_REDEMPTIONS_REQUEST", "GET_REDEMPTIONS_RESPONSE", null, handler);
    };

    this.RequestRedemption = function (request, handler) {
        doCommand("REQUEST_REDEMPTION_REQUEST", "REQUEST_REDEMPTION_RESPONSE", request, handler);
    };

    this.LoadReport = function (request, handler) {
        doCommand("LOAD_REPORT_REQUEST", "LOAD_REPORT_RESPONSE", request, handler);
    };

    this.GetPrice = function (request, handler) {
        doCommand("GET_PRICE_REQUEST", "GET_PRICE_RESPONSE", request, handler);
    }
};


let middlewareService = new MiddlewareService();

export default middlewareService;
