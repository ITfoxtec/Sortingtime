(function () {
    'use strict';

    Date.prototype.toDateOnly = function () {
        return new Date(this.toDateString());
    };   

    Date.prototype.toDateOnlyFirstDayInMonth = function () {
        return new Date(this.getFullYear(), this.getMonth(), 1);
    };

    Date.prototype.toDateOnlyLastDayInMonth = function () {
        return new Date(this.getFullYear(), this.getMonth() + 1, 0);
    };

    // Add days to Date object
    Date.prototype.addDays = function (days) {
        this.setTime(this.getTime() + (days * (1000 * 60 * 60 * 24)));
        return this;
    };

    Date.prototype.addMonths = function (months) {
        var thisYear = this.getFullYear();
        var thisMonth = this.getMonth();
        var thisDay = this.getDate();

        var tempMonth = thisMonth + months;
        if (tempMonth > 12) {
            var years = tempMonth / 12;
            thisYear = thisYear + years;
            thisMonth = thisMonth + months - (years * 12);
        }
        else if (tempMonth < 1) {
            do {
                thisYear = thisYear - 1;
                tempMonth = tempMonth + 12;
            }
            while (tempMonth < 1);
            thisMonth = tempMonth;
        }
        else {
            thisMonth = tempMonth;
        }

        var newDate = new Date(thisYear, thisMonth, thisDay);       
        return newDate;
    };

    Date.prototype.toServerDateString = function () {
        var month = this.getMonth() + 1;
        return this.getFullYear() + '-' + pad(month) + '-' + pad(this.getDate()) + 'T00:00:00';
    };

    var pad = function (n) {
        return (n < 10) ? ("0" + n) : n;
    };

})();
