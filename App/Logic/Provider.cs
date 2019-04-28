using App.Interfaces.Logic;
using Autofac;
using JetBrains.Annotations;

namespace App.Logic
{
    public class Provider<T> : IProvider<T>
    {
        [NotNull]
        private readonly ILifetimeScope _lifetimeScope;

        public Provider([NotNull] ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public T Get()
        {
            return _lifetimeScope.Resolve<T>();
        }
    }
}
