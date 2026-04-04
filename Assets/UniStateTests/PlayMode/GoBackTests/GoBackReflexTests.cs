using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
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

            builder.RegisterType(typeof(GoBackTestHelper), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterStateMachine(typeof(StateMachineGoBack), typeof(IVerifiableStateMachine));
            builder.RegisterState(typeof(StateGoBackFirst));
            builder.RegisterState(typeof(StateGoBackSecond));
            builder.RegisterState(typeof(StateGoBackThird));
        }
    }
}
