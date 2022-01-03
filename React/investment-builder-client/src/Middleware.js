//const uuidv1 = require('uuid/v1');

var uid = 1;

const Middleware = function () {
    var ws;
    var callQueue = [];

    this.Connect = function (server, username, password, onopen, onerror, onmessage) {
        ws = new WebSocket(server);

       ws.onopen = function () {
            console.log("onopen");
            sendLoginRequest(username, password);
        };

        ws.onerror = function (error) {
            console.log("error received: " + error);
            onerror(error);
        };

        let processResponse = function(message, success) {
            for(var i = 0; i < callQueue.length; i++) {
                var msg = callQueue[i];
                if(msg.id === message.RequestId) {
                    //message found. remove it from queue
                    callQueue.splice(i, 1);
                    if (success) {
                        msg.succeed(message.RequestId, message.Payload);
                    }
                    else {
                        msg.failed(message.Payload);
                    }
                    break;
                }
            }
        };

        let loginSuccess = function (message, payload) {
            if (onopen != null) {
                onopen(payload);
            }
        };

        let loginFail = function(message) {
            if (onerror != null) {
                onerror(message);
            }
        };

        let sendLoginRequest = function (username, password) {
            var loginRequest = {
                UserName: username,
                Password: password,
                Source: window.location.hostname,
                AppName: "Javascript App",
                Version: "1.0"
            };

            processRequestInternal("LOGIN", "DOLOGIN", 0, loginRequest, null, loginSuccess, loginFail);
        };

        ws.onmessage = function (data) {
            console.log("data received...");
            var message = JSON.parse(data.data);
            if (message !== null && message !== undefined) {
                switch (message.Type) {
                    case 0:
                    case 1:
                        //request or update. just forward to client
                        onmessage(message.RequestId, JSON.parse(message.Payload), message.BinaryPayload);
                        break;
                    case 2:
                        //error response
                        processResponse(message, false);
                        break;
                    case 3:
                        //succes response
                        processResponse(message, true);
                        break;
                    default:
                        console.error("invalid message type: " + message.Type);
                        break;
                }
            }
        };
    };

    this.Disconnect = function () {
        if (ws !== null && ws !== undefined) {
            ws.close();
            ws = null;
        }
    };

    this.IsConnectionClosed = function() {
            return ws === null || ws === undefined || ws.readyState === 2 || ws.readyState === 3; 
    };
    
    var processRequestInternal = function(channel, command, type, data, destination, resolve, reject) {
        let id = "test_" + uid++;
        let payload = {
            RequestId: id,
            Command: command,
            Channel: channel,
            Type: type,
            Payload: JSON.stringify(data),
            DestinationId: destination
        };

        //add al request type messages to call queue. this means that we expect a response
        //for thenm from the server
        if (type === 0) {
            callQueue.push({
                id: id,
                succeed: resolve,
                failed: reject
            });
        }

        //var arrData = new Blob([JSON.stringify(payload)], { type: 'application/json' })
        //var serialisedData = JSON.stringify(payload);
        ws.send(JSON.stringify(payload));
    };

    var processRequest = function(channel, command, type, data, destination) {
        return new Promise(function (resolve, reject) {
            processRequestInternal(channel, command, type, data, destination, resolve, reject);
        });

    };

    this.SubscribeChannel = function (channel) {
        return processRequest(channel, "SUBSCRIBETOCHANNEL", 0, null, null);
    };

    this.SendMessage  = function(channel, message, destination) {
        return processRequest(channel, "SENDMESSAGE", 1, message, destination);
    };

    this.AddListener = function (channel) {
        return processRequest(channel, "ADDLISTENER", 0, null, null);
    };

    this.SendRequest = function(channel, message) {
        return processRequest(channel, "SENDREQUEST", 0, message);
    };

    this.PublishMessage = function(channel, message) {
        return processRequest(channel, "PUBLISHMESSAGE", 1, message);
    };
};

let middleware = new Middleware();

export default middleware;