(function () {
    'use strict';

    angular
        .module('app')
        .factory('notificationFactory', notificationFactory)
    
    function notificationFactory() {
        // http://codeseven.github.io/toastr/demo.html
        return {
            success: function (text) {
                toastr.options = {
                    "showDuration": "100",
                    "hideDuration": "0",
                    "timeOut": "2000",
                    "extendedTimeOut": "3000",
                    "positionClass": "toast-top-center",
                }
                toastr.clear();
                toastr.options = {
                    "showDuration": "100",
                    "hideDuration": "100",
                    "timeOut": "2000",
                    "extendedTimeOut": "3000",
                    "positionClass": "toast-top-center",
                }
                if (text === undefined) {
                    text = 'Success.';
                }
                toastr.success(text);
            },
            error: function (error) {
                var text;
                if (error) {
                    if (error.isHandled === true) {
                        return;
                    }

                    var addStatusCode = true;
                    if (error.length && error.length > 0) {
                        text = error;
                        addStatusCode = false;
                    }
                    else if (error && error.data && angular.isObject(error.data))
                    {
                        for (var key in error.data) {
                            text = error.data[key];
                            break;
                        }
                    }

                    if (addStatusCode && error.status) {
                        if(!text) {
                            text = 'Http status ' + error.status + '. ' + error.statusText;
                        }
                    }
                }
                toastr.options = {
                    "showDuration": "100",
                    "hideDuration": "100",
                    "timeOut": "6000",
                    "extendedTimeOut": "8000",
                    "positionClass": "toast-top-center",
                }
                if (text === undefined) {
                    text = 'Unknown error occurred.';
                }
                toastr.error(text);
            },
            warning: function (text) {
                toastr.options = {
                    "showDuration": "100",
                    "hideDuration": "100",
                    "timeOut": "6000",
                    "extendedTimeOut": "8000",
                    "positionClass": "toast-top-center",
                }
                if (text === undefined) {
                    text = 'Unknown warning occurred.';
                }
                toastr.warning(text);
            },
        };
    }

})();
