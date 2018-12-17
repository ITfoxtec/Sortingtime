(function () {
    'use strict';

    angular
        .module('app')
        .controller('configController', configController);

    configController.$inject = ['$rootScope'];

    function configController($rootScope) {
        var self = this;

        // PRIVATE

        // PUBLIC

        // PUBLIC FUNCTIONS
        self.onUsersTabSelect = function () {
            $rootScope.$broadcast('configController.onUsersTabSelect', null);
        }

        // PRIVATE FUNCTIONS
        var activate = function () {
        }
        activate();
    }
})();
