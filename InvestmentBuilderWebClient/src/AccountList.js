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

    var onDisconnected = function () {
        $scope.IsConnected = false;
    };

    $scope.SelectedAcount = "";
    $scope.UpdateAccount = function () {
        MiddlewareService.UpdateCurrentAccount($scope.SelectedAcount, function (data) {
            NotifyService.InvokeAccountChange(); //this will also reload data for the current page
        });
    };

   //invoke the add account dialog view
    var showAccountPopup = function (title, account, isError) {

        if (isError === true) {
            return;
        }

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
            showAccountPopup("Edit Account", account, account.IsError);
        });
    };

    $scope.addAccount = function () {
        showAccountPopup("Add Account", null, false);
    };

    // Display the last transaction and give the user the option to undo it.
    $scope.getLastTransaction = function () {
        MiddlewareService.GetLastTransaction((transaction) => {
            var lastTransactionModal = $uibModal.open({
                animation: true,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'views/LastTransaction.html',
                controller: 'LastTransactionController',
                size: 'lg',
                resolve: {
                    transaction: function () {
                        return transaction;
                    }
                }
            });

            lastTransactionModal.result.then(function () {
                MiddlewareService.UndoLastTransaction(function (result) {
                    //undo succeded, refresh all data...
                    NotifyService.InvokeAccountChange();
                });
            }, function () {
                $log.info('LastTransactionModal dismissed at: ' + new Date());
            });

        });
    };

    $scope.logout = function () {
        var title = "logout";
        var description = "Are you sure?";
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
                    return title;
                },
                description: function () {
                    return description;
                },
                name: function () { return null },
                ok: function () { return false; }
            }
        });

        logoutModal.result.then(function (param) {
            //use wants to logout. do it here...
            NotifyService.InvokeDisconnectionListeners();

        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }
    NotifyService.RegisterDisconnectionListener(onDisconnected);
    NotifyService.RegisterConnectionListener(onConnected);

}