(function () {
    'use strict';

    angular
        .module('app')
        .controller('timeController', timeController);

    timeController.$inject = ['notificationFactory', 'restService', '$interval', '$scope'];

    function timeController(notificationFactory, restService, $interval, $scope) {
        var self = this;

        // PRIVATE
        var dashboardService, dashboardGroupService, dashboardTaskService, dashboardTaskItemService, dashboardTotalService, filterGroupService, filterTaskService;
        var uniqIdCount = 1;

        // PUBLIC
        // Indicates if the view is being loaded
        self.loading = false;
        self.dayTotal = 0, self.weekTotal = 0, self.monthTotal = 0;
        self.showTimerDialogToggle = false, self.timerData = null;
        self.pageItems = [], self.currentGroupIds, self.currentTaskIds = [], self.changedUniqIds = [];
        self.brakes = [
            { label: 'TIME.BRAKE_00MIN', time: 0 },
            { label: 'TIME.BRAKE_05MIN', time: 5 },
            { label: 'TIME.BRAKE_10MIN', time: 10 },
            { label: 'TIME.BRAKE_15MIN', time: 15 },
            { label: 'TIME.BRAKE_30MIN', time: 30 },
            { label: 'TIME.BRAKE_45MIN', time: 45 },
            { label: 'TIME.BRAKE_60MIN', time: 60 }
        ];

        // The date shown initialized with current date.
        self.dateCursor = new Date().toDateOnly();

        // PUBLIC FUNCTIONS        
        self.updateDateCursor = function (successCallBack, errorCallBack) {
            getAllItems();
            successCallBack();
        };

        self.getGroups = function (createValue, createData, updatecreateItemsCallBack) {
            filterGroupService.query({ filter: createValue, eId: getCurrentGroupIds() },
                function (successData) {
                    updatecreateItemsCallBack(successData);
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        self.getTasks = function (createValue, createData, updatecreateItemsCallBack) {
            filterTaskService.query({ filter: createValue, groupId: createData.groupId, eId: getCurrentTaskIds(createData.groupId) },
                function (successData) {
                    updatecreateItemsCallBack(successData);
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        self.activeSaveGroup = function (successCallBack, errorCallBack, data, selectItem, cancelNew, event) {
            if (event === 'open') {
                addChangedUniqId(data.item);
            }
            else if (cancelNew === true) {
                self.removeGroup(data.item);
            }
            else if (selectItem !== undefined) {
                dashboardGroupService.patch({ id: selectItem.id },
                    { date: self.dateCursor.toServerDateString(), name: selectItem.value },
                    function (successData) {
                        data.item.id = selectItem.id;
                        data.item.state = undefined;
                        data.item.name = successData.name;
                        self.currentGroupIds = undefined;
                        data.item.tasks = [];
                        self.addTaskButton(data.item);
                        successCallBack(successData);
                    },
                    function (error) {
                        errorCallBack(error, true);
                    });
            }
            else if (data.item.state !== 'n') {
                dashboardGroupService.patch({ id: data.item.id },
                    { date: self.dateCursor.toServerDateString(), name: data.item.name },
                    function (successData) {
                        data.item.name = successData.name;
                        successCallBack(successData);
                    },
                    function (error) { errorCallBack(error); });
            }
            else {
                data.form.group.$setDirty();
                if (data.form.$valid) {
                    dashboardGroupService.save({ date: self.dateCursor.toServerDateString(), name: data.item.name },
                        function (successData) {
                            data.item.id = successData.id;
                            data.item.state = undefined;
                            data.item.name = successData.name;
                            self.currentGroupIds = undefined;
                            data.item.tasks = [];
                            self.addTaskButton(data.item);
                            successCallBack(successData);
                        },
                        function (error) {
                            errorCallBack(error);
                        });
                }
            }
        };

        self.activeSaveTask = function (successCallBack, errorCallBack, data, selectItem, cancelNew, event) {
            if (event === 'open') {
                addChangedUniqId(data.item);
            }
            else if (cancelNew === true) {
                self.removeTask(data.item);
            }
            else if (selectItem !== undefined) {
                dashboardTaskService.patch({ id: selectItem.id },
                    { date: self.dateCursor.toServerDateString(), name: selectItem.value },
                    function (successData) {
                        data.item.id = selectItem.id;
                        data.item.state = undefined;
                        data.item.name = successData.name;
                        self.currentTaskIds[data.item.groupId] = undefined;
                        successCallBack(successData);
                    },
                    function (error) {
                        errorCallBack(error, true);
                    });
            }
            else if (data.item.state !== 'n') {
                dashboardTaskService.patch({ id: data.item.id },
                    { name: data.item.name },
                    function (successData) {
                        data.item.name = successData.name;
                        successCallBack(successData);
                    },
                    function (error) {
                        errorCallBack(error);
                    });
            }
            else {
                data.form.task.$setDirty();
                if (data.form.$valid) {
                    dashboardTaskService.save({ date: self.dateCursor.toServerDateString(), name: data.item.name, groupId: data.item.groupId },
                        function (successData) {
                            data.item.id = successData.id;
                            data.item.state = undefined;
                            data.item.name = successData.name;
                            self.currentTaskIds[data.item.groupId] = undefined;
                            successCallBack(successData);
                        },
                        function (error) {
                            errorCallBack(error);
                        });
                }
            }
        };

        var saveTaskItemMutex = new Mutex();
        self.activeSaveTaskItem = function (successCallBack, errorCallBack, data, x, y, event) {
            saveTaskItemMutex.acquire()
                .then(function (release) {
                    if (event === 'focus') {
                        addChangedUniqId(data.item);
                        release();
                    }
                    else if (data.item.taskItemId) {
                        dashboardTaskItemService.patch({ id: data.item.taskItemId },
                            { time: data.item.time ? data.item.time : 0 },
                            function (successData) {
                                data.item.weekTaskTotal = successData.weekTotal;
                                data.item.monthTaskTotal = successData.monthTotal;
                                getTimeTotal(data.item, data.item.groupId);
                                getTimeTotal();
                                release();
                                if (successCallBack) successCallBack(successData);
                            },
                            function (error) {
                                release();
                                if (errorCallBack) errorCallBack(error);
                            });
                    }
                    else {
                        data.form.taskItem.$setDirty();
                        if (data.form.$valid) {
                            dashboardTaskItemService.save({ date: self.dateCursor.toServerDateString(), time: data.item.time, taskId: data.item.id },
                                function (successData) {
                                    data.item.taskItemId = successData.id;
                                    data.item.weekTaskTotal = successData.weekTotal;
                                    data.item.monthTaskTotal = successData.monthTotal;
                                    getTimeTotal(data.item, data.item.groupId);
                                    getTimeTotal();
                                    release();
                                    if (successCallBack) successCallBack(successData);
                                },
                                function (error) {
                                    release();
                                    if (errorCallBack) errorCallBack(error);
                                });
                        }
                        else {
                            release();
                        }
                    }
                });
        };

        self.changeItem = function (item, state) {
            item.state = state;
        };

        self.addGroup = function (item) {
            var index = self.pageItems.indexOf(item);
            var newGroup = new Object();
            newGroup.uniqId = "u" + uniqIdCount++;
            newGroup.state = 'n';
            newGroup.type = 'g';
            newGroup.dayTaskTotal = 0;
            newGroup.weekTaskTotal = 0;
            newGroup.monthTaskTotal = 0;
            self.pageItems.splice(index, 0, newGroup);
        };

        self.addTask = function (item) {
            var index = self.pageItems.indexOf(item);
            var newTask = new Object();
            newTask.groupId = item.groupId;
            newTask.uniqId = "u" + uniqIdCount++;
            newTask.state = 'n';
            newTask.type = 't';
            newTask.weekTaskTotal = 0;
            newTask.monthTaskTotal = 0;
            self.pageItems.splice(index, 0, newTask);
        };

        self.addTaskButton = function (item) {
            var index = self.pageItems.indexOf(item);
            var newTask = new Object();
            newTask.groupId = item.id;
            newTask.uniqId = "u" + uniqIdCount++;
            newTask.state = 'b';
            newTask.type = 't';
            self.pageItems.splice(index + 1, 0, newTask);
        };

        self.removeGroup = function (item) {
            if (item.id) {
                dashboardGroupService.remove({ id: item.id }, { date: self.dateCursor.toServerDateString(), name: item.name },
                    function (success) {
                        self.currentGroupIds = undefined;
                        getTimeTotal();
                        removeGroupAndSubTasks(item);
                    },
                    function (error) {
                        notificationFactory.error(error);
                    });
            }
            else {
                removeGroupAndSubTasks(item);
            }
        };

        self.removeTask = function (item) {
            if (item.id) {
                dashboardTaskService.remove({ id: item.id }, { date: self.dateCursor.toServerDateString(), name: item.name, groupId: item.groupId },
                    function (success) {
                        self.currentTaskIds[item.groupId] = undefined;
                        getTimeTotal(item, item.groupId);
                        getTimeTotal();
                        var index = self.pageItems.indexOf(item);
                        self.pageItems.splice(index, 1);
                    },
                    function (error) {
                        notificationFactory.error(error);
                    });
            }
            else {
                var index = self.pageItems.indexOf(item);
                self.pageItems.splice(index, 1);
            }
        };

        // Timer Dialog
        self.showTimerDialog = function (item, form) {
            self.timerData = { item: item, form: form };
            if (self.timerData.item.calcBrakeTime === undefined) {
                self.timerData.item.calcBrakeTime = 0;
            }
            if (self.timerData.item.calcTime === undefined) {
                self.timerData.item.calcTime = 0;
            }

            if (self.timerData.item.time >= 0) {
                if (self.timerData.item.calcFromTime === undefined) {
                    self.timerData.item.calcFromTime = 0;
                }
                if (self.timerData.item.calcFromTime + self.timerData.item.time > 1440) {
                    self.timerData.item.calcFromTime = 0;
                }
                self.timerData.item.calcToTime = self.timerData.item.calcFromTime + self.timerData.item.time;
            }

            self.showTimerDialogToggle = true;
        };

        self.saveCalcTime = function () {
            self.activeSaveTaskItem(null, null, self.timerData, null, null, 'focus');
            self.timerData.item.time = self.timerData.item.calcTime;
            self.activeSaveTaskItem(self.closeCalcTime, self.closeCalcTime, self.timerData);
        };

        self.cancelTimerDialog = function () {
            self.closeCalcTime();
        };

        self.closeCalcTime = function () {
            self.showTimerDialogToggle = false;
        };
        
        self.activeCalcTime = function (successCallBack, errorCallBack, data) {
            if (self.timerData.item.calcToTime > 0) {
                if (self.timerData.item.calcFromTime === undefined) {
                    self.timerData.item.calcFromTime = 0;
                }
                var time = Number(self.timerData.item.calcToTime - self.timerData.item.calcFromTime);

                if (self.timerData.item.calcBrakeTime >= 0) {
                    time = time - self.timerData.item.calcBrakeTime;
                }

                if (time < 0) {
                    time = 0;
                }
                self.timerData.item.calcTime = time;
            }
        };

        // PRIVATE FUNCTIONS

        var activate = function () {
            dashboardService = restService.getService('./api/dashboard');
            dashboardGroupService = restService.getService('./api/dashboardGroup');
            dashboardTaskService = restService.getService('./api/dashboardTask');
            dashboardTaskItemService = restService.getService('./api/dashboardTaskItem');
            dashboardTotalService = restService.getService('./api/dashboardTotal');
            filterGroupService = restService.getService('./api/filtergroup');
            filterTaskService = restService.getService('./api/filtertask');

            // LOADS ALL ITEMS
            getAllItems();
        };

        var getAllItems = function () {
            self.loading = true;
            dashboardService.query({ date: self.dateCursor.toServerDateString(), cuid: self.changedUniqIds },
                function (success) {
                    self.currentGroupIds = undefined;
                    self.currentTaskIds = [];
                    self.changedUniqIds = [];
                    self.pageItems = success;
                    self.loading = false;
                },
                function (error) {
                    notificationFactory.error(error);
                });
            getTimeTotal();
        };

        var getTimeTotal = function (item, groupId) {
            if (item !== undefined) {
                var pageItems = self.pageItems, groupItem;
                var index = pageItems.indexOf(item);
                for (var i = index, ilen = 0; i >= ilen; i--) {
                    if (pageItems[i].type === 'g') {
                        groupItem = pageItems[i];
                        break;
                    }
                }
            }

            dashboardTotalService.get({ date: self.dateCursor.toServerDateString(), groupId: item !== undefined ? groupId : null },
                function (success) {
                    if (item !== undefined) {
                        if (groupItem !== undefined) {
                            groupItem.dayTaskTotal = success.dayTotal;
                            groupItem.weekTaskTotal = success.weekTotal;
                            groupItem.monthTaskTotal = success.monthTotal;
                        }
                    }
                    else {
                        self.dayTotal = success.dayTotal;
                        self.weekTotal = success.weekTotal;
                        self.monthTotal = success.monthTotal;
                    }
                },
                function (error) {
                    notificationFactory.error(error);
                });
        };

        var removeGroupAndSubTasks = function (item) {
            var pageItems = self.pageItems, count = 1;
            var index = pageItems.indexOf(item);

            for (var i = index + 1, ilen = pageItems.length; i < ilen; i++) {
                if (pageItems[i].type === 'g') {
                    break;
                }
                else if (pageItems[i].type === 't') {
                    count++;
                }
            }

            pageItems.splice(index, count);
        };

        var getCurrentGroupIds = function () {
            if (self.pageItems) {
                if (!self.currentGroupIds) {
                    var pageItems = self.pageItems, pageItem, groupIds = [];
                    for (var i = 0, ilen = pageItems.length; i < ilen; i++) {
                        pageItem = pageItems[i];
                        if (pageItem.type === 'g' && pageItem.state === undefined) {
                            groupIds.push(pageItem.id);
                        }
                    }
                    self.currentGroupIds = groupIds;
                    return groupIds;
                }
                else {
                    return self.currentGroupIds;
                }
            }
            else {
                self.currentGroupIds = undefined;
                return null;
            }
        };

        var getCurrentTaskIds = function (groupId) {
            if (self.pageItems) {
                if (!self.currentTaskIds[groupId]) {
                    var pageItems = self.pageItems, pageItem, taskIds = [];
                    for (var i = 0, ilen = pageItems.length; i < ilen; i++) {
                        pageItem = pageItems[i];
                        if (pageItem.type === 't' && pageItem.groupId === groupId && pageItem.id !== undefined) {
                            taskIds.push(pageItem.id);
                        }
                    }
                    self.currentTaskIds[groupId] = taskIds;
                    return taskIds;
                }
                else {
                    return self.currentTaskIds[groupId];
                }
            }
            else {
                self.currentTaskIds = [];
                return null;
            }
        };

        var addChangedUniqId = function (item) {
            var uniqId = item.uniqId, changedUniqIds = self.changedUniqIds;

            if (!changedUniqIds.includes(uniqId)) {
                changedUniqIds.push(uniqId);
            }
        };
        
        activate();
    }
})();
