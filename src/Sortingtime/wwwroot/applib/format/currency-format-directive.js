(function () {
    'use strict';

    angular
        .module('app')
        .directive("currencyFormat", currencyFormat)

    currencyFormat.$inject = ['$locale', '$filter'];

    function currencyFormat($locale, $filter) {
        return {
            restrict: 'A',
            require: ['ngModel'], 
            compile: function (tElement, tAttrs) {
                if (tElement[0].nodeName !== 'INPUT') {
                    throw ('Error. currencyFormat directive must be used inside an <input> element.');
                }            

                return function (scope, element, attrs, ctrls) {
                    var ngModel = ctrls[0],
                        debugLog = false;

                    var attrsTest = attrs;

                    var maxAttr = (attrs.hasOwnProperty('max') && attrs.max !== '') ? parseInt(attrs.max) : 9999999,
                        minAttr = (attrs.hasOwnProperty('min') && attrs.min !== '') ? parseInt(attrs.min) : 0,
                        stepAttr = (attrs.hasOwnProperty('step') && attrs.step !== '') ? parseInt(attrs.step) : 1,
                        fractionSizeAttr = (attrs.hasOwnProperty('fractionSize') && !isNaN(attrs.fractionSize)) ? parseInt(attrs.fractionSize) : false,
                        hideZeroAttr = (attrs.hasOwnProperty('hideZero') && (attrs.hideZero === 'true' || attrs.hideZero === 'True')) ? true : false;


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
                                return $filter('currency')(modelValue);
                            }
                            else {
                                return $filter('currency')(modelValue, fractionSizeAttr);
                            }
                        }
                    });

                    ngModel.$parsers.push(function (viewValue) {
                        if (debugLog) console.log("parsers.push viewValue: " + viewValue);

                        return convertToModelValue(viewValue);
                    });

                    // VALIDATORS
                    ngModel.$validators.currency = function (modelValue, viewValue) {
                        if (debugLog) console.log("validators.currency modelValue: " + modelValue + ", viewValue: " + viewValue);

                        if (viewValue === undefined || viewValue === null || viewValue === '') {
                            return true;
                        }
                        if (isValidCurrency(viewValue)) {
                            return true;
                        }
                        return false;
                    }; // end $validator currency

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
                            viewValue = viewValue.replace($locale.NUMBER_FORMATS.CURRENCY_SYM, '');
                            viewValue = viewValue.replace(/\s/g, '');

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

                    var isValidCurrency = function (value) {
                        if (value) {
                            value = value.replace($locale.NUMBER_FORMATS.GROUP_SEP, '');

                            if (currencyRegExp().test(value)) {
                                return true;
                            }
                        }
                        return false;
                    }

                    var currencyRegExp = function () {
                        var numberPattern = "\\s*(\\d*|(\\d*(\\" + $locale.NUMBER_FORMATS.DECIMAL_SEP + "\\d*)))\\s*";
                        var currencySym = $locale.NUMBER_FORMATS.CURRENCY_SYM;
                        if (currencySym == "$") {
                            currencySym = "\\$";
                        }
                        if ($locale.NUMBER_FORMATS.PATTERNS[1].posPre) {
                            // pre  currency
                            return new RegExp("^\\s*(" + currencySym + ")?" + numberPattern + "$");
                        }
                        else {
                            // post currency 
                            return new RegExp("^" + numberPattern + "(" + currencySym + ")?\\s*$");
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
