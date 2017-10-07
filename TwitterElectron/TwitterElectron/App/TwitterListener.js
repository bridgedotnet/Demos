Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    var Twitter = require("twitter");

    Bridge.define("TwitterElectron.RendererProcess.TwitterListener", {
        fields: {
            _client: null,
            _stream: null
        },
        events: {
            OnReceived: null,
            OnError: null
        },
        props: {
            Filter: null
        },
        ctors: {
            ctor: function (consumerKey, consumerSecret, accessTokenKey, accessTokenSecret) {
                this.$initialize();
                this._client = new Twitter({ consumer_key: consumerKey, consumer_secret: consumerSecret, access_token_key: accessTokenKey, access_token_secret: accessTokenSecret });
            }
        },
        methods: {
            Start: function () {
                this._stream = this._client.stream("statuses/filter", { track: this.Filter });

                this._stream.on("data", Bridge.fn.bind(this, function (tweet) {
                        !Bridge.staticEquals(this.OnReceived, null) ? this.OnReceived(this, tweet) : null;
                    }));

                this._stream.on("error", Bridge.fn.bind(this, function (error) {
                        !Bridge.staticEquals(this.OnError, null) ? this.OnError(this, error) : null;
                    }));
            },
            Stop: function () {
                if (this._stream != null) {
                    this._stream.destroy();
                    this._stream = null;
                }
            }
        }
    });
});
