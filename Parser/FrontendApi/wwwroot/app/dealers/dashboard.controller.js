'use strict';
class DashBoardCtrl {
    constructor($scope, $sce, powerBiService) {
        var iframe = $("#iFrameEmbedDashboard");
        if (iframe) {
            iframe.css("min-height", $(window).height()-88);
        }
        var token;
        $scope.iframeLoadedCallBack = function(){
            var m = { action: "loadDashboard", accessToken: token.access_token
            };
            var message = JSON.stringify(m);

            // push the message.
            var iframe = document.getElementById('iFrameEmbedDashboard');
            iframe.contentWindow.postMessage(message, "*");
        }
        powerBiService.getToken().then(function(token1) {
            token = token1;
            powerBiService.getDashboard(token.access_token).then(function(dasboard) {
                $scope.url = $sce.trustAsResourceUrl(dasboard.embedUrl);
            });            
        });
    }
}

DashBoardCtrl.$inject = ['$scope', '$sce', 'powerBiService'];

export default DashBoardCtrl;