(function () {
    'use strict';

    // https://www.pointblankdevelopment.com.au/blog/getting-the-bootstrap-3-navbar-and-angularjs-to-play-nicely-together

    angular
        .module('app')
        .controller('navController', navController);

    navController.$inject = ['$scope', '$location'];

    function navController($scope, $location) {
        var self = this;

        // PRIVATE

        // PUBLIC
        self.getClass = function (path) {
            if (path === '/') {
                if ($location.path() === '/') {
                    return "active";
                } else {
                    return "";
                }
            }

            if ($location.path().substr(0, path.length) === path) {
                return "active";
            } else {
                return "";
            }
        };

        // EVENT
        self.isCollapsed = true;
        $scope.$on('$routeChangeSuccess', function () {
            self.isCollapsed = true;
        });


        // PRIVATE FUNCTIONS
        var activate = function () {
        };
        activate();
    }
})();
