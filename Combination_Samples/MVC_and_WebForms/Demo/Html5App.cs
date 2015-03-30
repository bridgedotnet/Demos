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

            var form = new FormElement
            {
                ClassName = "col-md-6"
            };

            var panel = Html5App.CreatePanel();
            form.AppendChild(panel);

            Document.Body.AppendChild(form);

            UpdateButton_Click(null);
        }

        public static DivElement CreatePanel()
        {
            var panel = new DivElement
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

        public static DivElement CreatePanelHeader()
        {
            var header = new DivElement
            {
                ClassName = "panel-heading logo"
            };

            var title = new HeadingElement
            {
                InnerHTML = "Bridge.NET HTML5 Demo"
            };

            header.AppendChild(title);

            return header;
        }

        public static DivElement CreatePanelBody()
        {
            var body = new DivElement
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

        public static DivElement CreatePanelFooter()
        {
            var footer = new DivElement
            {
                ClassName = "panel-footer text-right"
            };

            var button = new InputElement
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

        public static DivElement CreateFormField(string name, string glyph)
        {
            var div = new DivElement
            {
                ClassName = "input-group",
                Style =
                {
                    MarginBottom = "10px"
                }
            };

            var span = new SpanElement
            {
                ClassName = "glyphicon glyphicon-" + glyph + " input-group-addon"
            };

            Element input;
            var placeholder = name + "...";

            if (name == "Message")
            {
                input = new TextAreaElement
                {
                    Name = name.ToLowerCase(),
                    Placeholder = placeholder,
                    Required = true,
                    ClassName = "form-control"
                };
            }
            else
            {
                input = new InputElement
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
            var label = new LabelElement
            {
                HtmlFor = "dateTimeInput",
                InnerHTML = "Server Date and Time:"
            };

            var div = new DivElement
            {
                ClassName = "input-group"
            };

            var spanPrefix = new SpanElement
            {
                ClassName = "input-group-addon glyphicon glyphicon-time"
            };

            var spanSuffix = new SpanElement
            {
                ClassName = "input-group-btn"
            };

            var button = new ButtonElement
            {
                Type = ButtonType.Button,
                ClassName = "btn btn-primary",
                InnerHTML = "<span class=\"glyphicon glyphicon-refresh\"></span>",
                OnClick = Html5App.UpdateButton_Click
            };

            var input = new InputElement
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
                var val = JSON.Parse<string>(request.ResponseText);
                var dateTime = DateTime.Parse(val);

                Document.GetElementById<InputElement>("dateTimeInput").Value = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
            };

            request.OnLoad = onLoad;
            request.Open("GET", Config.GET_SERVER_TIME_URL + "?" + DateTime.Now.GetTime(), true);
            request.Send();
        }
    }
}