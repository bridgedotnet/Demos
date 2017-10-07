using Bridge;

namespace Widgetoko.Twitter.API
{
    [ObjectLiteral]
    public class TwitterConfig
    {
        public string consumer_key { get; set; }

        public string consumer_secret { get; set; }

        public string access_token_key { get; set; }

        public string access_token_secret { get; set; }
    }
}