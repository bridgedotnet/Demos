using System;
using Bridge;

namespace TwitterElectron.RendererProcess
{
    [Bridge.ObjectLiteral]
    public class TwitterConfig
    {
        public string consumer_key { get; set; }
        public string consumer_secret { get; set; }
        public string access_token_key { get; set; }
        public string access_token_secret { get; set; }
    }

    [Bridge.External]
    [Bridge.Module(Bridge.ModuleType.CommonJS, "twitter", ExportAsNamespace = "Twitter")]
    [Bridge.GlobalMethods]
    public class Twitter
    {
        public extern Twitter(TwitterConfig config);

        public extern TwitterStream stream(string method, TwitterStreamConfig config);

        public extern void stream(string method, TwitterStreamConfig config, Action<TwitterStream> callback);
    }

    [Bridge.External]
    public class TwitterStream
    {
        [Template("on(\"data\", {0})")]
        public extern void onData(Action<Tweet> handler);

        [Template("on(\"error\", {0})")]
        public extern void onError(Action<string> handler);

        public extern void destroy();
    }

    [Bridge.ObjectLiteral]
    public class TwitterStreamConfig
    {
        public string track { get; set; }
    }

    [Bridge.External]
    public class Tweet
    {
        public string id_str { get; set; }

        public string text { get; set; }

        public TweetEntities entities { get; set; }

        public string lang { get; set; }

        public TweetUser user { get; set; }
    }

    [Bridge.External]
    public class TweetEntities
    {
        public string[] hashtags { get; set; }
    }

    [Bridge.External]
    public class TweetUser
    {
        public string name { get; set; }

        public string screen_name { get; set; }

        public string profile_image_url { get; set; }
    }
}