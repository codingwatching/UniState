using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.SubStateTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.SubStateTests
{
    [TestFixture]
    public class SubStateVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateSubStates_ExeptionRisedInSubState_AllSubStateDisposed() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IVerifiableStateMachine, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunCompositeState_SubStateDisposeThrows_HandlesStateDisposingError() =>
            UniTask.ToCoroutine(async () =>
            {
                var recorder = Container.Resolve<DisposeFailureRecorder>();
                var stateMachine = Container.Resolve<IStateMachine>();

                await stateMachine.Execute<SingleDisposeFailureCompositeState>(GetTimeoutToken());

                CollectionAssert.AreEqual(
                    new[]
                    {
                        nameof(SingleThrowingDisposeSubState),
                        nameof(SingleSuccessfulDisposeSubState)
                    },
                    recorder.DisposeOrder);

                Assert.AreEqual(1, recorder.Errors.Count);
                Assert.AreEqual(StateMachineErrorType.StateDisposing, recorder.Errors[0].ErrorType);
                Assert.IsInstanceOf<SingleDisposeFailureCompositeState>(recorder.Errors[0].State);
                Assert.IsInstanceOf<InvalidOperationException>(recorder.Errors[0].Exception);
            });

        [UnityTest]
        public IEnumerator RunCompositeState_MultipleSubStateDisposeThrows_HandlesAggregateStateDisposingError() =>
            UniTask.ToCoroutine(async () =>
            {
                var recorder = Container.Resolve<DisposeFailureRecorder>();
                var stateMachine = Container.Resolve<IStateMachine>();

                await stateMachine.Execute<MultipleDisposeFailureCompositeState>(GetTimeoutToken());

                CollectionAssert.AreEqual(
                    new[]
                    {
                        nameof(MultipleFirstThrowingDisposeSubState),
                        nameof(MultipleSecondThrowingDisposeSubState),
                        nameof(MultipleSuccessfulDisposeSubState)
                    },
                    recorder.DisposeOrder);

                Assert.AreEqual(1, recorder.Errors.Count);
                Assert.AreEqual(StateMachineErrorType.StateDisposing, recorder.Errors[0].ErrorType);
                Assert.IsInstanceOf<MultipleDisposeFailureCompositeState>(recorder.Errors[0].State);

                var exception = recorder.Errors[0].Exception as AggregateException;

                Assert.IsNotNull(exception);
                Assert.AreEqual(2, exception.InnerExceptions.Count);
            });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<DisposeFailureRecorder>(Lifetime.Singleton);
            builder.RegisterStateMachine<IStateMachine, DisposeFailureStateMachine>();
            builder.RegisterStateMachine<IVerifiableStateMachine, StateMachineSubStates>();

            builder.RegisterState<StateInitial>();
            builder.RegisterState<StateFinal>();
            builder.RegisterState<SubStateInitialFirst>();
            builder.RegisterState<SubStateInitialSecond>();
            builder.RegisterState<SubStateFinalFirst>();
            builder.RegisterState<SubStateFinalSecond>();

            builder.RegisterState<SingleDisposeFailureCompositeState>();
            builder.RegisterState<SingleThrowingDisposeSubState>();
            builder.RegisterState<SingleSuccessfulDisposeSubState>();
            builder.RegisterState<MultipleDisposeFailureCompositeState>();
            builder.RegisterState<MultipleFirstThrowingDisposeSubState>();
            builder.RegisterState<MultipleSecondThrowingDisposeSubState>();
            builder.RegisterState<MultipleSuccessfulDisposeSubState>();
        }

        public sealed class DisposeFailureRecorder
        {
            public List<string> DisposeOrder { get; } = new();
            public List<StateMachineErrorData> Errors { get; } = new();
        }

        public sealed class DisposeFailureStateMachine : StateMachine
        {
            private readonly DisposeFailureRecorder _recorder;

            public DisposeFailureStateMachine(DisposeFailureRecorder recorder)
            {
                _recorder = recorder;
            }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                _recorder.Errors.Add(errorData);
            }
        }

        public sealed class SingleDisposeFailureCompositeState : DefaultCompositeState
        {
        }

        public sealed class MultipleDisposeFailureCompositeState : DefaultCompositeState
        {
        }

        public sealed class SingleThrowingDisposeSubState :
            DisposeFailureSubState<SingleDisposeFailureCompositeState>
        {
            public SingleThrowingDisposeSubState(DisposeFailureRecorder recorder) : base(recorder)
            {
            }

            protected override void DisposeCore() => throw new InvalidOperationException("Single dispose exception");
        }

        public sealed class SingleSuccessfulDisposeSubState :
            DisposeFailureSubState<SingleDisposeFailureCompositeState>
        {
            public SingleSuccessfulDisposeSubState(DisposeFailureRecorder recorder) : base(recorder)
            {
            }
        }

        public sealed class MultipleFirstThrowingDisposeSubState :
            DisposeFailureSubState<MultipleDisposeFailureCompositeState>
        {
            public MultipleFirstThrowingDisposeSubState(DisposeFailureRecorder recorder) : base(recorder)
            {
            }

            protected override void DisposeCore() => throw new InvalidOperationException("First dispose exception");
        }

        public sealed class MultipleSecondThrowingDisposeSubState :
            DisposeFailureSubState<MultipleDisposeFailureCompositeState>
        {
            public MultipleSecondThrowingDisposeSubState(DisposeFailureRecorder recorder) : base(recorder)
            {
            }

            protected override void DisposeCore() => throw new InvalidOperationException("Second dispose exception");
        }

        public sealed class MultipleSuccessfulDisposeSubState :
            DisposeFailureSubState<MultipleDisposeFailureCompositeState>
        {
            public MultipleSuccessfulDisposeSubState(DisposeFailureRecorder recorder) : base(recorder)
            {
            }
        }

        public abstract class DisposeFailureSubState<TState> : SubStateBase<TState>
            where TState : IState<EmptyPayload>
        {
            private readonly DisposeFailureRecorder _recorder;

            protected DisposeFailureSubState(DisposeFailureRecorder recorder)
            {
                _recorder = recorder;
            }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());

            public override void Dispose()
            {
                base.Dispose();
                _recorder.DisposeOrder.Add(GetType().Name);
                DisposeCore();
            }

            protected virtual void DisposeCore()
            {
            }
        }
    }
}
