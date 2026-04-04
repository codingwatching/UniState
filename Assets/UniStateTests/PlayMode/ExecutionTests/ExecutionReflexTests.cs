using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.Execution.Infrastructure;
using UnityEngine.TestTools;
using FirstState = UniStateTests.PlayMode.Execution.Infrastructure.FirstState;

namespace UniStateTests.PlayMode.Execution
{
    [TestFixture]
    public class ExecutionReflexTests : ReflexTestsBase
    {
        [UnityTest]
        public IEnumerator RunStateMachineSeveralTime_EndExecution_ExecutionStatusValid() => UniTask.ToCoroutine(
            async () =>
            {
                var testHelper = Container.Resolve<ExecutionTestHelper>();
                testHelper.SetPath(StateMachineExecutionType.Default);

                await RunAndVerify<IVerifiableStateMachine, FirstState>();
                Assert.False(testHelper.CurrentStateMachine.IsExecuting);

                testHelper.SetPath(StateMachineExecutionType.WrongDependency);

                await RunAndVerify<IVerifiableStateMachine, FirstState>();
                Assert.False(testHelper.CurrentStateMachine.IsExecuting);

                testHelper.SetPath(StateMachineExecutionType.Exception);

                await RunAndVerify<IVerifiableStateMachine, FirstState>();
                Assert.False(testHelper.CurrentStateMachine.IsExecuting);
            });

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterType(typeof(ExecutionTestHelper), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterStateMachine(typeof(ExecutionStateMachine), typeof(IVerifiableStateMachine));
            builder.RegisterState(typeof(FirstState));
            builder.RegisterState(typeof(SecondState));
            builder.RegisterState(typeof(SecondStateWithException));
            builder.RegisterState(typeof(SecondStateWithWrongDependency));
        }
    }
}
