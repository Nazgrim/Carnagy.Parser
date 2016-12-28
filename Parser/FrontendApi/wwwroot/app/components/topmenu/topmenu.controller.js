'use strict';

class TopMenuCtrl {
    constructor($scope, accountService) {
        $scope.user = accountService.getCurrentUser();       
        $(function () {
            $('#menu_toggle').click(function () {
                if ($('body').hasClass('nav-md')) {
                    $('body').removeClass('nav-md').addClass('nav-sm');
                    $('.left_col').removeClass('scroll-view').removeAttr('style');
                    $('.sidebar-footer').hide();

                    if ($('#sidebar-menu li').hasClass('active')) {
                        $('#sidebar-menu li.active').addClass('active-sm').removeClass('active');
                    }
                } else {
                    $('body').removeClass('nav-sm').addClass('nav-md');
                    $('.sidebar-footer').show();

                    if ($('#sidebar-menu li').hasClass('active-sm')) {
                        $('#sidebar-menu li.active-sm').addClass('active').removeClass('active-sm');
                    }
                }
            });
        });
    }
}

TopMenuCtrl.$inject = ['$scope', 'accountService'];

export default TopMenuCtrl;
