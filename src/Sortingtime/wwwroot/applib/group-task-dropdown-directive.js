(function () {
    'use strict';

    angular
        .module('app')
        .directive('groupTaskDropdown', groupTaskDropdownDirective);

    //groupTaskDropdownDirective.$inject = [];

    function groupTaskDropdownDirective() {
        return {
            restrict: 'E',
            transclude: true,
            require: ['ngModel'],
            scope: {
                model: '=ngModel',
                groupTaskForm: '=', // two-ways
                selectItems: '&', // one-way
                createActiveSave: '&', // one-way
            },
            template: '<div class="create-dropdown" uib-dropdown is-open="isOpen">' +
                        '<div ng-transclude></div>' +
                        '<ul ng-if="isOpen" class="dropdown-menu option" uib-dropdown-menu style="width: 100%" role="menu" ng-click="stopClick($event)">' +
                            '<li><div class="row form-group">' +
                                '<div class="col-xs-12 text-right">' +
                                    '<button type="submit" class="btn btn-default btn-sm btn-primary" ng-if="showText(\'g\', \'n\')" ng-click="::save($event)" translate-once="GENERAL.CREATE_DROPDOWN.CREATE_GROUP"></button>' +
                                    '<button type="submit" class="btn btn-default btn-sm btn-primary" ng-if="showText(\'g\')" ng-click="::save($event)" translate-once="GENERAL.CREATE_DROPDOWN.CHANGE_GROUP"></button>' +
                                    '<button type="submit" class="btn btn-default btn-sm btn-primary" ng-if="showText(\'t\', \'n\')" ng-click="::save($event)" translate-once="GENERAL.CREATE_DROPDOWN.CREATE_TASK"></button>' +
                                    '<button type="submit" class="btn btn-default btn-sm btn-primary" ng-if="showText(\'t\')" ng-click="::save($event)" translate-once="GENERAL.CREATE_DROPDOWN.CHANGE_TASK"></button>' +
                                    '<button type="submit" class="btn btn-default btn-sm" ng-click="::cancel($event)" translate-once="GENERAL.BUTTONS.CANCEL"></button>' +
                                '</div>' +
                            '</div></li>' +
                            '<li ng-if="listItems && listItems.length > 0"><div class="row form-group">' +
                                '<div class="col-xs-12">' +                           
                                    '<small class="left-space" ng-if="showText(\'g\', \'s\')" translate-once="GENERAL.CREATE_DROPDOWN.SELECT_GROUP"></small>' +
                                    '<small class="left-space" ng-if="showText(\'t\', \'s\')" translate-once="GENERAL.CREATE_DROPDOWN.SELECT_TASK"></small>' +
                                '</div>' +
                            '</div></li>' +
                            '<li role="menuitem" ng-repeat="item in listItems track by item.id"><a class="btn btn-default btn-sm btn-select" ng-click="::selectValue($event, item)">{{item.value}}</a></li>' + 
                        '</ul>' +
                      '</div>',
            controller: function ($scope, $element) {
                var debugLog = false;
                
                this.open = function () {
                    if (debugLog) console.log('groupTaskDropdown; open');
                    if (!$scope.isOpen) {
                        $scope.isOpen = true;
                        $scope.createActiveSave()(null, null, { item: $scope.model, form: $scope.groupTaskForm }, null, null, 'open');
                        $scope.doBackup();
                        $scope.selectItems()($scope.model.name, $scope.model, $scope.updateItems);
                        $scope.bindEvents();

                        if ($scope.isCreating() && !$scope.groupTaskForm.pristine) {
                            $scope.groupTaskForm.$setPristine();
                        }
                    }
                }
                this.save = function () {
                    if (debugLog) console.log('groupTaskDropdown; save');

                    $scope.saveInternal();
                }
                this.cancel = function () {
                    if (debugLog) console.log('groupTaskDropdown; cancel');

                    $scope.isOpen = false;
                }
            },
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0], eventBinded = false,
                    debugLog = false;

                var hasValueBackup = false, roleBackBackupModelValue = null;

                //SCOPE VARIABLES
                scope.listItems = {};

                //SCOPE FUNCTIONS 
                scope.bindEvents = function () {
                    if (debugLog) console.log('groupTaskDropdown; bindEvents');
                    if (!eventBinded) {
                        eventBinded = true;

                        scope.$watch('model.name', function ngModelWatch(value) {
                            if (debugLog) console.log('groupTaskDropdown; watch model.name value: ' + value);

                            if (scope.isOpen) {
                                scope.selectItems()(scope.model.name, scope.model, scope.updateItems);
                            }
                        });

                        scope.$watch('isOpen', function isOpenWatch(value) {
                            if (debugLog) console.log('groupTaskDropdown; watch isOpen, value: ' + value);

                            if (value === false) {
                                if (scope.isCreating()) {
                                    scope.createActiveSave()(null, null, { item: scope.model, form: scope.groupTaskForm }, null, true);
                                }
                                else if (hasValueBackup && scope.model.name !== roleBackBackupModelValue) {
                                    scope.model.name = roleBackBackupModelValue;
                                }
                            }
                       });
                    }
                }

                scope.doBackup = function () {
                    if (debugLog) console.log('groupTaskDropdown; doBackup');
                    if (!hasValueBackup) {
                        if (debugLog) console.log('groupTaskDropdown; doBackup model.name, initValueBackup value: ' + scope.model.name);
                        roleBackBackupModelValue = angular.copy(scope.model.name);
                        hasValueBackup = true;
                    }
                }

                scope.isCreating = function () {
                    if (debugLog) console.log('groupTaskDropdown; isCreating');
                    return scope.model.state !== undefined;
                }
                scope.showText = function (type, action) {
                    if (debugLog) console.log('groupTaskDropdown; showText');
                    return scope.model.type === type && (action === 's' || scope.model.state === action);
                }
                scope.selectValue = function (event, item) {
                    if (debugLog) console.log('groupTaskDropdown; selectValue item.id: ' + item.id);
                    event.preventDefault();
                    event.stopPropagation();

                    scope.createActiveSave()(activeSaveSuccessCallBack, activeSaveErrorCallBack, { item: scope.model, form: scope.groupTaskForm }, item);
                }

                scope.updateItems = function (newcreateItems) {
                    if (debugLog) console.log('groupTaskDropdown; updateItems newcreateItems: ' + newcreateItems);
                    scope.listItems = angular.copy(newcreateItems);
                }

                scope.save = function (event) {
                    if (debugLog) console.log('groupTaskDropdown; save');
                    event.preventDefault();
                    event.stopPropagation();

                    scope.createActiveSave()(activeSaveSuccessCallBack, activeSaveErrorCallBack, { item: scope.model, form: scope.groupTaskForm });
                }
                scope.saveInternal = function () {
                    scope.createActiveSave()(activeSaveSuccessCallBack, activeSaveErrorCallBack, { item: scope.model, form: scope.groupTaskForm });
                }

                scope.cancel = function (event) {
                    if (debugLog) console.log('groupTaskDropdown; cancel');
                    event.preventDefault();
                    scope.isOpen = false;
                }

                scope.stopClick = function (event) {
                    if (debugLog) console.log('createDropdown; stopClick');
                    event.preventDefault();
                    event.stopPropagation();
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

                //PRIVATE FUNCTIONS
                var activeSaveSuccessCallBack = function (success) {
                    if (debugLog) console.log("groupTaskDropdown; activeSaveSuccessCallBack");
                    hasValueBackup = false;
                    scope.isOpen = false;
                }

                var activeSaveErrorCallBack = function (error, isSelctCallBack) {
                    if (debugLog) console.log("groupTaskDropdown; activeSaveErrorCallBack");

                    if (!isSelctCallBack && error && error.data && angular.isObject(error.data)) {
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

            },

        };
    }

})();
