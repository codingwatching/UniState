using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.RecoveryTransitionTests
{
    [TestFixture]
    public class RecoveryReflexTests : ReflexTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithDefaultRecovery_ExceptionDuringExecute_StateMachineExecuteGoBack() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineDefaultRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithGoToStateRecovery_ExceptionDuringExecute_StateMachineGoToRecoveryState() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineGoToStateRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithExitRecovery_ExceptionDuringExecute_StateMachineExit() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineExitRecovery, StateInitial>(); });

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.AddSingleton(typeof(RecoveryTestHelper));
            builder.AddStateMachine(typeof(StateMachineDefaultRecovery), typeof(IStateMachineDefaultRecovery));
            builder.AddStateMachine(typeof(StateMachineGoToStateRecovery), typeof(IStateMachineGoToStateRecovery));
            builder.AddStateMachine(typeof(StateMachineExitRecovery), typeof(IStateMachineExitRecovery));
            builder.AddState(typeof(StateInitial));
            builder.AddState(typeof(StateThrowTwoException));
            builder.AddState(typeof(StateWithFailExecution));
            builder.AddState(typeof(StateStartedAfterException));
        }
    }
}
