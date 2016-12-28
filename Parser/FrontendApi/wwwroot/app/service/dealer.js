angular
    .module("dealerModule", []);
angular
    .module("dealerModule")
    .config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.useXDomain = true;
        $httpProvider.defaults.withCredentials = true;
        delete $httpProvider.defaults.headers.common["X-Requested-With"];
        $httpProvider.defaults.headers.common["Accept"] = "application/json";
        $httpProvider.defaults.headers.common["Content-Type"] = "application/json";
    }
    ])
    .service("dealerService", function ($resource) {
        return {
            getDealerCars: function (dealerId) {
                var Dealer = $resource('/dealer/dealercars/:delearId', { delearId: '@id' });
                return Dealer.query({ delearId: dealerId }, function (dealerCars) {
                    dealerCars.forEach(function (car) {
                        car.imagePath = "/images/cars/" + car.id + ".jpg";;
                    });
                }).$promise;
            },
            getDealerInfomation: function (dealerId) {
                var Dealer = $resource('/dealer/dealerInformation/:id', { id: '@id' });
                return Dealer.get({ id: dealerId }).$promise;
            }
        }
    });