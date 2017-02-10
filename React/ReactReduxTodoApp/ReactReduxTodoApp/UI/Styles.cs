using Bridge.React;

namespace ReactReduxTodoApp.UI
{
    public static class Styles
    {
        public static ReactStyle TodoItemStyle = new ReactStyle
        {
            Margin = "5px",
            FontSize = 18,
        };


        public static ReactStyle TextStyleComplete = new ReactStyle
        {
            Margin = 5,
            TextDecoration = Bridge.Html5.TextDecoration.LineThrough,
            Color = "red"
        };


        public static ReactStyle TextStyleNotComplete = new ReactStyle
        {
            Margin = 5,
            TextDecoration = Bridge.Html5.TextDecoration.None
        };


    }
}