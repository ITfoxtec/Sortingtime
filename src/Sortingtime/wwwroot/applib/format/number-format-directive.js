(function () {
    'use strict';

    angular
        .module('app')
        .directive("numberFormat", numberFormat)

    numberFormat.$inject = ['$locale', '$filter'];

    function numberFormat($locale, $filter) {
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

                    var attrsTest = attrs;

                    var maxAttr = (attrs.hasOwnProperty('max') && attrs.max !== '') ? parseInt(attrs.max) : false,
                        minAttr = (attrs.hasOwnProperty('min') && attrs.min !== '') ? parseInt(attrs.min) : false,
                        stepAttr = (attrs.hasOwnProperty('step') && attrs.step !== '') ? parseInt(attrs.step) : 1,
                        fractionSizeAttr = (attrs.hasOwnProperty('fractionSize') && !isNaN(attrs.fractionSize)) ? parseInt(attrs.fractionSize) : false,
                        hideZeroAttr = (attrs.hasOwnProperty('hideZero') && (attrs.hideZero === 'true' || attrs.hideZero === 'True')) ? true : false,
                        wholeNumberAttr = (attrs.hasOwnProperty('wholeNumber') && (attrs.wholeNumber === 'true' || attrs.wholeNumber === 'True')) ? true : false;


                    //EVENTS
                    element.on('keydown', function (event) {
                        // Arrow key incrementation:
                        if (event.keyCode === 38 || event.keyCode === 40) {
                            event.preventDefault();
                            var step = (event.shiftKey) ? (stepAttr * 10) : stepAttr;
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
                            if (fractionSizeAttr == false) {
                                return $filter('number')(modelValue);
                            }
                            else {
                                return $filter('number')(modelValue, fractionSizeAttr);
                            }
                        }
                    });

                    ngModel.$parsers.push(function (viewValue) {
                        if (debugLog) console.log("parsers.push viewValue: " + viewValue);

                        return convertToModelValue(viewValue);
                    });

                    // VALIDATORS
                    ngModel.$validators.number = function (modelValue, viewValue) {
                        if (debugLog) console.log("validators.number modelValue: " + modelValue + ", viewValue: " + viewValue);

                        if (viewValue === undefined || viewValue === null || viewValue === '') {
                            return true;
                        }
                        if (isValidNumber(viewValue)) {
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


                    ngModel.$validators.wholeNumber = function (modelValue, viewValue) {
                        if (debugLog) console.log("validators.wholeNumberr modelValue: " + modelValue + ", viewValue: " + viewValue);

                        if (wholeNumberAttr !== false) {
                            if (modelValue % 1 == 0) {
                                if (new RegExp(/\./g).test(modelValue)) {
                                    return false;
                                }
                                return true;
                            } else {
                                return false;
                            }
                        }

                        return true;
                    }; 

                    //PRIVATE FUNCTIONS
                    var convertToModelValue = function (viewValue) {
                        if (viewValue && !angular.isString(viewValue)) {
                            viewValue = new String(viewValue);
                        }
                        if (viewValue) {
                            if ($locale.NUMBER_FORMATS.DECIMAL_SEP === ',') {
                                // Remove GROUP_SEP
                                viewValue = viewValue.replace(/\./g, '');
                                // Change DECIMAL_SEP
                                viewValue = viewValue.replace(/,/g, '.');
                            }
                            if ($locale.NUMBER_FORMATS.GROUP_SEP === ',') {
                                // Remove GROUP_SEP
                                viewValue = viewValue.replace(/,/g, '');
                            }
                        }
                        return viewValue;
                    }

                    var isValidNumber = function (value) {
                        if (value) {
                            value = value.replace($locale.NUMBER_FORMATS.GROUP_SEP, '');
                        }
                        if (numberRegExp().test(value)) {
                            return true;
                        }
                        return false;
                    }

                    var numberRegExp = function () {
                        if ($locale.NUMBER_FORMATS.DECIMAL_SEP === '.') {
                            return /^\s*(\-|\+)?(\d+|(\d*(\.\d*)))\s*$/;
                        }
                        else {
                            return /^\s*(\-|\+)?(\d+|(\d*(\,\d*)))\s*$/;
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
