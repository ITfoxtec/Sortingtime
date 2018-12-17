(function () {
    'use strict';

    angular
        .module('app')
        .controller('orgController', orgController);

    orgController.$inject = ['notificationFactory', 'restService', 'genericDataFactory', '$window'];

    function orgController(notificationFactory, restService, genericDataFactory, $window) {
        var self = this;

        // PRIVATE
        var currentOrganizationService

        // PUBLIC
        // Indicates if the view is being loaded
        self.loading = false;
        self.cultures = [
              { label: 'ORG.CULTURES.ENGLISH_USA', name: 'en-US' },
              { label: 'ORG.CULTURES.ENGLISH_UK', name: 'en-GB' },
              { label: 'ORG.CULTURES.DANISH', name: 'da-DK' },
        ];
        self.showLanguageDialogToggle = false;

        // PUBLIC FUNCTIONS
        self.activeSaveName = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveName(self.currentOrganization.name,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        }

        self.activeSaveAddress = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveAddress(self.currentOrganization.address,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        }

        self.activeSaveFirstInvoiceNumber = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveFirstInvoiceNumber(self.currentOrganization.firstInvoiceNumber,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        }

        // Culture Dialog
        self.showCultureDialog = function () {
            self.showCultureDialogToggle  = true;
        }

        self.cancelCultureDialog = function () {
            self.showCultureDialogToggle  = false;
        }

        self.saveCulture = function () {
            if (!self.currentOrganization.culture) {
                self.currentOrganization.culture = 'auto';
            }

            if (self.changeCultureForm.$valid) {

                currentOrganizationService.saveCulture(self.currentOrganization.culture,
                    function (success) {
                        self.showCultureDialogToggle = false;
                        $window.location.reload();
                    });
            }
        }

        // PRIVATE FUNCTIONS
        var getCurrentOrganization = function () {
            self.loading = true;

            self.currentOrganization = currentOrganizationService.getOrganization(
                function (success) {
                    self.loading = false;
                });
        }

        var activate = function () {
            currentOrganizationService = genericDataFactory.currentOrganization.getService();

            // LOAD
            getCurrentOrganization();
        }
        activate();
    }
})();
