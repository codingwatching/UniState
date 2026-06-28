using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.SubStateTests.Infrastructure;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode.SubStateTests
{
    [TestFixture]
    public class SubStateZenjectTests : ZenjectTestsBase
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

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindStateMachine<IVerifiableStateMachine, StateMachineSubStates>();
            container.BindStateMachine<IStateMachineSingleSubStateDisposeFailure, StateMachineSingleSubStateDisposeFailure>();
            container.BindStateMachine<IStateMachineMultipleSubStateDisposeFailure, StateMachineMultipleSubStateDisposeFailure>();

            container.BindState<StateInitial>();
            container.BindState<StateFinal>();
            container.BindState<SubStateInitialFirst>();
            container.BindState<SubStateInitialSecond>();
            container.BindState<SubStateFinalFirst>();
            container.BindState<SubStateFinalSecond>();

            container.BindState<SingleDisposeFailureCompositeState>();
            container.BindState<SingleThrowingDisposeSubState>();
            container.BindState<SingleSuccessfulDisposeSubState>();
            container.BindState<MultipleDisposeFailureCompositeState>();
            container.BindState<MultipleFirstThrowingDisposeSubState>();
            container.BindState<MultipleSecondThrowingDisposeSubState>();
            container.BindState<MultipleSuccessfulDisposeSubState>();
        }
    }
}
