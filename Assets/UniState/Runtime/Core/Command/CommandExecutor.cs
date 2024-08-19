using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState.Runtime.Core.Command
{
    public class CommandsExecutor
    {
        private readonly ITypeResolver _resolver;
        private readonly CancellationToken _stateToken;

        //TODO: Implement run for actions
        //TODO: Rename method
        //TODO: Add catch cencelation in state machine
        //TODO: Add Exiting / Catch exiting
        UniTask<TResult> RunCommand<TCommand, TResult, TPayload>(CancellationToken token, TPayload payload)
            where TCommand : class, ICommand<TPayload, TResult>
        {
            var command = _resolver.Resolve<TCommand>();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_stateToken, token);

            try
            {
                command.SetPayload(payload);
                return command.Execute(cts.Token);
            }
            finally
            {
                cts.Dispose();
                command.Dispose();
            }
        }
    }
}