using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoToStateTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.GoToStateTests
{
    [TestFixture]
    internal class GoToReflexTests : ReflexTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_ExitFromChain_ChainExecutedCorrectly() => UniTask.ToCoroutine(async () =>
        {
            await RunAndVerify<IVerifiableStateMachine, StateGoTo1>();
        });

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine(typeof(StateMachineGoToState), typeof(IVerifiableStateMachine));
            builder.RegisterState(typeof(StateGoTo1));
            builder.RegisterState(typeof(StateGoTo2));
            builder.RegisterState(typeof(StateGoTo3));
            builder.RegisterState(typeof(StateGoTo3), typeof(StateGoToAbstract3));
            builder.RegisterState(typeof(StateGoTo4));
            builder.RegisterState(typeof(StateGoTo5));
            builder.RegisterState(typeof(CompositeStateGoTo6));
            builder.RegisterState(typeof(SubStateGoTo6First));
            builder.RegisterState(typeof(SubStateGoTo6Second));
            builder.RegisterState(typeof(CompositeStateGoTo7));
            builder.RegisterState(typeof(SubStateGoTo7First));
            builder.RegisterState(typeof(SubStateGoTo7Second));
            builder.RegisterState(typeof(StateGoTo8));
        }
    }
}
