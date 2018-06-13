'use strict'

//controller handles management of the build report progress and the view tab
function Layout($scope, $log, $uibModal, NotifyService, MiddlewareService) {
    $scope.progressCount = 0;
    $scope.section = null;
    $scope.isBuilding = false;
    $scope.CanBuild = false;
    $scope.BuildStatus = "Not Building";

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
            $scope.progressCount = buildStatus.Progress;

            $scope.section = buildStatus.BuildSection;
            if ($scope.isBuilding == true && buildStatus.IsBuilding == false) {
                $scope.isBuilding = buildStatus.IsBuilding;
                //now display a dialog to show any errors during the build
                onReportFinished(buildStatus.Errors);
            }
            else {
                $scope.isBuilding = buildStatus.IsBuilding;
            }

            if ($scope.isBuilding == true) {
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

    //ensure this view is reloaded on connection
    NotifyService.RegisterConnectionListener(loadAccountSummary);
    //ensure this view is reloaded if the account is changed
    NotifyService.RegisterAccountListener(loadAccountSummary);
    //ensure this view is updated when the build status changes
    NotifyService.RegisterBuildStatusListener(onBuildStatusChanged);
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
