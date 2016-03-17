using Bridge.Html5;

using System;

/// <summary>
/// http://www.websocket.org/echo.html demo written in Bridge C#
/// </summary>
namespace WebSocketSample
{
    public class HtmlPageView
    {
        #region UI elements

        private DivElement ConsoleLog;
        private PreElement LastLogMessage;
        private DivElement WebSocketSupported;
        private DivElement WebSocketNotSupported;
        private ImageElement WebSocketSupportImage;
        private InputElement UseSecureWebSocketInput;
        private InputElement LocationInput;
        private InputElement MessageInput;
        private ButtonElement ConnectButton;
        private ButtonElement DisconnectButton;
        private ButtonElement SendButton;
        private ButtonElement ClearLogButton;

        #endregion UI elements

        #region View events

        public event EventHandler OnViewInitialized;
        public event EventHandler OnViewConnecting;
        public event EventHandler OnViewDisconnecting;
        public event EventHandler OnViewSending;
        public event EventHandler OnViewUseSecureWebSocketChanged;
        public event EventHandler OnViewLogClearing;
        public event EventHandler OnViewShuttingUp;

        #endregion View events

        #region UI event handlers

        private void OnClick_UseSecureWebSocket(MouseEvent<InputElement> e)
        {
            if (OnViewUseSecureWebSocketChanged != null)
            {
                OnViewUseSecureWebSocketChanged(this, new EventArgs());
            }
        }

        private void OnClick_ConnectButton(MouseEvent<ButtonElement> e)
        {
            if (OnViewConnecting != null)
            {
                OnViewConnecting(this, new EventArgs());
            }
        }

        private void OnClick_DisconnectButton(MouseEvent<ButtonElement> e)
        {
            if (OnViewDisconnecting != null)
            {
                OnViewDisconnecting(this, new EventArgs());
            }
        }

        private void OnClick_SendButton(MouseEvent<ButtonElement> e)
        {
            if (OnViewSending != null)
            {
                OnViewSending(this, new EventArgs());
            }
        }

        private void OnClick_ClearLogButton(MouseEvent<ButtonElement> e)
        {
            if (OnViewLogClearing != null)
            {
                OnViewLogClearing(this, new EventArgs());
            }
        }

        #endregion UI event handlers

        #region View properties

        public string Message { get { return MessageInput.Value; } }
        public string Port { get { return Window.Location.Port; } }
        public string WebSocketUrl { get { return LocationInput.Value; } set { LocationInput.Value = value; } }
        public bool UseSecureWebSocket { get { return UseSecureWebSocketInput.Checked; } }

        #endregion Veiw properties

        #region Methods

        public void Initialize()
        {
            ConsoleLog = Document.GetElementById<DivElement>("consoleLog");

            WebSocketSupportImage = Document.GetElementById<ImageElement>("wsSupportImg");

            WebSocketSupported = Document.GetElementById<DivElement>("webSocketSupp");

            WebSocketNotSupported = Document.GetElementById<DivElement>("noWebSocketSupp");

            UseSecureWebSocketInput = Document.GetElementById<InputElement>("secureCb");
            UseSecureWebSocketInput.Checked = false;
            UseSecureWebSocketInput.OnClick += OnClick_UseSecureWebSocket;

            LocationInput = Document.GetElementById<InputElement>("wsUri");

            MessageInput = Document.GetElementById<InputElement>("sendMessage");

            ConnectButton = Document.GetElementById<ButtonElement>("connect");
            ConnectButton.OnClick += OnClick_ConnectButton;

            DisconnectButton = Document.GetElementById<ButtonElement>("disconnect");
            DisconnectButton.OnClick += OnClick_DisconnectButton;

            SendButton = Document.GetElementById<ButtonElement>("send");
            SendButton.OnClick += OnClick_SendButton;

            ClearLogButton = Document.GetElementById<ButtonElement>("clearLogBut");
            ClearLogButton.OnClick += OnClick_ClearLogButton;

            Window.OnBeforeUnload += (e) => { if (OnViewShuttingUp != null) { OnViewShuttingUp(this, new EventArgs()); } };

            if (OnViewInitialized != null)
            {
                OnViewInitialized(this, new EventArgs());
            }
        }

        public bool CheckIfBrowserSupportsWebSocket()
        {
            var hasWebSocket = Window.WebSocket != null;

            if (hasWebSocket)
            {
                WebSocketSupportImage.Src = "img/check.png";
                WebSocketSupported.Style.Display = Display.Block;
            }
            else
            {
                WebSocketSupportImage.Src = "img/cross.png";
                WebSocketNotSupported.Style.Display = Display.Block;
            }

            return hasWebSocket;
        }

        public void RefreshUI(bool isConnected)
        {
            LocationInput.Disabled = isConnected;
            ConnectButton.Disabled = isConnected;
            DisconnectButton.Disabled = !isConnected;
            MessageInput.Disabled = !isConnected;
            SendButton.Disabled = !isConnected;
            UseSecureWebSocketInput.Disabled = isConnected;

            var labelColor = isConnected ? "#999999" : "black";
            UseSecureWebSocketInput.Style.Color = labelColor;
        }

        #endregion Methods

        #region LogConsole methods

        public void LogMessage(string message, bool newLine = true)
        {
            if (message == null)
            {
                message = "";
            }

            message = GetSecureTag() + message;

            var createdNew = false;
            if (LastLogMessage == null || newLine)
            {
                LastLogMessage = Document.CreateElement<PreElement>("p");
                LastLogMessage.Style.WordWrap = WordWrap.Normal;
                LastLogMessage.Style.Margin = "5px";
                createdNew = true;
            }

            if (createdNew)
            {
                LastLogMessage.InnerHTML = message;
                ConsoleLog.AppendChild(LastLogMessage);
            }
            else
            {
                LastLogMessage.InnerHTML += message;
            }

            ConsoleLog.ScrollTop = ConsoleLog.ScrollHeight;
        }

        public void LogResponse(string response)
        {
            LogMessage("<span style=\"color: blue;\">RESPONSE: " + (response ?? "") + "</span>");
        }

        public void LogError(string error)
        {
            LogMessage("<span style=\"color: red;\">ERROR:</span> " + (error ?? ""));
        }

        public void ClearLog()
        {
            LastLogMessage = null;

            while (ConsoleLog.ChildNodes.Length > 0)
            {
                ConsoleLog.RemoveChild(ConsoleLog.LastChild);
            }
        }

        private string GetSecureTag()
        {
            if (UseSecureWebSocket)
            {
                return "<img src=\"img/tls-lock.png\" width=\"6px\" height=\"9px\"> ";
            }
            else
            {
                return "";
            }
        }

        #endregion LogConsole methods

        [Ready]
        public static void Start()
        {
            new WebSocketController().Initialize(new HtmlPageView());
        }
    }
}
