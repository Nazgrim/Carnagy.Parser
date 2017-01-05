'use strict';

class SidebarCtrl {
    constructor($scope, accountService) {
        $scope.user = accountService.getCurrentUser();
        $(function () {
            $('#sidebar-menu li ul').slideUp();
            
            $('#sidebar-menu li').on('click', function (e) {
                e.stopPropagation();
                $('#sidebar-menu li.current-page').removeClass('current-page');
                var link = $('a', this).attr('href');
                if(!link)
                {                
                    if ($(this).is('.active') ) {
                        $(this).removeClass('active');
                        $('ul', this).slideUp();
                    } else {
                        $('#sidebar-menu li').removeClass('active');
                        $('#sidebar-menu li ul').slideUp();

                        $(this).addClass('active');
                        $('ul', this).slideDown();                   
                    }
                }
                else {
                    if(!$(this).is('.buick'))
                    {
                        if ($(this).is('.active') ) {
                            $(this).removeClass('active');
                            $('ul', this).slideUp();
                        } else {
                            $('#sidebar-menu li').removeClass('active');
                            $('#sidebar-menu li ul').slideUp();

                            $(this).addClass('active');
                            $('ul', this).slideDown();                   
                        }
                    }                   
                    $(this).addClass('current-page');
                }
            });

                     
        });
    }
}

SidebarCtrl.$inject = ['$scope', 'accountService'];

export default SidebarCtrl;
