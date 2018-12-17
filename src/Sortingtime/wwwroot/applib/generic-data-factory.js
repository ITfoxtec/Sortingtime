(function () {
    'use strict';

    angular
        .module('app')
        .service('genericDataFactory', genericDataFactory);

    genericDataFactory.$inject = ['notificationFactory', 'restService'];

    function genericDataFactory(notificationFactory, restService) {
        var genericData = [];
        genericData.currentUser = [];
        genericData.currentOrganization = [];
        genericData.currentLogo = [];        

        genericData.currentUser.getService = function () {
            return {
                getUser: function (successCallBack, errorCallBack) {
                    loadUser(successCallBack, errorCallBack);
                    return genericData.currentUser.user;
                },

                saveFullName: function (newFullName, successCallBack, errorCallBack) {
                    genericData.currentUser.service.patch({ id: genericData.currentUser.user.id }, { fullName: newFullName },
                        function (success) {
                            genericData.currentUser.user.fullName = success.fullName;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },

                savePassword: function (newPassword, successCallBack, errorCallBack) {
                    genericData.currentUser.service.patch({ id: genericData.currentUser.user.id }, { password: newPassword },
                        function (success) {
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {                            
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else
                            {
                                notificationFactory.error(error);
                            }
                        });
                },
            };
        }

        genericData.currentOrganization.getService = function () {
            return {
                getOrganization: function (successCallBack, errorCallBack) {
                    loadOrganization(successCallBack, errorCallBack);
                    return genericData.currentOrganization.organization;
                },

                saveName: function (newName, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { name: newName },
                        function (success) {
                            genericData.currentOrganization.organization.name = success.name;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },
                saveAddress: function (newAddress, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { address: newAddress },
                        function (success) {
                            genericData.currentOrganization.organization.address = success.address;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },
                

                saveVatNumber: function (newVatNumber, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { vatNumber: newVatNumber },
                        function (success) {
                            genericData.currentOrganization.organization.vatNumber = success.vatNumber;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },

                savePaymentDetails: function (newPaymentDetails, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { paymentDetails: newPaymentDetails },
                        function (success) {
                            genericData.currentOrganization.organization.paymentDetails = success.paymentDetails;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },                

                saveTaxPercentage: function (newTaxPercentage, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { taxPercentage: newTaxPercentage },
                        function (success) {
                            genericData.currentOrganization.organization.taxPercentage = success.taxPercentage;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },
                saveVatPercentage: function (newVatPercentage, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { vatPercentage: newVatPercentage },
                        function (success) {
                            genericData.currentOrganization.organization.vatPercentage = success.vatPercentage;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },

                saveFirstInvoiceNumber: function (newFirstInvoiceNumber, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { firstInvoiceNumber: newFirstInvoiceNumber },
                        function (success) {
                            genericData.currentOrganization.organization.firstInvoiceNumber = success.firstInvoiceNumber;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },

                saveCulture: function (newCulture, successCallBack, errorCallBack) {
                    genericData.currentOrganization.service.patch({ id: 0 }, { culture: newCulture },
                        function (success) {
                            genericData.currentOrganization.organization.culture = success.culture;
                            if (successCallBack) {
                                successCallBack(success);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },
            };
        }

        genericData.currentLogo.getService = function () {
            return {
                getLogo: function (successCallBack, errorCallBack) {
                    loadLogo(successCallBack, errorCallBack);                    
                },

                saveLogo: function (newLogo, successCallBack, errorCallBack) {
                    genericData.currentLogo.service.save({ image: newLogo },
                        function (success) {
                            genericData.currentLogo.logo = success.image;
                            if (successCallBack) {
                                successCallBack(genericData.currentLogo.logo);
                            }
                        },
                        function (error) {
                            if (errorCallBack) {
                                errorCallBack(error);
                            }
                            else {
                                notificationFactory.error(error);
                            }
                        });
                },
            };
        }

        var loadUser = function (successCallBack, errorCallBack) {
            if (!genericData.currentUser.user) {
                genericData.currentUser.user = genericData.currentUser.service.get({ id: -1 },
                    function (success) {
                        if (successCallBack) {
                            successCallBack(success);
                        }
                    },
                    function (error) {                        
                        if (errorCallBack) {
                            errorCallBack(error);
                        }
                        else {
                            notificationFactory.error(error);
                        }
                    });
            }
            else {
                setTimeout(successCallBack, 0);
            }
        }

        var loadOrganization = function (successCallBack, errorCallBack) {
            if (!genericData.currentOrganization.organization) {
                genericData.currentOrganization.organization = genericData.currentOrganization.service.get({},
                    function (success) {
                        configureOrganizationTaxVat(genericData.currentOrganization.organization);
                        if (successCallBack) {
                            successCallBack(success);
                        }
                    },
                    function (error) {
                        if (errorCallBack) {
                            errorCallBack(error);
                        }
                        else {
                            notificationFactory.error(error);
                        }
                    });
            }
            else {
                setTimeout(successCallBack, 0);
            }
        }

        var configureOrganizationTaxVat = function (organization) {
            if (organization.culture == "en-US") {
                // USA
                organization.tax = true;
                organization.vat = false;
                organization.taxPercentage = organization.taxPercentage ? organization.taxPercentage : 0;
            }
            else {
                //EU
                organization.tax = false;
                organization.vat = true;
                organization.vatPercentage = organization.vatPercentage ? organization.vatPercentage : 0;
            }
        }

        var loadLogo = function (successCallBack, errorCallBack) {
            if (!genericData.currentLogo.logo) {
                genericData.currentLogo.service.get({},
                    function (success) {
                        genericData.currentLogo.logo = success.image;
                        if (successCallBack) {
                            successCallBack(genericData.currentLogo.logo);
                        }
                    },
                    function (error) {
                        if (errorCallBack) {
                            errorCallBack(error);
                        }
                        else {
                            notificationFactory.error(error);
                        }
                    });               
            }
            else {
                setTimeout(function () {
                    if (successCallBack) {
                        successCallBack(genericData.currentLogo.logo);
                    }
                }, 0);
            }
        }

        var activate = function () {
            genericData.currentUser.service = restService.getService('./api/user');
            genericData.currentOrganization.service = restService.getService('./api/organization');
            genericData.currentLogo.service = restService.getService('./api/logo');

            // load after 2 sec
            setTimeout(
                function () {
                    loadUser();
                    loadOrganization();
                }, 2000); 

            // load after 10 sec
            setTimeout(
                function () {
                    loadLogo();
                }, 10000);
        }
        activate();

        return genericData;
    }

})();
