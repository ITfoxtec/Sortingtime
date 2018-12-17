(function () {
    'use strict';

    angular
        .module('app')
        .controller('reportingController', reportingController);

    reportingController.$inject = ['notificationFactory', 'restService', 'genericDataFactory', '$filter', '$translate', '$scope'];

    function reportingController(notificationFactory, restService, genericDataFactory, $filter, $translate, $scope) {
        var self = this;

        // PRIVATE       
        var reportingService, reportItemService, reportSettingService;
        var currentUserService, currentOrganizationService;

        // PUBLIC
        // Indicates if the view is being loaded
        self.loading = false;
        self.emptyReport = false;
        self.reportTabAvtive = true;
        self.showCreateReportDialogToggle = false;
        self.showDetailReportDialogToggle = false;        
        
        self.dateCursor = {};

        // PUBLIC FUNCTIONS        
        self.updateDateCursor = function (successCallBack, errorCallBack) {
            getAllItems();
            successCallBack();
        }

        self.toggleSelectUser = function ($event, user) {
            $event.stopPropagation();
            $event.preventDefault();

            user.selected = !user.selected;
            var groupTasks = user.groupTasks;
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                self.toggleSelectTask($event, groupTasks[i], user.selected);
            }            
        }

        self.toggleSelectTotalGroup = function ($event, groupTask) {
            $event.stopPropagation();
            $event.preventDefault();

            var group = groupTask.group;
            groupTask.selected = !groupTask.selected;
            var element;
            for (var i = 0, ilen = self.reportsFlattened.length; i < ilen; i++) {
                element = self.reportsFlattened[i];
                if (element.type == 'groupTask' && element.group == group) {
                    element.selected = groupTask.selected;
                }
            }
            for (var i = 0, ilen = self.report.groupTaskTotals.length; i < ilen; i++) {
                element = self.report.groupTaskTotals[i];
                if (element.group == group) {
                    element.selected = groupTask.selected;
                }
            }
        }

        self.toggleSelectTotalTask = function ($event, groupTask) {
            if ($event != null) {
                $event.stopPropagation();
                $event.preventDefault();
            }

            var group = groupTask.group;
            var task = groupTask.task;
            groupTask.selected = !groupTask.selected;
            var element;
            for (var i = 0, ilen = self.reportsFlattened.length; i < ilen; i++) {
                element = self.reportsFlattened[i];
                if (element.type == 'groupTask' && element.group == group && element.task == task) {
                    element.selected = groupTask.selected;
                }
            }
        }

        self.toggleSelectGroup = function ($event, groupTask) {
            $event.stopPropagation();
            $event.preventDefault();

            var selected = !groupTask.selected;
            groupTask.selected = selected;

            handleSelectionForAllEqualGroupTask(groupTask);

            var element, group = groupTask.group;
            for (var i = 0, ilen = groupTask.user.groupTasks.length; i < ilen; i++) {
                element = groupTask.user.groupTasks[i];
                if (element.type == 'groupTask' && element.group == group) {
                    if (element.selected != selected) {
                        element.selected = selected;
                        handleSelectionForAllEqualGroupTask(element);
                    }
                }
            }
        }     

        self.toggleSelectTask = function ($event, groupTask, selected) {
            $event.stopPropagation();
            $event.preventDefault();

            if (selected !== undefined) {
                groupTask.selected = selected;
            }
            else {
                groupTask.selected = !groupTask.selected;
            }

            handleSelectionForAllEqualGroupTask(groupTask);
        }

        var handleSelectionForAllEqualGroupTask = function (groupTask) {
            var group = groupTask.group, task = groupTask.task, selected = groupTask.selected;
            var element, allSelectedEqual = true;
            if (selected) {
                for (var i = 0, ilen = self.reportsFlattened.length; i < ilen; i++) {
                    element = self.reportsFlattened[i];
                    if (element.type == 'groupTask' && element.group == group && element.task == task) {
                        if (element.selected != selected) {
                            allSelectedEqual = false;
                            break;
                        }
                    }
                }
            }

            if (allSelectedEqual) {
                for (i = 0, ilen = self.report.groupTaskTotals.length; i < ilen; i++) {
                    element = self.report.groupTaskTotals[i];
                    if (element.group == group && element.task == task) {
                        element.selected = selected;
                    }
                }
            }
        }

        // Create Report Dialog
        self.showCreateReportDialog = function () {
            if (self.emptyReport || self.reportsFlattened == null || self.reportsFlattened.length == 0) {
                return;
            }

            self.createReportForm.$setPristine();
            self.CreateReportTabAvtive = 0; // first tab

            defaultSelectFirstTask();
            showGroupCollAndFindFirstRaportDialog();
            generateSelectedReport();

            getCurrentUserAndOrganization();
            getEmailAndReportText();

            self.showCreateReportDialogToggle = true;
        }

        self.cancelCreateReportDialog = function () {
            self.showCreateReportDialogToggle = false;
        }

        self.createReport = function () {
            self.createReportForm.sendToEmail.$setDirty();
            self.createReportForm.emailSubject.$setDirty();
            self.createReportForm.emailBody.$setDirty();
            self.createReportForm.reportTitle.$setDirty();
            self.createReportForm.reportSubTitle.$setDirty();
            self.createReportForm.reportText.$setDirty();

            if (self.createReportForm.$valid) {
                var reportContent = createReportContentObject(true);

                reportingService.save(reportContent,
                    // success response
                    function (success) {                        
                        saveReportSetting();
                        deselectedSortingReport();
                        self.reportItems.push(success);
                        self.reportItems.totalTime += success.totalTime;
                        self.showCreateReportDialogToggle = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("toemail") != -1) {
                                    fieldError = true;
                                    self.createReportForm.sendToEmail.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailsubject") != -1) {
                                    fieldError = true;
                                    self.createReportForm.emailSubject.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailbody") != -1) {
                                    fieldError = true;
                                    self.createReportForm.emailBody.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("reporttitle") != -1) {
                                    fieldError = true;
                                    self.createReportForm.reportTitle.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("reportsubtitle") != -1) {
                                    fieldError = true;
                                    self.createReportForm.reportSubTitle.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("reporttext") != -1) {
                                    fieldError = true;
                                    self.createReportForm.reportText.setServerErrorValidity(false, error.data[key]);
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
            else {
                self.CreateReportTabAvtive = 0; // first tab
            }
        }

        self.activeSaveOrganizationName = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveName(self.currentOrganization.name,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        }

        self.activeSaveOrganizationAddress = function (successCallBack, errorCallBack) {
            currentOrganizationService.saveAddress(self.currentOrganization.address,
                successCallBack,
                function (error) {
                    errorCallBack(error);
                });
        }

        // Report Item Menu
        self.showReportItemMenu = function ($event, $index, reportItem) {
            $event.preventDefault();
            $event.stopPropagation();

            if (self.reportItemSelected) {
                self.reportItemSelected.selected = false;
                self.reportItemSelected.selectedPopup = false;
            }
            reportItem.selected = true;
            reportItem.index = $index;
            self.reportItemSelected = reportItem;

            $($event.target).append($('#reportItemMenu'));

            self.reportItemMenuIsOpen = true;
        }

        var hideReportItemMenu = function () {
            self.reportItemMenuIsOpen = false;
            $('#reportItemMenuContainder').append($('#reportItemMenu'));

        }

        // Detail Report Dialog
        self.showDetailReportItem = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            self.detailReportForm.$setPristine();
            getCurrentUserAndOrganization();

            self.reportItemSelected.selectedPopup = true;
            hideReportItemMenu();

            self.reportId = self.reportItemSelected.id;
            reportItemService.get({ id: self.reportItemSelected.id },
                function (success) {
                    self.reportNumber = success.number;
                    self.reportTotalTime = success.totalTime;
                    self.reportSentToEmail = success.toEmail;
                    self.reportEmailSubject = success.emailSubject;
                    self.reportEmailBody = success.emailBody;
                    self.showDetailReportDialogToggle = true;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        }

        self.cancelDetailReportDialog = function () {
            self.reportItemSelected.selectedPopup = false;
            self.showDetailReportDialogToggle = false;
        }

        self.resendDetailReport = function () {
            self.detailReportForm.sendToEmail.$setDirty();
            self.detailReportForm.emailSubject.$setDirty();
            self.detailReportForm.emailBody.$setDirty();

            if (self.detailReportForm.$valid) {
                var reportContent = createReportContentObject(false);
                reportContent.relatedId = self.reportId;
                reportContent.report = { monthTotal: self.reportTotalTime };

                reportingService.save(reportContent,
                    // success response
                    function (success) {
                        self.reportItemSelected.status = 600;
                        self.reportItemSelected.toEmail = success.toEmail;
                        self.reportItemSelected.selectedPopup = false;
                        self.showDetailReportDialogToggle = false;
                    },
                    function (error) {
                        if (error && error.data && angular.isObject(error.data)) {
                            var fieldError = false;
                            for (var key in error.data) {
                                if (key.toLowerCase().indexOf("toemail") != -1) {
                                    fieldError = true;
                                    self.detailReportForm.sendToEmail.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailsubject") != -1) {
                                    fieldError = true;
                                    self.detailReportForm.emailSubject.setServerErrorValidity(false, error.data[key]);
                                }
                                else if (key.toLowerCase().indexOf("emailbody") != -1) {
                                    fieldError = true;
                                    self.detailReportForm.emailBody.setServerErrorValidity(false, error.data[key]);
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
        }

        // Delete Report Dialog
        self.showDeleteReportDialog = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            self.reportItemSelected.selectedPopup = true;
            hideReportItemMenu();

            self.reportNumber = self.reportItemSelected.number
            self.showDeleteReportDialogToggle = true;
        }

        self.sendDeleteReport = function ($event) {
            reportItemService.delete({ id: self.reportItemSelected.id },
                function (success) {
                    self.reportItems.totalTime -= self.reportItemSelected.totalTime;
                    self.reportItems.splice(self.reportItemSelected.index, 1);
                    self.reportItemSelected.selectedPopup = false;
                    self.showDeleteReportDialogToggle = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        }

        self.cancelDeleteReportDialog = function ($event) {
            self.reportItemSelected.selectedPopup = false;
            self.showDeleteReportDialogToggle = false;
        }

        // PRIVATE FUNCTIONS
        var getAllItems = function () {            
            setTimeout(
                function () {
                    getAllReportingItems();
                }, 0);

            setTimeout(
                function () {
                    getAllReportOverviewItems();
                }, 0);
        }

        var getAllReportingItems = function () {
            self.loading = true;
            reportingService.get({ fromDate: self.dateCursor.from.toServerDateString(), toDate: self.dateCursor.to.toServerDateString() },
                function (success) {
                    self.report = success;
                    flattenReport();
                    self.daysInMonth = self.report.daysInMonth;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        }
        var getAllReportOverviewItems = function () {
            self.loading = true;
            reportItemService.query({ fromDate: self.dateCursor.from.toServerDateString(), toDate: self.dateCursor.to.toServerDateString() },
                function (success) {
                    self.reportItems = success;
                    var totalTime = 0, element, reportItems = self.reportItems;
                    for (var i = 0, ilen = reportItems.length; i < ilen; i++) {
                        element = reportItems[i];
                        totalTime += element.totalTime;
                    }
                    self.reportItems.totalTime = totalTime;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
        }

        var createReportContentObject = function (includeReport) {
            var reportContent = {
                toEmail: self.reportSentToEmail,
                emailSubject: self.reportEmailSubject,
                emailBody: self.reportEmailBody,
            };

            if (includeReport) {
                reportContent.showGroupColl = self.showGroupCollRaportDialog;
                reportContent.reportTitle = self.reportTitle;
                reportContent.reportSubTitle = self.reportSubTitle;
                reportContent.reportText = self.reportText;
                reportContent.report = $.extend(false, {}, self.report);

                var element;
                reportContent.report.monthTotal = self.monthTotalSelected;
                reportContent.report.groupTaskTotals = [];
                for (var i = 0, ilen = self.groupTaskTotalsSelected.length; i < ilen; i++) {
                    element = self.groupTaskTotalsSelected[i];
                    reportContent.report.groupTaskTotals.push($.extend(false, {}, element));
                }

                reportContent.report.users = [];
                var activeUser, activeGroupTasks;
                for (var i = 0, ilen = self.reportsFlattenedSelected.length; i < ilen; i++) {
                    element = self.reportsFlattenedSelected[i];
                    if (element.type == 'user') {
                        activeUser = $.extend(false, {}, element);
                        activeUser.groupTasks = [];
                        activeUser.type = undefined;
                        activeUser.selected = undefined;
                        reportContent.report.users.push(activeUser);
                    }
                    else if (element.type == 'groupTask') {
                        activeGroupTasks = $.extend(false, {}, element);
                        activeGroupTasks.user = undefined;
                        activeGroupTasks.type = undefined;
                        activeGroupTasks.allWorks = undefined;
                        activeGroupTasks.selected = undefined;
                        activeGroupTasks.selectNext = undefined;
                        activeUser.groupTasks.push(activeGroupTasks);
                    }
                }
            }

            return reportContent;
        }

        var flattenReport = function () {
            var emptyReport = true, reportsFlattened = [];
            var user, groupTask, lastUser;
            for (var iu = 0, iulen = self.report.users.length; iu < iulen; iu++) {
                user = self.report.users[iu];
                if (lastUser) {
                    reportsFlattened.push({ type: 'space', user: lastUser });
                }
                lastUser = user;
                emptyReport = false;

                user.type = 'user';
                reportsFlattened.push(user);

                reportsFlattened.push({ type: 'groupTaskHead', user: user });

                for (var igt = 0, igtlen = user.groupTasks.length; igt < igtlen; igt++) {
                    groupTask = user.groupTasks[igt];
                    groupTask.type = 'groupTask';
                    groupTask.user = user;
                    reportsFlattened.push(groupTask);
                    
                    groupTask.allWorks = allWorks(groupTask);
                };
            };

            self.emptyReport = emptyReport;
            self.reportsFlattened = reportsFlattened;
        }

        // Add element for each day
        var allWorks = function (groupTask) {
            var currentDay, currentWork, dayWork;
            var worksIndex = 0;
            var allWorks = [];

            for (var day = 1; day <= self.report.daysInMonth; day++) {
                dayWork = null;

                currentWork = groupTask.works[worksIndex];
                if (currentWork) {
                    if (currentWork.day == day) {
                        dayWork = currentWork;
                        dayWork.timeView = $filter('time')(dayWork.time);
                        worksIndex++;
                    }
                }

                if (!dayWork) {
                    dayWork = { day: day };
                }

                allWorks.push(dayWork);
            }

            return allWorks;
        }

        var generateSelectedReport = function () {
            self.reportsFlattenedSelected = [];
            var element, userElement;
            for (var i = 0, ilen = self.reportsFlattened.length; i < ilen; i++) {
                element = self.reportsFlattened[i];
                if (element.selected) {
                    self.reportsFlattenedSelected.push(element);
                }
                else if (!element.selected && element.type == 'user') {
                    if (userIsSelectedOrHasSelectedElement(element)) {
                        userElement = $.extend(false, {}, element);
                        userCalcMonthTotalAndCountSelectedElement(element, userElement);
                        self.reportsFlattenedSelected.push(userElement);
                    }
                }
                else if ((element.type == 'groupTaskHead' || element.type == 'space') && userIsSelectedOrHasSelectedElement(element.user)) {
                    self.reportsFlattenedSelected.push(element);
                }
            }

            self.monthTotalSelected = 0
            self.groupTaskTotalsSelected = [];
            for (var i = 0, ilen = self.reportsFlattenedSelected.length; i < ilen; i++) {
                element = self.reportsFlattenedSelected[i];
                if (element.type == 'groupTask') {
                    var elementTotal = null;
                    for (var it = 0, itlen = self.groupTaskTotalsSelected.length; it < itlen; it++) {
                        var elementTotalTemp = self.groupTaskTotalsSelected[it];
                        if (elementTotalTemp.group == element.group && elementTotalTemp.task == element.task) {
                            elementTotal = elementTotalTemp;
                            break;
                        }
                    }
                    if (!elementTotal) {
                        elementTotal = {
                            group: element.group,
                            task: element.task,
                            monthTotal: 0,
                        };
                        self.groupTaskTotalsSelected.push(elementTotal);
                    }
                    elementTotal.monthTotal += element.monthTotal;
                    self.monthTotalSelected += element.monthTotal;
                }
            }
        }

        var userIsSelectedOrHasSelectedElement = function (user) {
            if (user.selected) {
                return true;
            }
            var groupTasks = user.groupTasks;
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                var element = groupTasks[i];
                if (element.selected) {
                    return true;
                }
            }
            return false;
        }

        var userCalcMonthTotalAndCountSelectedElement = function (userIn, userOut) {
            var totalTime = 0, count = 0, groupTasks = userIn.groupTasks;
            for (var i = 0, ilen = groupTasks.length; i < ilen; i++) {
                var element = groupTasks[i];
                if (element.selected) {
                    totalTime += element.monthTotal;
                    count++;
                }
            }

            userOut.monthTotal = totalTime
            userOut.selectedCount = count;
        }

        var defaultSelectFirstTask = function () {
            var element, someSelected = false;
            for (var i = 0, ilen = self.reportsFlattened.length; i < ilen; i++) {
                element = self.reportsFlattened[i];
                if (element.selected) {
                    someSelected = true;
                }
            }
            if (!someSelected) {
                if (self.report.groupTaskTotals && self.report.groupTaskTotals.length > 0) {
                    self.toggleSelectTotalTask(null, self.report.groupTaskTotals[0]);
                }
            }
        }

        var deselectedSortingReport = function () {
            var element, groupTaskTotals = self.report.groupTaskTotals, reportsFlattened = self.reportsFlattened;
            for (var i = 0, ilen = groupTaskTotals.length; i < ilen; i++) {
                groupTaskTotals[i].selected = false;
            }
            for (var i = 0, ilen = reportsFlattened.length; i < ilen; i++) {
                var element = reportsFlattened[i];
                if (element.selected) {
                    element.selected = false;
                }
            }
        }

        var showGroupCollAndFindFirstRaportDialog = function () {
            self.firstSelectedGroup = null;
            self.firstSelectedTask = null;
            var someSelected = false;
            var firstGroup;
            for (var i = 0, ilen = self.reportsFlattened.length; i < ilen; i++) {
                var element = self.reportsFlattened[i];
                if (element.type == 'groupTask' && element.selected) {
                    if(!self.firstSelectedGroup && element.group) {
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
                        self.showGroupCollRaportDialog = true;
                        return;
                    }
                }
            }

            // Only one group, not shown
            self.showGroupCollRaportDialog = false;
        }

        var saveReportSetting = function () {
            var referenceType = self.firstSelectedGroup ? 'group' : 'task';
            var referenceKey = self.firstSelectedGroup ? self.firstSelectedGroup : self.firstSelectedTask;

            reportSettingService.save(
                {
                    referenceType: referenceType,
                    referenceKey: referenceKey,
                    toEmail: self.reportSentToEmail,
                    emailSubject: self.reportEmailSubject,
                    emailBody: self.reportEmailBody,
                    reportTitle: self.reportTitle,
                    reportText: self.reportText,
                },
                // success response
                function (success) {
                },
                function (error) {
                    notificationFactory.error(error)
                });
        }

        var getEmailAndReportText = function () {
            var referenceType = self.firstSelectedGroup ? 'group' : 'task';
            var referenceKey = self.firstSelectedGroup ? self.firstSelectedGroup : self.firstSelectedTask;

            reportSettingService.get({ referenceType: referenceType, referenceKey: referenceKey },
                function (success) {
                    setEmailAndReportText(success);
                },
                function (error) {
                    notificationFactory.error(error);
                });
        }

        var setEmailAndReportText = function (reportSetting) {
            self.reportSentToEmail = reportSetting.toEmail;
            self.reportEmailSubject = reportSetting.emailSubject;
            self.reportEmailBody = reportSetting.emailBody;
            self.reportTitle = reportSetting.reportTitle;
            self.reportText = reportSetting.reportText;

            if (!self.reportEmailSubject) {
                $translate('REPORT.EMAIL_SUBJECT_DFAULTTEXT').then(function (text) {
                    self.reportEmailSubject = text;
                });
            }

            if (!self.reportEmailBody) {
                $translate('REPORT.EMAIL_BODY_DFAULTTEXT').then(function (text) {
                    self.reportEmailBody = text;
                });
            }


            if (!self.reportTitle) {
                $translate('REPORT.REPORT_TITLE_DFAULTTEXT').then(function (text) {
                    self.reportTitle = text;
                });
            }

            $translate('REPORT.REPORT_TITLE_SUB_DFAULTTEXT').then(function (text) {
                self.reportSubTitle = text.replace("<begin_time>", $filter('date')(self.dateCursor.from, 'mediumDate')).replace("<end_time>", $filter('date')(self.dateCursor.to, 'mediumDate'));
            });
        }

        var getCurrentUserAndOrganization = function () {
            self.currentUser = currentUserService.getUser();
            self.currentOrganization = currentOrganizationService.getOrganization();
        }

        var activate = function () {
            var dateCursorFrom = new Date().toDateOnlyFirstDayInMonth();
            self.dateCursor = {
                from: dateCursorFrom,
                to: dateCursorFrom.toDateOnlyLastDayInMonth(),
            };

            reportingService = restService.getService('./api/reporting');
            reportItemService = restService.getService('./api/reportItem');
            reportSettingService = restService.getService('./api/reportSetting');
            currentUserService = genericDataFactory.currentUser.getService();
            currentOrganizationService = genericDataFactory.currentOrganization.getService();

            // LOADS ALL ITEMS
            getAllItems();
        }
        activate();
    }
})();
