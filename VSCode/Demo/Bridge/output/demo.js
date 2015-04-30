Bridge.define('Demo.App', {
    statics: {
        config: {
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            console.log("Testing");
            prompt();
        }
    }
});