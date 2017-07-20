using Bridge;
using Bridge.Html5;
using System;

namespace SimpleHtml5
{
    class Storage
    {
        private const string KEY = "KEY";

        // Use the attribute to mark methods that should be run on DOMContentLoaded event
        [Ready]
        public static void Main()
        {
            // A root container for the elements we will use in this example -
            // text input and two buttons
            var div = new Bridge.Html5.HTMLDivElement();

            // Create an input element, with Placeholder text
            // and KeyPress listener to call Save method after Enter key pressed
            var input = new Bridge.Html5.HTMLInputElement()
            {
                Id = "number",
                Type = InputType.Text,
                Placeholder = "Enter a number to store...",
                Style =
                {
                    Margin = "5px"
                }
            };

            input.AddEventListener(EventType.KeyPress, InputKeyPress);
            div.AppendChild(input);

            // Add a Save button to save entered number into Storage
            var buttonSave = new Bridge.Html5.HTMLButtonElement()
            {
                Id = "save",
                InnerHTML = "Save"
            };

            buttonSave.AddEventListener(EventType.Click, Save);
            div.AppendChild(buttonSave);

            // Add a Restore button to get saved number and populate
            // the text input with its value
            var buttonRestore = new Bridge.Html5.HTMLButtonElement()
            {
                Id = "restore",
                InnerHTML = "Restore",
                Style =
                {
                    Margin = "5px"
                }
            };

            buttonRestore.AddEventListener(EventType.Click, Restore);
            div.AppendChild(buttonRestore);

            // Do not forget add the elements on the page
            Document.Body.AppendChild(div);

            // It is good to get the text element focused
            input.Focus();
        }

        private static void InputKeyPress(Event e)
        {
            // We added the listener to EventType.KeyPress so it should be a KeyboardEvent
            if (e.IsKeyboardEvent() && e.As<KeyboardEvent>().KeyCode == 13)
            {
                Save();
            }
        }

        private static void Save()
        {
            var input = Document.GetElementById<HTMLInputElement>("number");
            int i = Window.ParseInt(input.Value);

            if (!Window.IsNaN(i))
            {
                Window.LocalStorage.SetItem(KEY, i);
                Window.Alert(string.Format("Stored {0}", i));
                input.Value = string.Empty;
            }
            else
            {
                Window.Alert("Incorrect value. Please enter a number.");
            }
        }

        private static void Restore()
        {
            var input = Document.GetElementById<HTMLInputElement>("number");
            var o = Window.LocalStorage[KEY];

            if (o != null)
            {
                input.Value = o.ToString();
            }
            else
            {
                input.Value = string.Empty;
            }
        }
    }
}