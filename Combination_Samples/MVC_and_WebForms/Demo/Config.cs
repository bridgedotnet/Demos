using Bridge;

namespace Demo
{
    [FileName("Config.js")]
    public static class Config
    {
        public const string SUBMIT_URL = "/Submit";
        public const string GET_SERVER_TIME_URL = "/Data/GetServerTime";
        public const string LONG_RUNNING_PROCESS = "/Data/LongRunningProcess";
    }
}