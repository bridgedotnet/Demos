using System;
using System.Collections.Generic;

using Bridge.Html5;


/// <summary>
/// http://www.websocket.org/echo.html demo written in C# with Bridge.NET
/// </summary>
namespace WebSocketSample
{
    public class WebSocketController
    {
        private const string DEFAULT_SERVICE_URI = "ws://echo.websocket.org/";

        public WebSocket Socket { get; set; }

        public HtmlPageView View { get; set; }

        #region View events

        private void View_OnLogClearing(object sender, EventArgs e)
        {
            View.ClearLog();
        }

        private void View_OnUseSecureWebSocketChanged(object sender, EventArgs e)
        {
            ApplySecureWebSocketCheckbox();
        }

        private void View_OnInitialized(object sender, EventArgs e)
        {
            ApplySecureWebSocketCheckbox();
        }

        private void View_OnConnecting(object sender, EventArgs e)
        {
            try
            {
                View.ClearLog();

                Socket = new WebSocket(View.WebSocketUrl);

                Socket.OnOpen += OnSocketOpen;
                Socket.OnClose += OnSocketClose;
                Socket.OnMessage += OnSocketMessage;
                Socket.OnError += OnSocketError;
            }
            catch (Exception ex)
            {
                View.LogError(ex.ToString());
            }
        }

        private void View_OnSending(object sender, EventArgs e)
        {
            if (Socket != null)
            {
                var message = View.Message;

                View.LogMessage("SENDING: " + message);

                //TODO !Async?
                Socket.Send(message);
            }
        }

        private void View_OnDisconnecting(object sender, EventArgs e)
        {
            if (Socket != null)
            {
                Socket.Close(CloseEvent.StatusCode.CLOSE_NORMAL, "User would like to close");
            }
        }

        private void View_OnShuttingUp(object sender, EventArgs e)
        {
            if (Socket != null)
            {
                Socket.Close(CloseEvent.StatusCode.CLOSE_GOING_AWAY, "Shutting up");

                Window.Alert("AWAY");
            }
        }

        #endregion View events

        #region Socket events

        private void OnSocketOpen(Event e)
        {
            View.LogMessage("CONNECTED: " + Socket.Url);
            View.RefreshUI(true);
        }

        private void OnSocketClose(CloseEvent e)
        {
            View.LogMessage("DISCONNECTED: {Reason: " + e.Reason + ", Code: " + e.Code + ", WasClean: " + e.WasClean + "}");
            View.RefreshUI(false);
        }

        private void OnSocketMessage(MessageEvent e)
        {
            View.LogResponse(e.Data != null ? e.Data.ToString() : "no reponse data");
        }

        private void OnSocketError(Event e)
        {
            var error = e["message"];
            View.LogError(error != null ? error.ToString() : "");
        }

        #endregion Socket events

        public void Initialize(HtmlPageView view)
        {
            View = view;

            View.OnViewInitialized += View_OnInitialized;
            View.OnViewConnecting += View_OnConnecting;
            View.OnViewDisconnecting += View_OnDisconnecting;
            View.OnViewSending += View_OnSending;
            View.OnViewUseSecureWebSocketChanged += View_OnUseSecureWebSocketChanged;
            View.OnViewLogClearing += View_OnLogClearing;
            View.OnViewShuttingUp += View_OnShuttingUp;

            View.Initialize();

            View.LogMessage("Checking if browser supports WebSocket...");
            if (!view.CheckIfBrowserSupportsWebSocket())
            {
                View.LogError(" Not Supported!");
                return;
            }

            View.LogMessage(" Supported!", false);
        }

        private void ApplySecureWebSocketCheckbox()
        {
            var wsPort = string.IsNullOrEmpty(View.Port) ? "" : ":" + View.Port;

            var url = View.WebSocketUrl;

            if (string.IsNullOrEmpty(url))
            {
                url = WebSocketController.DEFAULT_SERVICE_URI + wsPort;
            }

            if (View.UseSecureWebSocket)
            {
                View.WebSocketUrl = url.Replace("ws:", "wss:");
            }
            else
            {
                View.WebSocketUrl = url.Replace("wss:", "ws:");
            }
        }
    }
}
