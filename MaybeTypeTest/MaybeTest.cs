using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Katis.Data.MaybeTest
{
    [TestClass]
    public class MaybeTest
    {
        [TestMethod]
        public void TestHasSome()
        {
            var none = new Maybe<object>(null);
            Assert.IsFalse(none.HasSome, "HasSome should be false for none");

            var some = new Maybe<int>(23);
            Assert.IsTrue(some.HasSome, "HasSome should be true for some");
        }

        [TestMethod]
        public void TestImplicitConversion()
        {
            Maybe<string> some = "string";
            var val = "";
            some.ForEach(v => val = v);
            Assert.AreEqual("string", val, "Implicit conversion to value should produce a value");

            Maybe<string> none = null;
            var val2 = "";
            none.ForEach(v => val2 = v);
            Assert.AreEqual("", val2, "Implicit conversion on null should produce an empty maybe");
        }

        [TestMethod]
        public void TestOrElse()
        {
            var greeting = "Hello ";
            var world = Maybe.Some("world!");
            var meatbags = Maybe.Some("meatbags.");
            var message = greeting + world.OrElse(meatbags).GetOrElse("Clarice.");
            Assert.AreEqual("Hello world!", message, "message should equal \"Hello world!\"");

            var none = Maybe.None<string>();
            var messageNone = greeting + none.OrElse(meatbags).GetOrElse("Clarice.");
            Assert.AreEqual("Hello meatbags.", messageNone, "messageNone should equal \"Hello meatbags.\"");

            var messageNoneNone = greeting + none.OrElse(Maybe.None<string>()).GetOrElse("Clarice.");
            Assert.AreEqual("Hello Clarice.", messageNoneNone, "messageNoneNone should equal \"Hello Clarice.\"");
        }

        [TestMethod]
        public void TestForEach()
        {
            var none = Maybe.Some((object) null);
            var i = 0;
            none.ForEach(l => i += 1);
            Assert.AreEqual(0, i, "Empty May should not run ForEach callback");

            var j = 0;
            var some = Maybe.Some(2);
            some.ForEach(n => j += n);
            Assert.AreEqual(2, j, "May with value should run ForEach callback once");
        }

        [TestMethod]
        public void TestMap()
        {
            var some = Maybe.Some(0);
            var one = some.Map(v => v + 1).GetOrElse(0);
            Assert.AreEqual(1, one, "Map should return new value for some");

            var none = Maybe.None<int>();
            var zero = none.Map(v => v + 1).GetOrElse(0);
            Assert.AreEqual(0, zero, "Map should return the else-value for none");
        }

        [TestMethod]
        public void TestFlatMap()
        {
            var janeDoe = Maybe.Some(new {
                Name = "Jane Doe",
                Partner = Maybe.Some(new {
                    Name = "John Doe",
                    Job = Maybe.Some("Gardener"),
                    Pet = Maybe.None<string>()
                })
            });
            var husbandsJob = janeDoe
                .FlatMap(jane => jane.Partner)
                .FlatMap(partner => partner.Job)
                .GetOrElse("");
            Assert.AreEqual("Gardener", husbandsJob, "FlatMap should return the janeDoe.Partner.Job-field");

            var husbandsPet = janeDoe
                .FlatMap(jane => jane.Partner)
                .FlatMap(partner => partner.Pet)
                .GetOrElse("");
            Assert.AreEqual("", husbandsPet, "FlatMap should return an empty string for janeDoe.Partner.Pet");
        }

        [TestMethod]
        public void TestFilter()
        {
            var some = Maybe.Some("hello");
            var hello = some.Filter(s => s == "hello").GetOrElse("");
            Assert.AreEqual("hello", hello, "Filter should not filter the value in some");

            var notHello = some.Filter(s => s != "hello").GetOrElse("");
            Assert.AreEqual("", notHello, "Filter should filter out the \"hello\" in some");
        }


        [TestMethod]
        public void TestLinq()
        {
            var jill = Maybe.Some(new {Name = "Jill", Partner = Maybe.Some("Jack")});
            var john = Maybe.Some(new {Name = "John", Partner = Maybe.None<string>()});

            var result = from i in jill
                      from o in john
                      select new { Girl = i.Name , Boy = o.Name };

            var girl = "";
            var boy = "";
            foreach (var item in result)
            {
                girl = item.Girl;
                boy = item.Boy;
            }

            Assert.AreEqual("Jill", girl, "the Linq query should produce both peoples names");
            Assert.AreEqual("John", boy, "the Linq query should produce both peoples names");

            var result2 = from i in jill
                          from p1 in i.Partner
                          from j in john
                          from p2 in j.Partner
                          select new { JillPartner = p1, JohnPartner = p2 };

            var jillPartner = "";
            var johnPartner = "";
            foreach (var item2 in result2) {
                jillPartner = item2.JillPartner;
                johnPartner = item2.JohnPartner;
            }

            Assert.AreEqual("", jillPartner, "the Linq query should not produce a value.");
            Assert.AreEqual("", johnPartner, "the Linq query should not produce a value.");
        }

        [TestMethod]
        public void TestMatch()
        {
            var some = Maybe.Some("Hello");
            var greeting = some.Match(
                v => v + " world!",
                () => "'ola"
            );
            Assert.AreEqual("Hello world!", greeting, "Match on some should have produced \"Hello world!\"");

            var none = Maybe.None<string>();
            var noneGreeting = none.Match(
                v => v + " world!",
                () => "'ola"
            );
            Assert.AreEqual("'ola", noneGreeting, "Match on none should have produced \"'ola\"");
        }

        [TestMethod]
        public void TestMatchAct()
        {
            var greeting = "Hello ";
            var some = Maybe.Some("world!");
            some.MatchAct(
                v => greeting += v,
                () => greeting += "meatbag."
            );
            Assert.AreEqual("Hello world!", greeting, "MatchAct on some should change greeting to \"Hello world!\"");

            var greetingNone = "Hello ";
            var none = Maybe.None<string>();
            none.MatchAct(
                v => greetingNone += v,
                () => greetingNone += "meatbag."
            ); 
            Assert.AreEqual("Hello meatbag.", greetingNone, "MatchAct on none should change greeting to \"Hello meatbag.\"");
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestGetOrElseThrow()
        {
            var some = Maybe.Some("hello");
            some.GetOrThrow(new Exception());

            var none = Maybe.Some((string)null);
            none.GetOrThrow(new IndexOutOfRangeException());
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestGetOrElseThrowLambda()
        {
            var some = Maybe.Some("hello");
            some.GetOrThrow(() => new Exception());

            var none = Maybe.Some((string)null);
            var s = none.GetOrThrow(() => new IndexOutOfRangeException());
        }

        [TestMethod]
        public void TestGetMaybeList()
        {
            var list = new List<int>();
            list.Add(1);
            list.Add(2);
            var two = list.GetMaybe(1).GetOrElse(0);
            Assert.AreEqual(2, two, "GetMaybe should get the second item in list");

            var zero = list.GetMaybe(2).GetOrElse(0);
            Assert.AreEqual(0, zero, "GetMaybe should return none if the index is not in the list");

            var zero2 = list.GetMaybe(-1).GetOrElse(0);
            Assert.AreEqual(0, zero, "GetMaybe should return none if the index is not in the list");
        }

        [TestMethod]
        public void TestGetMaybeArray()
        {
            var array = new int[] { 1, 2 };
            var two = array.GetMaybe(1).GetOrElse(0);
            Assert.AreEqual(2, two, "GetMaybe should get the second item in list");

            var zero = array.GetMaybe(2).GetOrElse(0);
            Assert.AreEqual(0, zero, "GetMaybe should return none if the index is not in the list");

            var zero2 = array.GetMaybe(-1).GetOrElse(0);
            Assert.AreEqual(0, zero, "GetMaybe should return none if the index is not in the list");
        }

        [TestMethod]
        public void TestGetMaybeDictionary()
        {
            var hash = new Dictionary<string, int>();
            hash.Add("one", 1);
            hash.Add("two", 2);
            var two = hash.GetMaybe("two").GetOrElse(0);
            Assert.AreEqual(2, two, "GetMaybe should get the second item in list");

            var zero = hash.GetMaybe("three").GetOrElse(0);
            Assert.AreEqual(0, zero, "GetMaybe should return none if the index is not in the list");
        }

        [TestMethod]
        public void TestToArray()
        {
            var someArr = Maybe.Some(10).ToArray();
            Assert.AreEqual(1, someArr.Length, "ToArray of some should return a array with a length of 1");
            Assert.AreEqual(10, someArr[0], "ToArray of some should contain the value 10");

            var noneArr = Maybe.None<int>().ToArray();
            Assert.AreEqual(0, noneArr.Length, "ToArray of none should return a array with a length of 0");
        }

        [TestMethod]
        public void TestToList()
        {
            var someArr = Maybe.Some(10).ToList();
            Assert.AreEqual(1, someArr.Count, "ToArray of some should return a array with a length of 1");
            Assert.AreEqual(10, someArr[0], "ToArray of some should contain the value 10");

            var noneArr = Maybe.None<int>().ToList();
            Assert.AreEqual(0, noneArr.Count, "ToArray of none should return a array with a length of 0");
        }

        [TestMethod]
        public void TestEnumerator()
        {
            var some = Maybe.Some(1);
            var i = 0;
            foreach (var n in some)
            {
                i += n;
            }
            Assert.AreEqual(1, i, "Iterating over some should modify the i-variable once");

            var none = Maybe.None<int>();
            var j = 0;
            foreach (var n in none)
            {
                j += n;
            }
            Assert.AreEqual(0, j, "Iterating over none should not do anything.");
        }

        [TestMethod]
        public void TestToString()
        {
            var some = Maybe.Some(1);
            Assert.AreEqual("Some(1)", some.ToString(), "ToString on some should return Some(1)");

            var none = Maybe.None<string>();
            Assert.AreEqual("None", none.ToString(), "ToString on none should return None");
        }
    }
}
