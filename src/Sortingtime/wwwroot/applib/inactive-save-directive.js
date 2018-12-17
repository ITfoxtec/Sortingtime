(function () {
    'use strict';

    angular
        .module('app')
        .directive('inactiveSave', inactiveSaveDirective);

    inactiveSaveDirective.$inject = ['$timeout'];

    function inactiveSaveDirective($timeout) {
        return {
            restrict: 'C',
            require: ['ngModel', '^?groupTaskDropdown'],
            scope: {
                setFocus: '@', // text
                model: '=ngModel',
            },
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0], eventBinded = false,
                    groupTaskDropdown = ctrls[1],
                    debugLog = false;

                //EVENTS
                element.bind("focus", function (event) {
                    if (debugLog) console.log("inactiveSaveDirective; event focus");
                    scope.bindEvents();
                    if (groupTaskDropdown) {
                        event.stopPropagation();
                        groupTaskDropdown.open();
                    }
                });

                element.bind("click", function (event) {
                    if (debugLog) console.log("inactiveSaveDirective; event click");
                    if (groupTaskDropdown) {
                        event.stopPropagation();
                    }
                });

                //SCOPE FUNCTIONS 
                scope.bindEvents = function () {
                    if (debugLog) console.log('inactiveSaveDirective; bindEvents');
                    if (!eventBinded) {
                        eventBinded = true;

                        scope.$watch('model', function ngModelWatch(value) {
                            ngModel.setServerErrorValidity(true);
                        });

                        element.bind("keydown", function (event) {
                            if (event.which == 13) {
                                if (debugLog) console.log("inactiveSaveDirective; keydown = 13");
                                if (ngModel.$valid) {
                                    event.preventDefault();
                                    event.stopPropagation();
                                    if (groupTaskDropdown) {
                                        groupTaskDropdown.save();
                                    }
                                    element.blur();
                                }
                            }
                            else if (event.which == 27) {
                                if (debugLog) console.log("inactiveSaveDirective; keydown = 27");
                                event.preventDefault();
                                event.stopPropagation();
                                if (groupTaskDropdown) {
                                    groupTaskDropdown.cancel();
                                }
                                event.target.blur();
                            }
                        });
                    }
                }

                //PUBLIC FUNCTIONS
                ngModel.setServerErrorValidity = function (isValid, error) {
                    if (isValid) {
                        ngModel.serverError = '';
                    }
                    else {
                        ngModel.serverError = error.join ? error.join(', ') : error;
                    }
                    ngModel.$setValidity('serverError', isValid);
                }

                ngModel.setModalValue = function (value) {
                    if (debugLog) console.log('inactiveSaveDirective; setModalValue, ModalValue: ' + value);
                    scope.model = value;
                    scope.$apply();
                }

                // PRIVATE FUNCTIONS

                if (scope.setFocus === "true") {
                    $timeout(function () {
                        element.focus();
                    }, 0);
                    
                }
            },
        };
    }

})();