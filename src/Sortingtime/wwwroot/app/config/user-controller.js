(function () {
    'use strict';

    angular
        .module('app')
        .controller('userController', userController);

    userController.$inject = ['notificationFactory', 'restService', 'genericDataFactory', '$scope', '$window'];

    function userController(notificationFactory, restService, genericDataFactory, $scope, $window) {
        var self = this;

        // PRIVATE
        var userService, currentUserService, currentOrganizationService;
        var maxUsers = 5;

        // PUBLIC
        // Indicates if the view is being loaded
        self.loading = false;
        self.showCreateUserDialogToggle = false;
        self.disabledAddUser = true;

        // EVENT
        $scope.$on('configController.onUsersTabSelect', function (event, arg) {
            if (!self.currentUser || !self.users) {
                getCurrentUser();
                getAllUsers();
            }
        });        

        // PUBLIC FUNCTIONS
        // Create User Dialog
        self.showCreateUserDialog = function () {
            self.createUserForm.$setPristine();
            if (self.newUser) {
                self.newUser.fullName = "";
                self.newUser.email = "";
            }
            self.showCreateUserDialogToggle = true;
        }

        self.cancelCreateUserDialog = function () {
            self.showCreateUserDialogToggle = false;
        }

        self.createUser = function () {
            self.createUserForm.fullName.$setDirty();
            self.createUserForm.email.$setDirty();

            if (self.createUserForm.$valid) {
                userService.save(self.newUser,
                    // success response
                    function (success) {
                        self.users.push(success);
                        self.disabledAddUser = !(!self.currentOrganization.isDemo && self.users.length < 5);

                        self.showCreateUserDialogToggle = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("fullName") != -1) {
                                    fieldError = true;
                                    self.createUserForm.fullName.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("email") != -1) {
                                    fieldError = true;
                                    self.createUserForm.email.setServerErrorValidity(false, error.data[key]);
                                }
                            }
                            if(fieldError)
                            {
                                error.isHandled = true;
                                return;
                            }
                        }

                        notificationFactory.error(error)
                    });
            }
        }

        // PRIVATE FUNCTIONS
        var getCurrentUser = function () {
            self.loading = true;

            self.currentUser = currentUserService.getUser(
                function (success) {
                    self.loading = false;
                });

            self.currentOrganization = currentOrganizationService.getOrganization(
                function (success) {                    
                });
        }

        var getAllUsers = function () {
            self.users = userService.query(
                function (successData) {
                    for (var key in successData) {
                        if (self.currentUser.id == successData[key].id) {
                            successData[key].you = true;
                        }
                    }
                    self.disabledAddUser = !(!self.currentOrganization.isDemo && successData.length < 5);
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error)
                });
        }

        var activate = function () {
            currentUserService = genericDataFactory.currentUser.getService();
            currentOrganizationService = genericDataFactory.currentOrganization.getService();
            userService = restService.getService('./api/user');
        }

        activate();
    }
})();
