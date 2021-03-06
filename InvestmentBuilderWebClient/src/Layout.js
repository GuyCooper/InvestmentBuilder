﻿'use strict'

//controller handles management of the build report progress and the view tab
function Layout($scope, $log, $uibModal, $http, NotifyService, MiddlewareService, Idle, Keepalive) {

    var initialiseLayout = function () {
        $scope.ProgressCount = 0;
        $scope.Section = null;
        $scope.IsBuilding = false;
        $scope.CanBuild = false;
        $scope.LoggedIn = false;
        $scope.BuildStatus = "Not Building";
        $scope.UserName = localStorage.getItem('userName');
        $scope.Password = localStorage.getItem('password');
        $scope.IsBusy = false;
        $scope.SaveCredentials = true;
        $scope.LoginFailed = false;
    };
     
    var servername = ""; //"ws://localhost:8080/MWARE";

    //callback displays the account summary details
    var onLoadAccountSummary = function (response) {
        $scope.AccountName = response.AccountName;
        $scope.ReportingCurrency = response.ReportingCurrency;
        $scope.ValuePerUnit = response.ValuePerUnit;
        $scope.NetAssets = response.NetAssets;

        $scope.BankBalance = response.BankBalance;
        $scope.MonthlyPnL = response.MonthlyPnL;

        var dtValuation = new Date(response.ValuationDate);
        $scope.ValuationDate = dtValuation.toDateString();
        $scope.$apply();
    };

    var loadAccountSummary = function () {
        MiddlewareService.GetInvestmentSummary(onLoadAccountSummary);
    }

    var onBuildStatusChanged = function (status) {
        $scope.CanBuild = status;
    };

    var onBuildProgress = function (response) {

        if ($scope.CanBuild == true) {
            $scope.CanBuild = false;
        }

        var buildStatus = response.Status;
        if(buildStatus != undefined && buildStatus != null) {
            $scope.ProgressCount = buildStatus.Progress;

            $scope.Section = buildStatus.BuildSection;
            if ($scope.IsBuilding == true && buildStatus.IsBuilding == false) {
                $scope.IsBuilding = buildStatus.IsBuilding;
                //now display a dialog to show any errors during the build
                loadAccountSummary();
                onReportFinished(buildStatus.Errors, buildStatus.CompletedReport);
            }
            else {
                $scope.IsBuilding = buildStatus.IsBuilding;
            }

            if ($scope.IsBuilding == true) {
                $scope.BuildStatus = "Building Report";
            }
            else {
                $scope.BuildStatus = "Not Building Report";
            }
            
            $scope.$apply();
        }
    };
    
    var onReportFinished = function (errors, completedReport) {
        var modalInstance = $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'Views/ReportCompletion.html',
            controller: 'ReportCompletion',
            controllerAs: '$report',
            size: 'lg',
            resolve: {
                errors: function () {
                    return errors;
                },
                completedReport: function () {
                    return completedReport;
                }
            }
        });
    };

    $scope.onPortfolio = function () {
        NotifyService.InvokePortfolio();
    };

    $scope.onAddInvestment = function () {
        NotifyService.InvokeAddTrade();
    };

    $scope.onCashFlow = function () {
        NotifyService.InvokeCashFlow();
    };

    $scope.onReports = function () {
        NotifyService.InvokeReports();
    };

    $scope.onRedemptions = function () {
        NotifyService.InvokeRedemptions();
    };

    $scope.buildReport = function () {
        MiddlewareService.BuildReport(onBuildProgress);
    };

    //connect to middleware layer with user supplied username and password
    $scope.doLogin = function () {
        $scope.LoginFailed = false;
        NotifyService.UpdateBusyState(true);
        //once conncted inform any connection listeners that connection is complete
        MiddlewareService.Connect(servername, $scope.UserName, $scope.Password).then(function (result) {
            console.log("connection to middleware succeded!");
            
            if ($scope.SaveCredentials) {
                localStorage.setItem('userName', $scope.UserName);
                localStorage.setItem('password', $scope.Password);
            }
            else {
                localStorage.removeItem('userName');
                localStorage.removeItem('password');
            }

            NotifyService.UpdateBusyState(false);
            $scope.LoggedIn = true;
            $scope.LoginFailed = false;
            var loginResult = JSON.parse(result);
            NotifyService.OnConnected(loginResult.ConnectionId, $scope.UserName);
            
        },
        function (error) {
            NotifyService.UpdateBusyState(false);
            $scope.LoginFailed = true;
            $scope.$apply();
            console.log("connection to middleware failed" + error);
        });
    }

    //method updates the isBusy state
    var busyStateChanged = function (busy, applyScope) {
        $scope.IsBusy = busy;
        if (applyScope === true) {
            $scope.$apply();
        }
    };

    var closeConnection = function () {
        MiddlewareService.Disconnect();
        initialiseLayout();
    };

    NotifyService.RegisterDisconnectionListener(closeConnection);

    initialiseLayout();

    function closeModals() {
        if ($scope.warning) {
            $scope.warning.close();
            $scope.warning = null;
        }

        if ($scope.timedout) {
            $scope.timedout.close();
            $scope.timedout = null;
        }
    }

    var startIdleWatcher = function () {
        $log.log("starting idle watcher");
        closeModals();
        Idle.watch();
    };

    var stopIdleWatcher = function () {
        $log.log("stoping idle watcher");
        closeModals();
        Idle.unwatch();
    };

    //load the config
    $http.get("./config.json").then(function (data) {
        servername = data.data.url;
        console.log('config loaded. url: ' + servername);
        //register handler to be invoked every time the isBusy state is changed
        NotifyService.RegisterBusyStateChangedListener(busyStateChanged);
        //ensure this view is reloaded on connection
        NotifyService.RegisterConnectionListener(loadAccountSummary);
        //ensure this view is reloaded if the account is changed
        NotifyService.RegisterAccountListener(loadAccountSummary);
        //ensure this view is updated when the build status changes
        NotifyService.RegisterBuildStatusListener(onBuildStatusChanged);
        //start the idle watcher on connection
        NotifyService.RegisterConnectionListener(startIdleWatcher);
        //stop the idle watcher on disconnect
        NotifyService.RegisterDisconnectionListener(stopIdleWatcher);

    },function (error) {
        alert("unable to load config file: " + error);
    });

    $scope.$on('IdleStart', function () {
        closeModals();

        $scope.warning = $uibModal.open({
            templateUrl: 'warning-dialog.html',
            windowClass: 'modal-danger'
        });
    });

    $scope.$on('IdleEnd', function () {
        closeModals();
    });

    $scope.$on('IdleTimeout', function () {
        closeModals();
        $log.log("idle timeout expired.logging out...");
        NotifyService.InvokeDisconnectionListeners();
    });
};

//controller to handle the report completion view
function ReportCompletion($uibModalInstance, NotifyService, errors, completedReport) {
    var $report = this;
    $report.success = errors == null || errors.length == 0;
    $report.errors = errors;

    $report.CompletedReport = completedReport + ";session=" + NotifyService.GetSessionID();
    $report.ok = function () {
        $uibModalInstance.dismiss('cancel');
    };
};
