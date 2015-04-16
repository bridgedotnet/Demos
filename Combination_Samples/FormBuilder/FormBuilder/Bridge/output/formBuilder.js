Bridge.define('FormBuilder.App', {
    statics: {
        FORM_TEMPLATE_DATA: "FormTemplate.ashx",
        FORM_CONTAINER: "#form1",
        SUBMIT_URL: "/Submit.aspx",
        config: {
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            // fetch form template from http handler and start building form
            $.ajax({ url: FormBuilder.App.FORM_TEMPLATE_DATA, cache: false, success: function (data, str, jqXHR) {
                var $t;
                var template = new DTO.Form();

                var rawFields = data.Fields;

                $t = Bridge.getEnumerator(rawFields);
                while ($t.moveNext()) {
                    var rawField = $t.getCurrent();
                    if (rawField.Kind === Bridge.cast(DTO.FormFieldType.radio, Bridge.Int)) {
                        template.getFields().add(Bridge.merge(new DTO.RadioFormField(), {
                            setId: rawField.Id, 
                            setLabel: rawField.Label, 
                            setOptions: rawField.Options
                        } ));
                    }
                    else  {
                        template.getFields().add(Bridge.merge(new DTO.TextFormField(), {
                            setId: rawField.Id, 
                            setLabel: rawField.Label, 
                            setRequired: rawField.Required
                        } ));
                    }
                }

                FormBuilder.App.createForm($(FormBuilder.App.FORM_CONTAINER), template);
            }             });
        },
        createForm: function (container, template) {
            var $t;
            $t = Bridge.getEnumerator(template.getFields());
            while ($t.moveNext()) {
                var field = $t.getCurrent();
                container.append(FormBuilder.App.createFormField(field));
            }

            container.append($("<div>").addClass("form-group").append($("<div>").addClass("col-sm-offset-2 col-sm-10").append($(Bridge.merge(document.createElement('input'), {
    type: "submit", 
    value: "Submit", 
    className: "btn btn-primary", 
    formAction: FormBuilder.App.SUBMIT_URL, 
    formMethod: "POST"
} )))));
        }        ,
        createFormField: function (template) {
            switch (template.getKind()) {
                case DTO.FormFieldType.radio: 
                    return FormBuilder.App.createRadioInput(template.getId(), template.getLabel(), (Bridge.cast(template, DTO.RadioFormField)).getOptions());
                default: 
                    return FormBuilder.App.createTextInput(template.getId(), template.getLabel(), (Bridge.cast(template, DTO.TextFormField)).getRequired());
            }
        },
        createRadioInput: function (id, label, options) {
            var divRadio = $("<div>");
            divRadio.addClass("col-sm-10");

            for (var i = 0; i < options.length; i++) {
                divRadio.append($("<label>").addClass("radio-inline").append(Bridge.merge(document.createElement('input'), {
                    type: "radio", 
                    id: id + "-" + i, 
                    name: id, 
                    value: options[i]
                } )).append(options[i]));
            }

            return $("<div>").addClass("form-group").append(Bridge.merge(document.createElement('label'), {
                className: "control-label col-sm-2", 
                htmlFor: id, 
                innerHTML: label + ":"
            } )).append(divRadio);
        },
        createTextInput: function (id, label, required) {
            return $("<div>").addClass("form-group").append(Bridge.merge(document.createElement('label'), {
                className: "control-label col-sm-2", 
                htmlFor: id, 
                innerHTML: label + ":"
            } )).append($("<div>").addClass("col-sm-10").append(Bridge.merge(document.createElement('input'), {
                type: "text", 
                id: id, 
                name: id, 
                required: required, 
                className: "form-control"
            } )));
        }
    }
});