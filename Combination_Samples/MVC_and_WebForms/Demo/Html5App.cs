using System;
using Bridge;
using Bridge.Html5;

namespace Demo
{
    [FileName("Html5App.js")]
    public class Html5App
    {
        [Ready]
        public static void Main()
        {
            Document.Body.Style.Padding = "20px";

            var form = new HTMLFormElement
            {
                ClassName = "col-md-6"
            };

            var panel = Html5App.CreatePanel();
            form.AppendChild(panel);

            Document.Body.AppendChild(form);

            UpdateButton_Click(null);
        }

        public static HTMLDivElement CreatePanel()
        {
            var panel = new HTMLDivElement
            {
                ClassName = "panel panel-default"
            };

            var header = Html5App.CreatePanelHeader();
            var body = Html5App.CreatePanelBody();
            var footer = Html5App.CreatePanelFooter();

            panel.AppendChild(header);
            panel.AppendChild(body);
            panel.AppendChild(footer);

            return panel;
        }

        public static HTMLDivElement CreatePanelHeader()
        {
            var header = new HTMLDivElement
            {
                ClassName = "panel-heading logo"
            };

            var title = new HTMLHeadingElement
            {
                InnerHTML = "Bridge.NET HTML5 Demo"
            };

            header.AppendChild(title);

            return header;
        }

        public static HTMLDivElement CreatePanelBody()
        {
            var body = new HTMLDivElement
            {
                ClassName = "panel-body"
            };

            var name = Html5App.CreateFormField("Name", "user");
            var email = Html5App.CreateFormField("Email", "globe");
            var message = Html5App.CreateFormField("Message", "pencil");
            Element[] dateTime = Html5App.CreateDateTimeField();

            body.AppendChild(name);
            body.AppendChild(email);
            body.AppendChild(message);
            body.AppendChild(dateTime[0]);
            body.AppendChild(dateTime[1]);

            return body;
        }

        public static HTMLDivElement CreatePanelFooter()
        {
            var footer = new HTMLDivElement
            {
                ClassName = "panel-footer text-right"
            };

            var button = new  HTMLInputElement
            {
                Type = InputType.Submit,
                Value = "Submit",
                ClassName = "btn btn-success",
                FormAction = Config.SUBMIT_URL,
                FormMethod = "POST"
            };

            footer.AppendChild(button);

            return footer;
        }

        public static HTMLDivElement CreateFormField(string name, string glyph)
        {
            var div = new HTMLDivElement
            {
                ClassName = "input-group",
                Style =
                {
                    MarginBottom = "10px"
                }
            };

            var span = new HTMLSpanElement
            {
                ClassName = "glyphicon glyphicon-" + glyph + " input-group-addon"
            };

            Element input;
            var placeholder = name + "...";

            if (name == "Message")
            {
                input = new HTMLTextAreaElement
                {
                    Name = name.ToLowerCase(),
                    Placeholder = placeholder,
                    Required = true,
                    ClassName = "form-control"
                };
            }
            else
            {
                input = new  HTMLInputElement
                {
                    Type = InputType.Text,
                    Name = name.ToLowerCase(),
                    Placeholder = placeholder,
                    Required = true,
                    ClassName = "form-control"
                };
            }

            div.AppendChild(span);
            div.AppendChild(input);

            return div;
        }

        public static Element[] CreateDateTimeField()
        {
            var label = new HTMLLabelElement
            {
                HtmlFor = "dateTimeInput",
                InnerHTML = "Server Date and Time:"
            };

            var div = new HTMLDivElement
            {
                ClassName = "input-group"
            };

            var spanPrefix = new HTMLSpanElement
            {
                ClassName = "input-group-addon glyphicon glyphicon-time"
            };

            var spanSuffix = new HTMLSpanElement
            {
                ClassName = "input-group-btn"
            };

            var button = new HTMLButtonElement
            {
                Type = ButtonType.Button,
                ClassName = "btn btn-primary",
                InnerHTML = "<span class=\"glyphicon glyphicon-refresh\"></span>",
                OnClick = Html5App.UpdateButton_Click
            };

            var input = new  HTMLInputElement
            {
                Id = "dateTimeInput",
                Type = InputType.Text,
                ClassName = "form-control",
                Placeholder = "Click update...",
                ReadOnly = true,
                Name = "datetime"
            };

            spanSuffix.AppendChild(button);

            div.AppendChild(spanPrefix);
            div.AppendChild(input);
            div.AppendChild(spanSuffix);

            return new Element[] { label, div };
        }

        public static void UpdateButton_Click(Event e)
        {
            var request = new XMLHttpRequest();

            Action<Event> onLoad = delegate
            {
                var val = JSON.Parse(request.ResponseText).As<string>();
                var dateTime = DateTime.Parse(val);

                Document.GetElementById< HTMLInputElement>("dateTimeInput").Value = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
            };

            request.OnLoad = onLoad;
            request.Open("GET", Config.GET_SERVER_TIME_URL + "?" + DateTime.Now.ToString(), true);
            request.Send();
        }
    }
}