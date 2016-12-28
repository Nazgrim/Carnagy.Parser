'use strict';
function unique(value, index, self) {
  return self.indexOf(value) === index;
}
function initFilter(scope, cars, filter) {
  filter.forEach(function (i) {
    scope[i + 's'] = cars
      .map(c => c[i])
      .filter(unique);
  });
}
class DealerCtrl {
  constructor($scope, highchartsNG, dealerService, uiGridConstants, $filter, accountService) {
    $scope.user = accountService.getCurrentUser();
    $scope.showDetailed = function (car) {
      if (car.isDetailed)
        car.isDetailed = false;
      else
        car.isDetailed = true;
    };
    $scope.isLoading = true;
    $scope.itemsByPage = 10;
    $scope.callServer = function (tableState) {
      if ($scope.isLoading) {
        dealerService.getDealerCars($scope.user.dealerId).then(function (dealerCars) {
          initFilter($scope, dealerCars, ['year', 'make', 'model', 'bodyType', 'styleTrim', 'drivetrain']);
          console.log(dealerCars.length);
          tableState.pagination.numberOfPages = Math.ceil(dealerCars.length / $scope.itemsByPage);
          $scope.dealerCars = dealerCars;
          $scope.displayCollection = dealerCars.slice(0, $scope.itemsByPage);
          $scope.isLoading = false;
        });
      }
      else {
        var start = tableState.pagination.start || 0;
        var filtered = $scope.dealerCars;
        if (tableState.search.predicateObject) {
          filtered = $filter('filter')($scope.dealerCars, tableState.search.predicateObject)
        }

        if (tableState.sort.predicate) {
          filtered = $filter('orderBy')(filtered, tableState.sort.predicate, tableState.sort.reverse);
        }

        var result = filtered.slice(start, start + $scope.itemsByPage);
        tableState.pagination.numberOfPages = Math.ceil(filtered.length / $scope.itemsByPage);
        $scope.displayCollection = result;
      }
    };
  }
}

DealerCtrl.$inject = ['$scope', 'highchartsNG', 'dealerService', 'uiGridConstants', '$filter', 'accountService'];

export default DealerCtrl;

