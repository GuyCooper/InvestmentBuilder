"use strict"

// Controller for LastTransaction Page
function LastTransaction($scope, $uibModalInstance, transaction) {

    $scope.Name =  transaction.InvestmentName;
    $scope.TransactionType = transaction.TransactionType;
    $scope.Quantity = transaction.Quantity;
    $scope.Amount = transaction.Amount;

    $scope.ok = function () {
        $uibModalInstance.close();
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}