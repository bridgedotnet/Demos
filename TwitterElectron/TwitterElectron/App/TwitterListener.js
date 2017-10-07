Bridge.assembly("TwitterElectron", function ($asm, globals) {
    "use strict";

    var Twitter = require("twitter");

    Bridge.define("TwitterElectron.Twitter.TwitterListener", {
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

                this._stream.on("data", Bridge.fn.bind(this, $asm.$.TwitterElectron.Twitter.TwitterListener.f1));

                this._stream.on("error", Bridge.fn.bind(this, $asm.$.TwitterElectron.Twitter.TwitterListener.f2));
            },
            Stop: function () {
                if (this._stream != null) {
                    this._stream.destroy();
                    this._stream = null;
                }
            }
        }
    });

    Bridge.ns("TwitterElectron.Twitter.TwitterListener", $asm.$);

    Bridge.apply($asm.$.TwitterElectron.Twitter.TwitterListener, {
        f1: function (tweet) {
            !Bridge.staticEquals(this.OnReceived, null) ? this.OnReceived(this, tweet) : null;
        },
        f2: function (error) {
            !Bridge.staticEquals(this.OnError, null) ? this.OnError(this, error) : null;
        }
    });
});
