'use strict';

class SidebarCtrl {
    constructor($scope, accountService) {
        $scope.user = accountService.getCurrentUser();
        $(function () {
            $('#sidebar-menu li ul').slideUp();

            $('#sidebar-menu li').on('click', function () {
                var link = $('a', this).attr('href');

                if (link) {
                    window.location.href = link;
                } else {
                    if ($(this).is('.active')) {
                        $(this).removeClass('active');
                        $('ul', this).slideUp();
                    } else {
                        $('#sidebar-menu li').removeClass('active');
                        $('#sidebar-menu li ul').slideUp();

                        $(this).addClass('active');
                        $('ul', this).slideDown();
                    }
                }
            });
        });
    }
}

SidebarCtrl.$inject = ['$scope', 'accountService'];

export default SidebarCtrl;
