using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoBackToTests.Infrastructure;

namespace UniStateTests.PlayMode.GoBackToTests
{
    [TestFixture]
    internal class GoBackToReflexTests : ReflexTestsBase
    {
        [Test]
        public void RunChainOfStates_GoBackToChain_LogsExpected()
            => RunAndVerify<IVerifiableStateMachine, GoBackToState1>().GetAwaiter().GetResult();

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine(typeof(GoBackToStateMachine), typeof(IVerifiableStateMachine));
            builder.RegisterState(typeof(GoBackToState1));
            builder.RegisterState(typeof(GoBackToState2));
            builder.RegisterState(typeof(GoBackToState3));
            builder.RegisterState(typeof(GoBackToState4));
            builder.RegisterType(typeof(GoBackToTestsHelper), Lifetime.Singleton, Resolution.Lazy);
        }
    }
}
