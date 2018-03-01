using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace InvestmentBuilderCore
{
    public static class ContainerManager
    {
        private static IUnityContainer _container = new UnityContainer();

        public static IUnityContainer GetContainer()
        {
            return _container;
        }

        public static void RegisterType(Type typeFrom, bool bHierachialLifetime)
        {
            var lifetimeManager = bHierachialLifetime ? new HierarchicalLifetimeManager() : new ContainerControlledLifetimeManager();
            _container.RegisterType(typeFrom, lifetimeManager);
        }

        public static void RegisterType(Type typeFrom, Type typeTo, bool bHierachialLifetime, params string[] prm)
        {
            var lifetimeManager = bHierachialLifetime ? new HierarchicalLifetimeManager() : new ContainerControlledLifetimeManager();
            if (prm == null || prm.Length == 0)
            {
                _container.RegisterType(typeFrom, typeTo, lifetimeManager);
            }
            else
            {
                _container.RegisterType(typeFrom, typeTo, lifetimeManager, new InjectionConstructor(prm));
            }
        }

        public static T ResolveValueOnContainer<T>(IUnityContainer container) where T : class
        {
            var result = container.Resolve<T>();
            if (result != null)
            {
                return result;
            }
            throw new ArgumentException(string.Format("type {0} has not been registered!"));

        }

        public static T ResolveValueOnContainer<T>(Type typeOf, IUnityContainer container) where T : class
        {
            var result = container.Resolve(typeOf);
            if(result == null)
            {
                throw new ArgumentException(string.Format("type {0} has not been registered!", typeOf));
            }
            return result as T;
        }

        public static T ResolveValue<T>() where T : class
        {
            return ResolveValueOnContainer<T>(_container);
        }

        public static IUnityContainer CreateChildContainer()
        {
            return _container.CreateChildContainer();
        }
    }
}
