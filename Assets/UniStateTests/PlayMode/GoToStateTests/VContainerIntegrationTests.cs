using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoToStateTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.GoToStateTests
{
    [TestFixture]
    internal class VContainerIntegrationTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_ExitFromChain_ChainExecutedCorrectly() => UniTask.ToCoroutine(async () =>
        {
            await RunAndVerify<StateMachineGoToState, StateGoTo1>();
        });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine<StateMachineGoToState>();
            builder.RegisterState<StateGoTo1>();
            builder.RegisterState<StateGoTo2>();
            builder.RegisterState<StateGoTo3>();
            builder.RegisterAbstractState<StateGoToAbstract3, StateGoTo3>();
            builder.RegisterState<StateGoTo4>();
            builder.RegisterState<StateGoTo5>();
            builder.RegisterState<CompositeStateGoTo6>();
            builder.RegisterState<SubStateGoToX6A>();
            builder.RegisterState<SubStateGoToX6B>();
        }
    }
}