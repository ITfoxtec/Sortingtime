(function () {
    'use strict';

    angular
        .module('app')
        .directive("modal", modalDialog);

    //modalDialog.$inject = [''];

    // http://jsfiddle.net/alexsuch/RLQhh/

    function modalDialog() {
        return {
            template: '<div class="modal fade">' +
                '<div class="modal-dialog {{ mSizes ? mSizes : \'\' }}">' +
                  '<div class="modal-content">' +
                    '<div class="modal-header"> {{ mTitle }}' +
                        '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                    '</div>' +
                    '<div class="modal-body" ng-transclude></div>' +
                  '</div>' +
                '</div>' +
              '</div>',
            restrict: 'E',
            transclude: true,
            replace: true,
            scope: {
                mVisible: '=', // two-ways
                mTitle: '@', // text
                mSizes: '@', // text
            },
            link: function postLink(scope, element, attrs) {

                scope.$watch('mVisible', function () {
                    if (scope.mVisible == true)
                        $(element).modal('show');
                    else
                        $(element).modal('hide');
                });

                $(element).on('hidden.bs.modal', function () {
                    scope.$apply(function () {
                        scope.mVisible = false;
                    });
                });
            },
        };
    }

})();