(function () {
    'use strict';

    angular
        .module('app')
        .service("restService", restService);

    restService.$inject = ['$resource'];

    function restService($resource) {
        this.getService = function (endPoint) {
            if (endPoint === '') {
                throw "Invalid end point";
            }

            // create resource to make REST calls
            return $resource(endPoint + '/:id',
            {
                id: '@id' // default URL params, '@' Indicates that the value should be obtained from a data property 
            },
            {
                'get': { method: 'GET', interceptor: { responseError: responseError }, timeout: 10000, warningAfter: 8000 },
                'save': { method: 'POST', interceptor: { responseError: responseError }, timeout: 8000, warningAfter: 5000 },
                'query': { method: 'GET', interceptor: { responseError: responseError }, timeout: 8000, warningAfter: 5000, isArray: true },
                'update': { method: 'PUT', interceptor: { responseError: responseError }, timeout: 8000, warningAfter: 5000 }, // add update to actions (is not defined by default)
                'patch': { method: 'PATCH', interceptor: { responseError: responseError }, timeout: 8000, warningAfter: 5000 }, // add patch to actions (is not defined by default)           
                'remove': { method: 'PUT', interceptor: { responseError: responseError }, timeout: 8000, warningAfter: 5000 }, // add remove to PUT actions - with PUT it is possible to send the HTTP body.
                'delete': { method: 'DELETE', interceptor: { responseError: responseError }, timeout: 8000, warningAfter: 5000 },
            });
        }

        var responseError = function (error) {
            if (!error.isHandled) {
                var errorMessage = "timeout";
                if (error && error.status && error.data) {
                    errorMessage = error.data.ExceptionMessage;
                }

                JL('Angular.Ajax').fatalException({ errorMessage: errorMessage, status: error.status, config: error.config }, error.data);
            }
        }
    }

})();
