angular
    .module("powerBiModule", []);
angular
    .module("powerBiModule")
    .config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.useXDomain = true;
        $httpProvider.defaults.withCredentials = true;
        delete $httpProvider.defaults.headers.common["X-Requested-With"];
        $httpProvider.defaults.headers.common["Accept"] = "application/json";
        $httpProvider.defaults.headers.common["Content-Type"] = "application/json";
    }
    ])
    .service("powerBiService", function ($resource) {
        var baseUrl = '/dashBoard';
        return {
            getDashboard: function (accessToken) {
                var Dashboard = $resource(baseUrl + '/dashBoardUrl/:token', { accessToken: '@accessToken' });
                return Dashboard.get({ accessToken: accessToken }).$promise;
            },
            getToken: function () {
                var Dashboard = $resource(baseUrl + '/token');
                return Dashboard.get().$promise;
            },
        }
    });