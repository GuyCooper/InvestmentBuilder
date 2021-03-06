﻿"use strict"

var module = angular.module("InvestmentRecord", ["ui.bootstrap","agGrid"]);

module.service('NotifyService', NotifyService);

module.controller("PortfolioController", Portfolio);

module.controller('TradeEditor', TradeEditor);

//inject everything angular needs here
//angular.module('InvestmentRecord', ['ui.bootstrap']);

//angular.module('InvestmentRecord', ['ui.bootstrap', 'agGrid']);
//angular.module('InvestmentRecord', ['ui.bootstrap'], ['agGrid']);
//angular.module('InvestmentRecord', ['ui.bootstrap']);

module.controller('CashFlowController', CashFlow);

module.controller('CashTransaction', CashTransaction);

module.controller('ReportCompletion', ReportCompletion);

module.controller('YesNoController', YesNoPicker);

module.controller('AddTradeController', CreateTrade);

module.controller('LayoutController', Layout);

//angular.module('InvestmentRecord')
//.controller('PortfolioController', Portfolio);
