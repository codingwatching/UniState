using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.RecoveryTransitionTests
{
    [TestFixture]
    public class RecoveryVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithDefaultRecovery_ExceptionDuringExecute_StateMachineExecuteGoBack() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineDefaultRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithGoToStateRecovery_ExceptionDuringExecute_StateMachineGoToRecoveryState() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineGoToStateRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithExitRecovery_ExceptionDuringExecute_StateMachineExit() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineExitRecovery, StateInitial>(); });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<RecoveryTestHelper>(Lifetime.Singleton);

            builder.RegisterStateMachine<StateMachineDefaultRecovery>();
            builder.RegisterStateMachine<StateMachineGoToStateRecovery>();
            builder.RegisterStateMachine<StateMachineExitRecovery>();

            builder.RegisterState<StateInitial>();
            builder.RegisterState<StateThrowTwoException>();
            builder.RegisterState<StateWithFailExecution>();
            builder.RegisterState<StateStartedAfterException>();
        }
    }
}