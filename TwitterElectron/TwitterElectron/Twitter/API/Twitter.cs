using System;
using Bridge;

namespace TwitterElectron.Twitter.API
{
    [External]
    [Module(Bridge.ModuleType.CommonJS, "twitter", ExportAsNamespace = "Twitter")]
    [GlobalMethods]
    public class Twitter
    {
        public extern Twitter(TwitterConfig config);

        public extern TwitterStream stream(string method, TwitterStreamConfig config);

        public extern void stream(string method, TwitterStreamConfig config, Action<TwitterStream> callback);
    }
}