using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace MarketDataServices
{
    public static class ContainerManager
    {
        private static IUnityContainer _container = new UnityContainer();

        public static IUnityContainer GetContainer()
        {
            return _container;
        }

        public static void RegisterType(Type typeFrom, Type typeTo)
        {
            _container.RegisterType(typeFrom,typeTo);
        }

        public static T ResolveValue<T>() where T : class
        {
            var result = _container.Resolve<T>();
            if(result != null)
            {
                return result;
            }
            throw new ArgumentException(string.Format("type {0} has not been registered!"));
        }
    }
}
