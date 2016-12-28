angular
    .module("dealerCompetitorsModule", ['ui.bootstrap', 'carModule', 'errSrcModule'])
    .directive('dealerCompetitors', function () {
        return {
            restrict: 'E',
            templateUrl: '/app/directives/dealer-competitors/dealer-competitors.html',
            controller: ['$scope', 'carService', '$filter', function ($scope, carService, $filter) {
                var itemsByPage = 10;
                $scope.itemsByPage = itemsByPage;
                $scope.displayCollection = [];
                $scope.filterBy = [];
                $scope.showDetailed = function (car) {
                    if (car.isDetailed)
                        car.isDetailed = false;
                    else
                        car.isDetailed = true;
                };
                $scope.$on('selectSeriesDown', function (event, data) {
                    console.log('on');
                    $scope.filterBy = data;
                    $scope.itemsByPage = $scope.itemsByPage+1;
                });
                $scope.isLoading = true;
                
                $scope.callServer = function (tableState) {
                    if ($scope.isLoading) {
                        carService.getDealerCompetitors($scope.stockCarId)
                            .$promise
                            .then(function (dealerCars) {
                                dealerCars.forEach(function(car) {
                                    car.imagePath = "../images/cars/" + car.id + ".jpg";
                                    car.priceDifference = car.price - $scope.carInformation.dealerPrice.value;
                                    if (car.isDealerCar) {
                                        console.log(car);
                                    }
                                });
                                tableState.pagination.numberOfPages = Math.ceil(dealerCars.length / itemsByPage);
                                $scope.dealerCars = dealerCars;
                                $scope.displayCollection = dealerCars.slice(0, itemsByPage);
                                $scope.isLoading = false;
                            });
                    }
                    else {
                        var start = tableState.pagination.start || 0;
                        var filtered = $scope.dealerCars.filter(function(letter) {
                            return $scope.filterBy.length == 0 || $scope.filterBy.indexOf(letter.id) !== -1;
                        });

                        var result = filtered.slice(start, start + itemsByPage);
                        tableState.pagination.numberOfPages = Math.ceil(filtered.length / itemsByPage);
                        $scope.displayCollection = result;
                    }
                };
            }]
        }
    })