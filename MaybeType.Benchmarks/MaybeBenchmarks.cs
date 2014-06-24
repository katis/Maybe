using pUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Katis.Data;

namespace MaybeType.Benchmarks
{
    [ProfileClass]
    public class MaybeBenchmarks
    {
        public static Dictionary<string, string> benchDict = newBenchDict();

        public static Dictionary<string, string> newBenchDict()
        {
            var dict = new Dictionary<string, string>();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (var i = 0; i < 999; i++)
            {
                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                dict.Add(new String(stringChars), "value");
            }
            dict.Add("asdfasd.", "value");
            return dict;
        }

        [ProfileMethod(2000)]
        public void BenchDictionaryTryGetValue()
        {
            for (var i = 0; i < 1000; i++)
            {
                var key = (i % 2 == 0) ? "asdfasd." : "ööööääää";
                var v = "";
                benchDict.TryGetValue(key, out v);
            }
        }

        [ProfileMethod(2000)]
        public void BenchGetMaybe()
        {
            for (var i = 0; i < 1000; i++)
            {
                var key = (i % 2 == 0) ? "asdfasd." : "ööööääää";
                var v = benchDict.GetMaybe(key).GetOrElse("");
            }
        }

        [ProfileMethod(2000)]
        public void BenchContainsAndGet()
        {
            for (var i = 0; i < 1000; i++)
            {
                var key = (i % 2 == 0) ? "asdfasd." : "ööööääää";
                var v = "";
                if (benchDict.ContainsKey(key))
                {
                    v = benchDict[key];
                }
            }
        }
    }
}
