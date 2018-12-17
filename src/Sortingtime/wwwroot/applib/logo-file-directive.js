(function () {
    'use strict';

    angular
        .module('app')
        .directive("logoFile", logoFile);

    // http://www.html5rocks.com/en/tutorials/file/dndfiles/ - god
    // https://github.com/SparrowJang/ngFileReader
    // http://jsfiddle.net/orion514/kkhxsgLu/136/
    // http://odetocode.com/blogs/scott/archive/2013/07/05/a-file-input-directive-for-angularjs.aspx

    logoFile.$inject = ['notificationFactory', 'genericDataFactory', '$translate'];

    function logoFile(notificationFactory, genericDataFactory, $translate) {
        return {
            template: '<div>' +
                        '<div ng-click="onClick($event)" class="form-control" style="height: 128px; width: 168px">' +
                            '<div ng-if="!model" class="placeholder" translate-once="GENERAL.LOGO.YOUR_LOGO"></div>' +
                            '<img id="imgLogo" translate-once-title="GENERAL.LOGO.YOUR_LOGO" ng-src="{{model}}" ng-if="model" />' +
                        '</div>' +
                        '<label tabindex="-1" style="visibility: hidden; position: absolute; overflow: hidden; width: 0px; height: 0px; border: none; margin: 0px; padding: 0px;">' +
                            'upload' + 
                            '<input type="file" accept="image/*" >' +
                        '</label>' +                        
                      '</div>',
            restrict: 'E',
            transclude: true,
            replace: true,
            scope: {
                model: '=ngModel',
            },
            link: function postLink(scope, element, attrs) {
                // PRIVATE
                var currentLogoService;
             
                //EVENTS
                scope.onClick = function ($event) {
                    $event.stopPropagation();
                    $event.preventDefault();

                    var inputFile = element[0].childNodes[1].childNodes[1];
                    inputFile.click();
                }

                //PRIVATE FUNCTIONS
                var getLogo = function (changeEvent) {
                    currentLogoService.getLogo(
                        // success response
                        function (success) {
                            if (success) {
                                scope.model = success;
                            }
                        });
                }

                var changeHandler = function (changeEvent) {
                    if (typeof FileReader == "undefined") {
                        $translate('GENERAL.LOGO.NOT_SUPPORTED').then(function (text) {
                            notificationFactory.error(text);
                        });                        
                    }

                    var file = changeEvent.target.files[0];

                    if (!file) {
                        return;
                    }

                    // Only process image files.
                    if (!file.type.match('image.*')) {
                        return;
                    }
                        
                    var size = file.size;
                    if (size > 500000) {
                        $translate('GENERAL.LOGO.SIZE_ERROR').then(function (text) {
                            notificationFactory.error(text);
                        });
                        return;
                    }
                        
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        // handle onload            
                        currentLogoService.saveLogo(e.target.result,
                            // success response
                            function (success) {
                                if (success) {
                                    scope.model = success;
                                }
                            });
                    };
                    reader.readAsDataURL(file);
                };

                //var updateModel = function (model) {
                //    scope.$apply(function () {
                //        scope.model = model;
                //    });
                //}

                element.bind('change', changeHandler);

                var activate = function () {
                    currentLogoService = genericDataFactory.currentLogo.getService();

                    // Load logo
                    getLogo();
                }
                activate();

            },
        };
    }

})();