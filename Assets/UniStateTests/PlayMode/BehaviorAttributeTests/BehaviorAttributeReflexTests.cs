using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.StateBehaviorAttributeTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.StateBehaviorAttributeTests
{
    [TestFixture]
    public class BehaviorAttributeReflexTests : ReflexTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithAttributes_ExitFromChain_ChainExecutedCorrectly() => UniTask.ToCoroutine(async () =>
        {
            await RunAndVerify<IVerifiableStateMachine, FirstState>();
        });

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine(typeof(StateMachineBehaviourAttribute), typeof(IVerifiableStateMachine));
            builder.RegisterState(typeof(FirstState));
            builder.RegisterState(typeof(NoReturnState));
            builder.RegisterState(typeof(FastInitializeState));
            builder.RegisterType(typeof(BehaviourAttributeTestHelper), Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
