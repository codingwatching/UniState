namespace UniState
{
    public sealed class StateMachineStateChangedData
    {
        public IExecutableState PreviousState { get; }
        public IExecutableState CurrentState { get; }
        public StateTransitionInfo PreviousTransition { get; }
        public StateTransitionInfo CurrentTransition { get; }
        public StateTransitionInfo RequestedTransition { get; }
        public StateMachineStateChangeType ChangeType { get; }

        public StateMachineStateChangedData(
            IExecutableState previousState,
            IExecutableState currentState,
            StateTransitionInfo previousTransition,
            StateTransitionInfo currentTransition,
            StateTransitionInfo requestedTransition,
            StateMachineStateChangeType changeType)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            PreviousTransition = previousTransition;
            CurrentTransition = currentTransition;
            RequestedTransition = requestedTransition;
            ChangeType = changeType;
        }
    }
}
