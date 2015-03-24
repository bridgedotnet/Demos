using System;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace Demo
{
    [FileName("jQueryApp.js")]
    public class jQueryApp
    {
        [Ready]
        public static void Main()
        {
            Document.Body.Style.Padding = "20px";

            new jQuery("<form>")
                .AddClass("col-md-6")
                .Append(jQueryApp.CreatePanel())
                .AppendTo(Document.Body);

            UpdateButton_Click(null);
        }

        public static jQuery CreatePanel()
        {
            return new jQuery("<div>")
                .AddClass("panel panel-default")
                .Append(jQueryApp.CreatePanelHeader())
                .Append(jQueryApp.CreatePanelBody())
                .Append(jQueryApp.CreatePanelFooter());
        }

        public static jQuery CreatePanelHeader()
        {
            return new jQuery("<div>")
                .AddClass("panel-heading logo")
                .Append(new jQuery("<h1>")
                    .Html("Bridge.NET jQuery Demo")
                );
        }

        public static jQuery CreatePanelBody()
        {
            return new jQuery("<div>")
                .AddClass("panel-body")
                .Append(jQueryApp.CreateFormField("Name", "user"))
                .Append(jQueryApp.CreateFormField("Email", "globe"))
                .Append(jQueryApp.CreateFormField("Message", "pencil"))
                .Append(jQueryApp.CreateDateTimeField());
        }

        public static jQuery CreatePanelFooter()
        {
            return new jQuery("<div>")
                .AddClass("panel-footer text-right")
                .Append(new InputElement
                {
                    Type = InputType.Submit,
                    Value = "Submit",
                    ClassName = "btn btn-success",
                    FormAction = Config.SUBMIT_URL,
                    FormMethod = "POST"
                });
        }

        public static jQuery CreateFormField(string name, string glyph)
        {
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

            return new jQuery("<div>")
                .AddClass("input-group")
                .Css("margin-bottom", "10px")
                .Append(new SpanElement
                {
                    ClassName = "glyphicon glyphicon-" + glyph + " input-group-addon"
                })
                .Append(input);
        }

        public static Array CreateDateTimeField()
        {
            return new object[] 
            { 
                new LabelElement
                {
                    HtmlFor = "dateTimeInput",
                    InnerHTML = "Server Date and Time:"
                }, 
                new jQuery(new DivElement
                    {
                        ClassName = "input-group"
                    })
                    .Append(new SpanElement
                    {
                        ClassName = "input-group-addon glyphicon glyphicon-time"
                    })
                    .Append(new InputElement
                    {
                        Id = "dateTimeInput",
                        Type = InputType.Text,
                        ClassName = "form-control",
                        ReadOnly = true,
                        Name = "datetime"
                    })
                    .Append(
                        new jQuery(new SpanElement
                        {
                            ClassName = "input-group-btn"
                        })
                        .Append(new ButtonElement
                        {
                            Type = ButtonType.Button,
                            ClassName = "btn btn-primary",
                            InnerHTML = "<span class=\"glyphicon glyphicon-refresh\"></span>",
                            OnClick = jQueryApp.UpdateButton_Click
                        })
                    )
            };
        }

        public static void UpdateButton_Click(Event e)
        {
            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = Config.GET_SERVER_TIME_URL,
                    Cache = false,
                    Success = delegate(object data, string textStatus, jqXHR request)
                    {
                        var val = JSON.Parse<string>((string)data);
                        var dateTime = DateTime.Parse(val);

                        jQuery.Select("#dateTimeInput").Val(dateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                }
             );
        }
    }
}