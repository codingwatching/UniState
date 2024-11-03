using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.HistoryTests.Infrastructure
{
    internal class StateFooHistory : StateBase
    {
        private readonly HistorySizeTestHelper _testHelper;
        private readonly ExecutionLogger _logger;

        public StateFooHistory(HistorySizeTestHelper testHelper, ExecutionLogger logger)
        {
            _testHelper = testHelper;
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _testHelper.ReportTransition();

            _logger.LogStep("StateFooHistory", $"{_testHelper.ShouldGoBack}");

            return _testHelper.ShouldGoBack ? Transition.GoBack() : Transition.GoTo<StateBarHistory>();
        }
    }
}