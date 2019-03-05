(function () {
    'use strict';

    angular
        .module('app')
        .controller('invoicingController', invoicingController);

    invoicingController.$inject = ['notificationFactory', 'restService', 'genericDataFactory', '$filter', '$translate', '$scope'];

    function invoicingController(notificationFactory, restService, genericDataFactory, $filter, $translate, $scope) {
        var self = this;

        // PRIVATE       
        var invoicingService, invoiceItemService, hourPriceSettingService, invoiceSettingService;
        var currentUserService, currentOrganizationService;

        // PUBLIC
        // Indicates if the view is being loaded
        self.loading = false;
        self.emptySorting = false;
        self.showGroupTaskTotals = false;
        self.sortingTabAvtive = true;
        self.showCreateInvoiceDialogToggle = false;
        self.showDetailInvoiceDialogToggle = false;

        self.dateCursor = {};

        // PUBLIC FUNCTIONS        
        self.updateDateCursor = function (successCallBack, errorCallBack) {
            getAllItems();
            successCallBack();
        };

        self.toggleSelectGroup = function ($event, groupTask) {
            $event.stopPropagation();
            $event.preventDefault();

            var group = groupTask.group;
            var selected = !groupTask.selected
            groupTask.selected = selected;
            var element, users, groupTasks = self.sorting.groupTasks;
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                element = groupTasks[i];
                if (element.group == group) {
                    element.selected = selected;
                    users = element.users;
                    for (var j = 0, jlen = users.length; j < jlen; j++) {
                        self.toggleSelectUser($event, users[j], null, selected);
                    }
                }
            }
        };

        self.toggleSelectTask = function ($event, groupTask) {
            if ($event !== null) {
                $event.stopPropagation();
                $event.preventDefault();
            }

            var group = groupTask.group, task = groupTask.task;
            var selected = !groupTask.selected
            groupTask.selected = selected;
            var element, users, groupTasks = self.sorting.groupTasks;
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                element = groupTasks[i];
                if (element.group == group && element.task == task) {
                    element.selected = selected;
                    users = element.users;
                    for (var j = 0, jlen = users.length; j < jlen; j++) {
                        self.toggleSelectUser($event, users[j], null, selected);
                    }
                }
            }
        };
        
        self.toggleSelectUser = function ($event, user, groupTask, selectedInput) {
            if ($event !== null) {
                $event.stopPropagation();
                $event.preventDefault();
            }

            var selected;
            if (selectedInput !== undefined) {
                selected = selectedInput;
            }
            else {
                selected = !user.selected;
            }

            if (user.selected !== selected) {
                if (user.price) {
                    self.sorting.selectedTotalPrice = self.sorting.selectedTotalPrice + (selected ? user.price : user.price * -1);
                }
                self.sorting.selectedUsers = self.sorting.selectedUsers + (selected ? 1 : -1);
            }
            user.selected = selected;

            if (groupTask) {
                if (selected) {
                    var element, users = groupTask.users, allSelected = true;
                    for (var i = 0, ilen = users.length; i < ilen; i++) {
                        element = users[i];
                        if (!element.selected) {
                            allSelected = false;
                            break;
                        }
                    }
                    groupTask.selected = allSelected;
                }
                else {
                    groupTask.selected = selected;
                }
            }
        };

        self.noSelectUser = function ($event) {
            if ($event !== null) {
                $event.stopPropagation();
                $event.preventDefault();
            }
        };     

        self.activeSaveUserHourPrice = function (successCallBack, errorCallBack, data) {
            hourPriceSettingService.save(
                {
                    groupReferenceKey: data.groupTask.group,
                    taskReferenceKey: data.groupTask.task,
                    userId: data.user.id,
                    hourPrice: data.user.hourPrice
                },
                function (success) {
                    var oldPrice = (data.user.price ? data.user.price : 0);
                    data.user.price = data.user.hourPrice * data.user.time / 60;
                    var priceChange = data.user.price - oldPrice;
                    data.groupTask.price = (data.groupTask.price ? data.groupTask.price : 0) + priceChange;
                    self.sorting.totalPrice = (self.sorting.totalPrice ? self.sorting.totalPrice : 0) + priceChange;
                    if (data.user.selected) {
                        self.sorting.selectedTotalPrice = (self.sorting.selectedTotalPrice ? self.sorting.selectedTotalPrice : 0) + priceChange;
                    }

                    // Copy the hour price to other lines                    
                    if (data.user.hourPrice) { // && rollbackOnError ???
                        var groupTaskElement, userElement;
                        for (var i = 0, ilen = self.sorting.groupTasks.length; i < ilen; i++) {
                            groupTaskElement = self.sorting.groupTasks[i];
                            for (var j = 0, jlen = groupTaskElement.users.length; j < jlen; j++) {
                                userElement = groupTaskElement.users[j];
                                if (userElement.id == data.user.id && userElement.hourPrice === null) {
                                    userElement.hourPrice = data.user.hourPrice;
                                    oldPrice = userElement.price;
                                    userElement.price = userElement.hourPrice * userElement.time / 60;
                                    priceChange = userElement.price - oldPrice;
                                    self.sorting.totalPrice = self.sorting.totalPrice + priceChange;
                                    self.sorting.selectedTotalPrice = self.sorting.selectedTotalPrice + priceChange;
                                }
                            }
                        }
                    }
                },
                function (error) {
                    if (error && error.data && angular.isObject(error.data)) {
                        var fieldError = false;
                        for (var key in error.data) {
                            if (key.toLowerCase().indexOf("hourPrice") !== -1) {
                                fieldError = true;
                                data.form.hourPrice.setServerErrorValidity(false, error.data[key]);
                            }
                        }
                        if (fieldError) {
                            error.isHandled = true;
                            return;
                        }
                    }

                    notificationFactory.error(error);
                });
        };

        // Create Invoice Dialog
        self.showCreateInvoiceDialog = function () {
            if (self.emptySorting) {
                return;
            }

            self.createInvoiceForm.$setPristine();
            self.createInvoiceTabAvtive = 0; // First tab

            defaultSelectFirstTask();
            self.invoiceDate = self.dateCursor.to;
            showGroupCollAndFindFirstInvoiceDialog();

            getEmailAndInvoiceText();

            generateSelectedInvoice();

            self.showCreateInvoiceDialogToggle = true;
        };

        self.cancelCreateInvoiceDialog = function () {
            self.showCreateInvoiceDialogToggle = false;
        };

        self.createInvoice = function () {
            self.createInvoiceForm.sendToEmail.$setDirty();
            self.createInvoiceForm.emailSubject.$setDirty();
            self.createInvoiceForm.emailBody.$setDirty();
            self.createInvoiceForm.invoiceTitle.$setDirty();
            self.createInvoiceForm.invoiceCustomer.$setDirty();
            self.createInvoiceForm.invoicePaymentDetails.$setDirty();
            self.createInvoiceForm.invoicePaymentTerms.$setDirty();
            self.createInvoiceForm.invoiceDate.$setDirty();
            self.createInvoiceForm.invoiceReference.$setDirty();
            self.createInvoiceForm.invoiceText.$setDirty();

            if (self.currentOrganization.tax) {
                self.createInvoiceForm.invoiceTaxPercentage1.$setDirty();
                self.createInvoiceForm.invoiceTaxPercentage2.$setDirty();
            }
            if (self.currentOrganization.vat) {
                self.createInvoiceForm.invoiceVatNumber.$setDirty();
                self.createInvoiceForm.invoiceVatPercentage1.$setDirty();
                self.createInvoiceForm.invoiceVatPercentage2.$setDirty();
            }

            if (self.createInvoiceForm.$valid) {
                var invoiceContent = createInvoiceContentObject(true);

                invoicingService.save(invoiceContent,
                    // success response
                    function (success) {
                        saveInvoiceSetting();
                        deselectedSortingInvoice();
                        self.invoiceItems.push(success);
                        self.invoiceItems.subTotalPrice += success.subTotalPrice;
                        self.showCreateInvoiceDialogToggle = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false, errorFirstTab = false, errorSecondTab = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("toemail") != -1) {
                                    fieldError = true;
                                    errorFirstTab = true;
                                    self.createInvoiceForm.sendToEmail.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailsubject") != -1) {
                                    fieldError = true;
                                    errorFirstTab = true;
                                    self.createInvoiceForm.emailSubject.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailbody") != -1) {
                                    fieldError = true;
                                    errorFirstTab = true;
                                    self.createInvoiceForm.emailBody.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("invoicetitle") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoiceTitle.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("invoicecustomer") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoiceCustomer.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("vatnumber") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoiceVatNumber.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("paymentdetails") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoicePaymentDetails.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("invoiceraymentterms") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoicePaymentTerms.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("invoicedate") != -1) {
                                    fieldError = true;
                                    self.createInvoiceTabAvtive = 1; // Second tab
                                    self.createInvoiceForm.invoiceDate.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("invoicereference") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoiceReference.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("invoicetext") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoiceText.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("taxpercentage") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoiceTaxPercentage1.setServerErrorValidity(false, error.data[key]);
                                    self.createInvoiceForm.invoiceTaxPercentage2.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("vatpercentage") != -1) {
                                    fieldError = true;
                                    errorSecondTab = true;
                                    self.createInvoiceForm.invoiceVatPercentage1.setServerErrorValidity(false, error.data[key]);
                                    self.createInvoiceForm.invoiceVatPercentage2.setServerErrorValidity(false, error.data[key]);
                                }
                            }
                            if (fieldError) {
                                error.isHandled = true;
                                if (errorFirstTab) {
                                    self.createInvoiceTabAvtive = 0;
                                }
                                else if (errorSecondTab) {
                                    self.createInvoiceTabAvtive = 1;
                                }
                                return;
                            }
                        }

                        notificationFactory.error(error);
                    });
            }
            else {
                if (self.createInvoiceForm.sendToEmail.$invalid ||
                    self.createInvoiceForm.emailSubject.$invalid ||
                    self.createInvoiceForm.emailBody.$invalid) {
                    self.createInvoiceTabAvtive = 0; // First tab
                }
                else if ((self.currentOrganization.tax && (self.createInvoiceForm.invoiceTaxPercentage1.$invalid || self.createInvoiceForm.invoiceTaxPercentage2.$invalid)) ||
                    (self.currentOrganization.vat && (self.createInvoiceForm.invoiceVatPercentage1.$invalid || self.createInvoiceForm.invoiceVatPercentage2.$invalid)) ||
                    self.createInvoiceForm.invoiceDate.$invalid) {
                    self.createInvoiceTabAvtive = 1; // Second tab
                }
                else {
                    self.createInvoiceTabAvtive = 0; // First tab
                }
            }
        };

        self.activeSaveOrganizationName = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveName(self.currentOrganization.name,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        };

        self.activeSaveOrganizationAddress = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveAddress(self.currentOrganization.address,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        };

        self.activeSaveOrganizationVatNumber = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveVatNumber(self.currentOrganization.vatNumber,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        };

        self.activeSaveOrganizationPaymentDetails = function (successCallBack, errorCallBack) {
            currentOrganizationService.savePaymentDetails(self.currentOrganization.paymentDetails,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        };     

        self.activeSaveOrganizationTaxPercentage = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveTaxPercentage(self.currentOrganization.taxPercentage,
                function (success) {
                    calculateTaxVatTotal(true, false);
                    successCallBack(success);
                },
                function (error) {
                    errorCallBack(error);
                });
        };

        self.activeSaveOrganizationVatPercentage = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveVatPercentage(self.currentOrganization.vatPercentage,
                function (success) {
                    calculateTaxVatTotal(false, true);
                    successCallBack(success);
                },
                function (error) {
                    errorCallBack(error);
                });
        };

        self.createInvoicePriceUpdate = function ($event, form, user) {
            if (form.$valid) {
                var oldPrice = (user.price ? user.price : 0);
                user.price = user.time / 60 * user.hourPrice;
                var priceChange = user.price - oldPrice;
                self.sorting.selected.subTotalPrice = self.sorting.selected.subTotalPrice + priceChange

                calculateTaxVatTotal(true, true);
            }
        };

        // Invoice Item Menu
        self.showInvoiceItemMenu = function ($event, $index, invoiceItem) {
            $event.preventDefault();
            $event.stopPropagation();

            if (invoiceItem.creditNote !== true) {

                if (self.invoiceItemSelected) {
                    self.invoiceItemSelected.selected = false;
                    self.invoiceItemSelected.selectedPopup = false;
                }
                invoiceItem.selected = true;
                invoiceItem.index = $index;
                self.invoiceItemSelected = invoiceItem;

                $($event.target).append($('#invoiceItemMenu'));

                self.invoiceItemMenuIsOpen = true;
            }
        };

        var hideInvoiceItemMenu = function () {
            self.invoiceItemMenuIsOpen = false;
            $('#invoiceItemMenuContainder').append($('#invoiceItemMenu'));
        };

        // Detail Invoice Dialog
        self.showDetailInvoiceDialog = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            self.invoiceItemSelected.selectedPopup = true;
            hideInvoiceItemMenu();

            self.detailInvoiceForm.$setPristine();

            self.invoiceId = self.invoiceItemSelected.id;
            invoiceItemService.get({ id: self.invoiceItemSelected.id },
                function (success) {
                    self.invoiceNumber = success.number;
                    self.invoiceSubTotalPrice = success.subTotalPrice;
                    self.invoiceSentToEmail = success.toEmail;
                    self.invoiceEmailSubject = success.emailSubject;
                    self.invoiceEmailBody = success.emailBody;
                    self.invoiceCreditNote = success.creditNote;
                    self.showDetailInvoiceDialogToggle = true;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        self.cancelDetailInvoiceDialog = function () {
            self.invoiceItemSelected.selectedPopup = false;
            self.showDetailInvoiceDialogToggle = false;
        };

        self.resendDetailInvoice = function () {
            self.detailInvoiceForm.sendToEmail.$setDirty();
            self.detailInvoiceForm.emailSubject.$setDirty();
            self.detailInvoiceForm.emailBody.$setDirty();

            if (self.detailInvoiceForm.$valid) {
                var invoiceContent = createInvoiceContentObject(false);
                invoiceContent.relatedId = self.invoiceId;
                invoiceContent.invoice = { subTotalPrice: self.invoiceSubTotalPrice };

                invoicingService.save(invoiceContent,
                    // success response
                    function (success) {
                        self.invoiceItemSelected.status = 600,
                            self.invoiceItemSelected.toEmail = success.toEmail,
                            self.invoiceItemSelected.selectedPopup = false;
                        self.showDetailInvoiceDialogToggle = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("toemail") != -1) {
                                    fieldError = true;
                                    self.detailInvoiceForm.sendToEmail.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailsubject") != -1) {
                                    fieldError = true;
                                    self.detailInvoiceForm.emailSubject.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailbody") != -1) {
                                    fieldError = true;
                                    self.detailInvoiceForm.emailBody.setServerErrorValidity(false, error.data[key]);
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
        };

        // Create Credit Note Dialog
        self.showCreateCreditNoteDialog = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            if (!self.creditNoteEmailSubject) {
                $translate('INVOICE.CREDIT_NOTE_EMAIL_SUBJECT_DFAULTTEXT').then(function (text) {
                    self.creditNoteEmailSubject = text;
                });
            }
            if (!self.creditNoteEmailBody) {
                $translate('INVOICE.CREDIT_NOTE_EMAIL_BODY_DFAULTTEXT').then(function (text) {
                    self.creditNoteEmailBody = text;
                });
            }
           
            self.invoiceItemSelected.selectedPopup = true;
            hideInvoiceItemMenu();

            self.detailInvoiceForm.$setPristine();

            self.invoiceId = self.invoiceItemSelected.id;
            invoiceItemService.get({ id: self.invoiceItemSelected.id },
                function (success) {
                    self.invoiceNumber = success.number;
                    self.invoiceSubTotalPrice = success.subTotalPrice;
                    self.invoiceSentToEmail = success.toEmail;
                    self.invoiceEmailSubject = success.emailSubject;
                    self.invoiceEmailBody = success.emailBody;
                    self.invoiceCreditNote = true;
                    self.showDetailInvoiceDialogToggle = true;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        self.cancelCreateCreditNoteDialog = function () {
            self.invoiceItemSelected.selectedPopup = false;
            self.showDetailInvoiceDialogToggle = false;
        };

        self.createCreditNoteInvoice = function () {
            self.detailInvoiceForm.sendToEmail.$setDirty();
            self.detailInvoiceForm.emailSubject.$setDirty();
            self.detailInvoiceForm.emailBody.$setDirty();

            if (self.detailInvoiceForm.$valid) {
                var invoiceContent = createInvoiceContentObject(false, true);
                invoiceContent.relatedId = self.invoiceId;
                invoiceContent.invoice = { subTotalPrice: self.invoiceSubTotalPrice };
                invoiceContent.creditNote = true;

                invoicingService.save(invoiceContent,
                    // success response
                    function (success) {
                        self.invoiceItems.push(success);
                        self.invoiceItems.subTotalPrice -= success.subTotalPrice;
                        self.invoiceItemSelected.selectedPopup = false;
                        self.showDetailInvoiceDialogToggle = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("toemail") != -1) {
                                    fieldError = true;
                                    self.detailInvoiceForm.sendToEmail.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailsubject") != -1) {
                                    fieldError = true;
                                    self.detailInvoiceForm.emailSubject.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailbody") != -1) {
                                    fieldError = true;
                                    self.detailInvoiceForm.emailBody.setServerErrorValidity(false, error.data[key]);
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
        };

        //self.showCreateCreditNoteDialog = function ($event) {
        //    $event.preventDefault();
        //    $event.stopPropagation();

        //    self.invoiceItemSelected.selectedPopup = true;
        //    hideInvoiceItemMenu();

        //    $translate('GENERAL.COMING_SOON').then(function (text) {
        //        notificationFactory.warning(text);
        //    });

        //    self.invoiceItemSelected.selectedPopup = false;


        //    //    self.detailInvoiceForm.$setPristine();

        //    //    self.invoiceId = self.invoiceItemSelected.id;
        //    //    invoiceItemService.get({ id: self.invoiceItemSelected.id },
        //    //        function (success) {
        //    //            self.invoiceNumber = success.number;
        //    //            self.invoiceSubTotalPrice = success.subTotalPrice;
        //    //            self.invoiceSentToEmail = success.toEmail;
        //    //            self.invoiceEmailSubject = success.emailSubject;
        //    //            self.invoiceEmailBody = success.emailBody;
        //    //            self.showDetailInvoiceDialogToggle = true;
        //    //            self.loading = false;
        //    //        },
        //    //        function (error) {
        //    //            notificationFactory.error(error);
        //    //        });
        //};

        // Delete Invoice Dialog
        self.showDeleteInvoiceDialog = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            self.invoiceItemSelected.selectedPopup = true;
            hideInvoiceItemMenu();

            self.invoiceNumber = self.invoiceItemSelected.number;
            self.showDeleteInvoiceDialogToggle = true;
        };

        self.sendDeleteInvoice = function ($event) {
            invoiceItemService.delete({ id: self.invoiceItemSelected.id },
                function (success) {
                    self.invoiceItems.subTotalPrice -= self.invoiceItemSelected.subTotalPrice;
                    self.invoiceItems.splice(self.invoiceItemSelected.index, 1);
                    self.invoiceItemSelected.selectedPopup = false;
                    self.showDeleteInvoiceDialogToggle = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        self.cancelDeleteInvoiceDialog = function ($event) {
            self.invoiceItemSelected.selectedPopup = false;
            self.showDeleteInvoiceDialogToggle = false;
        };
        
        // PRIVATE FUNCTIONS
        var getAllItems = function () {
            self.emptyReport = true;
            self.sorting = null;
            setTimeout(
                function () {
                    getAllInvoicingItems();
                }, 0);

            setTimeout(
                function () {
                    getAllInvoiceOverviewItems();
                }, 0);
        };

        var getAllInvoicingItems = function () {
            self.loading = true;
            invoicingService.get({ fromDate: self.dateCursor.from.toServerDateString(), toDate: self.dateCursor.to.toServerDateString() },
                function (success) {
                    self.sorting = success;
                    self.sorting.selectedTotalPrice = 0;
                    self.sorting.selectedUsers = 0;
                    self.emptySorting = self.sorting.groupTasks && self.sorting.groupTasks.length ? false : true;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };
        var getAllInvoiceOverviewItems = function () {
            self.loading = true;
            invoiceItemService.query({ fromDate: self.dateCursor.from.toServerDateString(), toDate: self.dateCursor.to.toServerDateString() },
                function (success) {
                    self.invoiceItems = success;
                    var subTotalPrice = 0, element, invoiceItems = self.invoiceItems;
                    for (var i = 0, ilen = invoiceItems.length; i < ilen; i++) {
                        element = invoiceItems[i];
                        if (element.creditNote !== true) {
                            subTotalPrice += element.subTotalPrice;
                        }
                        else {
                            subTotalPrice -= element.subTotalPrice;
                        }
                    }
                    self.invoiceItems.subTotalPrice = subTotalPrice;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        var createInvoiceContentObject = function (includeInvoice, creditNote) {
            var invoiceContent = {
                toEmail: self.invoiceSentToEmail,
                emailSubject: creditNote !== true ? self.invoiceEmailSubject : self.creditNoteEmailSubject,
                emailBody: creditNote !== true ? self.invoiceEmailBody : self.creditNoteEmailBody
            };

            if (includeInvoice) {
                invoiceContent.showGroupColl = self.showGroupCollInvoiceDialog;
                invoiceContent.invoiceTitle = self.invoiceTitle;
                invoiceContent.invoiceCustomer = self.invoiceCustomer;
                invoiceContent.paymentDetails = self.currentOrganization.paymentDetails;
                invoiceContent.invoicePaymentTerms = self.invoicePaymentTerms;
                invoiceContent.invoiceDate = self.invoiceDate.toServerDateString();
                invoiceContent.invoiceReference = self.invoiceReference;
                invoiceContent.invoiceText = self.invoiceText;
                invoiceContent.tax = self.currentOrganization.tax;
                invoiceContent.vat = self.currentOrganization.vat;
                if (self.currentOrganization.tax) {
                    invoiceContent.taxPercentage = self.currentOrganization.taxPercentage;
                    invoiceContent.taxPrice = self.sorting.selected.taxPrice;
                }
                if (self.currentOrganization.vat) {
                    invoiceContent.vatNumber = self.currentOrganization.vatNumber;
                    invoiceContent.vatPercentage = self.currentOrganization.vatPercentage;
                    invoiceContent.vatPrice = self.sorting.selected.vatPrice;
                }
                invoiceContent.totalPrice = self.sorting.selected.totalPrice;


                invoiceContent.invoice = {
                    subTotalPrice: self.sorting.selected.subTotalPrice,
                    groupTasks: [],
                };

                var element, groupTasks = self.sorting.selected.groupTasks, icGroupTasks = invoiceContent.invoice.groupTasks;
                var user, users;
                for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                    element = $.extend(false, {}, groupTasks[i]);
                    element.users = [];
                    element.usersSelected = undefined;
                    element.selected = undefined;
                    icGroupTasks.push(element);

                    users = groupTasks[i].usersSelected;
                    for (var j = 0, jlen = users.length; j < jlen; j++) {
                        user = $.extend(false, {}, users[j]);
                        user.selected = undefined;
                        element.users.push(user);
                    }
                }
            }
            return invoiceContent;
        };

        var defaultSelectFirstTask = function () {
            if (!self.sorting.selectedUsers) {
                if (self.sorting.groupTasks && self.sorting.groupTasks.length > 0) {
                    self.toggleSelectTask(null, self.sorting.groupTasks[0]);
                }
            }
        };

        var deselectedSortingInvoice = function () {
            var element, users, groupTasks = self.sorting.groupTasks;
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                element = groupTasks[i];
                element.selected = false;
                users = element.users;
                for (var j = 0, jlen = users.length; j < jlen; j++) {
                    users[j].selected = false;
                }
            }
            self.sorting.selectedTotalPrice = 0;
            self.sorting.selectedUsers = 0;
        };

        var calculateTaxVatTotal = function (calculateTax, calculateVat) {
            var sortingSelected = self.sorting.selected;
            if (!sortingSelected)
                return;

            if (calculateTax) {
                if (self.currentOrganization.tax) {
                    var taxPercentage = (self.currentOrganization.taxPercentage ? self.currentOrganization.taxPercentage : 0) / 100;
                    sortingSelected.taxPrice = (sortingSelected.subTotalPrice ? sortingSelected.subTotalPrice : 0) * taxPercentage;
                }
                else {
                    sortingSelected.taxPrice = 0;
                }
            }

            if (calculateVat) {
                if (self.currentOrganization.vat) {
                    var vatPercentage = (self.currentOrganization.vatPercentage ? self.currentOrganization.vatPercentage : 0) / 100;
                    sortingSelected.vatPrice = (sortingSelected.subTotalPrice ? sortingSelected.subTotalPrice : 0) * vatPercentage;
                }
                else {
                    sortingSelected.vatPrice = 0;
                }
            }

            sortingSelected.totalPrice = (sortingSelected.subTotalPrice ? sortingSelected.subTotalPrice : 0) + sortingSelected.taxPrice + sortingSelected.vatPrice;
        };

        var generateSelectedInvoice = function () {
            self.sorting.selected = { groupTasks: [] };
            var subTotalPrice = 0, selectedGroupTasks = self.sorting.selected.groupTasks;
            var element, elementSelected, groupTasks = self.sorting.groupTasks;
            var user, users, usersSelected = [];
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                element = groupTasks[i];
                users = element.users, usersSelected = [];
                for (var j = 0, jlen = users.length; j < jlen; j++) {
                    user = users[j];
                    if (user.selected) {
                        usersSelected.push($.extend(false, {}, user));
                        subTotalPrice = subTotalPrice + (user.price ? user.price : 0);
                    }
                }
                if (usersSelected.length > 0) {
                    elementSelected = $.extend(false, {}, element);
                    elementSelected.usersSelected = usersSelected;
                    selectedGroupTasks.push(elementSelected);
                }
            }

            self.sorting.selected.subTotalPrice = subTotalPrice;
            calculateTaxVatTotal(true, true);
        };

        var showGroupCollAndFindFirstInvoiceDialog = function () {
            // Init only one group, not shown
            self.showGroupCollInvoiceDialog = false;
            self.firstTask = null;
            self.firstSelectedGroup = null;
            self.firstSelectedTask = null;
            var someSelected = false;
            var firstGroup, groupTasks = self.sorting.groupTasks;
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                var element = groupTasks[i];
                if (!self.firstTask) {
                    self.firstTask = element.task;
                }
                if (element.selected) {
                    if (!self.firstSelectedGroup && element.group) {
                        self.firstSelectedGroup = element.group;
                    }
                    if (!self.firstSelectedTask && !element.group) {
                        self.firstSelectedTask = element.task;
                    }

                    if (!someSelected) {
                        someSelected = true;
                        firstGroup = element.group;
                    }
                    else if (element.group != firstGroup) {
                        // More then one groupe to show
                        self.showGroupCollInvoiceDialog = true;
                        break;
                    }
                }
            }

            // If a user is selected
            if (!self.firstSelectedGroup && !self.firstSelectedTask) {
                self.firstSelectedTask = self.firstTask;
            }
        };

        var saveInvoiceSetting = function () {
            var referenceType = self.firstSelectedGroup ? 'group' : 'task';
            var referenceKey = self.firstSelectedGroup ? self.firstSelectedGroup : self.firstSelectedTask;

            invoiceSettingService.save(
                {
                    referenceType: referenceType,
                    referenceKey: referenceKey,
                    toEmail: self.invoiceSentToEmail,
                    emailSubject: self.invoiceEmailSubject,
                    emailBody: self.invoiceEmailBody,
                    invoiceTitle: self.invoiceTitle,
                    invoiceCustomer: self.invoiceCustomer,
                    invoicePaymentTerms: self.invoicePaymentTerms,
                    invoiceReference: self.invoiceReference,
                    invoiceText: self.invoiceText,
                },
                // success response
                function (success) {
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        var getEmailAndInvoiceText = function () {
            var referenceType = self.firstSelectedGroup ? 'group' : 'task';
            var referenceKey = self.firstSelectedGroup ? self.firstSelectedGroup : self.firstSelectedTask;

            invoiceSettingService.get({ referenceType: referenceType, referenceKey: referenceKey },
                function (success) {
                    setEmailAndInvoiceText(success);
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        var setEmailAndInvoiceText = function (invoiceSetting) {
            self.invoiceSentToEmail = invoiceSetting.toEmail;
            self.invoiceEmailSubject = invoiceSetting.emailSubject;
            self.invoiceEmailBody = invoiceSetting.emailBody;
            self.invoiceTitle = invoiceSetting.invoiceTitle;
            self.invoiceCustomer = invoiceSetting.invoiceCustomer;
            self.invoicePaymentTerms = invoiceSetting.invoicePaymentTerms;
            self.invoiceReference = invoiceSetting.invoiceReference;
            self.invoiceText = invoiceSetting.invoiceText;

            if (!self.invoiceEmailSubject) {
                $translate('INVOICE.EMAIL_SUBJECT_DFAULTTEXT').then(function (text) {
                    self.invoiceEmailSubject = text;
                });
            }

            if (!self.invoiceEmailBody) {
                $translate('INVOICE.EMAIL_BODY_DFAULTTEXT').then(function (text) {
                    self.invoiceEmailBody = text;
                });
            }

            if (!self.invoiceTitle) {
                $translate('INVOICE.INVOICE_TITLE_DFAULTTEXT').then(function (text) {
                    self.invoiceTitle = text;
                });
            }

            if (!self.invoicePaymentTerms) {
                $translate('INVOICE.PAYMENT_TERMS_DFAULTTEXT').then(function (text) {
                    self.invoicePaymentTerms = text;
                });
            }

            calculateTaxVatTotal(true, true);
        };

        var getCurrentUserAndOrganization = function () {
            self.currentUser = currentUserService.getUser();
            self.currentOrganization = currentOrganizationService.getOrganization(
                function (success) {
                    if (self.currentOrganization.userCount > 1) {
                        self.showGroupTaskTotals = true;
                    }
                });
        };

        var activate = function () {
            var dateCursorFrom = new Date().toDateOnlyFirstDayInMonth();
            self.dateCursor = {
                from: dateCursorFrom,
                to: dateCursorFrom.toDateOnlyLastDayInMonth(),
            };

            invoicingService = restService.getService('./api/invoicing');
            invoiceItemService = restService.getService('./api/invoiceItem');
            hourPriceSettingService = restService.getService('./api/HourPriceSetting');
            invoiceSettingService = restService.getService('./api/invoiceSetting');
            currentUserService = genericDataFactory.currentUser.getService();
            currentOrganizationService = genericDataFactory.currentOrganization.getService();

            getCurrentUserAndOrganization();
            // LOADS ALL ITEMS
            getAllItems();
        };
        activate();
    }
})();
