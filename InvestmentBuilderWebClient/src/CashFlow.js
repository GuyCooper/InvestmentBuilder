﻿"use strict"

function CashFlow($scope, $uibModal, $log, NotifyService, MiddlewareService) {

    $scope.cashFlows = [];
    this.receiptParamTypes = null;
    this.paymentParamTypes = null;
    this.reportingCurrency = null;

    this.cashFlowFromDate = new Date();

    var onLoadContents = function (response) {

        if (response) {
            $scope.cashFlows = response.CashFlows;
            this.receiptParamTypes = response.ReceiptParamTypes;
            this.paymentParamTypes = response.PaymentParamTypes;
            this.reportingCurrency = response.ReportingCUrrency;
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

    this.addTransactionDialog = function (title, paramTypes, dateFrom, currency, updateMethod) {
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
                },
                currency: function () {
                    return currency;
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
        this.addTransactionDialog('receipt', this.receiptParamTypes, this.cashFlowFromDate, this.reportingCurrency, function (transaction) { MiddlewareService.AddReceiptTransaction(transaction, onLoadContents); });
    }.bind(this);

    this.addPayment = function () {
        this.addTransactionDialog('payment', this.paymentParamTypes, this.cashFlowFromDate, this.reportingCurrency, function (transaction) { MiddlewareService.AddPaymentTransaction(transaction, onLoadContents); });
    };

    this.deleteTransaction = function (transaction) {
        MiddlewareService.RemoveTransaction({
            TransactionID: transaction.TransactionID
        }, this.reloadContents);
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


