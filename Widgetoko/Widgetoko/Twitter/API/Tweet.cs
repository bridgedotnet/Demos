using Bridge;

namespace Widgetoko.Twitter.API
{
    [External]
    public class Tweet
    {
        public string id_str { get; set; }

        public string text { get; set; }

        public TweetEntities entities { get; set; }

        public string lang { get; set; }

        public TweetUser user { get; set; }
    }
}