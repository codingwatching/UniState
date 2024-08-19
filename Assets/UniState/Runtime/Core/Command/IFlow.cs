using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState.Runtime.Core.Command
{
    public interface IFlow<in TPayload, TResult>: IPayloadSetter<TPayload>, IFinishable
    {
        UniTask<TResult> Start(CancellationToken token);
    }
}