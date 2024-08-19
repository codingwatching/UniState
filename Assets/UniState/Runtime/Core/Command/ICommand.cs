using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState.Runtime.Core.Command
{
    public interface ICommand<in TPayload, TResult>: IPayloadSetter<TPayload>, IDisposable
    {
        UniTask<TResult> Execute(CancellationToken token);
    }
}