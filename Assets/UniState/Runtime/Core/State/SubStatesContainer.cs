using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
            for (var i = 0; i < _subStates.Count; i++)
            {
                try
                {
                    _subStates[i].Dispose();
                }
                catch (Exception e)
                {
                    // How to log? Do we need log?
                    Debug.LogException(e);
                }
            }
        }
    }
}