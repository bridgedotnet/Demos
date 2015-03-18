Bridge.Class.define('Demo.Html5App', {
    statics: {
        config:  {
            init: function () {
                Bridge.on('DOMContentLoaded', document, this.main);
            }
        },
        main: function () {
            document.body.style.padding = "20px";
            var form = Bridge.merge(document.createElement('form'), {
                className: "col-md-6"
            } );
            var panel = Demo.Html5App.createPanel();
            form.appendChild(panel);
            document.body.appendChild(form);
            Demo.Html5App.updateButton_Click(null);
        },
        createPanel: function () {
            var panel = Bridge.merge(document.createElement('div'), {
                className: "panel panel-default"
            } );
            var header = Demo.Html5App.createPanelHeader();
            var body = Demo.Html5App.createPanelBody();
            var footer = Demo.Html5App.createPanelFooter();
            panel.appendChild(header);
            panel.appendChild(body);
            panel.appendChild(footer);
            return panel;
        },
        createPanelHeader: function () {
            var header = Bridge.merge(document.createElement('div'), {
                className: "panel-heading logo"
            } );
            var title = Bridge.merge(document.createElement('h1'), {
                innerHTML: "Bridge.NET HTML5 Demo"
            } );
            header.appendChild(title);
            return header;
        },
        createPanelBody: function () {
            var body = Bridge.merge(document.createElement('div'), {
                className: "panel-body"
            } );
            var name = Demo.Html5App.createFormField("Name", "user");
            var email = Demo.Html5App.createFormField("Email", "globe");
            var message = Demo.Html5App.createFormField("Message", "pencil");
            var dateTime = Demo.Html5App.createDateTimeField();
            body.appendChild(name);
            body.appendChild(email);
            body.appendChild(message);
            body.appendChild(dateTime[0]);
            body.appendChild(dateTime[1]);
            return body;
        },
        createPanelFooter: function () {
            var footer = Bridge.merge(document.createElement('div'), {
                className: "panel-footer text-right"
            } );
            var button = Bridge.merge(document.createElement('input'), {
                type: "submit", 
                value: "Submit", 
                className: "btn btn-success", 
                formAction: Demo.Config.SUBMIT_URL, 
                formMethod: "POST"
            } );
            footer.appendChild(button);
            return footer;
        },
        createFormField: function (name, glyph) {
            var div = Bridge.merge(document.createElement('div'), {
                className: "input-group", 
                style: {
                    marginBottom: "10px"
                }
            } );
            var span = Bridge.merge(document.createElement('span'), {
                className: "glyphicon glyphicon-" + glyph + " input-group-addon"
            } );
            var input;
            var placeholder = name + "...";
            if (name === "Message") {
                input = Bridge.merge(document.createElement('textarea'), {
                    name: name.toLowerCase(), 
                    placeholder: placeholder, 
                    required: true, 
                    className: "form-control"
                } );
            }
            else  {
                input = Bridge.merge(document.createElement('input'), {
                    type: "text", 
                    name: name.toLowerCase(), 
                    placeholder: placeholder, 
                    required: true, 
                    className: "form-control"
                } );
            }
            div.appendChild(span);
            div.appendChild(input);
            return div;
        },
        createDateTimeField: function () {
            var label = Bridge.merge(document.createElement('label'), {
                htmlFor: "dateTimeInput", 
                innerHTML: "Server Date and Time:"
            } );
            var div = Bridge.merge(document.createElement('div'), {
                className: "input-group"
            } );
            var spanPrefix = Bridge.merge(document.createElement('span'), {
                className: "input-group-addon glyphicon glyphicon-time"
            } );
            var spanSuffix = Bridge.merge(document.createElement('span'), {
                className: "input-group-btn"
            } );
            var button = Bridge.merge(document.createElement('button'), {
                type: "button", 
                className: "btn btn-primary", 
                innerHTML: "<span class=\"glyphicon glyphicon-refresh\"></span>", 
                onclick: Demo.Html5App.updateButton_Click
            } );
            var input = Bridge.merge(document.createElement('input'), {
                id: "dateTimeInput", 
                type: "text", 
                className: "form-control", 
                placeholder: "Click update...", 
                readOnly: true, 
                name: "datetime"
            } );
            spanSuffix.appendChild(button);
            div.appendChild(spanPrefix);
            div.appendChild(input);
            div.appendChild(spanSuffix);
            return [label, div];
        },
        updateButton_Click: function (e) {
            var request = new XMLHttpRequest();
            var onLoad = function () {
                var val = JSON.parse(request.responseText);
                var dateTime = Bridge.Date.parse(val);
                document.getElementById("dateTimeInput").value = Bridge.Date.format(dateTime, "yyyy-MM-dd hh:mm:ss");
            };
            request.onload = onLoad;
            request.open("GET", "/Data/GetServerTime?" + new Date().getTime(), true);
            request.send();
        }
    }
});

