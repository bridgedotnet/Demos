Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    Bridge.define("TwitterElectron.RendererProcess.TwitterCredentials", {
        $literal: true,
        methods: {
            AreValid: function () {
                return !System.String.isNullOrEmpty(this.ApiKey) && !System.String.isNullOrEmpty(this.ApiSecret) && !System.String.isNullOrEmpty(this.AccessToken) && !System.String.isNullOrEmpty(this.AccessTokenSecret);
            }
        }
    });
});
