"use strict"

//Controller for Request Redemption dialog page.
function RequestRedemption($scope, $uibModalInstance, users) {

    $scope.UserName = '';
    $scope.Amount = 0;
    $scope.Users = users;

    $scope.ok = function () {
        $uibModalInstance.close({
            TransactionDate: $scope.dt,
            UserName: $scope.UserName,
            Amount : $scope.Amount
        });
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    //datetime picker scoped methods

    $scope.today = function () {
        $scope.dt = new Date();
    };

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

    $scope.datepicker = {
        opened: false
    };

    $scope.today();

};