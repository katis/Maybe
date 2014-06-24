using pUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katis.Data.MaybeTypeBenchMark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting benchmarks");
            var runner = new ProfileRunner();
            runner.Run();
            Console.WriteLine("benchmarks done");
        }
    }
}
