'use strict'

function MiddlewareService()
{
    var mw = new Middleware();
    var subscriptionList = [];
    var pendingRequestList = [];

    var server_ = '';
    var username_ = '';
    var password_ = '';

    var onConnected = function(message) {
        console.log('connected to middleware. ' + message);
    };

    var onError = function (message) {
        console.log('Error from middleware. ' + message);
    };

    var isNullOrUndefined = function(object) {
        return object === null || object === undefined;
    }

    var onMessage = function (message) {
        for (var i = 0; i < pendingRequestList.length; i++) {
            var request = pendingRequestList[i];
            if (request.pendingId === message.RequestId) {
                //pendingRequestList.splice(i, 1);
                if (isNullOrUndefined(request.callback) === false) {
                    request.callback(JSON.parse(message.Payload));
                }
                break;
            }
        }
    };

    this.Connect = function (server, username, password) {
        server_ = server;
        username_ = username;
        password_ = password;
        return new Promise(function (resolve, reject) {
            mw.Connect(server_, username_, password_, resolve, reject, onMessage);
        });
    };

    var sendRequestToChannel = function (channel, message, handler) {
        var payload = message != null ? JSON.stringify(message) : null;
        mw.SendRequest(channel, payload).then((id) => {
            pendingRequestList.push({"pendingId": id,"callback": handler });
        }, (error) => { console.log("unable to send request to channel " + channel + ". error: " +  error); });
    };

    var doCommand = function (request, response, message, handler) {
        //first check we are subscribing to the response channel

        if (subscriptionList.findIndex(function (val) {
            return val === response;
        }) === -1) {
            mw.SubscribeChannel(response).then(() => {
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

    this.LoadRecentReports = function (handler) {
        doCommand("GET_RECENT_REPORTS_REQUEST", "GET_RECENT_REPORTS_RESPONSE", null, handler);
    };

    this.UpdateAccountDetails = function (account, handle) {
        doCommand("UPDATE_CURRENT_ACCOUNT_REQUEST", "UPDATE_CURRENT_ACCOUNT_RESPONSE", account, handler);
    };

    this.GetAccountDetails = function (accountName, handler) {
        var dto = { AccountName: accountName };
        doCommand("GET_ACCOUNT_DETAILS_REQUEST", "GET_ACCOUNT_DETAILS_RESPONSE", dto, handler);
    };

    this.GetCurrencies = function (handler) {
        doCommand("GET_CURRENCIES_REQUEST", "GET_CURRENCIES_RESPONSE", null, handler);
    };

    this.GetBrokers = function (handler) {
        doCommand("GET_BROKERS_REQUEST", "GET_BROKERS_RESPONSE", null, handler);
    };
}