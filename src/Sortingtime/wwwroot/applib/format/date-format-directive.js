(function () {
    'use strict';

    angular
        .module('app')
        .directive("dateFormat", dateFormat)

    dateFormat.$inject = ['$filter', '$dateParser'];

    function dateFormat($filter, $dateParser) {
        return {
            restrict: 'A',
            require: ['ngModel'],
            compile: function (tElement, tAttrs) {
                if (tElement[0].nodeName !== 'INPUT') {
                    throw ('Error. numberFormat directive must be used inside an <input> element.');
                }

                return function (scope, element, attrs, ctrls) {
                    var ngModel = ctrls[0],
                        debugLog = false;

                    var maxAttr = (attrs.hasOwnProperty('max') && attrs.max !== '') ? new Date(attrs.max) : false,
                        minAttr = (attrs.hasOwnProperty('min') && attrs.min !== '') ? new Date(attrs.min) : false,
                        stepAttr = (attrs.hasOwnProperty('step') && attrs.step !== '') ? parseInt(attrs.step) : 1,
                        format = (attrs.hasOwnProperty('format') && attrs.format !== '') ? attrs.format : false;

                    //EVENTS
                    element.on('keydown', function (event) {
                        // Arrow key incrementation:
                        if (event.keyCode === 38 || event.keyCode === 40) {
                            event.preventDefault();
                            var step = stepAttr;
                            if (event.keyCode === 40) // Arrow down
                            {
                                step *= -1;
                            }

                            var newValue;
                            if(ngModel.$modelValue == null || !angular.isDate(ngModel.$modelValue)) {
                                newValue = new Date();
                            }
                            else {
                                if (event.shiftKey || format == 'month') {
                                    newValue = new Date(ngModel.$modelValue).addMonths(step);
                                }
                                else {
                                    newValue = new Date(ngModel.$modelValue).addDays(step);
                                }
                            }

                            if (maxAttr !== false && newValue > maxAttr) {
                                newValue = maxAttr;
                            }
                            else if (minAttr !== false && newValue < minAttr) {
                                newValue = minAttr;
                            }

                            if (debugLog) console.log("keydown newValue: " + newValue);
                            ngModel.$setDirty();
                            if (!ngModel.setModalValue) {
                                throw ('Error. Missing the method setModalValue. Probably either activeSave or inactiveSave directive is required.');
                            }
                            ngModel.setModalValue(newValue);
                        }
                    }); // end on keydown

                    element.bind("blur", function (event) {
                        if (debugLog) console.log("event blur");
                        if (ngModel.$valid) {
                            reformatViewValue();
                        }
                    });

                    // FORMATTERS AND PARSERS
                    ngModel.$formatters.push(function (modelValue) {
                        if (debugLog) console.log("formatters.push modelValue: " + modelValue);

                        return $filter('date')(modelValue, getFormat());
                    });

                    ngModel.$parsers.push(function (viewValue) {
                        if (debugLog) console.log("parsers.push viewValue: " + viewValue);

                        if (viewValue && format == 'month') {
                            viewValue = '01 ' + viewValue;
                        }
                        var modelValue = $dateParser(viewValue, getFormat(format == 'month'));
                        //if (modelValue !== undefined && format == 'month') {
                        //    modelValue = modelValue.toDateOnlyFirstDayInMonth();
                        //}                        
                        return modelValue !== undefined ? modelValue : null;
                    });

                    // VALIDATORS
                    ngModel.$validators.date = function (modelValue, viewValue) {
                        if (debugLog) console.log("validators.date modelValue: " + modelValue + ", viewValue: " + viewValue);

                        if (viewValue === undefined || viewValue === null || viewValue === '') {
                            return true;
                        }
                        if (viewValue && format == 'month') {
                            viewValue = '01 ' + viewValue;
                        }
                        var date = $dateParser(viewValue, getFormat(format == 'month'));
                        if (date !== undefined) {
                            return true;
                        }
                        return false;
                    }; // end $validator number

                    ngModel.$validators.min = function (modelValue, viewValue) {
                        if (debugLog) console.log("validators.min modelValue: " + modelValue + ", viewValue: " + viewValue);

                        if (minAttr !== false && modelValue < minAttr) {
                            return false;
                        }
                        return true;
                    }; // end $validator range

                    ngModel.$validators.max = function (modelValue, viewValue) {
                        if (debugLog) console.log("validators.max modelValue: " + modelValue + ", viewValue: " + viewValue);

                        if (maxAttr !== false && modelValue > maxAttr) {
                            return false;
                        }
                        return true;
                    }; // end $validator range

                    //PUBLIC FUNCTIONS
                    ngModel.setFormat = function (formatValue) {
                        if (formatValue == 'month') {
                            format = 'month';
                        }
                        else {
                            format = false;
                        }
                        if (ngModel.$valid) {
                            reformatViewValue();
                        }
                    }

                    ngModel.setModalDateAndDirty = function (date) {
                        if (debugLog) console.log('setModalDate, ModalValue: ' + date);
                        scope.model = date;
                        ngModel.$setDirty();
                    }

                    ngModel.setModalDateFromAndDirty = function (date, isNotDirty) {
                        if (debugLog) console.log('setModalDateTo, ModalValue: ' + date);
                        scope.model.from = date;
                        if (!isNotDirty) {
                            ngModel.$setDirty();
                        }
                    }

                    ngModel.setModalDateToAndDirty = function (date, isNotDirty) {
                        if (debugLog) console.log('setModalDateFrom, ModalValue: ' + date);
                        scope.model.to = date;
                        if (!isNotDirty) {
                            ngModel.$setDirty();
                        }
                    }

                    //PRIVATE FUNCTIONS
                    var getFormat = function (includeDayInMonth) {                        
                        if (format === false) {
                            return 'mediumDate';
                        }
                        else if (format == 'month') {
                            if (includeDayInMonth) {
                                return 'dd MMM y';
                            }
                            else {
                                return 'MMM y';
                            }
                        }
                        else {
                            return format;
                        }

                    }

                    var reformatViewValue = function () {
                        var formatters = ngModel.$formatters,
                            idx = formatters.length;

                        var viewValue = ngModel.$$rawModelValue;
                        while (idx--) {
                            viewValue = formatters[idx](viewValue);
                        }

                        ngModel.$setViewValue(viewValue);
                        ngModel.$render();
                    }

                };  // end link function
            }, // end compile function
        };
    }

})();
