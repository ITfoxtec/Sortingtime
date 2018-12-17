(function () {
    'use strict';

    var app = angular
        .module('app', [
        // Angular modules
        'pascalprecht.translate',
        'ngRoute',
        'ngAnimate',
        'ui.bootstrap',
        'ngResource',
        'dateParser',
        'ngMessages',
        'logToServer'


        // Custom modules 

        // 3rd Party Modules

        ]);

    app.config(['$httpProvider', function ($httpProvider) {
        //initialize get if not there
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        //disable IE ajax request caching
        $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';
        $httpProvider.interceptors.push('logToServerInterceptor');
    }])
       .config(['$routeProvider', function ($routeProvider) {
           $routeProvider.
              when('/time', {
                  templateUrl: './app/time/timePage.html',
                  controller: 'timeController as ttCtrl'
              }).
              when('/reporting', {
                  templateUrl: './app/reporting/reportingPage.html',
                  controller: 'reportingController as rpCtrl'
              }).
              when('/invoicing', {
                  templateUrl: './app/invoicing/invoicingPage.html',
                  controller: 'invoicingController as ivCtrl'
              }).
              when('/config', {
                  templateUrl: './app/config/configPage.html',
                  controller: 'configController as configCtrl'
              }).
              otherwise({
                  redirectTo: '/time'
              });
       }])
       .config(['$translateProvider', function ($translateProvider) {
           $translateProvider
              .useSanitizeValueStrategy('escaped')
              .translations(app.translationsCultureName, app.translations)
              .preferredLanguage(app.translationsCultureName)              
              .useMissingTranslationHandlerLog();
       }])
       .run(function ($templateCache, $http) {
           $http.get('./ui/template/messagesScriptTemplate.html')
           .then(function (response) {
               $templateCache.put('error-messages', response.data);
           })
       });

})();
