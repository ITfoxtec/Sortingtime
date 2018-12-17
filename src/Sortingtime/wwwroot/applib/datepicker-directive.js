(function () {
    'use strict';

    angular
        .module('app')
        .directive('datepicker', datepickerDirective);

    //datepickerDirective.$inject = [];

    function datepickerDirective() {
        return {
            restrict: 'E',
            require: ['ngModel'],
            transclude: true,
            scope: {
                model: '=ngModel',
            },
            template: '<div class="input-group">' +
                        '<input type="text" style="position: absolute; bottom: 0px; left: 0px; padding: 0px; margin: 0px; line-height: 0px; height: 0px; width: 0px; visibility: hidden" uib-datepicker-popup show-button-bar="true" datepicker-popup-template-url="ui/template/datepicker/popup.html" current-text="{{\'GENERAL.DATE_SELECT.TODAY\'|translate}}" is-open="openDatepicker" ng-model="model" />' +
                        '<span ng-transclude></span>' +
                        '<span class="input-group-btn">' +
                            '<button  style="margin-right: 0px" class="btn btn-invisible glyphicon glyphicon-calendar" type="button" translate-once-title="GENERAL.DATE_SELECT.SELECT" ng-click="toggleDatepicker($event)" />' +
                        '</span>' +
                      '</div>',
            link: function (scope, element, attrs, ctrls) {

                //SCOPE VARIABLES
                scope.openDatepicker = false;

                //SCOPE FUNCTIONS       
                scope.toggleDatepicker = function ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    if (scope.openDatepicker) {
                        scope.openDatepicker = false;
                    }
                    else {
                        scope.openDatepicker = true;
                    }
                }
            },

        };
    }

})();
