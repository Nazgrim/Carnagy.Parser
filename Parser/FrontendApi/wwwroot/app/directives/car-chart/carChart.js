angular
    .module("carChartModule", ['carModule'])
    .directive('carChart', function () {
        return {
            restrict: 'E',
            templateUrl: '/app/directives/car-chart/car-chart.html',
            controller: function ($scope, carService) {
                $scope.selectedLegend = {
                    selected: false,
                    people: 10,
                    from: 30936,
                    to: 31248
                };
                $scope.chartConfigPromise = carService.getChartConfig($scope).then(function (result) {
                    $scope.chartConfig = result;
                });               
            }
        }
    })