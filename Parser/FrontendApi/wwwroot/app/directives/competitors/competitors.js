angular
    .module("competitorsModule", ['carModule'])
    .directive('competitors', function () {
        return {
            restrict: 'E',
            templateUrl: '/app/directives/competitors/competitors.html',
            controller: ['$scope', 'dealerService', '$rootScope', function ($scope, dealerService, $rootScope) {
                $scope.$emit('selectSeriesUp', 12312312);
                $scope.$apply();
                $rootScope.$on('selectSeriesDown', function (event, data) {
                    console.log('on');
                    console.log(data); // Данные, которые нам прислали
                });
                $scope.$on('selectSeriesDown', function (event, data) {
                    console.log('on');
                    console.log(data); // Данные, которые нам прислали
                });
                $scope.dealers = dealerService.getCompetitors($scope.dealerId, $scope.car.carId);
            }]
        }
    })