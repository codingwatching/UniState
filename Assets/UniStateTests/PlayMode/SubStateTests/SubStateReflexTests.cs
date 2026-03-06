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

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.AddStateMachine(typeof(StateMachineSubStates), typeof(IVerifiableStateMachine));
            builder.AddState(typeof(StateInitial));
            builder.AddState(typeof(StateFinal));
            builder.AddState(typeof(SubStateInitialFirst));
            builder.AddState(typeof(SubStateInitialSecond));
            builder.AddState(typeof(SubStateFinalFirst));
            builder.AddState(typeof(SubStateFinalSecond));
        }
    }
}
