using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.Bootstrap3;
using DTO;

namespace FormBuilder
{
    public class App
    {
        public const string FORM_TEMPLATE_DATA = "FormTemplate.ashx";
        public const string FORM_CONTAINER = "#form1";
        public const string SUBMIT_URL = "/Submit.aspx";

        [Ready]
        public static void Main()
        {
            // fetch form template from http handler and start building form
            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = App.FORM_TEMPLATE_DATA,
                    Cache = false,
                    Success = delegate(object data, string str, jqXHR jqXHR)
                    {
                        Form template = new Form(data);
                        App.CreateForm(jQuery.Select(FORM_CONTAINER), template);
                    }
                }
            );
        }

        public static void CreateForm(jQuery container, Form template)
        {
            foreach (FormField field in template.Fields)
            {
                container.Append(App.CreateFormField(field));
            }

            container.Append(new jQuery("<div>")
                .AddClass("form-group")
                .Append(new jQuery("<div>")
                    .AddClass("col-sm-offset-2 col-sm-10")
                    .Append(new jQuery(new HTMLInputElement
                    {
                        Type = InputType.Submit,
                        Value = "Submit",
                        ClassName = "btn btn-primary",
                        FormAction = SUBMIT_URL,
                        FormMethod = "POST"
                    }))
                )
            );
        }

        public static jQuery CreateFormField(FormField template)
        {
            switch (template.Kind)
            {
                case FormFieldType.Radio: return CreateRadioInput(template.Id, template.Label, ((RadioFormField)template).Options);
                default: return CreateTextInput(template.Id, template.Label, ((TextFormField)template).Required);
            }
        }

        public static jQuery CreateRadioInput(string id, string label, string[] options)
        {
            jQuery divRadio = new jQuery("<div>");
            divRadio.AddClass("col-sm-10");

            for (int i = 0; i < options.Length; i++)
            {
                divRadio
                    .Append(new jQuery("<label>")
                        .AddClass("radio-inline")
                        .Append(new HTMLInputElement
                        {
                            Type = InputType.Radio,
                            Id = id + "-" + i,
                            Name = id,
                            Value = options[i]
                        })
                        .Append(options[i])
                    );
            }

            return new jQuery("<div>")
                .AddClass("form-group")
                .Append(new HTMLLabelElement
                {
                    ClassName = "control-label col-sm-2",
                    HtmlFor = id,
                    InnerHTML = label + ":"
                })
                .Append(divRadio);
        }

        public static jQuery CreateTextInput(string id, string label, bool required = false)
        {
            return new jQuery("<div>")
                .AddClass("form-group")
                .Append(new HTMLLabelElement
                {
                    ClassName = "control-label col-sm-2",
                    HtmlFor = id,
                    InnerHTML = label + ":"
                })
                .Append(new jQuery("<div>")
                    .AddClass("col-sm-10")
                    .Append(new HTMLInputElement
                    {
                        Type = InputType.Text,
                        Id = id,
                        Name = id,
                        Required = required,
                        ClassName = "form-control"
                    })
                );
        }
    }
}
