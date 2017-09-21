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
                this._stream.on("data", Bridge.fn.bind(this, $asm.$.TwitterElectron.RendererProcess.TwitterListener.f1));

                this._stream.on("error", Bridge.fn.bind(this, $asm.$.TwitterElectron.RendererProcess.TwitterListener.f2));
            },
            Stop: function () {
                if (this._stream != null) {
                    this._stream.destroy();
                    this._stream = null;
                }
            }
        }
    });

    Bridge.ns("TwitterElectron.RendererProcess.TwitterListener", $asm.$);

    Bridge.apply($asm.$.TwitterElectron.RendererProcess.TwitterListener, {
        f1: function (tweet) {
            !Bridge.staticEquals(this.OnReceived, null) ? this.OnReceived(this, tweet) : null;
        },
        f2: function (error) {
            !Bridge.staticEquals(this.OnError, null) ? this.OnError(this, error) : null;
        }
    });
});
