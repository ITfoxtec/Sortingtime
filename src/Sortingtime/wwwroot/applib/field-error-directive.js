(function () {
    'use strict';

    angular
        .module('app')
        .directive('fieldError', fieldErrorDirective);

    //fieldErrorDirective.$inject = [];

    function fieldErrorDirective() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                field: '=field',
            },
            template: '<div><div class="error-block" ng-messages="field.$error" ng-if="field.$dirty">' +
                          '<div ng-messages-include="error-messages"></div>' +
                          '<div ng-message="serverError">{{field.serverError}}</div>' +
                      '</div></div>',

        };
    }

})();
