using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState.Runtime.Core.Command
{
    public interface IFinishable: IDisposable
    {
        UniTask Finish(CancellationToken token);
    }
}