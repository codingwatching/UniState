using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
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

            builder.AddSingleton(typeof(ExecutionTestHelper));
            builder.AddStateMachine(typeof(ExecutionStateMachine), typeof(IVerifiableStateMachine));
            builder.AddState(typeof(FirstState));
            builder.AddState(typeof(SecondState));
            builder.AddState(typeof(SecondStateWithException));
            builder.AddState(typeof(SecondStateWithWrongDependency));
        }
    }
}
