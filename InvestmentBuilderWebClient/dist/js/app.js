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

function NotifyService() {
    
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

    var busyStateChangedListener = null;
    //flag to determine if system is busy with request / connecting etc..
    var isBusy = false;
    this.RegisterBusyStateChangedListener = function (listener) {
        busyStateChangedListener = listener;
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

    this.InvokeConnectionListeners = function (username) {
        invokeCallbacks(ConnectionListeners, username);
    };

    this.SetBusyState = function (busy) {
        IsBusy = busy;
    };

    this.GetBusyState = function () {
        return IsBusy;
    };
}

'use strict'

//controller handles management of the build report progress and the view tab
function Layout($scope, $log, $uibModal, $http, NotifyService, MiddlewareService) {
    $scope.ProgressCount = 0;
    $scope.Section = null;
    $scope.IsBuilding = false;
    $scope.CanBuild = false;
    $scope.LoggedIn = false;
    $scope.BuildStatus = "Not Building";
    $scope.UserName = localStorage.getItem('userName');
    $scope.Password = localStorage.getItem('password');
    $scope.IsBusy = false;
    $scope.SaveCredentials = true;
    $scope.LoginFailed = false;

    var servername = ""; //"ws://localhost:8080/MWARE";

    //callback displays the account summary details
    var onLoadAccountSummary = function (response) {
        $scope.AccountName = response.AccountName;
        $scope.ReportingCurrency = response.ReportingCurrency;
        $scope.ValuePerUnit = response.ValuePerUnit;
        $scope.NetAssets = response.NetAssets;

        $scope.BankBalance = response.BankBalance;
        $scope.MonthlyPnL = response.MonthlyPnL;
        $scope.$apply();
    };

    var loadAccountSummary = function () {
        MiddlewareService.GetInvestmentSummary(onLoadAccountSummary);
    }

    var onBuildStatusChanged = function (status) {
        $scope.CanBuild = status;
    };

    var onBuildProgress = function (response) {

        if ($scope.CanBuild == true) {
            $scope.CanBuild = false;
        }

        var buildStatus = response.Status;
        if(buildStatus != undefined && buildStatus != null) {
            $scope.ProgressCount = buildStatus.Progress;

            $scope.Section = buildStatus.BuildSection;
            if ($scope.IsBuilding == true && buildStatus.IsBuilding == false) {
                $scope.IsBuilding = buildStatus.IsBuilding;
                //now display a dialog to show any errors during the build
                onReportFinished(buildStatus.Errors);
            }
            else {
                $scope.IsBuilding = buildStatus.IsBuilding;
            }

            if ($scope.IsBuilding == true) {
                $scope.BuildStatus = "Building Report";
            }
            else {
                $scope.BuildStatus = "Not Building Report";
            }
            
            $scope.$apply();
        }
    };
    
    var onReportFinished = function (errors) {
        var modalInstance = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'Views/ReportCompletion.html',
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

    $scope.onPortfolio = function () {
        NotifyService.InvokePortfolio();
    };

    $scope.onAddTrade = function () {
        NotifyService.InvokeAddTrade();
    };

    $scope.onCashFlow = function () {
        NotifyService.InvokeCashFlow();
    };

    $scope.onReports = function () {
        NotifyService.InvokeReports();
    };

    $scope.buildReport = function () {
        MiddlewareService.BuildReport(onBuildProgress);
    };

    //connect to middleware layer with user supplied username and password
    $scope.doLogin = function () {
        $scope.LoginFailed = false;
        NotifyService.UpdateBusyState(true);
        //once conncted inform any connection listeners that connection is complete
        MiddlewareService.Connect(servername, $scope.UserName, $scope.Password).then(function () {
            console.log("connection to middleware succeded!");
            
            if ($scope.SaveCredentials) {
                localStorage.setItem('userName', $scope.UserName);
                localStorage.setItem('password', $scope.Password);
            }
            else {
                localStorage.removeItem('userName');
                localStorage.removeItem('password');
            }

            NotifyService.UpdateBusyState(false);
            $scope.LoggedIn = true;
            $scope.LoginFailed = false;
            NotifyService.InvokeConnectionListeners($scope.UserName);
        },
        function (error) {
            NotifyService.UpdateBusyState(false);
            $scope.LoginFailed = true;
            $scope.$apply();
            console.log("connection to middleware failed" + error);
        });
    }

    //method updates the isBusy state
    var busyStateChanged = function (busy) {
        $scope.IsBusy = busy;
    };

    //load the config
    $http.get("./config.json").then(function (data) {
        servername = data.data.url;
        console.log('config loaded. url: ' + servername);
        //register handler to be invoked every time the isBusy state is changed
        NotifyService.RegisterBusyStateChangedListener(busyStateChanged);
        //ensure this view is reloaded on connection
        NotifyService.RegisterConnectionListener(loadAccountSummary);
        //ensure this view is reloaded if the account is changed
        NotifyService.RegisterAccountListener(loadAccountSummary);
        //ensure this view is updated when the build status changes
        NotifyService.RegisterBuildStatusListener(onBuildStatusChanged);

    },function (error) {
        alert("unable to load config file: " + error);
    });
};

//controller to handle the report completion view
function ReportCompletion($uibModalInstance, errors) {
    var $report = this;
    $report.success = errors == null || errors.length == 0;
    $report.errors = errors;

    $report.ok = function () {
        $uibModalInstance.dismiss('cancel');
    };
};

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

    this.GetCurrencies = function (handler) {
        doCommand("GET_CURRENCIES_REQUEST", "GET_CURRENCIES_RESPONSE", null, handler);
    };

    this.GetBrokers = function (handler) {
        doCommand("GET_BROKERS_REQUEST", "GET_BROKERS_RESPONSE", null, handler);
    };
}
"use strict"

function CashFlow($scope, $uibModal, $log, NotifyService, MiddlewareService) {

    $scope.cashFlows = [];
    this.receiptParamTypes = null;
    this.paymentParamTypes = null;

    this.cashFlowFromDate = new Date();

    var onLoadContents = function (response) {

        if (response) {
            $scope.cashFlows = response.CashFlows;
            this.receiptParamTypes = response.ReceiptParamTypes;
            this.paymentParamTypes = response.PaymentParamTypes;
        }

        if ($scope.cashFlows.length > 0) {
            this.cashFlowFromDate = new Date($scope.cashFlows[$scope.cashFlows.length - 1].ValuationDate);
            NotifyService.InvokeBuildStatusChange($scope.cashFlows[0].CanBuild);
        }

        $scope.$apply();

    }.bind(this);

    NotifyService.RegisterCashFlowListener(function () {
        MiddlewareService.GetCashFlowContents(null, onLoadContents);
    });

    this.addTransactionDialog = function (title, paramTypes, dateFrom, updateMethod) {
        var modalInstance = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'views/AddTransaction.html',
            controller: 'CashTransaction',
            controllerAs: '$transaction',
            size: 'lg',
            resolve: {
                transactionType: function () {
                    return title;
                },
                paramTypes: function () {
                    return paramTypes;
                },
                dateFrom: function () {
                    return dateFrom;
                }
            }
        });

        modalInstance.result.then(function (transaction) {
            //$ctrl.selected = selectedItem;
            //use has clicked ok , we need to update the cash transactions
            updateMethod(transaction);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }.bind(this);

    this.addReceipt = function () {
        this.addTransactionDialog('receipt', this.receiptParamTypes, this.cashFlowFromDate, function (transaction) { MiddlewareService.AddReceiptTransaction(transaction, onLoadContents); });
    }.bind(this);

    this.addPayment = function () {
        this.addTransactionDialog('payment', this.paymentParamTypes, this.cashFlowFromDate, function (transaction) { MiddlewareService.AddPaymentTransaction(transaction, onLoadContents); });
    };

    this.deleteTransaction = function (transaction) {
        MiddlewareService.RemoveTransaction({
            valuationDate: transaction.ValuationDate,
            transactionDate: transaction.TransactionDate,
            transactionType: transaction.TransactionType,
            parameter: transaction.Parameter
        }, onLoadContents);
    };

    this.deleteReceipt = function (index) {
        var item = $scope.cashFlows[0];
        var transaction = item.Receipts[index];
        this.deleteTransaction(transaction);
    }.bind(this);

    this.deletePayment = function (index) {
        var item = $scope.cashFlows[0];
        var transaction = item.Payments[index];
        this.deleteTransaction(transaction);
    }.bind(this);

    this.reloadContents = function () {
        //var url = 'CashFlowContents/' + this.cashFlowFromDate;
        MiddlewareService.GetCashFlowContents(this.cashFlowFromDate, onLoadContents);
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

    //this.formatTransactionDate = function (transaction) {
    //    var transactionDate = Date.parse(transaction.TransactionDate);
    //    return transactionDate.toDateString();
    //}
}

function CashTransaction($scope, $uibModalInstance, transactionType, paramTypes, dateFrom, MiddlewareService) {
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
    $scope.Parameters = [];
    $transaction.SelectedParameter = '';
    $transaction.Amount = 0;

    $transaction.ok = function () {
        var sendParams = [];
        if ($transaction.SelectedParameter == "ALL") {
            sendParams = $scope.Parameters.slice(0, $scope.Parameters.length - 1);
        }
        else {
            sendParams.push($transaction.SelectedParameter)
        }

        $uibModalInstance.close({
            TransactionDate: $transaction.dt,
            ParamType: $transaction.SelectedParamType,
            Parameter: sendParams,
            Amount: $transaction.Amount,
            DateRequestedFrom : dateFrom
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
        if (response) {
            $scope.Parameters = response.Parameters;
            if ($scope.Parameters.length > 0) {
                $transaction.SelectedParameter = $scope.Parameters[0];
            }
            $scope.$apply();
        }
    };

    $transaction.changeParamType = function () {
        
    MiddlewareService.GetTransactionParameters(
        { ParameterType: $transaction.SelectedParamType }, $transaction.onLoadParameters);
    };

    $transaction.changeParamType();
};


'use strict'

function CreateTrade(NotifyService, MiddlewareService) {
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
        MiddlewareService.UpdateTrade({
            TransactionDate: addTrade.dt,
            ItemName: addTrade.Name,
            Symbol: addTrade.Symbol,
            Quantity: addTrade.Quantity,
            ScalingFactor: addTrade.ScalingFactor,
            Currency: addTrade.currency,
            Exchange: addTrade.Exchange,
            TotalCost: addTrade.totalCost
        }, onTradeSubmitted);
    };

    addTrade.cancel = function () {

    };

    NotifyService.RegisterAddTradeListener = function () {
        addTrade.today();
    };
}
"use strict"

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

    var onLoadContents = function (data) {

        if (data && data.Portfolio) {
            $scope.gridOptions.api.setRowData(data.Portfolio);
            $scope.gridOptions.api.sizeColumnsToFit();
        }
        NotifyService.UpdateBusyState(false);
    }.bind(this);

    function editTradeRenderer() {
        return "<div style=\"display:flex;justify-content:center\"><button style=\"margin-right:2px;\" ng-click=\"editTrade()\">Edit</button><button style=\"margin-left:2px;\" ng-click=\"sellTrade()\">Sell</button></div>";
    }

    function dateFormatter(val) {
        var dateobj = new Date(val.value);
        return dateobj.toLocaleDateString();
    }

    function loadPortfolio() {
        NotifyService.UpdateBusyState(true);
        MiddlewareService.LoadPortfolio(onLoadContents);
    };

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: null,
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
            MiddlewareService.UpdateTrade(trade, loadPortfolio);
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
            ItemName: $trade.Name,
            TransactionDate: $trade.dt,
            Action: $trade.SelectedAction,
            Quantity: $trade.Quantity,
            TotalCost: $trade.TotalCost        
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


"use strict"

// controller for add / edit account
function AddAccount($scope, $uibModalInstance, user, currencies, account, brokers) {

    //set the default values
    $scope.AccountName = {};
    $scope.AccountDescription;
    $scope.Currencies = currencies;
    $scope.Brokers = brokers;
    $scope.ReportingCurrency = currencies[0];
    $scope.SelectedBroker = brokers[0];
    $scope.AccountType = "Club";

    var members = [];

    if (account != null) {
        $scope.Title = "Edit Account";
        $scope.AccountName = account.AccountName;
        $scope.AccountDescription = account.Description;
        $scope.ReportingCurrency = account.ReportingCurrency;
        $scope.AccountType = account.AccountType;
        $scope.SelectedBroker = account.Broker;
        for(var index = 0; index < account.Members.length; index++)
        {
            members.push({ Name: account.Members[index].Name, Permission: account.Members[index].Permission });
        }
    }
    else {
        $scope.Title = "Add Account";
         members.push({ Name: user, Permission: "ADMINISTRATOR" });
    }
    
    $scope.EditAccount = account != null;

    $scope.gridOptions = {
        columnDefs: [
            { headerName: "Name", field: "Name", editable: true },
            {
                headerName: "Permission", field: "Permission", editable: true,
                cellEditor: 'select',
                cellEditorParams : { values : ["ADMINISTRATOR", "NONE", "READ", "UPDATE"] }
            }
        ],
        rowData: members,
        angularCompileRows: true,
        enableColResize: true,
        enableSorting: true,
        enableFilter: true,
        suppressRowClickSelection: true,
        singleClickEdit: true,
        rowSelection: 'multiple',
        defaultColDef: {
            width: 100,
            headerCheckboxSelection: isFirstColumn,
            checkboxSelection: isFirstColumn
        },
        //cellValueChanged : onCellValueChanged 
    };

    function isFirstColumn(params) {
        var displayedColumns = params.columnApi.getAllDisplayedColumns();
        var thisIsFirstColumn = displayedColumns[0] === params.column;
        return thisIsFirstColumn;
    };

    $scope.gridOptions.onCellValueChanged = function (data) {
        console.log(data);
    };

    $scope.onTypeChange = function () {
        $scope.IsClubTypeAccount = $scope.AccountType === "Club";
    };

    $scope.onTypeChange();

    $scope.accountNameChanged = function () {
        $scope.CanSubmit = $scope.AccountName.Name != "";
    };

    $scope.accountNameChanged();

    $scope.addMember = function () {
        members.push({ Name: "", Permission: "READ"});
        $scope.gridOptions.api.setRowData(members);
    };

    $scope.removeMembers = function () {
        var selectedNodes = $scope.gridOptions.api.getSelectedNodes();
        console.log("removing nodes: " + selectedNodes.length);
        
        var updated = members.filter((member, index) => {
            return selectedNodes.find(node => node.rowIndex === index) === undefined;
        });
        members = updated;
        $scope.gridOptions.api.setRowData(members);
    };

    $scope.ok = function () {
        console.log("name: " + $scope.AccountName + ", AccountType: " + $scope.AccountType);
        $uibModalInstance.close({
            AccountName : $scope.AccountName,
            Description:  $scope.AccountDescription,
            ReportingCurrency: $scope.ReportingCurrency,
            Broker: $scope.SelectedBroker,
            AccountType: $scope.AccountType,
            Enabled : true,
            Members : members
        }, $scope.EditAccount);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};
'use strict'

function AccountList($scope, $log, NotifyService, $uibModal, MiddlewareService) {

    $scope.IsConnected = false;

    $scope.LoggedInUser = "";

    var currencies = null;
    var brokers = null;

    var onLoadAccountNames = function (data) {

        $scope.Accounts = data.AccountNames;
        if ($scope.Accounts.length > 0) {
            $scope.SelectedAcount = $scope.Accounts[0];
        }
        $scope.$apply();
    };

    //method called when web app has successfully connected and logged in
    var onConnected = function (username) {
        $scope.IsConnected = true;
        $scope.LoggedInUser = username;

        //retrieve the list of account names for logged in user
        MiddlewareService.GetAccountsForUser(onLoadAccountNames);

        //get list of available currencies
        MiddlewareService.GetCurrencies(function (data) {
            currencies = data.Currencies;
        });

        //get list of available brokers
        MiddlewareService.GetBrokers(function (data) {
            brokers = data.Brokers;
        });
    };

    $scope.SelectedAcount = "";
    $scope.UpdateAccount = function () {
        MiddlewareService.UpdateCurrentAccount($scope.SelectedAcount, function (data) {
            NotifyService.InvokeAccountChange(); //this will also reload data for the current page
        });
    };

   //invoke the add account dialog view
    var showAccountPopup = function (title, account) {
        var modalInstance = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'views/AddAccount.html',
            controller: 'AddAccount',
            controllerAs: '$account',
            size: 'lg',
            resolve: {
                user: function () {
                    return $scope.LoggedInUser;
                },
                currencies: function () {
                    return currencies;
                },
                account: function () {
                    return account;
                },
                brokers: function () {
                    return brokers;
                }
            }
        });

        var onAccountUpdated = function (data) {
            if (data.Status === false) {
                alert("update account failed: " + data.Error);
            }
            else {
                //successfull, reload the account names
                onLoadAccountNames(data);
            }
        };

        modalInstance.result.then(function (account, edit) {
            //$ctrl.selected = selectedItem;
            //user has clicked ok , we need to update the account information for this user
            if (edit === true) {
                MiddlewareService.UpdateAccountDetails(account, onAccountUpdated);
            }
            else {
                MiddlewareService.CreateAccount(account, onAccountUpdated);
            }
            //updateMethod(transaction);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };

    $scope.editAccount = function () {
        MiddlewareService.GetAccountDetails($scope.SelectedAcount, (account) => {
            showAccountPopup("Edit Account", account);
        });
    };

    $scope.addAccount = function () {
        showAccountPopup("Add Account", null);
    }

    NotifyService.RegisterConnectionListener(onConnected);
}
"use strict"

function Reports($scope, NotifyService, MiddlewareService) {

    $scope.RecentReports = [];

    var onLoadContents = function (response) {
        $scope.RecentReports = response.RecentReports;
        $scope.$apply();
    };

    NotifyService.RegisterReportsListener(function () {
        MiddlewareService.LoadRecentReports(onLoadContents);
    });
}

"use strict"

var module = angular.module("InvestmentRecord", ["ui.bootstrap","agGrid"]);

agGrid.initialiseAgGridWithAngular1(angular);

module.service('NotifyService', NotifyService);

module.service('MiddlewareService', MiddlewareService);

module.controller("PortfolioController", Portfolio);

module.controller('TradeEditor', TradeEditor);

module.controller('AccountListController', AccountList);
//inject everything angular needs here

module.controller('CashFlowController', CashFlow);

module.controller('CashTransaction', CashTransaction);

module.controller('ReportCompletion', ReportCompletion);

module.controller('YesNoController', YesNoPicker);

module.controller('AddTradeController', CreateTrade);

module.controller('LayoutController', Layout);

module.controller('ReportsController', Reports);

module.controller('AddAccount', AddAccount);
