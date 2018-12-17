(function () {
    'use strict';

    angular
        .module('app')
        .directive("timeFormat", timeFormat)

    timeFormat.$inject = ['$locale', '$filter'];

    function timeFormat($locale, $filter) {
        return {
            restrict: 'A',
            require: ['ngModel'], 
            compile: function (tElement, tAttrs) {
                if (tElement[0].nodeName !== 'INPUT') {
                    throw ('Error. timeFormat directive must be used inside an <input> element.');
                }            

                return function (scope, element, attrs, ctrls) {
                    var ngModel = ctrls[0],
                        debugLog = false;

                    var attrsTest = attrs;

                    var maxAttr = (attrs.hasOwnProperty('max') && attrs.max !== '') ? parseInt(attrs.max) : false,
                        minAttr = (attrs.hasOwnProperty('min') && attrs.min !== '') ? parseInt(attrs.min) : 0,
                        stepAttr = (attrs.hasOwnProperty('step') && attrs.step !== '') ? parseInt(attrs.step) : 1,
                        hideZeroAttr = (attrs.hasOwnProperty('hideZero') && (attrs.hideZero === 'true' || attrs.hideZero === 'True')) ? true : false;


                    //EVENTS
                    element.on('keydown', function (event) {
                        // Arrow key incrementation:
                        if (event.keyCode === 38 || event.keyCode === 40) {
                            event.preventDefault();
                            var step = (event.shiftKey) ? (stepAttr * 60) : stepAttr;
                            if (event.keyCode === 40) // Arrow down
                            {
                                step *= -1;
                            }

                            var newValue = (ngModel.$modelValue == null || isNaN(ngModel.$modelValue)) ? step : Number(ngModel.$modelValue) + step;

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

                        if (hideZeroAttr && modelValue == 0) {
                            return '';
                        }
                        else {
                            return $filter('time')(modelValue);
                        }
                    });

                    ngModel.$parsers.push(function (viewValue) {
                        if (debugLog) console.log("parsers.push viewValue: " + viewValue);

                        return convertToModelValue(viewValue);
                    });

                    // VALIDATORS
                    ngModel.$validators.time = function (modelValue, viewValue) {
                        if (debugLog) console.log("validators.time modelValue: " + modelValue + ", viewValue: " + viewValue);

                        if (viewValue === undefined || viewValue === null || viewValue === '') {
                            return true;
                        }
                        if (isValidTime(viewValue)) {
                            return true;
                        }
                        return false;
                    }; // end $validator time

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

                    //PRIVATE FUNCTIONS
                    var convertToModelValue = function (viewValue) {
                        if (viewValue && !angular.isString(viewValue)) {
                            viewValue = new String(viewValue);
                        }
                        if (viewValue) {
                            if (viewValue.indexOf(':') > -1) {
                                var valueArr = viewValue.split(':');
                                if (valueArr[1].length == 1) {
                                    valueArr[1] = valueArr[1] + '0';
                                }
                                if (valueArr[1].length > 2) {
                                    valueArr[1] = valueArr[1] / Math.pow(10, valueArr[1].length - 2);
                                }
                                viewValue = Number(valueArr[0]) * 60 + Number(valueArr[1]);
                            }
                            else if (viewValue.indexOf($locale.NUMBER_FORMATS.DECIMAL_SEP) > -1) {                                
                                var valueArr = viewValue.split($locale.NUMBER_FORMATS.DECIMAL_SEP);
                                if (valueArr[1].length == 1) {
                                    valueArr[1] = valueArr[1] + '0';
                                }
                                viewValue = Number(valueArr[0]) * 60 + Number(valueArr[1]) * 60 / 100;
                            }
                            else {
                                viewValue = Number(viewValue) * 60;
                            }
                        }
                        return Math.round(viewValue);
                    }

                    var isValidTime = function (value) {
                        if (timeOrNumberRegExp().test(value)) {
                            return true;
                        }
                        return false;
                    }

                    var timeOrNumberRegExp = function () {
                        return new RegExp("^\\s*((\\d+|(\\d*(\\:\\d*)))|(\\d+|(\\d*(\\" + $locale.NUMBER_FORMATS.DECIMAL_SEP + "\\d*))))\\s*");
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
