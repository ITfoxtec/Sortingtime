/// <reference path="../lib/angular-bootstrap/template/popup.html" />
(function () {
    'use strict';

    angular
        .module('app')
        .directive("dateDaySelect", dateDaySelect)

    dateDaySelect.$inject = ['$compile'];

    function dateDaySelect($compile) {
        var dayMonthYearTemplate = '<div ng-form="dateCursorForm" class="date-select">' +
                    '<div class="input-group">' +
                        '<span class="input-group-btn" style="vertical-align: bottom">' +
                            '<button class="btn btn-invisible glyphicon glyphicon-menu-left" type="button" translate-once-title="GENERAL.DATE_SELECT.BACK" ng-click="back()" />' +
                            '<button class="btn btn-invisible glyphicon glyphicon-calendar" type="button" translate-once-title="GENERAL.DATE_SELECT.SELECT" ng-click="toggleDatepicker($event)" />' +
                            '<input type="text" style="position: absolute; bottom: 0px; left: 37px; padding: 0px; margin: 0px; line-height: 0px; height: 0px; width: 0px; visibility: hidden" uib-datepicker-popup show-button-bar="true" datepicker-popup-template-url="ui/template/datepicker/popup.html" current-text="{{\'GENERAL.DATE_SELECT.TODAY\'|translate}}" is-open="openDatepicker" ng-model="hiddenDateModel" />' +
                        '</span>' +
                        '<strong>' +
                            '<input name="dateCursor" type="text" class="form-control text-center strong" active-save="updateDateCursor" active-save-mode="instant" date-format required ng-model="model" />' +
                        '</strong>' +
                        '<span class="input-group-btn">' +
                            '<button class="btn btn-invisible glyphicon glyphicon-record" type="button" translate-once-title="GENERAL.DATE_SELECT.TODAY" ng-click="today()" />' +
                            '<button class="btn btn-invisible glyphicon glyphicon-menu-right" type="button" translate-once-title="GENERAL.DATE_SELECT.FORWARD" ng-click="forward()" />' +
                        '</span>' +
                    '</div>' +
                    '<field-error class="text-center" field="dateCursorForm.dateCursor" />' +
                  '</div>';

        return {
            restrict: 'E',
            replace: true,
            scope: {
                update: '&', // one-way 
                model: '=ngModel',
            },
            link: function (scope, element, attrs) {
                element.html(dayMonthYearTemplate).show();
                $compile(element.contents())(scope);

                var ngModelDateCursor = scope.dateCursorForm.dateCursor;

                //EVENTS
                scope.$watch('hiddenDateModel', function ngModelWatch(newModelValue, oldModelValue) {
                    if (updateModel(newModelValue, scope.model)) {
                        setDate(newModelValue);
                    }
                });

                scope.$watch('model', function ngModelWatch(newModelValue, oldModelValue) {
                    if (updateModel(newModelValue, scope.hiddenDateModel)) {
                        scope.hiddenDateModel = newModelValue;
                    }
                });

                //SCOPE VARIABLES
                scope.openDatepicker = false;

                //SCOPE FUNCTIONS
                scope.updateDateCursor = function (successCallBack, errorCallBack) {
                    scope.update()(successCallBack, errorCallBack);
                }

                scope.back = function () {
                    if (scope.model === null || scope.model === undefined) {
                        scope.today();
                    }
                    else {
                        setDate(new Date(scope.model).addDays(-1));
                    }
                }

                scope.forward = function () {
                    if (scope.model === null || scope.model === undefined) {
                        scope.today();
                    }
                    else {
                        setDate(new Date(scope.model).addDays(1));
                    }
                }

                scope.today = function () {
                    setDate(new Date().toDateOnly());
                }

                scope.toggleDatepicker = function ($event) {
                    $event.preventDefault();
                    //$event.stopPropagation();

                    if (scope.openDatepicker) {
                        scope.openDatepicker = false;
                    }
                    else {
                        scope.openDatepicker = true;
                    }
                }

                //PRIVATE FUNCTIONS
                var setDate = function (date) {
                    if (!ngModelDateCursor.setModalDateAndDirty) {
                        throw ('Error. Missing the method setModalDateAndDirty. Probably dateFormat directive is required.');
                    }
                    if (scope.model != date) {
                        ngModelDateCursor.setModalDateAndDirty(date);
                    }
                }

                var updateModel = function (newValue, oldValue) {
                    if (newValue === undefined || newValue === null) {
                        return false;
                    }

                    if (angular.isDate(newValue) && angular.isDate(oldValue)) {
                        if (newValue.getTime() != oldValue.getTime()) {
                            return true
                        }
                    }
                    else if (angular.isDate(newValue) && !angular.isDate(oldValue)) {
                        return true;
                    }

                    return false;
                }

            },
        };
    }

})();
