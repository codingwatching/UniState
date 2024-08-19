using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState.Runtime.Core.Command
{
    public class CommandsExecutor
    {
        private readonly ITypeResolver _resolver;
        private readonly CancellationToken _stateToken;

        private List<IFinishable> _activeFlows;

        private List<IFinishable> ActiveFlows => _activeFlows ??= new(4);

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

        UniTask<TResult> RunFlow<TFlow, TResult, TPayload>(CancellationToken token, TPayload payload)
            where TFlow : class, IFlow<TPayload, TResult>
        {
            var flow = _resolver.Resolve<TFlow>();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_stateToken, token);

            try
            {
                flow.SetPayload(payload);
                var startResult = flow.Start(cts.Token);
                ActiveFlows.Add(flow);

                return startResult;
            }
            catch
            {
                cts.Dispose();
                flow.Dispose();

                throw;
            }
        }

        public UniTask FinishActiveFlows(CancellationToken token)
        {
            if (_activeFlows?.Count > 0)
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(_stateToken, token);

                try
                {
                    return UniTask.WhenAll(ActiveFlows.Select(s => s.Finish(cts.Token)).ToArray());
                }
                finally
                {
                    cts.Dispose();

                    foreach (var activeFlow in _activeFlows)
                    {
                        activeFlow.Dispose();
                    }
                }
            }

            return UniTask.CompletedTask;
        }
    }
}