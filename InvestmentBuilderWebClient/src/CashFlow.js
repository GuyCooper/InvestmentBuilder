"use strict"

function CashFlow($scope, $uibModal, $log, $interval, NotifyService, MiddlewareService) {

    $scope.cashFlows = [];
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
        MiddlewareService.CheckBuildStatus(onCheckStatus);
    };

    var onLoadContents = function (response) {

        if (response) {
            $scope.cashFlows = response.CashFlows;
            this.receiptParamTypes = response.ReceiptParamTypes;
            this.paymentParamTypes = response.PaymentParamTypes;
        }

        if ($scope.cashFlows.length > 0) {
            this.cashFlowFromDate = new Date($scope.cashFlows[$scope.cashFlows.length - 1].ValuationDate);
            this.canBuild = $scope.cashFlows[0].CanBuild;
        }

        if (this.isBuilding == true && progress == null) {
            progress = $interval(checkStatusRequest, 1000);
        }
        $scope.$apply();

    }.bind(this);

    NotifyService.RegisterCashFlowListener(function () {
        MiddlewareService.GetCashFlowContents(null, onLoadContents);
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
        this.addTransactionDialog('receipt', this.receiptParamTypes, function (transaction) { MiddlewareService.AddReceiptTransaction(transaction, onLoadContents); });
    }.bind(this);

    this.addPayment = function () {
        this.addTransactionDialog('payment', this.paymentParamTypes, function (transaction) { MiddlewareService.AddPaymentTransaction(transaction, onLoadContents); });
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

    this.BuildReport = function () {

        this.isBuilding = true;
        MiddlewareService.BuildReport(onLoadContents);

    }.bind(this);

    //this.formatTransactionDate = function (transaction) {
    //    var transactionDate = Date.parse(transaction.TransactionDate);
    //    return transactionDate.toDateString();
    //}
}

function CashTransaction($scope, $uibModalInstance, transactionType, paramTypes, MiddlewareService) {
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

function ReportCompletion($uibModalInstance, errors) {
    var $report = this;
    $report.success = errors == null || errors.length == 0;
    $report.errors = errors;

    $report.ok = function () {
        $uibModalInstance.dismiss('cancel');
    };
};

