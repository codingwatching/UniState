namespace UniState.Runtime.Core.Command
{
    public interface ICommand<out T>: IExecutableCommand<T>, ITokenSetter
    {

    }
}