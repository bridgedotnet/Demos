using Bridge;

namespace ReactReduxTodoApp.Models
{
    [Enum(Emit.StringName)]
    public enum TodoVisibility
    {
        All,
        Completed,
        YetToComplete
    }
}