using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public interface IFoo { }

    public interface IBar { }

    public abstract class ABar : IBar { }

    public class BarFromIBar : IBar
    {
        IFoo foo;
        public BarFromIBar(IFoo foo)
        {
            this.foo = foo;
        }
    }

    public class BarFromABar : ABar
    {

    }

    public class Foo : IFoo
    {
        public ABar Bar { get; }
        public Foo()
        {
            Console.WriteLine("I am Foo!");
        }
        public Foo(bool k)
        {
            Console.WriteLine("I am Foo with bool!");
        }
        public Foo(ABar bar)
        {
            Bar = bar;
            Console.WriteLine("I am Foo with ABar!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DependencyConfiguration dc = new DependencyConfiguration();

            dc.Register<IBar, BarFromIBar>();
            dc.Register<ABar, BarFromABar>();
            dc.Register<IFoo, Foo>();

            DependencyProvider dp = new DependencyProvider(dc);

            var foo = dp.Resolve<IFoo>();

            DependencyConfiguration newDC = new DependencyConfiguration();

            newDC.Register<IBar, BarFromIBar>();
            newDC.Register<IBar, BarFromABar>();

            DependencyProvider newDP = new DependencyProvider(newDC);
            var bars = newDP.ResolveAll<IBar>();

            Console.WriteLine(bars.Count());

            Console.ReadLine();
        }
    }
}
