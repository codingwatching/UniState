using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.StateMachineLifecycle
{
    [TestFixture]
    public class StateMachineStateChangedTests
    {
        [Test]
        public void Execute_WithGoToAndExit_ReportsStateChanges()
        {
            var stateMachine = new TrackingStateMachine();
            stateMachine.SetResolver(new TestResolver());

            stateMachine.Execute<FirstState>(CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreEqual(3, stateMachine.Changes.Count);

            Assert.AreEqual(StateMachineStateChangeType.Started, stateMachine.Changes[0].ChangeType);
            Assert.IsNull(stateMachine.Changes[0].PreviousState);
            Assert.IsInstanceOf<FirstState>(stateMachine.Changes[0].CurrentState);
            Assert.AreEqual(TransitionType.State, stateMachine.Changes[0].RequestedTransition.Transition);

            Assert.AreEqual(StateMachineStateChangeType.Changed, stateMachine.Changes[1].ChangeType);
            Assert.IsInstanceOf<FirstState>(stateMachine.Changes[1].PreviousState);
            Assert.IsInstanceOf<SecondState>(stateMachine.Changes[1].CurrentState);
            Assert.AreEqual(typeof(SecondState), stateMachine.Changes[1].CurrentTransition.Creator.StateType);
            Assert.AreEqual(TransitionType.State, stateMachine.Changes[1].RequestedTransition.Transition);

            Assert.AreEqual(StateMachineStateChangeType.Exited, stateMachine.Changes[2].ChangeType);
            Assert.IsInstanceOf<SecondState>(stateMachine.Changes[2].PreviousState);
            Assert.IsNull(stateMachine.Changes[2].CurrentState);
            Assert.IsNull(stateMachine.Changes[2].CurrentTransition);
            Assert.AreEqual(TransitionType.Exit, stateMachine.Changes[2].RequestedTransition.Transition);
        }

        [Test]
        public void Execute_WithGoBack_ReportsStateChangeFromHistory()
        {
            var stateMachine = new TrackingStateMachine();
            stateMachine.SetResolver(new TestResolver());

            stateMachine.Execute<BackFirstState>(CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreEqual(4, stateMachine.Changes.Count);

            Assert.AreEqual(StateMachineStateChangeType.Started, stateMachine.Changes[0].ChangeType);
            Assert.IsInstanceOf<BackFirstState>(stateMachine.Changes[0].CurrentState);

            Assert.AreEqual(StateMachineStateChangeType.Changed, stateMachine.Changes[1].ChangeType);
            Assert.IsInstanceOf<BackFirstState>(stateMachine.Changes[1].PreviousState);
            Assert.IsInstanceOf<BackSecondState>(stateMachine.Changes[1].CurrentState);

            Assert.AreEqual(StateMachineStateChangeType.Changed, stateMachine.Changes[2].ChangeType);
            Assert.IsInstanceOf<BackSecondState>(stateMachine.Changes[2].PreviousState);
            Assert.IsInstanceOf<BackFirstState>(stateMachine.Changes[2].CurrentState);
            Assert.AreEqual(TransitionType.Back, stateMachine.Changes[2].RequestedTransition.Transition);
            Assert.AreEqual(typeof(BackFirstState), stateMachine.Changes[2].CurrentTransition.Creator.StateType);

            Assert.AreEqual(StateMachineStateChangeType.Exited, stateMachine.Changes[3].ChangeType);
            Assert.IsInstanceOf<BackFirstState>(stateMachine.Changes[3].PreviousState);
            Assert.IsNull(stateMachine.Changes[3].CurrentState);
        }

        [Test]
        public void Execute_WhenStateChangedHandlerThrows_ClearsExecutionStatus()
        {
            var stateMachine = new ThrowingStateChangedStateMachine();
            stateMachine.SetResolver(new TestResolver());

            stateMachine.Execute<FirstState>(CancellationToken.None).GetAwaiter().GetResult();

            Assert.False(stateMachine.IsExecuting);
            Assert.NotNull(stateMachine.LastError);
            Assert.AreEqual(StateMachineErrorType.StateMachineFail, stateMachine.LastError.ErrorType);
        }

        private sealed class TrackingStateMachine : StateMachine
        {
            public readonly List<StateMachineStateChangedData> Changes = new();

            protected override void HandleStateChanged(StateMachineStateChangedData changeData)
            {
                Changes.Add(changeData);
            }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                throw new Exception("State machine error.", errorData.Exception);
            }
        }

        private sealed class ThrowingStateChangedStateMachine : StateMachine
        {
            public StateMachineErrorData LastError { get; private set; }

            protected override void HandleStateChanged(StateMachineStateChangedData changeData)
            {
                throw new InvalidOperationException("State changed handler failure.");
            }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                LastError = errorData;
            }
        }

        private sealed class TestResolver : ITypeResolver
        {
            private readonly BackScenario _backScenario = new();

            public object Resolve(Type type)
            {
                if (type == typeof(FirstState))
                {
                    return new FirstState();
                }

                if (type == typeof(SecondState))
                {
                    return new SecondState();
                }

                if (type == typeof(BackFirstState))
                {
                    return new BackFirstState(_backScenario);
                }

                if (type == typeof(BackSecondState))
                {
                    return new BackSecondState();
                }

                throw new InvalidOperationException(type.FullName);
            }
        }

        private sealed class FirstState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoTo<SecondState>());
        }

        private sealed class SecondState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());
        }

        private sealed class BackScenario
        {
            public int FirstStateExecutions { get; set; }
        }

        private sealed class BackFirstState : StateBase
        {
            private readonly BackScenario _scenario;

            public BackFirstState(BackScenario scenario)
            {
                _scenario = scenario;
            }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                _scenario.FirstStateExecutions++;

                return UniTask.FromResult(_scenario.FirstStateExecutions == 1
                    ? Transition.GoTo<BackSecondState>()
                    : Transition.GoToExit());
            }
        }

        private sealed class BackSecondState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoBack());
        }
    }
}
