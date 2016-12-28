angular
    .module("carWidgetsPanelModule", [])
    .directive('carWidgetsPanel', function () {
        return {
            restrict: 'E',
            templateUrl: '/app/directives/car-widgets-panel/car-widgets-panel.html'
        }
    })