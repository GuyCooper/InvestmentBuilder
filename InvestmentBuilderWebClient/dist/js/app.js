'use strict'

function YesNoPicker($uibModalInstance, title, description, name) {
    var $item = this;
    $item.Title = title;
    $item.Description = description;
    $item.Name = name;
    $item.ok = function () {
        $uibModalInstance.close({
            name: $item.Name
        });
    };

    $item.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};
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
                //message found. remove it from queue
                pendingRequestList.splice(i, 1);
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

    this.EditTrade = function (trade, handler) {
        doCommand("UPDATE_TRADE_REQUEST", "UPDATE_TRADE_RESPONSE", trade, handler);
    };

    this.SellTrade = function (name, handler) {
        var dto = { TradeName: name };
        doCommand("SELL_TRADE_REQUEST", "SELL_TRADE_RESPONSE", dto, handler)
    };

    this.GetAccountsForUser = function (handler) {
        doCommand("GET_ACCOUNT_NAMES_REQUEST", "GET_ACCOUNT_NAMES_RESPONSE", null, handler);
    }

    this.UpdateAccount = function (account, handler) {
        var dto = { AccountName: account };
        doCommand("UPDATE_ACCOUNT_REQUEST", "UPDATE_ACCOUNT_RESPONSE", dto, handler);
    }
}
"use strict"

function CashFlow($http, $uibModal, $log, $interval, NotifyService) {

    this.cashFlows = null;
    this.receiptParamTypes = null;
    this.paymentParamTypes = null;
    this.progressCount = 0;
    this.section = null;
    this.isBuilding = false;

    var progress;

    this.cashFlowFromDate = new Date();
    this.canBuild = false;

    var onCheckStatus = function (response) {
        this.progressCount = response.data.Progress;

        this.section = response.data.BuildSection;
        if (this.isBuilding == true && response.data.IsBuilding == false) {
            this.isBuilding = response.data.IsBuilding;
            $interval.cancel(progress);
            progress = null;
            //now display a dialog to show any errors during the build
            this.onReportFinished(response.data.Errors);
        }

    }.bind(this);

    var checkStatusRequest = function () {
        $http.get('CheckBuildStatus')
        .then(onCheckStatus);
    };

    var onLoadContents = function (response) {

        if (response.data) {
            this.cashFlows = response.data.CashFlows;
            this.receiptParamTypes = response.data.ReceiptParamTypes;
            this.paymentParamTypes = response.data.PaymentParamTypes;
        }

        if (this.cashFlows.length > 0) {
            this.cashFlowFromDate = new Date(this.cashFlows[this.cashFlows.length - 1].ValuationDate);
            this.canBuild = this.cashFlows[0].CanBuild;
        }

        if (this.isBuilding == true && progress == null) {
            progress = $interval(checkStatusRequest, 1000);
        }

    }.bind(this);

    NotifyService.RegisterCashFlowListener(function () {
        $http.get('CashFlowContents')
        .then(onLoadContents);
    });

    this.onReportFinished = function (errors) {
        var modalInstance = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'ReportCompletionView',
            controller: 'ReportCompletion',
            controllerAs: '$report',
            size: 'lg',
            resolve: {
                errors: function () {
                    return errors;
                }
            }
        });
    };

    this.addTransactionDialog = function (title, paramTypes, updateMethod) {
        var modalInstance = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'AddCashTransaction',
            controller: 'CashTransaction',
            controllerAs: '$transaction',
            size: 'lg',
            resolve: {
                transactionType: function () {
                    return title;
                },
                paramTypes: function () {
                    return paramTypes;
                }
            }
        });

        modalInstance.result.then(function (transaction) {
            //$ctrl.selected = selectedItem;
            //use has clicked ok , we need to update the cash transactions
            $http({
                url: updateMethod,
                method: 'POST',
                params: transaction
            })
            .then(onLoadContents);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }.bind(this);

    this.addReceipt = function () {
        this.addTransactionDialog('receipt', this.receiptParamTypes, 'AddReceiptTransaction');
    }.bind(this);

    this.addPayment = function () {
        this.addTransactionDialog('payment', this.paymentParamTypes, 'AddPaymentTransaction');
    };

    this.deleteTransaction = function (transaction) {
        $http({
            url: "RemoveTransaction",
            method: 'POST',
            params: {
                valuationDate: transaction.ValuationDate,
                transactionDate: transaction.TransactionDate,
                transactionType: transaction.TransactionType,
                parameter: transaction.Parameter
            }
        })
        .then(onLoadContents);
    };

    this.deleteReceipt = function (index) {
        var item = this.cashFlows[0];
        var transaction = item.Receipts[index];
        this.deleteTransaction(transaction);
    }.bind(this);

    this.deletePayment = function (index) {
        var item = this.cashFlows[0];
        var transaction = item.Payments[index];
        this.deleteTransaction(transaction);
    }.bind(this);

    this.reloadContents = function () {
        //var url = 'CashFlowContents/' + this.cashFlowFromDate;
        //$http.get('CashFlowContents/"01-06-2016"')
        $http({
            url: 'CashFlowContents',
            method: 'GET',
            params: { sDateRequestedFrom: this.cashFlowFromDate }
        })
        .then(onLoadContents);
    };

    this.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    this.openDatePicker = function () {
        this.datepicker.opened = true;
    }.bind(this);

    this.format = 'dd-MMMM-yyyy';
    this.altInputFormats = ['M!/d!/yyyy'];

    this.datepicker = {
        opened: false
    };

    this.BuildReport = function () {

        this.isBuilding = true;
        $http({
            url: 'BuildReport',
            method: 'GET',
            params: { sDateRequestedFrom: this.cashFlowFromDate }
        })
        .then(onLoadContents);

    }.bind(this);

    //this.formatTransactionDate = function (transaction) {
    //    var transactionDate = Date.parse(transaction.TransactionDate);
    //    return transactionDate.toDateString();
    //}
}

function CashTransaction($http, $uibModalInstance, transactionType, paramTypes) {
    var $transaction = this;
    if (transactionType == 'receipt') {
        $transaction.title = 'Add Receipt';
    }
    else {
        $transaction.title = 'Add Payment';
    }

    //$transaction.dt = null;
    $transaction.ParamTypes = paramTypes;
    $transaction.SelectedParamType = '';
    if (paramTypes.length > 0) {
        $transaction.SelectedParamType = paramTypes[0];
    }
    $transaction.Parameters = [];
    $transaction.SelectedParameter = '';
    $transaction.Amount = 0;

    $transaction.ok = function () {
        $uibModalInstance.close({
            transactionDate: $transaction.dt,
            paramType: $transaction.SelectedParamType,
            param: $transaction.SelectedParameter,
            amount: $transaction.Amount,
            dateRequestedFrom: this.cashFlowFromDate
        });
    };

    $transaction.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $transaction.today = function () {
        $transaction.dt = new Date();
    };

    $transaction.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    $transaction.openDatePicker = function () {
        $transaction.datepicker.opened = true;
    };

    $transaction.format = 'dd-MMMM-yyyy';
    $transaction.altInputFormats = ['M!/d!/yyyy'];

    $transaction.datepicker = {
        opened: false
    };

    $transaction.today();

    $transaction.onLoadParameters = function (response) {
        if (response.data) {
            $transaction.Parameters = response.data.parameters;
            if ($transaction.Parameters.length > 0) {
                $transaction.SelectedParameter = $transaction.Parameters[0];
            }
        }
    };

    $transaction.changeParamType = function () {
        $http({
            url: 'GetParametersForTransaction',
            method: 'GET',
            params: { ParameterType: $transaction.SelectedParamType }
        })
        .then($transaction.onLoadParameters);
    };

    $transaction.changeParamType();
};

function ReportCompletion($uibModalInstance, errors) {
    var $report = this;
    $report.success = errors == null || errors.length == 0;
    $report.errors = errors;

    $report.ok = function () {
        $uibModalInstance.dismiss('cancel');
    };
};


'use strict'

function CreateTrade($http, NotifyService) {
    var addTrade = this;
    addTrade.today = function () {
        addTrade.dt = new Date();
    };

    addTrade.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    addTrade.openDatePicker = function () {
        addTrade.datepicker.opened = true;
    };

    addTrade.format = 'dd-MMMM-yyyy';
    addTrade.altInputFormats = ['M!/d!/yyyy'];

    var onTradeSubmitted = function (response) {
        if (response.status == 'fail') {
            //trade submission failed. display error messages in error dialog

        }
    };

    addTrade.datepicker = {
        opened: false
    };

    addTrade.submitTrade = function () {
        $http({
            url: 'SubmitTrade',
            method: 'POST',
            params: {
                transactionDate: addTrade.dt,
                name: addTrade.Name,
                symbol: addTrade.Symbol,
                quantity: addTrade.Quantity,
                scalingFactor: addTrade.ScalingFactor,
                currency: addTrade.currency,
                exchange: addTrade.Exchange,
                totalCost: addTrade.totalCost
            }
        })
        .then(onTradeSubmitted);
    };

    addTrade.cancel = function () {

    };

    NotifyService.RegisterAddTradeListener = function () {
        addTrade.today();
    };
}
"use strict"

agGrid.initialiseAgGridWithAngular1(angular);

//var module = angular.module("example", ["agGrid"]);

function Portfolio($scope, $log, $uibModal, NotifyService, MiddlewareService) {

    var columnDefs = [
        { headerName: "Name", field: "Name" },
        { headerName: "Quantity", field: "Quantity" },
        { headerName: "Last Brought", field: "LastBrought", cellFormatter: dateFormatter },
        { headerName: "Avg.Price Paid", field: "AveragePricePaid" },
        { headerName: "Total Cost", field: "TotalCost" },
        { headerName: "Current Price", field: "SharePrice" },
        { headerName: "Net Value", field: "NetSellingValue" },
        { headerName: "Profit/Loss", field: "ProfitLoss" },
        { headerName: "Month Change%", field: "MonthChangeRatio" },
        { headerName: "Options", cellRenderer:editTradeRenderer }
    ];

    this.portfolioData = []; //[{Name:"bob", Quantity:256, TotalCost:354.76 }];

    var onLoadContents = function (data) {

        if (data && data.Portfolio) {
            $scope.gridOptions.api.setRowData(data.Portfolio);
            $scope.gridOptions.api.sizeColumnsToFit();
        }
    }.bind(this);

    function editTradeRenderer() {
        return "<div style=\"display:flex;justify-content:center\"><button style=\"margin-right:2px;\" ng-click=\"editTrade()\">Edit</button><button style=\"margin-left:2px;\" ng-click=\"sellTrade()\">Sell</button></div>";
    }

    function dateFormatter(val) {
        var dateobj = new Date(val.value);
        return dateobj.toLocaleDateString();
    }

    function loadPortfolio() {
        MiddlewareService.LoadPortfolio(onLoadContents);
    };

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: null,// this.portfolioData,
        angularCompileRows: true,
        enableColResize: true,
        enableSorting: true,
        enableFilter: true
    }

    $scope.editTrade = function () {
        console.log("editing trade " + this.data.Name);
        var name = this.data.Name;
        var editModal = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'views/EditTrade.html',
            controller: 'TradeEditor',
            controllerAs: '$trade',
            size: 'lg',
            resolve: {
                name: function () {
                    return name;
                }
            }
        });

        editModal.result.then(function (trade) {
            //$ctrl.selected = selectedItem;
            //use has clicked ok , we need to update the trade
            MiddlewareService.EditTrade(trade, loadPortfolio);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.sellTrade = function () {
        console.log("selling trade " + this.data.Name);
        var title = "Sell Trade";
        var name = this.data.Name;
        var description = "Are you sure you want to sell " + name;
        var sellModal = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'views/YesNoChooser.html',
            controller: 'YesNoController',
            controllerAs: '$item',
            size: 'lg',
            resolve: {
                title: function () {
                    return title;
                },
                description: function () {
                    return description;
                },
                name: function () {
                    return name;
                }
            }
        });
        
        sellModal.result.then(function (param) {
            //$ctrl.selected = selectedItem;
            //use has clicked ok , we need to update the trade
            MiddlewareService.SellTrade(param, loadPortfolio);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    //register as a portfolio listener so  loadPortfolio will be called every time the
    //portfolio tab is clicked
    NotifyService.RegisterPortfolioListener(loadPortfolio);

    //also, we want the portfolio to be loaded on startup so add is as a connectionlistener as well.
    //this means it will be loaded once the connection to the server has been made
    NotifyService.RegisterConnectionListener(loadPortfolio);
};

function TradeEditor($uibModalInstance, name) {
    var $trade = this;

    $trade.Name = name;
    $trade.Actions = ['Buy', 'Sell', 'Change'];
    $trade.SelectedAction = '';
    $trade.Quantity = 0;
    $trade.TotalCost = 0;

    $trade.ok = function () {
        $uibModalInstance.close({
            name: $trade.Name,
            transactionDate: $trade.dt,
            tradeType: $trade.SelectedAction,
            quantity: $trade.Quantity,
            totalcost: $trade.TotalCost        
        });
    };

    $trade.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $trade.today = function () {
        $trade.dt = new Date();
    };

    $trade.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    $trade.openDatePicker = function () {
        $trade.datepicker.opened = true;
    };

    $trade.format = 'dd-MMMM-yyyy';
    $trade.altInputFormats = ['M!/d!/yyyy'];

    $trade.datepicker = {
        opened: false
    };

    $trade.today();
};

//module.controller("exampleCtrl", Portfolio);
//agGrid.initialiseAgGridWithAngular1(angular)


'use strict'

function AccountList($scope, NotifyService, MiddlewareService) {

    var loadAccountList = function() {
        MiddlewareService.GetAccountsForUser(function (data) {
            $scope.Accounts = data.AccountNames;
            if ($scope.Accounts.length > 0) {
                $scope.SelectedAcount = $scope.Accounts[0];
            }
        });
    };

    $scope.SelectedAcount = "";
    $scope.UpdateAccount = function () {
        MiddlewareService.UpdateAccount($scope.SelectedAcount, function (data) {
            NotifyService.InvokePortfolio();
        });
    };

    NotifyService.RegisterConnectionListener(loadAccountList);
}
"use strict"

var module = angular.module("InvestmentRecord", ["ui.bootstrap","agGrid"]);

module.service('NotifyService', NotifyService);

module.service('MiddlewareService', MiddlewareService);

module.controller("PortfolioController", Portfolio);

module.controller('TradeEditor', TradeEditor);

module.controller('AccountListController', AccountList);
//inject everything angular needs here
//angular.module('InvestmentRecord', ['ui.bootstrap']);

//angular.module('InvestmentRecord', ['ui.bootstrap', 'agGrid']);
//angular.module('InvestmentRecord', ['ui.bootstrap'], ['agGrid']);
//angular.module('InvestmentRecord', ['ui.bootstrap']);

module.controller('CashFlowController', CashFlow);

module.controller('CashTransaction', CashTransaction);

module.controller('ReportCompletion', ReportCompletion);

module.controller('YesNoController', YesNoPicker);

module.controller('AddTradeController', CreateTrade);

module.controller('LayoutController', Layout);

//angular.module('InvestmentRecord')
//.controller('PortfolioController', Portfolio);
