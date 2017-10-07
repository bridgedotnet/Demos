using Bridge;

namespace Widgetoko.Twitter
{
    [ObjectLiteral]
    public class TwitterCredentials
    {
        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }
    }
}