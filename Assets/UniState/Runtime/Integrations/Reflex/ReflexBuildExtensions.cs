#if UNISTATE_REFLEX_SUPPORT

using System;
using System.Collections.Generic;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Resolvers;

namespace UniState
{
    public static class ReflexBuildExtensions
    {
        public static void AddStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract)
        {
            AddStateMachineInternal(builder, stateMachineImplementation, stateMachineContract, Lifetime.Transient);
        }

        public static void AddSingletonStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract)
        {
            AddStateMachineInternal(builder, stateMachineImplementation, stateMachineContract, Lifetime.Singleton);
        }

        public static void AddState(this ContainerBuilder builder, Type state)
        {
            ValidateStateBindingInput(state);

            builder.AddTransient(state);
        }

        public static void AddState(this ContainerBuilder builder, Type stateImplementation, Type stateContract)
        {
            ValidateStateBindingInput(stateImplementation, stateContract);

            builder.AddTransient(stateImplementation, stateContract);
        }

        public static void AddSingletonState(this ContainerBuilder builder, Type state)
        {
            ValidateStateBindingInput(state);

            builder.AddSingleton(state);
        }

        public static void AddSingletonState(this ContainerBuilder builder, Type stateImplementation,
            Type stateContract)
        {
            ValidateStateBindingInput(stateImplementation, stateContract);

            builder.AddSingleton(stateImplementation, stateContract);
        }

        private static void ValidateStateBindingInput(Type stateImplementation, Type stateContract)
        {
            ValidateStateBindingInput(stateImplementation);

            if (!stateContract.IsAssignableFrom(stateImplementation))
            {
                throw new ArgumentException(
                    $"AddState({stateImplementation.Name}): Type parameter state must implement {stateContract.Name}.");
            }
        }

        private static void ValidateStateBindingInput(Type state)
        {
            if (!typeof(IExecutableState).IsAssignableFrom(state))
            {
                throw new ArgumentException(
                    $"AddState({state.Name}): Type parameter state must implement IState<TPayload>");
            }
        }

        private static void ValidateStateMachineBindingInput(Type stateMachineImplementation, Type stateMachineContract)
        {
            if (stateMachineImplementation == stateMachineContract)
            {
                throw new ArgumentException(
                    $"AddStateMachine<{stateMachineImplementation.Name}>: Type parameters must differ : " +
                    "use AddStateMachine() where stateMachineImplementation implements stateMachineContract.\");");
            }

            if (!stateMachineContract.IsAssignableFrom(stateMachineImplementation))
            {
                throw new ArgumentException(
                    $"AddStateMachine: Type {stateMachineImplementation.Name} " +
                    $"must implement {stateMachineContract.Name}.");
            }

            if (!typeof(IStateMachine).IsAssignableFrom(stateMachineContract))
            {
                throw new ArgumentException(
                    $"AddStateMachine: Type {stateMachineContract.Name} " +
                    $"must implement IStateMachine.");
            }
        }

        private static void AddStateMachineInternal(
            ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract,
            Lifetime lifetime)
        {
            ValidateStateMachineBindingInput(stateMachineImplementation, stateMachineContract);

            builder.Bindings.Add(Binding.Validated(
                new ReflexStateMachineResolver(stateMachineImplementation, lifetime),
                stateMachineImplementation,
                stateMachineImplementation,
                stateMachineContract));
        }

        private sealed class ReflexStateMachineResolver : IResolver
        {
            private readonly Type _stateMachineImplementation;
            private readonly Lifetime _lifetime;
            private readonly List<IDisposable> _disposables = new();

            private object _instance;

            public Lifetime Lifetime => _lifetime;

            public ReflexStateMachineResolver(Type stateMachineImplementation, Lifetime lifetime)
            {
                _stateMachineImplementation = stateMachineImplementation;
                _lifetime = lifetime;
            }

            public object Resolve(Container container)
            {
                if (_lifetime == Lifetime.Singleton && _instance != null)
                {
                    return _instance;
                }

                var stateMachine = (IStateMachine)container.Construct(_stateMachineImplementation);
                stateMachine.SetResolver(container.ToTypeResolver());

                if (_lifetime == Lifetime.Singleton)
                {
                    _instance = stateMachine;
                }

                if (stateMachine is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }

                return stateMachine;
            }

            public void Dispose()
            {
                for (var i = _disposables.Count - 1; i >= 0; i--)
                {
                    _disposables[i].Dispose();
                }

                _disposables.Clear();
                _instance = null;
            }
        }
    }
}

#endif
