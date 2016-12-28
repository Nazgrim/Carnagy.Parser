angular
    .module("dealerHeaderModule", [])
    .directive('dealerHeader', function () {
        return {
            restrict: 'E',
            templateUrl: '/app/directives/dealer-header/dealer-header.html',
            controller: ['$scope', 'dealerService', function ($scope, dealerService) {
                $scope.dealerInfomation = dealerService.getDealerInfomation($scope.user.dealerId).then(function (dealer) {
                    $scope.dealer = dealer;
                });
            }]
        }
    })