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
            NotifyService.InvokeAccountChange(); //this will also reload data for the current page
        });
    };

    NotifyService.RegisterConnectionListener(loadAccountList);
}