(function () {
    'use strict';

    angular
        .module('app')
        .directive('activeSave', activeSaveDirective);

    activeSaveDirective.$inject = ['notificationFactory', '$timeout'];

    function activeSaveDirective(notificationFactory, $timeout) {
        return {
            restrict: 'A',
            require: ['ngModel'],
            scope: {
                activeSave: '&', // one-way
                activeSaveData: '=', // two-ways
                activeSaveModel: '=', // two-ways
                activeSaveForm: '=', // two-ways
                activeSaveMode: '@', // text, default no value else instant
                focusEvent: '@', // text, default no value else instant
                model: '=ngModel',
            },
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0], eventBinded = false,
                    debugLog = false;

                var initValueBackup = true;
                var timeout = null, rbTimeout = null;
                var roleBackBackupModelValue = null, backupModelValue = null, lastSavedModelValue = null;
                var valueSelected = false;

                //EVENTS
                element.bind("focus", function (event) {
                    if (debugLog) console.log("activeSaveDirective; event focus");
                    if (scope.focusEvent === 'true') {
                        scope.activeSave()(null, null, getActiveSaveData(), null, null, 'focus');
                    }
                    scope.bindEvents();
                    //event.stopPropagation();
                    event.preventDefault();
                    rollbackModalOnError(true);
                    valueSelected = false;
                });

                //SCOPE FUNCTIONS 
                scope.bindModelEvent = function () {
                    scope.$watch('model', function ngModelWatch(newModelValue, oldModelValue) {
                        if (initValueBackup) {
                            if (debugLog) console.log('activeSaveDirective; initValueBackup');
                            backupModal(true);
                            initValueBackup = false;
                        }

                        ngModel.setServerErrorValidity(true);
                        if (!initValueBackup && (valueSelected || ngModel.$valid && ngModel.$dirty)) {
                            if (debugLog) console.log('activeSaveDirective; watch model newVal: ' + newModelValue + ', oldVal: ' + oldModelValue);
                            saveModal();
                        }
                    });
                }

                if (scope.activeSaveMode === 'instant') {
                    scope.bindModelEvent();
                }

                scope.bindEvents = function () {
                    if (debugLog) console.log('activeSaveDirective; bindEvents');
                    if (!eventBinded) {
                        eventBinded = true;

                        if (scope.activeSaveMode !== 'instant') {
                            scope.bindModelEvent();
                        }

                        element.bind("blur", function (event) {
                            if (debugLog) console.log("activeSaveDirective; event blur");
                            //event.stopPropagation();
                            event.preventDefault();
                            rollbackModalOnError();
                        });

                        element.bind("keydown", function (event) {
                            if (event.which === 13) {
                                if (debugLog) console.log("activeSaveDirective; keydown = 13");
                                if (element.context.tagName !== "TEXTAREA" || event.ctrlKey) {
                                    if (ngModel.$valid) {
                                        event.preventDefault();
                                        event.stopPropagation();
                                        element.blur();
                                    }
                                }
                            }
                            else if (event.which === 27) {
                                if (debugLog) console.log("activeSaveDirective; keydown = 27");
                                event.preventDefault();
                                event.stopPropagation();
                                rollbackModal();
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
                    if (debugLog) console.log('activeSaveDirective; setModalValue, ModalValue: ' + value);
                    scope.model = value;
                    scope.$apply();
                }

                //PRIVATE FUNCTIONS
                var getActiveSaveData = function () {
                    if (scope.activeSaveData !== undefined) {
                        return scope.activeSaveData;
                    }
                    else {
                        return { item: scope.activeSaveModel, form: scope.activeSaveForm }
                    }
                }

                var saveModal = function () {
                    if (debugLog) console.log("activeSaveDirective; saveModel");

                    if (timeout) {
                        $timeout.cancel(timeout);
                    }
                    timeout = $timeout(function () { handleSaveModal(); }, 0); // 1000 = 1 second
                }

                var handleSaveModal = function () {
                    if (valueSelected || ngModel.$valid && ngModel.$dirty) {
                        if (debugLog) console.log("activeSaveDirective; handleSaveModal do activeSave");
                        if (!valuesIsEqual(lastSavedModelValue, scope.model)) {
                            if (debugLog) console.log("activeSaveDirective; handleSaveModal do activeSave, saving");
                            lastSavedModelValue = angular.copy(scope.model);
                            scope.activeSave()(activeSaveSuccessCallBack, activeSaveErrorCallBack, getActiveSaveData());
                        }
                    }
                }

                var activeSaveSuccessCallBack = function (success) {
                    if (debugLog) console.log("activeSaveDirective; activeSaveSuccessCallBack");
                    backupModal(false);
                }

                var activeSaveErrorCallBack = function (error) {
                    if (debugLog) console.log("activeSaveDirective; activeSaveErrorCallBack");

                    if (error && error.data && angular.isObject(error.data)) {
                        for (var key in error.data) {
                            ngModel.setServerErrorValidity(false, error.data[key]);
                            error.isHandled = true;
                            break;
                        }
                    }
                    else {
                        notificationFactory.error(error)
                    }                    
                }

                var backupModal = function (includeRolebackBackup) {
                    if (includeRolebackBackup) {
                        roleBackBackupModelValue = angular.copy(scope.model);
                    }
                    if (!valuesIsEqual(backupModelValue, scope.model)) {
                        backupModelValue = angular.copy(scope.model);
                        if (debugLog) console.log('activeSaveDirective; backupModal backupModelValue: ' + backupModelValue);
                    }
                }


                var rollbackModalOnError = function (cancel) {
                    if (debugLog) console.log("activeSaveDirective; rollbackModalOnError, cancel: " + cancel);

                    if (cancel) {
                        if (rbTimeout) {
                            $timeout.cancel(rbTimeout);
                        }
                    }
                    else {
                        if (rbTimeout) {
                            $timeout.cancel(rbTimeout);
                        }
                        rbTimeout = $timeout(function () { handleRollbackModalOnError(); }, 4000);  // 1000 = 1 second

                    }
                } 

                var handleRollbackModalOnError = function () {
                    if (!valueSelected && !ngModel.$valid && ngModel.$dirty) {
                        if (debugLog) console.log("activeSaveDirective; handleRollbackModalOnError");
                        rollbackModal();
                    }
                } 

                var rollbackModal = function () {
                    if (debugLog) console.log('activeSaveDirective; roleBackBackupModelValue: ' + roleBackBackupModelValue + ', backupModelValue: ' + backupModelValue);
                    if (roleBackBackupModelValue) {
                        ngModel.setModalValue(roleBackBackupModelValue);
                        ngModel.$setDirty();
                    }
                    else {
                        if (initValueBackup && !valueSelected) {
                            ngModel.$setPristine();
                            ngModel.setModalValue(backupModelValue);
                        }
                    }
                }

                var valuesIsEqual = function (newValue, oldValue) {
                    if (angular.isDate(newValue) && angular.isDate(oldValue)) {
                        if (newValue.getTime() == oldValue.getTime()) {
                            return true
                        }
                    }
                    else if (newValue == oldValue) {
                        return true;
                    }

                    return false;
                }

            },
        };
    }

})();