/* global Bridge */

Bridge.define('BridgeStub.App', {
    statics: {
        config: {
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            console.log("Live Bridge.NET started.");
        }
    }
});