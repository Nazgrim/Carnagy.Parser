angular
  .module("similarCarModule", ['ngAnimate', 'ui.bootstrap', 'carModule'])
  .directive('similarCar', function ($uibModal, carService) {
    return {
      restrict: 'E',
      templateUrl: '/app/directives/similar-car/similar-car.html',
      link: function (scope, element, attrs) {
        carService.getSimilarCars().$promise.then(function (result) {
          scope.cars = result;
        });
        scope.remove = function (car) {
          for (var i = 0; i < scope.cars.length; i++) {
            if (scope.cars[i].id == car.id) {
              if (car.isAdd) {
                scope.removeToCompare(car);
              }
              scope.removedCars.push(car);
              scope.cars[i] = { isEmpty: true, order: i };
              break;
            }
          }
        }
        scope.removedCars = [];
        scope.add = function () {
          var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'modalSimilarCar.html',
            controller: 'ModalSimilarCarCtrl',
            resolve: {
              items: function () {
                return scope.removedCars;
              }
            }
          });
          modalInstance.result.then(function (selectedItem) {
            scope.cars = selectedItem;
          }, function () {
            $log.info('Modal dismissed at: ' + new Date());
          });
        }
        scope.removeToCompare = function (car) {
          car.isAdd = false;
          scope.$emit("removeToCompareUpEvent", car.id);
        };
        scope.addToCompare = function (car) {
          car.isAdd = true;
          scope.$emit("addToCompareUpEvent", car.chartSeries);
        }
      }
    };
  });