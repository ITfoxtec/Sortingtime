(function () {
    'use strict';

    angular
        .module('app')
        .filter("time", timeFormatFilter)

    //timeFormatFilter.$inject = [];

    function timeFormatFilter() {
        return function (input) {
            if (angular.isDefined(input)) {
                if (input && !angular.isNumber(input)) {
                    input = Number(input);
                }
                if (input) {
                    var minutes = input % 60;
                    var hours = (input - minutes) / 60;

                    input = new String(hours) + ':' + ('0' + minutes).slice(-2);
                }
            }
            return input;
        };
    }

})();
