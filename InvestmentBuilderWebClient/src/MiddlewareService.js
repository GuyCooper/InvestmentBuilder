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

    var onMessage = function (message) {
        for (var i = 0; i < pendingRequestList.length; i++) {
            var request = pendingRequestList[i];
            if (request.pendingId === message.RequestId) {
                //message found. remove it from queue
                pendingRequestList.splice(i, 1);
                request.callback(JSON.parse(message.Payload));
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

    this.EditTrade = function (trade, handler) {
        doCommand("UPDATE_TRADE_REQUEST", "UPDATE_TRADE_RESPONSE", trade, handler);
    };

    this.SellTrade = function (name, handler) {
        doCommand("SELL_TRADE_REQUEST", "SELL_TRADE_RESPONSE", payload, handler)
    };
}