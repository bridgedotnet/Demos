using Bridge.Html5;
using Bridge.React;
using Bridge.Redux;
using ReactReduxTodoApp.Models;
using ReactReduxTodoApp.Reducers;
using ReactReduxTodoApp.UI;
using ReactReduxTodoApp.Actions;
using Bridge;
using System.Linq;

namespace ReactReduxTodoApp
{
    public static class App
    {
        public static void Main()
        {
            var initialState = new TodoAppState
            {
                DescriptionInput = "",
                Visibility = TodoVisibility.All,
                Todos = new Todo[]
                {
                    new Todo
                    {
                        Id = 0,
                        Description = "Learn React + Redux in C#",
                        IsCompleted = true
                    }, 
                    new Todo
                    {
                        Id = 1,
                        Description = "Build an awesome app with them",
                        IsCompleted = false
                    }
                }
            };

            var store = Redux.CreateStore(Todos.Reducer(initialState));

            React.Render(Components.TodoItemList(store), Document.GetElementById("root"));
        }
    }
}
