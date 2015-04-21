Bridge.define('DTO.Form', {
    config: {
        properties: {
            Fields: null
        }
    },
    constructor: function () {
        this.setFields(new Bridge.List$1(DTO.FormField)());
    },
    constructor$1: function (obj) {
        this.$constructor();

        var rawFields = obj.Fields;

        $t = Bridge.getEnumerator(rawFields);
        while ($t.moveNext()) {
            var rawField = $t.getCurrent();
            if (rawField.Kind === Bridge.cast(DTO.FormFieldType.radio, Bridge.Int)) {
                this.getFields().add(Bridge.merge(new DTO.RadioFormField(), {
                    setId: rawField.Id, 
                    setLabel: rawField.Label, 
                    setOptions: rawField.Options
                } ));
            }
            else  {
                this.getFields().add(Bridge.merge(new DTO.TextFormField(), {
                    setId: rawField.Id, 
                    setLabel: rawField.Label, 
                    setRequired: rawField.Required
                } ));
            }
        }
    }
});

Bridge.define('DTO.FormField', {
    config: {
        properties: {
            Kind: 0,
            Id: null,
            Label: null
        }
    }
});

Bridge.define('DTO.FormFieldType', {
    statics: {
        text: 0,
        radio: 1
    }
});

Bridge.define('DTO.RadioFormField', {
    inherits: [DTO.FormField],
    config: {
        properties: {
            Options: null
        }
    },
    constructor: function () {
        DTO.FormField.prototype.$constructor.call(this);

        this.setKind(DTO.FormFieldType.radio);
    }
});

Bridge.define('DTO.TextFormField', {
    inherits: [DTO.FormField],
    config: {
        properties: {
            Required: false
        }
    },
    constructor: function () {
        DTO.FormField.prototype.$constructor.call(this);

        this.setKind(DTO.FormFieldType.text);
        this.setRequired(false);
    }
});

