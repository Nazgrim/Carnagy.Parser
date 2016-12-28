angular
    .module("accountModule", []);
angular
    .module("accountModule")
    .service("accountService", function () {
        return {
            getCurrentUser: function () {
                return {
                    dealerId: 1,
                    name: 'John Doe',
                    authorImg: 'img.jpg'
                }
            }
        };
    });