Bridge.define('DTO.Form', {
    config: {
        properties: {
            Fields: null
        }
    },
    constructor: function () {
        this.setFields(new Bridge.List$1(DTO.FormField)());
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

