#if UNISTATE_REFLEX_SUPPORT

using System;
using System.Linq;
using System.Reflection;
using Reflex.Core;

namespace UniState
{
    public static class ReflexBuildExtensions
    {
        private static readonly MethodInfo AddTransientFactoryMethod = GetFactoryRegistrationMethod(nameof(ContainerBuilder.AddTransient));
        private static readonly MethodInfo AddSingletonFactoryMethod = GetFactoryRegistrationMethod(nameof(ContainerBuilder.AddSingleton));
        private static readonly MethodInfo CreateStateMachineFactoryMethod =
            typeof(ReflexBuildExtensions).GetMethod(nameof(CreateStateMachineFactory), BindingFlags.NonPublic | BindingFlags.Static);

        public static void AddStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract)
        {
            ValidateStateMachineBindingInput(stateMachineImplementation, stateMachineContract);

            builder.AddTransient(stateMachineImplementation);
            RegisterStateMachineFactory(builder, stateMachineImplementation, stateMachineContract, AddTransientFactoryMethod);
        }

        public static void AddSingletonStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract)
        {
            ValidateStateMachineBindingInput(stateMachineImplementation, stateMachineContract);

            builder.AddSingleton(stateMachineImplementation);
            RegisterStateMachineFactory(builder, stateMachineImplementation, stateMachineContract, AddSingletonFactoryMethod);
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

        private static void RegisterStateMachineFactory(
            ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract,
            MethodInfo registrationMethod)
        {
            var closedFactoryMethod = CreateStateMachineFactoryMethod.MakeGenericMethod(stateMachineImplementation);
            var factoryType = typeof(Func<,>).MakeGenericType(typeof(Container), stateMachineImplementation);
            var factory = Delegate.CreateDelegate(factoryType, closedFactoryMethod);
            var closedRegistrationMethod = registrationMethod.MakeGenericMethod(stateMachineImplementation);

            closedRegistrationMethod.Invoke(builder, new object[] { factory, new[] { stateMachineContract } });
        }

        private static TStateMachine CreateStateMachineFactory<TStateMachine>(Container container)
            where TStateMachine : class, IStateMachine
        {
            var stateMachine = container.Resolve<TStateMachine>();
            stateMachine.SetResolver(container.ToTypeResolver());

            return stateMachine;
        }

        private static MethodInfo GetFactoryRegistrationMethod(string methodName)
        {
            return typeof(ContainerBuilder)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(method =>
                    method.Name == methodName &&
                    method.IsGenericMethodDefinition &&
                    method.GetParameters().Length == 2 &&
                    method.GetParameters()[0].ParameterType.IsGenericType &&
                    method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));
        }
    }
}

#endif