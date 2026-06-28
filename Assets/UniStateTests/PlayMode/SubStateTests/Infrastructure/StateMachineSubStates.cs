using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    public class StateMachineSubStates : VerifiableStateMachine
    {
        public StateMachineSubStates(ExecutionLogger logger) : base(logger)
        {
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateExitTransition();

        protected override void HandleError(StateMachineErrorData errorData)
        {
        }

        protected override string ExpectedLog =>
            "SubStateInitialSecond (Execute) -> SubStateInitialFirst (Execute, Disposables) -> SubStateInitialSecond (Disposables) -> " +
            "SubStateFinalSecond (Execute) -> SubStateFinalFirst (Execute, Disposables) -> SubStateFinalSecond (Disposables)";
    }

    internal class StateMachineSingleSubStateDisposeFailure : VerifiableStateMachine,
        IStateMachineSingleSubStateDisposeFailure
    {
        private readonly ExecutionLogger _logger;

        public StateMachineSingleSubStateDisposeFailure(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            _logger.LogStep(
                nameof(StateMachineSingleSubStateDisposeFailure),
                $"HandleError ({errorData.ErrorType}, {errorData.Exception.GetType().Name}, {errorData.State.GetType().Name})");
        }

        protected override string ExpectedLog =>
            "SingleThrowingDisposeSubState (Dispose) -> SingleSuccessfulDisposeSubState (Dispose) -> " +
            "StateMachineSingleSubStateDisposeFailure (HandleError (StateDisposing, InvalidOperationException, SingleDisposeFailureCompositeState))";
    }

    internal class StateMachineMultipleSubStateDisposeFailure : VerifiableStateMachine,
        IStateMachineMultipleSubStateDisposeFailure
    {
        private readonly ExecutionLogger _logger;

        public StateMachineMultipleSubStateDisposeFailure(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            var exception = (AggregateException)errorData.Exception;

            _logger.LogStep(
                nameof(StateMachineMultipleSubStateDisposeFailure),
                $"HandleError ({errorData.ErrorType}, {errorData.Exception.GetType().Name}, {exception.InnerExceptions.Count}, {errorData.State.GetType().Name})");
        }

        protected override string ExpectedLog =>
            "MultipleFirstThrowingDisposeSubState (Dispose) -> MultipleSecondThrowingDisposeSubState (Dispose) -> " +
            "MultipleSuccessfulDisposeSubState (Dispose) -> " +
            "StateMachineMultipleSubStateDisposeFailure (HandleError (StateDisposing, AggregateException, 2, MultipleDisposeFailureCompositeState))";
    }

    internal class SingleDisposeFailureCompositeState : DefaultCompositeState
    {
    }

    internal class MultipleDisposeFailureCompositeState : DefaultCompositeState
    {
    }

    internal class SingleThrowingDisposeSubState : DisposeFailureSubState<SingleDisposeFailureCompositeState>
    {
        public SingleThrowingDisposeSubState(ExecutionLogger logger) : base(logger)
        {
        }

        protected override void DisposeCore() => throw new InvalidOperationException("Single dispose exception");
    }

    internal class SingleSuccessfulDisposeSubState : DisposeFailureSubState<SingleDisposeFailureCompositeState>
    {
        public SingleSuccessfulDisposeSubState(ExecutionLogger logger) : base(logger)
        {
        }
    }

    internal class MultipleFirstThrowingDisposeSubState :
        DisposeFailureSubState<MultipleDisposeFailureCompositeState>
    {
        public MultipleFirstThrowingDisposeSubState(ExecutionLogger logger) : base(logger)
        {
        }

        protected override void DisposeCore() => throw new InvalidOperationException("First dispose exception");
    }

    internal class MultipleSecondThrowingDisposeSubState :
        DisposeFailureSubState<MultipleDisposeFailureCompositeState>
    {
        public MultipleSecondThrowingDisposeSubState(ExecutionLogger logger) : base(logger)
        {
        }

        protected override void DisposeCore() => throw new InvalidOperationException("Second dispose exception");
    }

    internal class MultipleSuccessfulDisposeSubState : DisposeFailureSubState<MultipleDisposeFailureCompositeState>
    {
        public MultipleSuccessfulDisposeSubState(ExecutionLogger logger) : base(logger)
        {
        }
    }

    internal abstract class DisposeFailureSubState<TState> : SubStateBase<TState>
        where TState : IState<EmptyPayload>
    {
        private readonly ExecutionLogger _logger;
        private bool _disposed;

        protected DisposeFailureSubState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
            UniTask.FromResult(Transition.GoToExit());

        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            base.Dispose();
            _logger.LogStep(GetType().Name, "Dispose");
            DisposeCore();
        }

        protected virtual void DisposeCore()
        {
        }
    }

    public interface IStateMachineSingleSubStateDisposeFailure : IVerifiableStateMachine
    {
    }

    public interface IStateMachineMultipleSubStateDisposeFailure : IVerifiableStateMachine
    {
    }
}
