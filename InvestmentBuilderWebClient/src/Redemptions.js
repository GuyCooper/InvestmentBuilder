'use strict'

// Redemptions view controller.
function Redemptions($scope, $log, $uibModal, NotifyService, MiddlewareService) {
    var columnDefs = [
        { headerName: "User", field: "User" },
        { headerName: "Amount", field: "Amount" },
        { headerName: "TransactionDate", field: "TransactionDate", cellFormatter: dateFormatter },
        { headerName: "Status", field: "Status" }
    ];

    $scope.RedemptionRequestFailed = false;
    $scope.RedemptionRequestError = '';

    var onLoadContents = function (data) {

        if (data && data.Redemptions) {
            $scope.gridOptions.api.setRowData(data.Redemptions);
            $scope.gridOptions.api.sizeColumnsToFit();
        }
    }.bind(this);

    function dateFormatter(val) {
        var dateobj = new Date(val.value);
        return dateobj.toLocaleDateString();
    }

    function loadRedemptions() {
        MiddlewareService.GetRedemptions(onLoadContents);
    };

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: null,
        angularCompileRows: true,
        enableColResize: true,
        enableSorting: true,
        enableFilter: true
    };

    //User requesting a new redemption
    $scope.requestRedemption = function () {
        var requestRedemptionModal = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'views/RequestRedemption.html',
            controller: 'RequestRedemptionController',
            size: 'lg'
        });

        requestRedemptionModal.result.then(function (redemption) {
            MiddlewareService.RequestRedemption(redemption, (response) => {
                if (response.Success === true) {
                    loadRedemptions();
                }
                else {
                    $scope.RedemptionRequestFailed = true;
                    $scope.RedemptionRequestError = response.Error;
                }
            });
        },
        function () {
            $log.info('RequestRdemptionModal dismissed at: ' + new Date());
        });
    };

    NotifyService.RegisterRedemptionListener(loadRedemptions);
};