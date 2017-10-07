using System;
using Widgetoko.Twitter.API;

namespace Widgetoko.Twitter
{
    public class TwitterListener
    {
        private TwitterStream _stream;
        private readonly string _filter;
        private readonly API.Twitter _client;

        public TwitterListener(TwitterCredentials credentials, string filter)
        {
            _filter = filter;

            _client = new API.Twitter(new TwitterConfig
            {
                consumer_key = credentials.ApiKey,
                consumer_secret = credentials.ApiSecret,
                access_token_key = credentials.AccessToken,
                access_token_secret = credentials.AccessTokenSecret
            });
        }

        public void Start()
        {
            _stream = _client.stream("statuses/filter", new TwitterStreamConfig { track = _filter });

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