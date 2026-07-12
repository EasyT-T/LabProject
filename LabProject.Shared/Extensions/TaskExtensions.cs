namespace LabProject.Extensions;

using System.Threading.Tasks;

public static class TaskExtensions
{
    extension(Task task)
    {
        public ValueTask ToValueTask()
        {
            return new ValueTask(task);
        }
    }

    public static ValueTask<T> ToValueTask<T>(this Task<T> task)
    {
        return new ValueTask<T>(task);
    }
}