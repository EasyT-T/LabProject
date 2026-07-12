namespace LabProject.Extensions;

using System.Threading;
using System.Threading.Tasks;

public static class ValueTaskExtension
{
    extension(ValueTask valueTask)
    {
        public static ValueTask FromCanceled(CancellationToken cancellationToken)
        {
            return new ValueTask(Task.FromCanceled(cancellationToken));
        }

        public static ValueTask<TResult> FromResult<TResult>(TResult result)
        {
            return new ValueTask<TResult>(result);
        }

        public static ValueTask<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
        {
            return new ValueTask<TResult>(Task.FromCanceled<TResult>(cancellationToken));
        }
    }
}