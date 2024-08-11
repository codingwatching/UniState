using System.Threading;

namespace UniState.Runtime.Core.Command
{
    public class CommandFactory
    {
        IExecutableCommand<TResult> Create<TCommand, TResult>(CancellationToken token) where TCommand : class, ICommand<TResult>
        {

        }
    }
}