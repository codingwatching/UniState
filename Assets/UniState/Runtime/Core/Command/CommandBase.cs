using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState.Runtime.Core.Command
{
    public class CommandBase<TPayload, TResult>: ICommand<TPayload, TResult>
    {
        public UniTask<TResult> Execute(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void SetPayload(TPayload payload)
        {
            throw new System.NotImplementedException();
        }

        public void SetToken(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}