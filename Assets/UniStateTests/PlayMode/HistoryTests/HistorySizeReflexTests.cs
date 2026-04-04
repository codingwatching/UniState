using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.HistoryTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.HistoryTests
{
    [TestFixture]
    public class HistorySizeReflexTests : ReflexTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithLongHistory_GoBack_ChainExecutedCorrectly() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineLongHistory, StateInitLongHistory>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithZeroHistory_GoBack_ExitFromStateMachine() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineZeroHistory, StateInitZeroHistory>(); });

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterType(typeof(HistorySizeTestHelper), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterStateMachine(typeof(StateMachineLongHistory), typeof(IStateMachineLongHistory));
            builder.RegisterStateMachine(typeof(StateMachineZeroHistory), typeof(IStateMachineZeroHistory));
            builder.RegisterState(typeof(StateInitLongHistory));
            builder.RegisterState(typeof(StateInitZeroHistory));
            builder.RegisterState(typeof(StateFooHistory));
            builder.RegisterState(typeof(StateBarHistory));
        }
    }
}
