/// <reference path="../lib/angular-bootstrap/template/popup.html" />
(function () {
    'use strict';

    angular
        .module('app')
        .directive("dateMonthSelect", dateMonthSelect)

    dateMonthSelect.$inject = ['$compile'];

    function dateMonthSelect($compile) {
        var monthYearTemplate = '<div ng-form="dateCursorForm" class="date-select">' +
                    '<div class="input-group">' +
                        '<span class="input-group-btn" style="vertical-align: bottom">' +
                            '<button ng-show="!dividedScale" class="btn btn-invisible glyphicon glyphicon-menu-left" type="button" translate-once-title="GENERAL.DATE_SELECT.BACK" ng-click="back()" />' +
                            '<button class="btn btn-invisible glyphicon glyphicon-calendar" type="button" translate-once-title="GENERAL.DATE_SELECT.SELECT" ng-click="toggleDatepickerFrom($event)" />' +
                            '<input type="text" style="position: absolute; bottom: 0px; left: 37px; padding: 0px; margin: 0px; line-height: 0px; height: 0px; width: 0px; visibility: hidden" uib-datepicker-popup datepicker-options="{minMode: \'month\', datepickerMode: \'month\'}" show-button-bar="true" datepicker-popup-template-url="ui/template/datepicker/popup.html" current-text="{{\'GENERAL.DATE_SELECT.TODAY\'|translate}}" is-open="openDatepickerFrom" ng-model="hiddenDateModelFrom" />' +
                            '<input type="text" style="position: absolute; bottom: 0px; left: 37px; padding: 0px; margin: 0px; line-height: 0px; height: 0px; width: 0px; visibility: hidden" uib-datepicker-popup show-button-bar="true" datepicker-popup-template-url="ui/template/datepicker/popup.html" current-text="{{\'GENERAL.DATE_SELECT.TODAY\'|translate}}" is-open="openDatepickerFromScale" ng-model="hiddenDateModelFrom" />' +
                        '</span>' +
                        '<strong>' +
                            '<input name="dateCursorFrom" type="text" class="form-control text-center strong" active-save="updateDateCursor" active-save-mode="instant" date-format format="month" required ng-model="model.from" />' +
                        '</strong>' +
                        '<span ng-show="dividedScale" class="input-group-btn" style="vertical-align: bottom">' +
                           '<span class="input-group-addon">-</span>' +
                        '</span>' +
                        '<span ng-show="dividedScale" class="input-group-btn" style="vertical-align: bottom">' +
                            '<button class="btn btn-invisible glyphicon glyphicon-calendar" type="button" translate-once-title="GENERAL.DATE_SELECT.SELECT" ng-click="toggleDatepickerTo($event)" />' +
                            '<input type="text" style="position: absolute; bottom: 0px; left: 37px; padding: 0px; margin: 0px; line-height: 0px; height: 0px; width: 0px; visibility: hidden" uib-datepicker-popup show-button-bar="true" datepicker-popup-template-url="ui/template/datepicker/popup.html" current-text="{{\'GENERAL.DATE_SELECT.TODAY\'|translate}}" is-open="openDatepickerTo" ng-model="hiddenDateModelTo" />' +
                        '</span>' +
                        '<strong ng-show="dividedScale">' +
                            '<input name="dateCursorTo" type="text" class="form-control text-center strong" active-save="updateDateCursorTo" active-save-mode="instant" date-format required ng-model="model.to" />' +
                        '</strong>' +
                        '<span class="input-group-btn">' +
                            '<button class="btn btn-invisible glyphicon glyphicon-scale" style="Display: none" type="button" translate-once-title="GENERAL.DATE_SELECT.CHANGE_SCALE" ng-click="changeScale()" />' +
                            '<button class="btn btn-invisible glyphicon glyphicon-record" type="button" translate-once-title="GENERAL.DATE_SELECT.TODAY" ng-click="today()" />' +
                            '<button ng-show="!dividedScale" class="btn btn-invisible glyphicon glyphicon-menu-right" type="button" translate-once-title="GENERAL.DATE_SELECT.FORWARD" ng-click="forward()" />' +
                        '</span>' +
                    '</div>' +
                    '<field-error class="text-center" field="dateCursorForm.dateCursorFrom" />' +
                    '<field-error class="text-center" field="dateCursorForm.dateCursorTo" />' +
                  '</div>';
        return {
            restrict: 'E',
            replace: true,
            scope: {
                update: '&', // one-way 
                dividedScale: '=', // text (one or two datepickers)
                model: '=ngModel',
            },
            link: function (scope, element, attrs) {
                element.html(monthYearTemplate).show();
                $compile(element.contents())(scope);

                var ngModelDateCursorFrom = scope.dateCursorForm.dateCursorFrom;
                var ngModelDateCursorTo = scope.dateCursorForm.dateCursorTo;

                //EVENTS
                scope.$watch('hiddenDateModelFrom', function ngModelWatch(newModelValue, oldModelValue) {
                    if (updateModel(newModelValue, scope.model.from)) {
                        setDateFrom(newModelValue);
                    }
                });
                scope.$watch('hiddenDateModelTo', function ngModelWatch(newModelValue, oldModelValue) {
                    if (updateModel(newModelValue, scope.model.to)) {
                        setDateTo(newModelValue);
                    }
                });

                scope.$watch('model.from', function ngModelWatch(newModelValue, oldModelValue) {
                    if (updateModel(newModelValue, scope.hiddenDateModelFrom)) {
                        scope.hiddenDateModelFrom = newModelValue;
                    }
                });
                scope.$watch('model.to', function ngModelWatch(newModelValue, oldModelValue) {
                    if (scope.dividedScale && updateModel(newModelValue, scope.hiddenDateModelTo)) {
                        scope.hiddenDateModelTo = newModelValue;
                    }
                });

                //SCOPE VARIABLES
                scope.openDatepickerFrom = false;
                scope.openDatepickerFromScale = false;
                scope.openDatepickerTo = false;

                //SCOPE FUNCTIONS
                scope.updateDateCursor = function (successCallBack, errorCallBack) {
                    scope.update()(successCallBack, errorCallBack);
                }
                scope.updateDateCursorTo = function (successCallBack, errorCallBack) {
                    scope.update()(successCallBack, errorCallBack);
                }

                scope.changeScale = function () {
                    var dividedScale = !scope.dividedScale;
                    if (!dividedScale) {
                        ngModelDateCursorFrom.setFormat('month');
                        setDateFrom(scope.model.from.toDateOnlyFirstDayInMonth(), true);
                    }
                    else {
                        ngModelDateCursorFrom.setFormat('day');
                    }

                    scope.dividedScale = dividedScale;
                }

                scope.back = function () {
                    if (scope.model.from === null || scope.model.from === undefined) {
                        scope.today();
                    }
                    else {
                        setDateFrom(new Date(scope.model.from.toDateOnlyFirstDayInMonth()).addMonths(-1), true);
                    }
                }

                scope.forward = function () {
                    if (scope.model.from === null || scope.model.from === undefined) {
                        scope.today();
                    }
                    else {
                        setDateFrom(new Date(scope.model.from.toDateOnlyFirstDayInMonth()).addMonths(1), true);
                    }
                }

                scope.today = function () {
                    var date = new Date();
                    setDateFrom(date.toDateOnlyFirstDayInMonth(), true);
                }

                scope.toggleDatepickerFrom = function ($event) {
                    $event.preventDefault();
                    //$event.stopPropagation();

                    if (!scope.dividedScale) {
                        if (scope.openDatepickerFrom) {
                            scope.openDatepickerFrom = false;
                        }
                        else {
                            scope.openDatepickerTo = false;
                            scope.openDatepickerFromScale = false;
                            scope.openDatepickerFrom = true;
                        }
                    }
                    else {
                        if (scope.openDatepickerFromScale) {
                            scope.openDatepickerFromScale = false;
                        }
                        else {
                            scope.openDatepickerTo = false;
                            scope.openDatepickerFrom = false;
                            scope.openDatepickerFromScale = true;
                        }
                    }
                }

                scope.toggleDatepickerTo = function ($event) {
                    $event.preventDefault();
                    //$event.stopPropagation();

                    if (scope.openDatepickerTo) {
                        scope.openDatepickerTo = false;
                    }
                    else {
                        scope.openDatepickerFrom = false;
                        scope.openDatepickerFromScale = false;
                        scope.openDatepickerTo = true;
                    }
                }

                //PRIVATE FUNCTIONS
                var setDateFrom = function (date, updateDateTo, isRelatedUpdate) {
                    if (!ngModelDateCursorFrom.setModalDateFromAndDirty) {
                        throw ('Error. Missing the method setModalDateAndDirty. Probably dateFormat directive is required.');
                    }
                    if (scope.model.from != date) {
                        if (!isRelatedUpdate) {
                            if (!scope.dividedScale || updateDateTo) {
                                setDateTo(date.toDateOnlyLastDayInMonth(), true);
                            }
                            else {
                                if (date < scope.model.to) {
                                    var testDateTo = new Date(date).addDays(30);
                                    if (scope.model.to > testDateTo) {
                                        setDateTo(testDateTo, true);
                                    }
                                }
                                else if (date > scope.model.to) {
                                    setDateTo(new Date(date).addDays(30), true);
                                }
                                else {
                                    setDateTo(new Date(date).addDays(1), true);
                                }
                            }
                        }

                        ngModelDateCursorFrom.setModalDateFromAndDirty(date, isRelatedUpdate);
                        scope.hiddenDateModelFrom = date;
                    }
                }
                var setDateTo = function (date, isRelatedUpdate) {
                    if (!ngModelDateCursorTo.setModalDateToAndDirty) {
                        throw ('Error. Missing the method setModalDateAndDirty. Probably dateFormat directive is required.');
                    }
                    if (scope.model.to != date) {
                        if (!isRelatedUpdate) {
                            if (scope.dividedScale) {
                                if (date > scope.model.from) {
                                    var testDateFrom = new Date(date).addDays(-30);
                                    if (scope.model.from < testDateFrom) {
                                        setDateFrom(testDateFrom, true);
                                    }
                                }
                                else if (date < scope.model.from) {
                                    setDateFrom(new Date(date).addDays(-30), true);
                                }
                                else {
                                    setDateFrom(new Date(date).addDays(-1), true);
                                }
                            }
                        }

                        ngModelDateCursorTo.setModalDateToAndDirty(date, isRelatedUpdate);
                        scope.hiddenDateModelTo = date;
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
