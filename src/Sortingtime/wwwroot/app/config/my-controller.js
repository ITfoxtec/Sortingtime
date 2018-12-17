(function () {
    'use strict';

    angular
        .module('app')
        .controller('myController', myController);

    myController.$inject = ['notificationFactory', 'restService', 'genericDataFactory', '$window'];

    function myController(notificationFactory, restService, genericDataFactory, $window) {
        var self = this;

        // PRIVATE
        var currentUserService, currentOrganizationService;

        // PUBLIC
        // Indicates if the view is being loaded
        self.loading = false;
        self.showPasswordDialogToggle = false;
        self.disabledChangeParssword = true;

        // PUBLIC FUNCTIONS
        self.activeSaveFullName = function (successCallBack, errorCallBack) {
            currentUserService.saveFullName(self.currentUser.fullName,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        }

        // Password Dialog
        self.showPasswordDialog = function () {
            self.changePasswordForm.$setPristine();

            self.currentUser.password = "";
            self.showPasswordDialogToggle  = true;
        }

        self.cancelPasswordDialog = function () {
            self.showPasswordDialogToggle  = false;
        }

        self.savePassword = function () {
            self.changePasswordForm.password.$setDirty();

            if (self.changePasswordForm.$valid) {
                currentUserService.savePassword(self.currentUser.password,
                    function (success) {
                        self.showPasswordDialogToggle = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("password") != -1) {
                                    fieldError = true;
                                    self.changePasswordForm.password.setServerErrorValidity(false, error.data[key]);
                                }
                            }
                            if (fieldError) {
                                error.isHandled = true;
                                return;
                            }
                        }
                        
                        notificationFactory.error(error);                        
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
                    self.disabledChangeParssword = success.isDemo;
                });
        }

        var activate = function () {
            currentUserService = genericDataFactory.currentUser.getService();
            currentOrganizationService = genericDataFactory.currentOrganization.getService();

            // LOAD
            getCurrentUser();
        }
        activate();
    }
})();
