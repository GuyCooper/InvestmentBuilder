"use strict"

// Controller for LastTransaction Page
function LastTransaction($scope, $uibModalInstance, transaction) {

    $scope.ok = function () {
        $uibModalInstance.close();
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.Success = transaction.Success;
    if ($scope.Success === true) {
        $scope.Name = transaction.InvestmentName;
        $scope.TransactionType = transaction.TransactionType;
        $scope.Quantity = transaction.Quantity;
        $scope.Amount = transaction.Amount;
    }
}