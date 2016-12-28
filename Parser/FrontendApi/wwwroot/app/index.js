'use strict';

import SidebarCtrl from '../app/components/sidebar/sidebar.controller';
import TopMenuCtrl from '../app/components/topmenu/topmenu.controller';
import DealerCtrl from '../app/dealers/dealer.controller';
import CarCtrl from '../app/dealers/car.controller';
import ModalSimilarCarCtrl from '../app/dealers/modalSimilarCar.controller';
import DashBoardCtrlfrom from '../app/dealers/dashboard.controller';

angular.module('carnagy', ['ui.router', 'ngAnimate', 'ngCookies', 'ngSanitize', 'ngResource', 'ui.bootstrap', 'ui.select', 'highcharts-ng',
    'dealerModule', 'similarCarModule',
    'carChartModule', 'powerBiModule',
    'carWidgetsPanelModule', 'carInformationModule',
    'carModule', 'priceTrendModule', 'dealerCompetitorsModule', 'errSrcModule', 'ui.grid', 'ui.grid.edit',
    'smart-table', 'accountModule', 'dealerHeaderModule','cgBusy'])
    .controller('SidebarCtrl', SidebarCtrl)
    .controller('DealerCtrl', DealerCtrl)
    .controller('CarCtrl', CarCtrl)
    .controller('TopMenuCtrl', TopMenuCtrl)
    .controller('ModalSimilarCarCtrl', ModalSimilarCarCtrl)
    .controller('DashBoardCtrlfrom', DashBoardCtrlfrom)
    .filter('myStrictFilter', function ($filter) {
        return function (input, predicate) {
            return $filter('filter')(input, predicate, true);
        }
    })
    .filter('unique1', function () {
        return function (arr, field) {
            var o = {}, i, l = arr.length, r = [];
            for (i = 0; i < l; i += 1) {
                o[arr[i][field]] = arr[i];
            }
            for (i in o) {
                r.push(o[i]);
            }
            return r;
        };
    })
    .directive('iframeOnload', [function(){
        return {
            scope: {
                callBack: '&iframeOnload'
            },
            link: function(scope, element, attrs) {
                element.on('load', function() {
                    return scope.callBack();
                });
            }
        }}])
    .config(function ($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('car', {
                url: '/car/:carId/:stockCarId',
                resolve: {
                    carId: function ($stateParams) {
                        return $stateParams.carId
                    },
                    stockCarId: function ($stateParams) {
                        return $stateParams.stockCarId
                    },
                },
                templateUrl: 'app/dealers/car.html',
                controller: 'CarCtrl',
            })
             .state('dashboard', {
                url: '/dashboard',
                templateUrl: 'app/dealers/dashboard.html',
                controller: 'DashBoardCtrlfrom',
            })
            .state('dealer', {
                url: '/dealer',
                // resolve: {
                //     dealer: function ($stateParams, dealerService) {
                //         return dealerService.getDealerById();
                //     },
                // },
                templateUrl: 'app/dealers/dealer.html',
                controller: 'DealerCtrl',
            });
        $urlRouterProvider.otherwise('/dealer');
    });
