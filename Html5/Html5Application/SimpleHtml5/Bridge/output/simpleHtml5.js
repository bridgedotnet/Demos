/**
 * @version 1.0.0
 * @author Object.NET, Inc.
 * @copyright Copyright 2008-2015 Object.NET, Inc.
 * @compiler Bridge.NET 16.0.0-beta5
 */
Bridge.assembly("SimpleHtml5", function ($asm, globals) {
    "use strict";

    Bridge.define("SimpleHtml5.Storage", {
        statics: {
            fields: {
                KEY: null
            },
            ctors: {
                init: function () {
                    this.KEY = "KEY";
                    Bridge.ready(this.Main);
                }
            },
            methods: {
                Main: function () {
                    var $t;
                    // A root container for the elements we will use in this example -
                    // text input and two buttons
                    var div = document.createElement('div');

                    // Create an input element, with Placeholder text
                    // and KeyPress listener to call Save method after Enter key pressed
                    var input = ($t = document.createElement('input'), $t.id = "number", $t.type = "text", $t.placeholder = "Enter a number to store...", $t.style.margin = "5px", $t);

                    input.addEventListener("keypress", SimpleHtml5.Storage.InputKeyPress);
                    div.appendChild(input);

                    // Add a Save button to save entered number into Storage
                    var buttonSave = ($t = document.createElement('button'), $t.id = "save", $t.innerHTML = "Save", $t);

                    buttonSave.addEventListener("click", SimpleHtml5.Storage.Save);
                    div.appendChild(buttonSave);

                    // Add a Restore button to get saved number and populate
                    // the text input with its value
                    var buttonRestore = ($t = document.createElement('button'), $t.id = "restore", $t.innerHTML = "Restore", $t.style.margin = "5px", $t);

                    buttonRestore.addEventListener("click", SimpleHtml5.Storage.Restore);
                    div.appendChild(buttonRestore);

                    // Do not forget add the elements on the page
                    document.body.appendChild(div);

                    // It is good to get the text element focused
                    input.focus();
                },
                InputKeyPress: function (e) {
                    // We added the listener to EventType.KeyPress so it should be a KeyboardEvent
                    if (Bridge.is(e, KeyboardEvent) && e.keyCode === 13) {
                        SimpleHtml5.Storage.Save();
                    }
                },
                Save: function () {
                    var input = document.getElementById("number");
                    var i = parseInt(input.value);

                    if (!isNaN(i)) {
                        window.localStorage.setItem(SimpleHtml5.Storage.KEY, i);
                        window.alert(System.String.format("Stored {0}", Bridge.box(i, System.Int32)));
                        input.value = "";
                    } else {
                        window.alert("Incorrect value. Please enter a number.");
                    }
                },
                Restore: function () {
                    var input = document.getElementById("number");
                    var o = window.localStorage[System.Array.index(SimpleHtml5.Storage.KEY, window.localStorage)];

                    if (o != null) {
                        input.value = o.toString();
                    } else {
                        input.value = "";
                    }
                }
            }
        },
        $entryPoint: true
    });
});
