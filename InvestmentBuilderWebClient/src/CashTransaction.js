"use strict"

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
            DateRequestedFrom: dateFrom
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
