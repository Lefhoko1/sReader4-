using System;
using System.Collections.Generic;

namespace SReader.Core.DependencyInjection
{
    /// <summary>
    /// Minimal DI container. Registrations happen ONLY inside the
    /// AppCompositionRoot; nothing else may call Resolve (that would be a
    /// service locator). Views receive their ViewModels by injection.
    /// </summary>
    public sealed class ServiceContainer
    {
        readonly Dictionary<Type, Func<ServiceContainer, object>> factories =
            new Dictionary<Type, Func<ServiceContainer, object>>();
        readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();

        /// <summary>Register an already-built instance as a singleton.</summary>
        public void RegisterInstance<TService>(TService instance) where TService : class
        {
            singletons[typeof(TService)] = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>Register a lazily-created singleton via factory.</summary>
        public void RegisterSingleton<TService>(Func<ServiceContainer, TService> factory) where TService : class
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            factories[typeof(TService)] = c => factory(c);
        }

        public TService Resolve<TService>() where TService : class
        {
            return (TService)Resolve(typeof(TService));
        }

        object Resolve(Type type)
        {
            if (singletons.TryGetValue(type, out var instance))
                return instance;

            if (factories.TryGetValue(type, out var factory))
            {
                var created = factory(this);
                singletons[type] = created; // cache: every registration is a singleton
                return created;
            }

            throw new InvalidOperationException(
                $"[ServiceContainer] No registration for {type.Name}. Register it in AppCompositionRoot.");
        }
    }
}
