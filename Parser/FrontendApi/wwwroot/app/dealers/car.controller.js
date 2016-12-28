'use strict';
class CarCtrl {
    constructor($scope, carService, carId, stockCarId, highchartsNG) {
        $scope.carId = carId;
        $scope.stockCarId = stockCarId;
        $scope.carInformationPromise = carService.getInformationById($scope.carId)
            .$promise
            .then(function (carInformation) {
                $scope.carInformation = carInformation;
            });

        $scope.$on('selectSeriesUp', function (event, data) {
            $scope.$broadcast('selectSeriesDown', data);
        });
    }
}

CarCtrl.$inject = ['$scope', 'carService', 'carId', 'stockCarId', 'highchartsNG'];

export default CarCtrl;