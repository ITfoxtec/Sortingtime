(function () {
    'use strict';

    angular
        .module('app')
        .controller('footerController', footerController);

    footerController.$inject = ['notificationFactory', 'restService', '$translate'];

    function footerController(notificationFactory, restService, $translate) {
        var self = this;

        // PRIVATE
        var supportService;

        // PUBLIC
        self.showDialog = false;
        self.size = '';

        // PUBLIC FUNCTIONS
        self.showUrlDialog = function (url) {
            self.size = 'modal-lg';
            self.url = url;
            self.showDialog = true;
        };

        self.showSupport = function () {
            self.size = '';
            self.url = './applib/layout/supportPage.html';
            self.showDialog = true;
        };

        self.sendSupportMessage = function () {
            self.supportForm.message.$setDirty();

            if (self.supportForm.$valid) {
                supportService.save({ message: self.supportMessage },
                    // success response
                    function (success) {
                        $translate('SUPPORT.CONFIRMATION_TEXT').then(function (text) {
                            notificationFactory.success(text);
                        });

                        self.showDialog = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("message") != -1) {
                                    fieldError = true;
                                    self.supportForm.message.setServerErrorValidity(false, error.data[key]);
                                }
                            }
                            if (fieldError) {
                                error.isHandled = true;
                                return;
                            }
                        }

                        notificationFactory.error(error)
                    });
            }
        };

        self.close = function () {
            self.showDialog = false;
        };
        

        // PRIVATE FUNCTIONS
        var activate = function () {
            supportService = restService.getService('./api/support');
        };
        activate();
    }
})();
