"use strict"

var module = angular.module("InvestmentRecord", ["ui.bootstrap","agGrid"]);

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

module.controller('AddTradeController', CreateTrade);

module.controller('LayoutController', Layout);

module.controller('ReportsController', Reports);

module.controller('AddAccount', AddAccount);
