/* topnav.controller.js */

/**
* @desc this controller manages the top navigation section of page
*/

(function () {
    'use strict';

    angular
        .module('webvellaAdmin')  //only gets the module, already initialized in the base.module of the plugin. The lack of dependency [] makes the difference.
        .controller('WebVellaAdminTopnavController', controller);

    // Controller ///////////////////////////////
    controller.$inject = ['$log', '$state', '$rootScope', '$timeout', 'webvellaRootService'];

    /* @ngInject */
    function controller($log, $state, $rootScope, $timeout, webvellaRootService) {
        $log.debug('webvellaAdmin>topnav> BEGIN controller.exec');
        /* jshint validthis:true */
        var topnavData = this;
        topnavData.currentArea = null;
        topnavData.navigateToDekstopBrowse = function () {
            $timeout(function () {
                $state.go('webvella-desktop-browse');
            }, 0);
        }

        topnavData.toggleSideNav = function () {
            webvellaRootService.toggleSidebar();
        }

        $log.debug('webvellaAdmin>topnav> END controller.exec');

    }
    
})();
