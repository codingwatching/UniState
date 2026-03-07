using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoBackTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.GoBackTests
{
    [TestFixture]
    internal class GoBackReflexTests : ReflexTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_GoBackFromTheChain_ExitFromStateMachineWithCorrectOrderOfStates() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IVerifiableStateMachine, StateGoBackFirst>();
            });

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.AddSingleton(typeof(GoBackTestHelper));
            builder.AddStateMachine(typeof(StateMachineGoBack), typeof(IVerifiableStateMachine));
            builder.AddState(typeof(StateGoBackFirst));
            builder.AddState(typeof(StateGoBackSecond));
            builder.AddState(typeof(StateGoBackThird));
        }
    }
}
