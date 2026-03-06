using Cysharp.Threading.Tasks;
using Reflex.Core;
using UniState;

namespace UniStateTests.Common
{
    public abstract class ReflexTestsBase : TestsBase
    {
        private Container _container;

        protected Container Container => _container;

        public override void Setup()
        {
            base.Setup();

            var builder = new ContainerBuilder();
            SetupBindings(builder);
            _container = builder.Build();
        }

        public override void TearDown()
        {
            base.TearDown();

            _container?.Dispose();
            _container = null;
        }

        protected async UniTask RunAndVerify<TStateMachine, TState>()
            where TStateMachine : class, IStateMachine, IVerifiableStateMachine
            where TState : class, IState<EmptyPayload>
        {
            await StateMachineTestHelper.RunAndVerify<TStateMachine, TState>(Container.ToTypeResolver(),
                GetTimeoutToken());
        }

        protected virtual void SetupBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(ExecutionLogger));
        }
    }
}