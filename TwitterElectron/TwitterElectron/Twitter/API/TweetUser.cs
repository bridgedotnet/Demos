using Bridge;

namespace TwitterElectron.Twitter.API
{
    [External]
    public class TweetUser
    {
        public string name { get; set; }

        public string screen_name { get; set; }

        public string profile_image_url { get; set; }
    }
}