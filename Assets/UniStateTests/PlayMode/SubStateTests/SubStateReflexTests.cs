using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.SubStateTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.SubStateTests
{
    [TestFixture]
    public class SubStateReflexTests : ReflexTestsBase
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

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine(typeof(StateMachineSubStates), typeof(IVerifiableStateMachine));
            builder.RegisterStateMachine(
                typeof(StateMachineSingleSubStateDisposeFailure),
                typeof(IStateMachineSingleSubStateDisposeFailure));
            builder.RegisterStateMachine(
                typeof(StateMachineMultipleSubStateDisposeFailure),
                typeof(IStateMachineMultipleSubStateDisposeFailure));

            builder.RegisterState(typeof(StateInitial));
            builder.RegisterState(typeof(StateFinal));
            builder.RegisterState(typeof(SubStateInitialFirst));
            builder.RegisterState(typeof(SubStateInitialSecond));
            builder.RegisterState(typeof(SubStateFinalFirst));
            builder.RegisterState(typeof(SubStateFinalSecond));

            builder.RegisterState(typeof(SingleDisposeFailureCompositeState));
            builder.RegisterState(typeof(SingleThrowingDisposeSubState));
            builder.RegisterState(typeof(SingleSuccessfulDisposeSubState));
            builder.RegisterState(typeof(MultipleDisposeFailureCompositeState));
            builder.RegisterState(typeof(MultipleFirstThrowingDisposeSubState));
            builder.RegisterState(typeof(MultipleSecondThrowingDisposeSubState));
            builder.RegisterState(typeof(MultipleSuccessfulDisposeSubState));
        }
    }
}
