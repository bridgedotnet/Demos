(function (globals) {
    "use strict";

    Bridge.define('WebSocketSample.HtmlPageView', {
        statics: {
            config: {
                init: function () {
                    Bridge.ready(this.start);
                }
            },
            start: function () {
                new WebSocketSample.WebSocketController().initialize(new WebSocketSample.HtmlPageView());
            }
        },
        consoleLog: null,
        lastLogMessage: null,
        webSocketSupported: null,
        webSocketNotSupported: null,
        webSocketSupportImage: null,
        useSecureWebSocketInput: null,
        locationInput: null,
        messageInput: null,
        connectButton: null,
        disconnectButton: null,
        sendButton: null,
        clearLogButton: null,
        config: {
            events: {
                OnViewInitialized: null,
                OnViewConnecting: null,
                OnViewDisconnecting: null,
                OnViewSending: null,
                OnViewUseSecureWebSocketChanged: null,
                OnViewLogClearing: null,
                OnViewShuttingUp: null
            }
        },
        getMessage: function () {
            return this.messageInput.value;
        },
        getPort: function () {
            return window.location.port;
        },
        getWebSocketUrl: function () {
            return this.locationInput.value;
        },
        setWebSocketUrl: function (value) {
            this.locationInput.value = value;
        },
        getUseSecureWebSocket: function () {
            return this.useSecureWebSocketInput.checked;
        },
        onClick_UseSecureWebSocket: function (e) {
            if (Bridge.hasValue(this.OnViewUseSecureWebSocketChanged)) {
                this.OnViewUseSecureWebSocketChanged(this, new Object());
            }
        },
        onClick_ConnectButton: function (e) {
            if (Bridge.hasValue(this.OnViewConnecting)) {
                this.OnViewConnecting(this, new Object());
            }
        },
        onClick_DisconnectButton: function (e) {
            if (Bridge.hasValue(this.OnViewDisconnecting)) {
                this.OnViewDisconnecting(this, new Object());
            }
        },
        onClick_SendButton: function (e) {
            if (Bridge.hasValue(this.OnViewSending)) {
                this.OnViewSending(this, new Object());
            }
        },
        onClick_ClearLogButton: function (e) {
            if (Bridge.hasValue(this.OnViewLogClearing)) {
                this.OnViewLogClearing(this, new Object());
            }
        },
        initialize: function () {
            this.consoleLog = document.getElementById("consoleLog");
    
            this.webSocketSupportImage = document.getElementById("wsSupportImg");
    
            this.webSocketSupported = document.getElementById("webSocketSupp");
    
            this.webSocketNotSupported = document.getElementById("noWebSocketSupp");
    
            this.useSecureWebSocketInput = document.getElementById("secureCb");
            this.useSecureWebSocketInput.checked = false;
            this.useSecureWebSocketInput.onclick = Bridge.fn.combine(this.useSecureWebSocketInput.onclick, Bridge.fn.bind(this, this.onClick_UseSecureWebSocket));
    
            this.locationInput = document.getElementById("wsUri");
    
            this.messageInput = document.getElementById("sendMessage");
    
            this.connectButton = document.getElementById("connect");
            this.connectButton.onclick = Bridge.fn.combine(this.connectButton.onclick, Bridge.fn.bind(this, this.onClick_ConnectButton));
    
            this.disconnectButton = document.getElementById("disconnect");
            this.disconnectButton.onclick = Bridge.fn.combine(this.disconnectButton.onclick, Bridge.fn.bind(this, this.onClick_DisconnectButton));
    
            this.sendButton = document.getElementById("send");
            this.sendButton.onclick = Bridge.fn.combine(this.sendButton.onclick, Bridge.fn.bind(this, this.onClick_SendButton));
    
            this.clearLogButton = document.getElementById("clearLogBut");
            this.clearLogButton.onclick = Bridge.fn.combine(this.clearLogButton.onclick, Bridge.fn.bind(this, this.onClick_ClearLogButton));
    
            window.onbeforeunload = Bridge.fn.combine(window.onbeforeunload, Bridge.fn.bind(this, $_.WebSocketSample.HtmlPageView.f1));
    
            if (Bridge.hasValue(this.OnViewInitialized)) {
                this.OnViewInitialized(this, new Object());
            }
        },
        checkIfBrowserSupportsWebSocket: function () {
            var hasWebSocket = Bridge.hasValue(window.WebSocket);
    
            if (hasWebSocket) {
                this.webSocketSupportImage.src = "img/check.png";
                this.webSocketSupported.style.display = "block";
            }
            else  {
                this.webSocketSupportImage.src = "img/cross.png";
                this.webSocketNotSupported.style.display = "block";
            }
    
            return hasWebSocket;
        },
        refreshUI: function (isConnected) {
            this.locationInput.disabled = isConnected;
            this.connectButton.disabled = isConnected;
            this.disconnectButton.disabled = !isConnected;
            this.messageInput.disabled = !isConnected;
            this.sendButton.disabled = !isConnected;
            this.useSecureWebSocketInput.disabled = isConnected;
    
            var labelColor = isConnected ? "#999999" : "black";
            this.useSecureWebSocketInput.style.color = labelColor;
        },
        logMessage: function (message, newLine) {
            if (newLine === void 0) { newLine = true; }
            if (!Bridge.hasValue(message)) {
                message = "";
            }
    
            message = this.getSecureTag() + message;
    
            var createdNew = false;
            if (!Bridge.hasValue(this.lastLogMessage) || newLine) {
                this.lastLogMessage = document.createElement("p");
                this.lastLogMessage.style.wordWrap = "normal";
                this.lastLogMessage.style.margin = "5px";
                createdNew = true;
            }
    
            if (createdNew) {
                this.lastLogMessage.innerHTML = message;
                this.consoleLog.appendChild(this.lastLogMessage);
            }
            else  {
                this.lastLogMessage.innerHTML += message;
            }
    
            this.consoleLog.scrollTop = this.consoleLog.scrollHeight;
        },
        logResponse: function (response) {
            var $t;
            this.logMessage("<span style=\"color: blue;\">RESPONSE: " + (($t = response, Bridge.hasValue($t) ? $t : "")) + "</span>");
        },
        logError: function (error) {
            var $t;
            this.logMessage("<span style=\"color: red;\">ERROR:</span> " + (($t = error, Bridge.hasValue($t) ? $t : "")));
        },
        clearLog: function () {
            this.lastLogMessage = null;
    
            while (this.consoleLog.childNodes.length > 0) {
                this.consoleLog.removeChild(this.consoleLog.lastChild);
            }
        },
        getSecureTag: function () {
            if (this.getUseSecureWebSocket()) {
                return "<img src=\"img/tls-lock.png\" width=\"6px\" height=\"9px\"> ";
            }
            else  {
                return "";
            }
        }
    });
    
    var $_ = {};
    
    Bridge.ns("WebSocketSample.HtmlPageView", $_)
    
    Bridge.apply($_.WebSocketSample.HtmlPageView, {
        f1: function (e) {
            if (Bridge.hasValue(this.OnViewShuttingUp)) {
                this.OnViewShuttingUp(this, new Object());
            }
        }
    });
    
    Bridge.define('WebSocketSample.WebSocketController', {
        statics: {
            DEFAULT_SERVICE_URI: "ws://echo.websocket.org/"
        },
        config: {
            properties: {
                Socket: null,
                View: null
            }
        },
        view_OnLogClearing: function (sender, e) {
            this.getView().clearLog();
        },
        view_OnUseSecureWebSocketChanged: function (sender, e) {
            this.applySecureWebSocketCheckbox();
        },
        view_OnInitialized: function (sender, e) {
            this.applySecureWebSocketCheckbox();
        },
        view_OnConnecting: function (sender, e) {
            try {
                this.getView().clearLog();
    
                this.setSocket(new WebSocket(this.getView().getWebSocketUrl()));
    
                this.getSocket().onopen = Bridge.fn.combine(this.getSocket().onopen, Bridge.fn.bind(this, this.onSocketOpen));
                this.getSocket().onclose = Bridge.fn.combine(this.getSocket().onclose, Bridge.fn.bind(this, this.onSocketClose));
                this.getSocket().onmessage = Bridge.fn.combine(this.getSocket().onmessage, Bridge.fn.bind(this, this.onSocketMessage));
                this.getSocket().onerror = Bridge.fn.combine(this.getSocket().onerror, Bridge.fn.bind(this, this.onSocketError));
            }
            catch (ex) {
                ex = Bridge.Exception.create(ex);
                this.getView().logError(ex.toString());
            }
        },
        view_OnSending: function (sender, e) {
            if (Bridge.hasValue(this.getSocket())) {
                var message = this.getView().getMessage();
    
                this.getView().logMessage("SENDING: " + message);
    
                //TODO !Async?
                this.getSocket().send(message);
            }
        },
        view_OnDisconnecting: function (sender, e) {
            if (Bridge.hasValue(this.getSocket())) {
                this.getSocket().close(1000, "User would like to close");
            }
        },
        view_OnShuttingUp: function (sender, e) {
            if (Bridge.hasValue(this.getSocket())) {
                this.getSocket().close(1001, "Shutting up");
    
                window.alert("AWAY");
            }
        },
        onSocketOpen: function (e) {
            this.getView().logMessage("CONNECTED: " + this.getSocket().url);
            this.getView().refreshUI(true);
        },
        onSocketClose: function (e) {
            this.getView().logMessage("DISCONNECTED: {Reason: " + e.reason + ", Code: " + e.code + ", WasClean: " + e.wasClean + "}");
            this.getView().refreshUI(false);
        },
        onSocketMessage: function (e) {
            this.getView().logResponse(Bridge.hasValue(e.data) ? e.data.toString() : "no reponse data");
        },
        onSocketError: function (e) {
            var error = e.message;
            this.getView().logError(Bridge.hasValue(error) ? error.toString() : "");
        },
        initialize: function (view) {
            this.setView(view);
    
            this.getView().addOnViewInitialized(Bridge.fn.bind(this, this.view_OnInitialized));
            this.getView().addOnViewConnecting(Bridge.fn.bind(this, this.view_OnConnecting));
            this.getView().addOnViewDisconnecting(Bridge.fn.bind(this, this.view_OnDisconnecting));
            this.getView().addOnViewSending(Bridge.fn.bind(this, this.view_OnSending));
            this.getView().addOnViewUseSecureWebSocketChanged(Bridge.fn.bind(this, this.view_OnUseSecureWebSocketChanged));
            this.getView().addOnViewLogClearing(Bridge.fn.bind(this, this.view_OnLogClearing));
            this.getView().addOnViewShuttingUp(Bridge.fn.bind(this, this.view_OnShuttingUp));
    
            this.getView().initialize();
    
            this.getView().logMessage("Checking if browser supports WebSocket...");
            if (!view.checkIfBrowserSupportsWebSocket()) {
                this.getView().logError(" Not Supported!");
                return;
            }
    
            this.getView().logMessage(" Supported!", false);
        },
        applySecureWebSocketCheckbox: function () {
            var wsPort = Bridge.String.isNullOrEmpty(this.getView().getPort()) ? "" : ":" + this.getView().getPort();
    
            var url = this.getView().getWebSocketUrl();
    
            if (Bridge.String.isNullOrEmpty(url)) {
                url = Bridge.get(WebSocketSample.WebSocketController).DEFAULT_SERVICE_URI + wsPort;
            }
    
            if (this.getView().getUseSecureWebSocket()) {
                this.getView().setWebSocketUrl(Bridge.String.replaceAll(url, "ws:", "wss:"));
            }
            else  {
                this.getView().setWebSocketUrl(Bridge.String.replaceAll(url, "wss:", "ws:"));
            }
        }
    });
    
    Bridge.init();
})(this);
