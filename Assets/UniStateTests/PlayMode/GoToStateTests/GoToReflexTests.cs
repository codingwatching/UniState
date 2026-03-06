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

            builder.AddStateMachine(typeof(StateMachineGoToState), typeof(IVerifiableStateMachine));
            builder.AddState(typeof(StateGoTo1));
            builder.AddState(typeof(StateGoTo2));
            builder.AddState(typeof(StateGoTo3));
            builder.AddState(typeof(StateGoTo3), typeof(StateGoToAbstract3));
            builder.AddState(typeof(StateGoTo4));
            builder.AddState(typeof(StateGoTo4), typeof(IStateGoTo4));
            builder.AddState(typeof(StateGoTo5));
            builder.AddState(typeof(CompositeStateGoTo6));
            builder.AddState(typeof(SubStateGoTo6First));
            builder.AddState(typeof(SubStateGoTo6Second));
            builder.AddState(typeof(CompositeStateGoTo7));
            builder.AddState(typeof(SubStateGoTo7First));
            builder.AddState(typeof(SubStateGoTo7Second));
            builder.AddState(typeof(StateGoTo8));
            builder.AddState(typeof(StateGoTo8), typeof(IStateGoTo8));
        }
    }
}
