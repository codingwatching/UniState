using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public class SubStatesContainer<TPayload> : ISubStatesContainer<TPayload>, ISetupable<TPayload>
    {
        private List<IState<TPayload>> _subStates = new();

        public List<IState<TPayload>> List => _subStates;

        public void Initialize(List<IState<TPayload>> subStates)
        {
            _subStates = subStates;
        }

        public void SetPayload(TPayload payload)
        {
            for (var i = 0; i < _subStates.Count; i++)
            {
                _subStates[i].SetPayload(payload);
            }
        }

        public void SetTransitionFacade(IStateTransitionFacade transitionFacade)
        {
            for (var i = 0; i < _subStates.Count; i++)
            {
                _subStates[i].SetTransitionFacade(transitionFacade);
            }
        }

        public UniTask Initialize(CancellationToken token)
        {
            var tasks = new UniTask[_subStates.Count];
            for (var i = 0; i < _subStates.Count; i++)
            {
                tasks[i] = _subStates[i].Initialize(token);
            }

            return UniTask.WhenAll(tasks);
        }

        public async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            if (_subStates.Count == 0)
            {
                throw new NoSubStatesException();
            }

            StateTransitionInfo result;

            var ctx = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                var tasks = new UniTask<StateTransitionInfo>[_subStates.Count];
                for (var i = 0; i < _subStates.Count; i++)
                {
                    tasks[i] = _subStates[i].Execute(ctx.Token);
                }

                var first = await UniTask.WhenAny(tasks);
                result = first.result;
            }
            finally
            {
                ctx.Cancel();
                ctx.Dispose();
            }

            return result;
        }

        public UniTask Exit(CancellationToken token)
        {
            var tasks = new UniTask[_subStates.Count];
            for (var i = 0; i < _subStates.Count; i++)
            {
                tasks[i] = _subStates[i].Exit(token);
            }

            return UniTask.WhenAll(tasks);
        }

        public void Dispose()
        {
            List<Exception> exceptions = null;

            for (var i = 0; i < _subStates.Count; i++)
            {
                try
                {
                    _subStates[i].Dispose();
                }
                catch (Exception e)
                {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(e);
                }
            }

            if (exceptions == null)
            {
                return;
            }

            if (exceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            }

            throw new AggregateException("One or more substate dispose operations failed.", exceptions);
        }
    }
}
