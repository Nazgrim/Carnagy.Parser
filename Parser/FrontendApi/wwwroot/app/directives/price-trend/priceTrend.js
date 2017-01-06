angular
    .module("priceTrendModule", ['carModule'])
    .directive('priceTrend', function () {
        return {
            restrict: 'E',
            templateUrl: '/app/directives/price-trend/price-trend.html',
            controller: function ($scope, carService) {
                $scope.chartPriceConfigPromise = carService.getPriceTrend($scope.stockCarId,$scope.carId).then(function (result) {
                    $scope.chartConfig1 = result;
                });
            }
        }
    })