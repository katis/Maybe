using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katis.Data
{
    internal class Ref<T>
    {
        private T _v;
        public T Value { get { return _v; } }

        public Ref(T v)
        {
            _v = v;
        }
    }

    public static class Maybe
    {
        public static Maybe<T> None<T>()
        {
            return default(Maybe<T>);
        }

        public static Maybe<T> Create<T>(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return Maybe.Create(value);
        }
    }

    public struct Maybe<T> : IEnumerable<T>
    {
        private readonly Ref<T> v;

        public Maybe(T value)
        {
            v = (value == null) ? null : new Ref<T>(value);
        }

        public bool HasSome { get { return (v != null); } }

        public Maybe<U> Map<U>(Func<T, U> converter)
        {
            return (v == null) ? default(Maybe<U>) : new Maybe<U>(converter(v.Value));
        }

        public Maybe<U> FlatMap<U>(Func<T, Maybe<U>> converter)
        {
            return (v == null) ? default(Maybe<U>) : converter(v.Value);
        }

        public Maybe<U> FlatMap<U>(Maybe<U> nextValue)
        {
            return (v == null) ? default(Maybe<U>) : nextValue;
        }

        public Maybe<T> Filter(Predicate<T> pred)
        {
            return (v == null || !pred(v.Value)) ? default(Maybe<T>) : new Maybe<T>(v.Value);
        }

        public void ForEach(Action<T> action)
        {
            if (v == null) return;
            action(v.Value);
        }

        public T GetOrElse(T defaultValue)
        {
            return (v == null) ? defaultValue : v.Value;
        }

        public Maybe<T> OrElse(Maybe<T> elseValue)
        {
            return (v == null) ? elseValue : new Maybe<T>(v.Value);
        }

        public T GetOrThrow<E>(E ex) where E : Exception
        {
            if (v == null) throw ex;
            else return v.Value;
        }

        public T GetOrThrow<E>(Func<E> newException) where E : Exception
        {
            if (v == null) throw newException();
            else return v.Value;
        }

        public U Match<U>(Func<T, U> onSome, Func<U> onNone)
        {
            return (v == null) ? onNone() : onSome(v.Value);
        }

        public void MatchAct(Action<T> onSome, Action onNone)
        {
            if (v == null) onNone();
            else onSome(v.Value);
        }

        public T[] ToArray()
        {
            return (v == null) ? new T[]{} : new T[] { v.Value };
        }

        public List<T> ToList()
        {
            var list = new List<T>();
            if (v != null) list.Add(v.Value);
            return list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (v == null)
            {
                yield break;
            }
            else
            {
                yield return v.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            if (v == null)
            {
                yield break;
            }
            else
            {
                yield return v.Value;
            }
        }
    }
}
