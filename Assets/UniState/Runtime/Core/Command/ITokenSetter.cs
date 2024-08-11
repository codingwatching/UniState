using System.Threading;

namespace UniState.Runtime.Core.Command
{
    public interface ITokenSetter
    {
        void SetToken(CancellationToken token);
    }
}