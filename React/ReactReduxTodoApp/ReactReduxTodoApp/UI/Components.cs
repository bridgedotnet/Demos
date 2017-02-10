using Bridge.React;
using Bridge.Redux;
using Bridge.ReactRedux;

using System;
using System.Collections.Generic;

using ReactReduxTodoApp.Models;
using ReactReduxTodoApp.Actions;
using System.Linq;
using Bridge.Linq;

namespace ReactReduxTodoApp.UI
{
    public static class Components
    {
        public static ReactElement TodoItem(Todo todo, Action<int> onToggleComplete, Action<int> onDelete)
        {
            return DOM.Div(
                      new Attributes { Style = Styles.TodoItemStyle },
                      DOM.Div(
                          new Attributes { Style = todo.IsCompleted ? Styles.TextStyleComplete : Styles.TextStyleNotComplete },
                          todo.Description
                      ),
                      DOM.Button(new ButtonAttributes
                      {
                          Style = new ReactStyle { Margin = "5px" },
                          OnClick = e => onToggleComplete(todo.Id),
                          ClassName = "btn btn-primary"
                      }, "Toggle complete"),
                      DOM.Button(new ButtonAttributes
                      {
                          Style = new ReactStyle { Margin = "5px" },
                          OnClick = e => onDelete(todo.Id),
                          ClassName = "btn btn-danger"
                      }, "Delete")
                   );
        }

        public static ReactElement TodoItemList(Store<TodoAppState> appStore)
        {
            return ReactRedux.Component(new ContainerProps<TodoAppState, TodoAppState>
            {
                Store = appStore,
                StateToPropsMapper = state => state,
                Renderer = appState =>
                {
                    Func<Todo, bool> todoIsCompleted = todo => todo.IsCompleted;
                    Func<Todo, bool> todoNotCompleted = todo => todo.IsCompleted == false;

                    IEnumerable<Todo> todos;

                    if (appState.Visibility == TodoVisibility.Completed)
                        todos = appState.Todos.Where(todoIsCompleted);
                    else if (appState.Visibility == TodoVisibility.YetToComplete)
                        todos = appState.Todos.Where(todoNotCompleted);
                    else
                        todos = appState.Todos;

                    var todoItems = todos.Select(todo =>
                    {
                        return TodoItem
                               (
                                    todo: todo,
                                    onToggleComplete: id => appStore.Dispatch(new ToggleTodoCompleted { Id = id }),
                                    onDelete: id => appStore.Dispatch(new DeleteTodo { Id = id })
                               );
                    });

                    return DOM.Div(new Attributes { Style = new ReactStyle { Padding = 10 }  },
                                DOM.H1("React + Redux todo app in C#"),
                                DOM.UL(new Attributes { }, todoItems.Select(DOM.Li)), 
                                DOM.Hr(new HRAttributes { }),
                                DOM.H3("Add a new Todo item"),
                                DOM.Input(new InputAttributes
                                {
                                    Style = new ReactStyle { Margin = "5px", MaxWidth = 400 },
                                    ClassName = "form-control",
                                    Placeholder = "Todo descriptions",
                                    OnInput = e => appStore.Dispatch(new UpdateDescriptionInput { Description = e.CurrentTarget.Value }),
                                    Value = appState.DescriptionInput
                                }),
                                DOM.Button(new ButtonAttributes
                                {
                                    Style = new ReactStyle { Margin = "5px" },
                                    ClassName = "btn btn-success",
                                    OnClick = e =>
                                    {
                                        if (!string.IsNullOrWhiteSpace(appState.DescriptionInput))
                                        {
                                            appStore.Dispatch(new AddTodo());
                                            appStore.Dispatch(new UpdateDescriptionInput { Description = "" });
                                        }
                                    }
                                }, "Add"),
                                DOM.Hr(new HRAttributes { }),
                                DOM.H3("Visibility"),
                                DOM.Div(new Attributes { }, 
                                    DOM.Span("Show"),
                                    DOM.Button(new ButtonAttributes
                                    {
                                        Style = new ReactStyle { Margin = 5 },
                                        ClassName = appState.Visibility == TodoVisibility.All ? "btn btn-success" : "btn btn-default",
                                        OnClick = e => appStore.Dispatch(new SetVisibility { Visibility = TodoVisibility.All })
                                    }, "All"),
                                    DOM.Button(new ButtonAttributes
                                    {
                                        Style = new ReactStyle { Margin = 5 },
                                        ClassName = appState.Visibility == TodoVisibility.Completed ? "btn btn-success" : "btn btn-default",
                                        OnClick = e => appStore.Dispatch(new SetVisibility { Visibility = TodoVisibility.Completed })
                                    }, "Completed only"),
                                    DOM.Button(new ButtonAttributes
                                    {
                                        Style = new ReactStyle { Margin = 5 },
                                        ClassName = appState.Visibility == TodoVisibility.YetToComplete ? "btn btn-success" : "btn btn-default",
                                        OnClick = e => appStore.Dispatch(new SetVisibility { Visibility = TodoVisibility.YetToComplete })
                                    }, "Not finished")
                                )           
                           );
                }
            });
        }
    }
}