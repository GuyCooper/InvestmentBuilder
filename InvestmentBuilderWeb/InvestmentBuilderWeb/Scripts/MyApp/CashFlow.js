"use strict"

function CashFlow($http, $uibModal, $log, $interval) {

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

    $http.get('CashFlowContents')
    .then(onLoadContents);

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
        this.addTransactionDialog('receipt', this.receiptParamTypes, 'AddReceiptTransactionAngular');
    }.bind(this);

    this.addPayment = function () {
        this.addTransactionDialog('payment', this.paymentParamTypes, 'AddPaymentTransactionAngular');
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

//inject everything angular needs here
angular.module('InvestmentRecord', ['ui.bootstrap']);

angular.module('InvestmentRecord')
.controller('CashFlowController', CashFlow);

angular.module('InvestmentRecord')
.controller('CashTransaction', CashTransaction);

angular.module('InvestmentRecord')
.controller('ReportCompletion', ReportCompletion);
