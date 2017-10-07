using System;
using Bridge;

namespace Widgetoko.Twitter.API
{
    [External]
    public class TwitterStream
    {
        [Template("on(\"data\", {0})")]
        public extern void onData(Action<Tweet> handler);

        [Template("on(\"error\", {0})")]
        public extern void onError(Action<string> handler);

        public extern void destroy();
    }
}