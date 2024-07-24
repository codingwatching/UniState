using System;
using System.Collections.Generic;

namespace UniState
{
    public class StateTransitionFactory : IStateTransitionFactory
    {
        private readonly ITypeResolver _resolver;
        private readonly IStateTransitionFacade _transitionFacade;
        private readonly IStateMachineFactory _stateMachineFactory;

        public StateTransitionFactory(ITypeResolver resolver)
        {
            _resolver = resolver;
            _transitionFacade = new StateTransitionFacade(this);
            _stateMachineFactory = new StateMachineFactory(resolver);
        }

        public StateTransitionInfo CreateStateTransition<TState, TPayload>(TPayload payload)
            where TState : class, IState<TPayload>
        {
            var factory = new StateFactory<TState, TPayload>(_resolver);

            factory.Setup(payload, _transitionFacade, _stateMachineFactory);

            return new StateTransitionInfo()
            {
                Creator = factory,
                Transition = TransitionType.State,
                StateBehaviourData = BuildStateBehaviourData(typeof(TState))
            };
        }

        public StateTransitionInfo CreateStateTransition<TState>()
            where TState : class, IState<EmptyPayload>
        {
            var factory = new StateFactory<TState, EmptyPayload>(_resolver);

            factory.Setup(new EmptyPayload(), _transitionFacade, _stateMachineFactory);

            return new StateTransitionInfo()
            {
                Creator = factory,
                Transition = TransitionType.State,
                StateBehaviourData = BuildStateBehaviourData(typeof(TState))
            };
        }

        public StateTransitionInfo CreateBackTransition() => new() { Transition = TransitionType.Back };

        public StateTransitionInfo CreateExitTransition() => new() { Transition = TransitionType.Exit };

        private StateBehaviourData BuildStateBehaviourData(Type stateType)
        {
            var data = new StateBehaviourData();

            var attribute =
                (StateBehaviourAttribute)Attribute.GetCustomAttribute(stateType, typeof(StateBehaviourAttribute));

            if (attribute != null)
            {
                data.ProhibitReturnToState = attribute.ProhibitReturnToState;
                data.InitializeOnStateTransition = attribute.InitializeOnStateTransition;
            }

            return data;
        }
    }
}