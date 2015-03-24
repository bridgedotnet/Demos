using System;
using System.Threading.Tasks;
using Bridge;
using Bridge.Bootstrap3;
using Bridge.Html5;
using Bridge.jQuery2;

namespace Demo
{
    [FileName("BootstrapApp.js")]
    public class BootstrapApp
    {
        [Ready]
        public static void Main()
        {
            Document.Body.Style.Padding = "20px";

            new jQuery("<form>")
                .AddClass("col-md-6")
                .Append(BootstrapApp.CreatePanel())
                .AppendTo(Document.Body);

            UpdateButton_Click(null);
            ListenToBootstrapEvents();
        }

        public static jQuery CreatePanel()
        {
            return new jQuery("<div>")
                .AddClass("panel panel-default")
                .Append(BootstrapApp.CreatePanelHeader())
                .Append(BootstrapApp.CreatePanelBody())
                .Append(BootstrapApp.CreatePanelFooter());
        }

        public static jQuery CreatePanelHeader()
        {
            return new jQuery("<div>")
                .AddClass("panel-heading logo")
                .Append(new jQuery("<h1>")
                    .Html("Bridge.NET Bootstrap Demo")
                );
        }

        public static jQuery CreatePanelBody()
        {
            return new jQuery("<div>")
                .AddClass("panel-body collapse in")
                .Append(BootstrapApp.CreateFormField("Name", "user"))
                .Append(BootstrapApp.CreateFormField("Email", "globe"))
                .Append(BootstrapApp.CreateFormField("Message", "pencil"))
                .Append(BootstrapApp.CreateDateTimeField());
        }

        public static jQuery CreatePanelFooter()
        {
            return new jQuery("<div>")
                .AddClass("panel-footer text-right")
                .Append(BootstrapApp.CreateProcessButton())
                .Append(
                    new jQuery(new ButtonElement
                    {
                        ClassName = "btn btn-primary",
                        InnerHTML = "Collapse",
                        Style =
                        {
                            MarginRight = "10px"
                        }
                    })
                    .On("click", (Action<jQueryEvent>)delegate(jQueryEvent e)
                    {
                        jQuery.Select(".panel-body").CollapseToggle();

                        jQuery.This.Text(jQuery.This.Text() == "Collapse" ? "Expand" : "Collapse");

                        e.PreventDefault();
                    })
                )
                .Append(
                    new jQuery(new ButtonElement
                    {
                        ClassName = "btn btn-primary",
                        InnerHTML = "Toggle Tooltips and Popovers",
                        Style =
                        {
                            MarginRight = "10px"
                        }
                    })
                    .On("click", (Action<jQueryEvent>)delegate(jQueryEvent e)
                    {
                        jQuery.Select("[rel=\"popover\"]").PopoverToggle();
                        jQuery.Select("[rel=\"tooltip\"]").Tooltip(PopupOperation.Toggle);
                        e.PreventDefault();
                    })
                )
                .Append(
                    new jQuery(new InputElement
                    {
                        Type = InputType.Submit,
                        Value = "Submit",
                        ClassName = "btn btn-success",
                        FormAction = Config.SUBMIT_URL,
                        FormMethod = "POST"
                    })
                    .Attr("rel", "tooltip")
                );
        }

        public static jQuery CreateProcessButton()
        {
            return new jQuery(new ButtonElement
            {
                ClassName = "btn btn-primary",
                InnerHTML = "Async Process",
                Style =
                {
                    MarginRight = "10px"
                }
            })
            .On(EventType.Click, (Action<jQueryEvent>)async delegate(jQueryEvent e)
            {
                e.PreventDefault();

                BootstrapApp.CreateModalWithProgressBar()
                    .AppendTo(Document.Body)
                    .Modal(new ModalOptions { Backdrop = "static" });

                var i = 0;
                var title = jQuery.Select(".modal-title");

                var intervalId = Window.SetInterval((Action)delegate
                {
                    var percent = (i++ % 20) * 10;

                    jQuery.Select(".progress-bar")
                        .Attr("aria-valuenow", percent)
                        .Attr("style", "width:" + percent + "%;");

                    title.Html("Long running process: " + percent + "% complete");
                }, 490);

                var result = await Task.FromPromise<string>(jQuery.Ajax(new AjaxOptions
                {
                    Url = Config.LONG_RUNNING_PROCESS,
                    Cache = false
                }),
                (Func<string>)(() => { return Script.Arguments[0].ToString(); }));

                var msg = JSON.Parse<string>(result);

                if (msg == "ok")
                {
                    Window.ClearInterval(intervalId);

                    jQuery.Select(".progress-bar")
                        .Attr("aria-valuenow", 100)
                        .Attr("style", "width: 100%;")
                        .Html("Done");

                    jQuery.Select(".modal-title")
                        .Html("Success!");

                    await Task.Delay(2000);

                    var modal = jQuery.Select(".modal")
                        .On(ModalEvent.Hidden, (Action)delegate
                        {
                            jQuery.This.Remove();
                        })
                        .ModalHide();
                }
            });
        }

        public static jQuery CreateModalWithProgressBar()
        {
            return new jQuery("<div>")
                .AddClass("modal fade")
                .Append(new jQuery("<div>")
                    .AddClass("modal-dialog")
                    .Append(new jQuery("<div>")
                        .AddClass("modal-content")
                        .Append(new jQuery("<div>")
                            .AddClass("modal-header")
                            .Append(new jQuery("<h4>")
                                .AddClass("modal-title")
                                .Html("Long running process...")
                            )
                        )
                        .Append(new jQuery("<div>")
                            .AddClass("modal-body")
                            .Append(new jQuery("<div>")
                                .AddClass("progress")
                                .Append(new jQuery("<div>")
                                    .AddClass("progress-bar")
                                    .Attr("role", "progressbar")
                                    .Attr("aria-valuemin", "0")
                                    .Attr("aria-valuemax", "100")
                                )
                            )
                        )
                    )
                );
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
                .Append(new jQuery(input)
                    .Attr("rel", "tooltip")
                    .Tooltip(new TooltipOptions
                    {
                        Title = "Required Field",
                        Container = "body",
                        Placement = PopupPlacement.Right
                    })
                );
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
                    .Append(
                        new jQuery(new InputElement
                        {
                            Id = "dateTimeInput",
                            Type = InputType.Text,
                            ClassName = "form-control",
                            ReadOnly = true,
                            Name = "datetime"
                        })
                        .Attr("rel", "popover")
                        .Popover(new PopoverOptions
                        {
                            Content = "This is a ReadOnly Field",
                            Container = "body",
                            Trigger = PopupTrigger.Hover,
                            Placement = PopupPlacement.Top
                        })
                    )
                    .Append(
                        new jQuery(new SpanElement
                        {
                            ClassName = "input-group-btn"
                        })
                        .Append(
                            new jQuery(new ButtonElement
                            {
                                Type = ButtonType.Button,
                                ClassName = "btn btn-primary",
                                InnerHTML = "<span class=\"glyphicon glyphicon-refresh\"></span>"
                            })
                            .Attr("data-loading-text", "Loading...")
                            .Attr("rel", "popover")
                            .On("click", (Action<jQueryEvent>)UpdateButton_Click)
                            .Popover(new PopoverOptions
                            {
                                Title = "Get the server Date and Time",
                                Content = "Clicking this Button initiates an AJAX request to the server",
                                Container = "body",
                                Trigger = PopupTrigger.Hover
                            })
                        )
                    )
            };
        }

        public static async void UpdateButton_Click(jQueryEvent e)
        {
            jQuery btn = null;
            var delay = 0;

            if (e != null)
            {
                btn = new jQuery(e.CurrentTarget);
                btn.ButtonLoading();
                delay = 2000;
            }

            // Delaying is to demonstrate the Button Loading state functionality of Bootstrap
            await Task.Delay(delay);

            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = Config.GET_SERVER_TIME_URL,
                    Cache = false,

                    Success = delegate(object data, string textStatus, jqXHR request)
                    {
                        if (btn != null)
                        {
                            btn.ButtonReset();
                        }

                        var val = JSON.Parse<string>((string)data);
                        var dateTime = DateTime.ParseExact(val, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        jQuery.Select("#dateTimeInput").Val(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                );
        }

        public static void ShowAlert(string message)
        {
            var alert = new jQuery("<div>")
                .AddClass("alert alert-info col-md-4")
                .Attr("role", "alert")
                .Attr("style", "display: block; position: relative;")
                .Html(message)
                .AppendTo(Document.Body);

            Window.SetTimeout(() => alert.Remove(), 3000);
        }

        public static void ListenToBootstrapEvents()
        {
            jQuery.Select(".collapse")
                // Adding Event with Delegate
                .On(CollapseEvent.Hidden, delegate { ShowAlert("The Panel's body has been collapsed"); })
                // Adding Event with Lambda
                .On(CollapseEvent.Shown, () => ShowAlert("The Panel's body has been expanded"));
        }
    }
}