"use strict"

function Reports($scope, NotifyService, MiddlewareService) {

    $scope.RecentReports = [];

    var onLoadContents = function (response) {
        $scope.RecentReports = response.RecentReports;
        $scope.$apply();
    };

    NotifyService.RegisterReportsListener(function () {
        MiddlewareService.LoadRecentReports(onLoadContents);
    });
}
