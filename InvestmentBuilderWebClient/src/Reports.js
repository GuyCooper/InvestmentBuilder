"use strict"

function Reports($scope, NotifyService, MiddlewareService) {

    $scope.RecentReports = [];

    var onLoadContents = function (response) {
        $scope.RecentReports = response.RecentReports.map(report => 
        {
            report.Link = report.Link + ";session=" + NotifyService.GetSessionID();
            return report;
        });
        $scope.$apply();
    };

    NotifyService.RegisterReportsListener(function () {
        MiddlewareService.LoadRecentReports(onLoadContents);
    });
}
