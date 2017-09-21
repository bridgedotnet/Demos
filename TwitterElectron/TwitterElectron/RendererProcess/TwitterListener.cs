using System;

namespace TwitterElectron.RendererProcess
{
    public class TwitterListener
    {
        private readonly Twitter _client;
        private TwitterStream _stream;

        public string Filter { get; set; }

        public TwitterListener(
            string consumerKey,
            string consumerSecret,
            string accessTokenKey,
            string accessTokenSecret)
        {
            _client = new Twitter(new TwitterConfig
            {
                consumer_key = consumerKey,
                consumer_secret = consumerSecret,
                access_token_key = accessTokenKey,
                access_token_secret = accessTokenSecret
            });
        }

        public void Start()
        {
            _stream = _client.stream("statuses/filter", new TwitterStreamConfig { track = Filter });
            _stream.onData(tweet =>
            {
                OnReceived?.Invoke(this, tweet);
            });

            _stream.onError(error =>
            {
                OnError?.Invoke(this, error);
            });
        }

        public void Stop()
        {
            if (_stream != null)
            {
                _stream.destroy();
                _stream = null;
            }
        }

        public event EventHandler<Tweet> OnReceived;
        public event EventHandler<string> OnError;
    }
}