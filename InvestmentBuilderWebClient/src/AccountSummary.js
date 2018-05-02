'use strict'

function AccountSummary($scope, NotifyService, MiddlewareService) {

    var onLoadAccountSummary = function (response) {
        $scope.AccountName = response.AccountName;
        $scope.ReportingCurrency = response.ReportingCurrency;
        $scope.ValuePerUnit = response.ValuePerUnit;
        $scope.NetAssets = response.NetAssets;

        $scope.BankBalance = response.BankBalance;
        $scope.MonthlyPnL = response.MonthlyPnL;
        $scope.$apply();
    };

    $scope.refresh = function() {
    };

    var loadAccountSummary = function () {
        MiddlewareService.GetInvestmentSummary(onLoadAccountSummary);
    }

    //ensure this view is reloaded on connection
    NotifyService.RegisterConnectionListener(loadAccountSummary);
    //ensure this view is reloaded if the account is changed
    NotifyService.RegisterAccountListener(loadAccountSummary);
};