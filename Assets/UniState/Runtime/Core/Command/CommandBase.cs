using System.Threading;

namespace UniState.Runtime.Core.Command
{
    public class CommandBase<T>: ICommand<T>
    {
        public T Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {

        }

        public void SetToken(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}