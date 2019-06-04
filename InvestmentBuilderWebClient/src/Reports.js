"use strict"

function Reports($scope, NotifyService, MiddlewareService) {

    $scope.RecentReports = [];

    var dt = new Date();
    var latestDate = dt.toDateString();
    
    // Handler for load reports.
    var onLoadContents = function (response) {
        $scope.RecentReports = response.RecentReports.map(report => 
        {
            report.Link = report.Link + ";session=" + NotifyService.GetSessionID();
            return report;
        });

        $scope.$apply();
    };

    // Call server to load the reports...
    var loadReports = function (dt) { 

        var request = {
            DateFrom: dt
        };
        MiddlewareService.LoadRecentReports(request, onLoadContents);
    };

    // Load the previous 5 reports.
    $scope.previous = function () {
        if ($scope.RecentReports.length > 0) {
            var fromDate = $scope.RecentReports[$scope.RecentReports.length - 1].ValuationDate;
            loadReports(fromDate);
        }
    };

    // Load the most recent reports
    $scope.recent = function () {
        loadReports(latestDate);
    };

    // register listener so when page is displayed the reports are loaded.
    NotifyService.RegisterReportsListener(function () {
        loadReports(latestDate);
    });
}
