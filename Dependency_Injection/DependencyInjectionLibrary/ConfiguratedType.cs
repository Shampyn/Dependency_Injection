using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionLibrary
{
    public class ConfiguratedType
    {
        public bool IsSingleton { get; set; }

        public Type ImplementationInterface { get; }

        public Type Implementation { get; }

        public object Instance { get; set; }

        public ConfiguratedType(Type interf, Type implementation, bool isSingleton = false)
        {
            ImplementationInterface = interf;
            Implementation = implementation;
            IsSingleton = isSingleton;
            Instance = null;
        }
    }
}
