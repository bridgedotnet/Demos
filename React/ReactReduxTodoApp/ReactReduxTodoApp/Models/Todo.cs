using Bridge;

namespace ReactReduxTodoApp.Models
{
    [ObjectLiteral]
    public class Todo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}