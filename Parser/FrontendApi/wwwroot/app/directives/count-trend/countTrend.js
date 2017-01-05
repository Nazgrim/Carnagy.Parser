angular
    .module("countTrendModule", ['carModule'])
    .directive('countTrend', function () {
        return {
            restrict: 'E',
            templateUrl: '/app/directives/count-trend/count-trend.html',
            controller: function ($scope, carService) {
                $scope.chartCountConfigPromise = carService.getCountTrend($scope.stockCarId).then(function (result) {
                    $scope.chartConfig2 = result;
                });
            }
        }
    })