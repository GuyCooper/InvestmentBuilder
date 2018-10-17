'use strict'

//controller handles management of the build report progress and the view tab
function Layout($scope, $log, $uibModal, $http, NotifyService, MiddlewareService) {
    $scope.ProgressCount = 0;
    $scope.Section = null;
    $scope.IsBuilding = false;
    $scope.CanBuild = false;
    $scope.LoggedIn = false;
    $scope.BuildStatus = "Not Building";
    $scope.UserName = "guy@guycooper.plus.com";
    $scope.Password = "rangers";
    $scope.IsBusy = false;

    var servername = ""; //"ws://localhost:8080/MWARE";

    //callback displays the account summary details
    var onLoadAccountSummary = function (response) {
        $scope.AccountName = response.AccountName;
        $scope.ReportingCurrency = response.ReportingCurrency;
        $scope.ValuePerUnit = response.ValuePerUnit;
        $scope.NetAssets = response.NetAssets;

        $scope.BankBalance = response.BankBalance;
        $scope.MonthlyPnL = response.MonthlyPnL;
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
                onReportFinished(buildStatus.Errors);
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
    
    var onReportFinished = function (errors) {
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
                }
            }
        });
    };

    $scope.onPortfolio = function () {
        NotifyService.InvokePortfolio();
    };

    $scope.onAddTrade = function () {
        NotifyService.InvokeAddTrade();
    };

    $scope.onCashFlow = function () {
        NotifyService.InvokeCashFlow();
    };

    $scope.onReports = function () {
        NotifyService.InvokeReports();
    };

    $scope.buildReport = function () {
        MiddlewareService.BuildReport(onBuildProgress);
    };

    //connect to middleware layer with user supplied username and password
    $scope.doLogin = function () {
        NotifyService.UpdateBusyState(true);
        //once conncted inform any connection listeners that connection is complete
        MiddlewareService.Connect(servername, $scope.UserName, $scope.Password).then(function () {
            console.log("connection to middleware succeded!");
            NotifyService.UpdateBusyState(false);
            $scope.LoggedIn = true;
            NotifyService.InvokeConnectionListeners($scope.UserName);
        },
        function (error) {
            console.log("connection to middleware failed" + error);
        });
    }

    //method updates the isBusy state
    var busyStateChanged = function (busy) {
        $scope.IsBusy = busy;
    };

    //load the config
    $http.get("/config.json").then(function (data) {
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

    },function (error) {
        alert("unable to load config file: " + error);
    });
};

//controller to handle the report completion view
function ReportCompletion($uibModalInstance, errors) {
    var $report = this;
    $report.success = errors == null || errors.length == 0;
    $report.errors = errors;

    $report.ok = function () {
        $uibModalInstance.dismiss('cancel');
    };
};
