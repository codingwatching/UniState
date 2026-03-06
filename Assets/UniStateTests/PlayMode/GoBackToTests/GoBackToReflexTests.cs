using NUnit.Framework;
using Reflex.Core;
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

            builder.AddStateMachine(typeof(GoBackToStateMachine), typeof(IVerifiableStateMachine));
            builder.AddState(typeof(GoBackToState1));
            builder.AddState(typeof(GoBackToState2));
            builder.AddState(typeof(GoBackToState3));
            builder.AddState(typeof(GoBackToState4));
            builder.AddSingleton(typeof(GoBackToTestsHelper));
        }
    }
}