Bridge.assembly("Widgetoko", function ($asm, globals) {
    "use strict";

    var Twitter = require("twitter");

    Bridge.define("Widgetoko.Twitter.TwitterListener", {
        fields: {
            _stream: null,
            _filter: null,
            _client: null
        },
        events: {
            OnReceived: null,
            OnError: null
        },
        ctors: {
            ctor: function (credentials, filter) {
                this.$initialize();
                this._filter = filter;

                this._client = new Twitter({ consumer_key: credentials.ApiKey, consumer_secret: credentials.ApiSecret, access_token_key: credentials.AccessToken, access_token_secret: credentials.AccessTokenSecret });
            }
        },
        methods: {
            Start: function () {
                this._stream = this._client.stream("statuses/filter", { track: this._filter });

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
