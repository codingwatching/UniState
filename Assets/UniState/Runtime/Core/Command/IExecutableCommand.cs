using System;

namespace UniState.Runtime.Core.Command
{
    public interface IExecutableCommand<out T>: IDisposable
    {
        T Execute();
    }
}