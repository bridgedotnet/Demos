using Bridge;

namespace Widgetoko
{
    [External]
    public static class Constants
    {
        [InlineConst]
        public const string AppTitle = "Widgetoko";

        [InlineConst]
        public const string UserSettingsFileName = "UserSettings.json";

        public static class IPC
        {
            [InlineConst]
            public const string StartCapture = "cmd-start-capture";

            [InlineConst]
            public const string StopCapture = "cmd-stop-capture";

            [InlineConst]
            public const string ClearCapture = "cmd-clear-capture";

            [InlineConst]
            public const string GetCredentialsSync = "cmd-get-credentials-sync";

            [InlineConst]
            public const string SetCredentials = "cmd-set-credentials";

            [InlineConst]
            public const string ToggleTheme = "cmd-toggle-theme";
        }
    }
}