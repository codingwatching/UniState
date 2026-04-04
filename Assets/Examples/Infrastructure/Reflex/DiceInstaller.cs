using Examples.States;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Enums;
using UniState;
using UnityEngine;

namespace Examples.Infrastructure.Reflex
{
    public class DiceInstaller : MonoBehaviour, IInstaller
    {
        [Inject]
        private readonly DiceEntryPoint _entryPoint;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterStateMachine(typeof(StateMachine), typeof(IStateMachine));

            builder.RegisterState(typeof(LostState));
            builder.RegisterState(typeof(RollDiceState));
            builder.RegisterState(typeof(StartGameState));
            builder.RegisterState(typeof(WinState));

            builder.RegisterFactory<DiceEntryPoint>(
                container =>
                {
                    DiceEntryPoint entryPoint = new(container.Resolve<IStateMachine>());
                    entryPoint.Start();

                    return entryPoint;
                },
                Lifetime.Singleton,
                global::Reflex.Enums.Resolution.Lazy);
        }
    }
}
