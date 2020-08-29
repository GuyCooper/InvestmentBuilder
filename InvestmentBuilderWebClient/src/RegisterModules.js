"use strict"

var module = angular.module("InvestmentRecord", ["ui.bootstrap", "agGrid", "ngIdle"])

//module.config([
//   '$compileProvider',
//    'KeepaliveProvider',
//    'IdleProvider',
//    function ($compileProvider) {
//        $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|file|blob|ftp|mailto|chrome-extension):/);
//        // Angular before v1.2 uses $compileProvider.urlSanitizationWhitelist(...)
//    },
//    function (KeepaliveProvider, IdleProvider) {
//    IdleProvider.idle(30);
//    IdleProvider.timeout(10);
//    KeepaliveProvider.interval(10);
//    }
//]);

module.config([
    'KeepaliveProvider',
    'IdleProvider',
    function (KeepaliveProvider, IdleProvider) {
    IdleProvider.idle(300);
    IdleProvider.timeout(30);
    KeepaliveProvider.interval(10);
    }
]);

agGrid.initialiseAgGridWithAngular1(angular);

module.service('NotifyService', NotifyService);

module.service('MiddlewareService', MiddlewareService);

module.controller("PortfolioController", Portfolio);

module.controller('TradeEditor', TradeEditor);

module.controller('AccountListController', AccountList);
//inject everything angular needs here

module.controller('CashFlowController', CashFlow);

module.controller('CashTransaction', CashTransaction);

module.controller('ReportCompletion', ReportCompletion);

module.controller('YesNoController', YesNoPicker);

module.controller('AddInvestmentController', AddInvestment);

module.controller('LayoutController', Layout);

module.controller('ReportsController', Reports);

module.controller('AddAccount', AddAccount);

module.controller('LastTransaction', LastTransaction);

module.controller('RedemptionsController', Redemptions);

module.controller('RequestRedemptionController', RequestRedemption);
