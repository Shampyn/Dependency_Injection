using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
   public class DependencyConfiguration
    {
        private readonly Dictionary<Type, List<ConfiguratedType>> _configuration;
        public IDictionary<Type, List<ConfiguratedType>> Configuration => _configuration;

        public DependencyConfiguration()
        {
            _configuration = new Dictionary<Type, List<ConfiguratedType>>();
        }

        public void Register<TImplementation>()
            where TImplementation : class
        {
            RegisterType(typeof(TImplementation), typeof(TImplementation));
        }

        public void Register<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class
        {
            RegisterType(typeof(TInterface), typeof(TImplementation));
        }

        public void Register(Type TInterface, Type TImplementation)
        {
            RegisterType(TInterface, TImplementation);
        }

        public void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            RegisterType(typeof(TImplementation), typeof(TImplementation), true);
        }

        public void RegisterSingleton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class
        {
            RegisterType(typeof(TInterface), typeof(TImplementation), true);
        }

        public void RegisterSingleton(Type TInterface, Type TImplementation)
        {
            RegisterType(TInterface, TImplementation, true);
        }

        public void RegisterType(Type TInterface, Type TImplementation, bool isSingleton = false)
        {
            if (!TImplementation.IsInterface && !TImplementation.IsAbstract && TInterface.IsAssignableFrom(TImplementation))
            {
                ConfiguratedType configuratedType = new ConfiguratedType(TInterface, TImplementation, isSingleton);

                if (_configuration.ContainsKey(TInterface))
                {
                    _configuration[TInterface].Add(configuratedType);
                }
                else
                {
                    _configuration.Add(TInterface, new List<ConfiguratedType> { configuratedType });
                }

            }
            else
            {
                throw new Exception($"{TImplementation.ToString()} can't be added with {TInterface.ToString()}");
            }
        }

        public ConfiguratedType GetConfiguratedType(Type TInterface)
        {
            return _configuration.TryGetValue(TInterface, out var configuratedTypes) ? configuratedTypes.Last() : null;
        }

        public IEnumerable<ConfiguratedType> GetConfiguratedTypes(Type TInterface)
        {
            return _configuration.TryGetValue(TInterface, out var configuratedTypes) ? configuratedTypes : null;
        }
    }
}
