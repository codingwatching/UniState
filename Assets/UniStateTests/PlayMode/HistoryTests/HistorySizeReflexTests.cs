using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
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

            builder.AddSingleton(typeof(HistorySizeTestHelper));
            builder.AddStateMachine(typeof(StateMachineLongHistory), typeof(IStateMachineLongHistory));
            builder.AddStateMachine(typeof(StateMachineZeroHistory), typeof(IStateMachineZeroHistory));
            builder.AddState(typeof(StateInitLongHistory));
            builder.AddState(typeof(StateInitZeroHistory));
            builder.AddState(typeof(StateFooHistory));
            builder.AddState(typeof(StateBarHistory));
        }
    }
}
