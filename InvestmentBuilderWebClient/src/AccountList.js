'use strict'

function AccountList($scope, $log, NotifyService, $uibModal, MiddlewareService) {

    $scope.IsConnected = false;

    var loggedInUser;
    var currencies = null;
    var brokers = null;

    //method called when web app has successfully connected and logged in
    var onConnected = function (username) {
        $scope.IsConnected = true;
        loggedInUser = username;
        //retrieve the list of account names for logged in user
        MiddlewareService.GetAccountsForUser(function (data) {
            $scope.Accounts = data.AccountNames;
            if ($scope.Accounts.length > 0) {
                $scope.SelectedAcount = $scope.Accounts[0];
            }
        });

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
                    return loggedInUser;
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

        modalInstance.result.then(function (account) {
            //$ctrl.selected = selectedItem;
            //user has clicked ok , we need to update the account information for this user
            MiddlewareService.UpdateAccountDetails(account, function (data) {
                if (data.Status === false) {
                    alert("update account failed: " + data.Error);
                }
            });

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