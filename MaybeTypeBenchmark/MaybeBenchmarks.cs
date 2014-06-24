using pUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Katis.Data.MaybeTypeBenchmark;

namespace Katis.Data.MaybeTypeBenchmark
{
    [ProfileClass]
    public class MaybeBenchmarks
    {
        public static Dictionary<string, string> benchDict = newBenchDict();

        public static Dictionary<string, string> newBenchDict()
        {
            Console.WriteLine("SADF");
            var dict = new Dictionary<string, string>();
            dict.Add("foo", "value1");
            dict.Add("bar", "value2");
            dict.Add("baz", "value3");
            dict.Add("ASDFF", "value4");
            return dict;
        }

        [ProfileMethod(100)]
        public void BenchDictionaryTryGetValue()
        {
            Console.WriteLine("1");
            var v = default(string);
            benchDict.TryGetValue("baz", out v);
        }

        [ProfileMethod(100)]
        public void BenchGetMaybe()
        {
            Console.WriteLine("2");
            var v = benchDict.GetMaybe("baz");
        }

        [ProfileMethod(100)]
        public void BenchContainsAndGet()
        {
            Console.WriteLine("3");
            if (benchDict.ContainsKey("baz"))
            {
                var v = benchDict["baz"];
            }
        }
    }
}
