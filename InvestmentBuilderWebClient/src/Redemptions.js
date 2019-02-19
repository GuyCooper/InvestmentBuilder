'use strict'

// Redemptions view controller.
function Redemptions($scope, $log, $uibModal, NotifyService, MiddlewareService) {
    var columnDefs = [
        { headerName: "User", field: "User" },
        { headerName: "Amount", field: "Amount" },
        { headerName: "TransactionDate", field: "TransactionDate", cellFormatter: dateFormatter },
        { headerName: "Status", field: "Status" }
    ];

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
    }

    NotifyService.RegisterRedemptionListener(loadRedemptions);
};