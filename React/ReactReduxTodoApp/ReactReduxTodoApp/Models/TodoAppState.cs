using Bridge;
using System.Collections.Generic;

namespace ReactReduxTodoApp.Models
{
    [ObjectLiteral]
    public class TodoAppState
    {
        public IEnumerable<Todo> Todos { get; set; }
        public string DescriptionInput { get; set; }
        public TodoVisibility Visibility { get; set; }
    }
}