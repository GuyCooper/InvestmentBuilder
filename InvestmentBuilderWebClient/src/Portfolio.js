﻿"use strict"

//var module = angular.module("example", ["agGrid"]);

function Portfolio($scope, $log, $uibModal, NotifyService, MiddlewareService) {

    var columnDefs = [
        { headerName: "Name", field: "Name" },
        { headerName: "Quantity", field: "Quantity" },
        { headerName: "Last Brought", field: "LastBrought", cellFormatter: dateFormatter },
        { headerName: "Avg.Price Paid", field: "AveragePricePaid" },
        { headerName: "Total Cost", field: "TotalCost" },
        { headerName: "Current Price", field: "SharePrice" },
        { headerName: "Net Value", field: "NetSellingValue" },
        { headerName: "Profit/Loss", field: "ProfitLoss" },
        { headerName: "Month Change%", field: "MonthChangeRatio" },
        { headerName: "Options", cellRenderer:editTradeRenderer }
    ];

    var reloadPortfolio = false;

    var onLoadContents = function (data) {

        if (data && data.Portfolio) {
            $scope.gridOptions.api.setRowData(data.Portfolio);
            $scope.gridOptions.api.sizeColumnsToFit();
        }
        NotifyService.UpdateBusyState(false, false);
    }.bind(this);

    function editTradeRenderer() {
        return "<div style=\"display:flex;justify-content:center\"><button style=\"margin-right:2px;\" ng-click=\"editTrade()\">Edit</button><button style=\"margin-left:2px;\" ng-click=\"sellTrade()\">Sell</button></div>";
    }

    function dateFormatter(val) {
        var dateobj = new Date(val.value);
        return dateobj.toLocaleDateString();
    }

    function loadPortfolio() {
        if (reloadPortfolio === true) {
            NotifyService.UpdateBusyState(true, false);
            MiddlewareService.LoadPortfolio(onLoadContents);
            reloadPortfolio = false;
        }
    };

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: null,
        angularCompileRows: true,
        enableColResize: true,
        enableSorting: true,
        enableFilter: true
    }

    $scope.editTrade = function () {
        console.log("editing trade " + this.data.Name);
        var name = this.data.Name;
        var editModal = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'views/EditTrade.html',
            controller: 'TradeEditor',
            controllerAs: '$trade',
            size: 'lg',
            resolve: {
                name: function () {
                    return name;
                }
            }
        });

        editModal.result.then(function (trade) {
            //$ctrl.selected = selectedItem;
            //use has clicked ok , we need to update the trade
            MiddlewareService.UpdateTrade(trade, refreshPortfolio);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.sellTrade = function () {
        console.log("selling trade " + this.data.Name);
        var title = "Sell Trade";
        var name = this.data.Name;
        var description = "Are you sure you want to sell " + name;
        var sellModal = $uibModal.open({
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
                name: function () {
                    return name;
                },
                ok: function () { return false; }
            }
        });
        
        sellModal.result.then(function (param) {
            //$ctrl.selected = selectedItem;
            //use has clicked ok , we need to update the trade
            MiddlewareService.SellTrade(param.name, refreshPortfolio);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    //register as a portfolio listener so  loadPortfolio will be called every time the
    //portfolio tab is clicked
    NotifyService.RegisterPortfolioListener(loadPortfolio);

    //also, we want the portfolio to be loaded on startup so add is as a connectionlistener as well.
    //this means it will be loaded once the connection to the server has been made
    NotifyService.RegisterConnectionListener(refreshPortfolio);

    // reload the portfolio from the server
    function refreshPortfolio() {
        reloadPortfolio = true;
        loadPortfolio();
    };

    function onAccountChanged() {
        reloadPortfolio = true;
    };

    NotifyService.RegisterAccountListener(onAccountChanged);
};

function TradeEditor($uibModalInstance, name) {
    var $trade = this;

    $trade.Name = name;
    $trade.Actions = ['Buy', 'Sell', 'Change'];
    $trade.SelectedAction = '';
    $trade.Quantity = 0;
    $trade.TotalCost = 0;

    $trade.ok = function () {
        $uibModalInstance.close({
            ItemName: $trade.Name,
            TransactionDate: $trade.dt,
            Action: $trade.SelectedAction,
            Quantity: $trade.Quantity,
            TotalCost: $trade.TotalCost        
        });
    };

    $trade.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $trade.today = function () {
        $trade.dt = new Date();
    };

    $trade.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    $trade.openDatePicker = function () {
        $trade.datepicker.opened = true;
    };

    $trade.format = 'dd-MMMM-yyyy';
    $trade.altInputFormats = ['M!/d!/yyyy'];

    $trade.datepicker = {
        opened: false
    };

    $trade.today();
};

//module.controller("exampleCtrl", Portfolio);
//agGrid.initialiseAgGridWithAngular1(angular)

