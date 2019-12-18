using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionLibrary;
using System.Linq;

namespace UnitTestDependencyInjection
{
    public class A : AInterface
    {
        BInterface b;

        public A(BInterface B)
        {
            b = B;
        }
    }

    public interface AInterface
    {

    }

    public abstract class AbstractBar : IBar
    {

    }

    public abstract class AbstractFoo : IFoo
    {
        public void GetMe()
        {
            throw new NotImplementedException();
        }
    }

    public class B : BInterface
    {
        AInterface a;

        public B(AInterface A)
        {
            a = A;
        }
    }

    public interface BInterface
    {

    }

    public class BarFromAbstract : AbstractBar
    {

    }

    public class BarFromInterface : IBar
    {
        AbstractFoo Foo;

        public BarFromInterface(AbstractFoo foo)
        {
            Foo = foo;
        }
    }

    public class C : CInterface
    {
        public IEnumerable<DInterface> dEnum { get; }

        public C(IEnumerable<DInterface> d)
        {
            dEnum = d;
        }
    }

    public interface CInterface
    {
        IEnumerable<DInterface> dEnum { get; }
    }

    public interface DInterface
    {

    }

    public class FooFromInterface : IFoo
    {
        public AbstractBar Bar;

        public FooFromInterface(AbstractBar bar)
        {
            Bar = bar;
        }

        public void GetMe()
        {
            throw new NotImplementedException();
        }
    }

    public class D : DInterface
    {

    }

    public class GenericFoo<TInterface> : IGenericFoo<TInterface> where TInterface : IFoo
    {
        public TInterface DoSmth()
        {
            throw new NotImplementedException();
        }
    }

    public interface IBar
    {

    }

    public interface IFoo
    {
        void GetMe();
    }

    public interface IGenericFoo<TInterface>
       where TInterface : IFoo
    {
        TInterface DoSmth();
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ShouldRegisterInstancePerDependency()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<IBar, BarFromInterface>();

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var bar1 = dependencyProvider.Resolve<IBar>();
            var bar2 = dependencyProvider.Resolve<IBar>();

            Assert.AreNotEqual(bar1, bar2);
        }

        [TestMethod]
        public void ShouldRegisterSingleton()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.RegisterSingleton<AbstractBar, BarFromAbstract>();

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var bar1 = dependencyProvider.Resolve<AbstractBar>();
            var bar2 = dependencyProvider.Resolve<AbstractBar>();

            Assert.AreSame(bar1, bar2);
        }

        [TestMethod]
        public void ShouldCreateDependencyCorrectly()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<IBar, BarFromInterface>();
            dependencyConfiguration.Register<AbstractBar, BarFromAbstract>();
            dependencyConfiguration.Register<FooFromInterface, FooFromInterface>();

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var foo = dependencyProvider.Resolve<FooFromInterface>();

            Assert.IsNotNull(foo.Bar);
        }

        [TestMethod]
        public void ShouldThrowExceptionToAbstractClassImpl()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            try
            {
                dependencyConfiguration.Register<AbstractBar, AbstractBar>();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e, e.Message);
            }
        }

        [TestMethod]
        public void ShouldReturnNull()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<IFoo, FooFromInterface>();

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            IBar bar = dependencyProvider.Resolve<BarFromInterface>();
            Assert.IsNull(bar);
        }

        [TestMethod]
        public void ShouldReturnAsSelfCreation()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<FooFromInterface>();

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var foo = dependencyProvider.Resolve<FooFromInterface>();

            Assert.IsNotNull(foo);
            Assert.AreEqual(typeof(FooFromInterface), foo.GetType());
        }

        [TestMethod]
        public void ShouldAvoidCycle()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<AInterface, A>();
            dependencyConfiguration.Register<BInterface, B>();

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var A = dependencyProvider.Resolve<AInterface>();

            Assert.IsNotNull(A);
        }

        [TestMethod]
        public void ShouldResolveGenericType()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<IFoo, FooFromInterface>();
            dependencyConfiguration.Register(typeof(GenericFoo<IFoo>), typeof(GenericFoo<IFoo>));

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var GenericFoo = dependencyProvider.Resolve<GenericFoo<IFoo>>();
            Assert.IsNotNull(GenericFoo);
        }

        [TestMethod]
        public void ShouldReturnCollectionWithTwoElements()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<IBar, BarFromInterface>();
            dependencyConfiguration.Register<IBar, BarFromAbstract>();

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var bars = dependencyProvider.ResolveAll<IBar>();

            Assert.AreEqual(2, bars.Count());
        }

        [TestMethod]
        public void ShouldResolveOpenGenericType()
        {
            DependencyConfiguration dependencyConfiguration = new DependencyConfiguration();
            dependencyConfiguration.Register<IBar, BarFromAbstract>();
            dependencyConfiguration.Register<IFoo, FooFromInterface>();
            dependencyConfiguration.Register(typeof(GenericFoo<>), typeof(GenericFoo<>));

            DependencyProvider dependencyProvider = new DependencyProvider(dependencyConfiguration);

            var OpenGenFoo = dependencyProvider.Resolve<GenericFoo<FooFromInterface>>();
            Assert.IsNotNull(OpenGenFoo);
        }
    }
}
