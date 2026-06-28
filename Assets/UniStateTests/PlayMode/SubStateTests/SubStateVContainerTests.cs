using System.Collections;
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
                await RunAndVerify<IStateMachineSingleSubStateDisposeFailure, SingleDisposeFailureCompositeState>();
            });

        [UnityTest]
        public IEnumerator RunCompositeState_MultipleSubStateDisposeThrows_HandlesAggregateStateDisposingError() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IStateMachineMultipleSubStateDisposeFailure, MultipleDisposeFailureCompositeState>();
            });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine<IVerifiableStateMachine, StateMachineSubStates>();
            builder.RegisterStateMachine<IStateMachineSingleSubStateDisposeFailure, StateMachineSingleSubStateDisposeFailure>();
            builder.RegisterStateMachine<IStateMachineMultipleSubStateDisposeFailure, StateMachineMultipleSubStateDisposeFailure>();

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
    }
}
