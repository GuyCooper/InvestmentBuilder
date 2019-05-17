'use strict'

function AddInvestment($uibModal, $scope, NotifyService, MiddlewareService) {

    $scope.Busy = false;

    $scope.today = function () {
        $scope.dt = new Date();
    };

    $scope.ValidatingSymbol = false;

    $scope.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    $scope.openDatePicker = function () {
        $scope.datepicker.opened = true;
    };

    $scope.format = 'dd-MMMM-yyyy';
    $scope.altInputFormats = ['M!/d!/yyyy'];

    var onTradeSubmitted = function (response) {
        $scope.Busy = false;
        var description = "";
        if (response.status == 'fail') {
            description = "Add investment failed";
        }
        else {
            description = "Add investment succeded";
            NotifyService.InvokeAccountChange();
        }

        var logoutModal = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'views/YesNoChooser.html',
            controller: 'YesNoController',
            controllerAs: '$item',
            size: 'lg',
            resolve: {
                title: function () {
                    return "Add Investment";
                },
                description: function () {
                    return description;
                },
                name: function () { return null},
                ok: function () { return true;}
            }
        });
    };

    var onSymbolValidated = function (response) {
        $scope.Busy = false;
        $scope.SymbolValidated = response.IsError === false;
        $scope.$apply();
    };

    $scope.datepicker = {
        opened: false
    };

    $scope.submitTrade = function () {

        var trade = {
            TransactionDate: $scope.dt,
            ItemName: $scope.Name,
            Symbol: $scope.Symbol,
            Quantity: $scope.Quantity,
            Currency: $scope.Currency,
            TotalCost: $scope.TotalCost,
            Action: "Buy"
        };

        $scope.Busy = true;
        MiddlewareService.UpdateTrade(trade,onTradeSubmitted);
    };

    $scope.cancel = function () {

    };

    $scope.ValidateSymbol = function () {
        $scope.ValidatingSymbol = true;
        var payload = {
            Symbol: $scope.Symbol
        };

        $scope.Busy = true;
        MiddlewareService.GetPrice(payload, onSymbolValidated);
    };

    NotifyService.RegisterAddTradeListener = function () {
        $scope.today();
    };
}