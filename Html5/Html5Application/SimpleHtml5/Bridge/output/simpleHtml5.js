Bridge.define('SimpleHtml5.Storage', {
    statics: {
        KEY: "KEY",
        config: {
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            // A root container for the elements we will use in this example -
            // text input and two buttons
            var div = document.createElement('div');

            // Create an input element, with Placeholder text
            // and KeyPress listener to call Save method after Enter key pressed
            var input = Bridge.merge(document.createElement('input'), {
                id: "number", 
                type: "text", 
                placeholder: "Enter a number to store...", 
                style: {
                    margin: "5px"
                }
            } );

            input.addEventListener("keypress", SimpleHtml5.Storage.inputKeyPress);
            div.appendChild(input);

            // Add a Save button to save entered number into Storage
            var buttonSave = Bridge.merge(document.createElement('button'), {
                id: "save", 
                innerHTML: "Save"
            } );

            buttonSave.addEventListener("click", SimpleHtml5.Storage.save);
            div.appendChild(buttonSave);

            // Add a Restore button to get saved number and populate
            // the text input with its value
            var buttonRestore = Bridge.merge(document.createElement('button'), {
                id: "restore", 
                innerHTML: "Restore", 
                style: {
                    margin: "5px"
                }
            } );

            buttonRestore.addEventListener("click", SimpleHtml5.Storage.restore);
            div.appendChild(buttonRestore);

            // Do not forget add the elements on the page
            document.body.appendChild(div);

            // It is good to get the text element focused
            input.focus();
        },
        inputKeyPress: function (e) {
            // We added the listener to EventType.KeyPress so it should be a KeyboardEvent
            if (Bridge.is(e, KeyboardEvent) && e.keyCode === 13) {
                SimpleHtml5.Storage.save();
            }
        },
        save: function () {
            var input = document.getElementById("number");
            var i = parseInt(input.value);

            if (!isNaN(i)) {
                window.localStorage.setItem(SimpleHtml5.Storage.KEY, i);
                window.alert(Bridge.String.format("Stored {0}", i));
                input.value = "";
            }
            else  {
                window.alert("Incorrect value. Please enter a number.");
            }
        },
        restore: function () {
            var input = document.getElementById("number");
            var o = window.localStorage[SimpleHtml5.Storage.KEY];

            if (o !== null) {
                input.value = o.toString();
            }
            else  {
                input.value = "";
            }
        }
    }
});