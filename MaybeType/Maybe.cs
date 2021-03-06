﻿using System;
using System.Collections.Generic;

namespace Katis.Data
{
    public static class Maybe
    {
        /// <summary>
        /// Creates a new Maybe value with nothing inside.
        /// </summary>
        /// <typeparam name="T">Type of the potentially contained value</typeparam>
        /// <returns>Empty Maybe</returns>
        public static Maybe<T> None<T>()
        {
            return default(Maybe<T>);
        }

        /// <summary>
        /// Creates a new Maybe value with the provided value inside.
        /// Providing null as a parameter returns a Maybe with nothing inside.
        /// </summary>
        /// <typeparam name="T">Type of the value potentially inside.</typeparam>
        /// <param name="value">Value to contain in Maybe. If null, an empty Maybe is returned.</param>
        /// <returns>Maybe with the provided value inside.</returns>
        public static Maybe<T> Some<T>(T value)
        {
            return new Maybe<T>(value);
        }

        /// <summary>
        /// ToMaybe is an extension method for creating a Maybe value from the caller.
        /// Null value returns an empty Maybe.
        /// </summary>
        /// <returns>Caller wrapped in a Maybe</returns>
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return Maybe.Some(value);
        }

        /// <summary>
        /// GetMaybe is an extension method for lists returning the value at the provided index
        /// as a Maybe. An empty Maybe is returned if the index is not in range of the list.
        /// </summary>
        /// <param name="index">Index of the value to retrieve.</param>
        /// <returns>Value at index or empty.</returns>
        public static Maybe<T> GetMaybe<T>(this IList<T> list, int index)
        {
            if (index < list.Count && index >= 0) return Maybe.Some(list[index]);
            return Maybe.None<T>();
        }

        /// <summary>
        /// GetMaybe is an extension method for Array returning the value at the provided index
        /// as a Maybe. An empty Maybe is returned if the index is not in range of the array.
        /// </summary>
        /// <param name="index">Index of the value to retrieve.</param>
        /// <returns>Value at index or empty.</returns>
        public static Maybe<T> GetMaybe<T>(this T[] arr, int index)
        {
            if (index < arr.Length && index >= 0) return Maybe.Some(arr[index]);
            return Maybe.None<T>();
        }

        /// <summary>
        /// GetMaybe is an extension method for dictionary returning the value at the provided index
        /// as a Maybe. An empty Maybe is returned if the index is not found in the dictionary.
        /// </summary>
        /// <param name="key">Index of the value to retrieve.</param>
        /// <returns>Value at index or empty.</returns>
        public static Maybe<V> GetMaybe<K, V>(this IDictionary<K, V> dict, K key)
        {
            var v = default(Maybe<V>);
            v.has = dict.TryGetValue(key, out v.value);
            return v;
        }
    }

    /// <summary>
    /// Maybe represents a potentially empty value.
    /// It can also be thought of as a container containing 0 or 1 values.
    /// </summary>
    /// <typeparam name="T">Type of the value it potentially contains.</typeparam>
    public struct Maybe<T> : IEnumerable<T>
    {
        internal bool has;
        internal T value;

        private T Value { get { return value; } }

        /// <summary>
        /// Implicit conversion to a maybe value.
        /// </summary>
        /// <returns>Value in a maybe.</returns>
        public static implicit operator Maybe<T>(T v)
        {
            return Maybe.Some(v);
        }

        /// <summary>
        /// Creates a new Maybe value containing the provided value, or an empty Maybe if
        /// the value is null.
        /// </summary>
        public Maybe(T value)
        {
            if (value == null)
            {
                this.value = default(T);
                this.has = false;
            }
            else
            {
                this.value = value;
                this.has = true;
            }
        }

        /// <summary>
        /// Does the Maybe contain a value.
        /// </summary>
        public bool HasSome { get { return has; } }

        /// <summary>
        /// Maps over the value contained in the Maybe producing a new Maybe value.
        /// </summary>
        /// <param name="converter">Converter function called with the contained value if one exists.</param>
        /// <returns>Mapped over value in a Maybe, or empty.</returns>
        public Maybe<U> Map<U>(Func<T, U> converter)
        {
            return (!HasSome) ? default(Maybe<U>) : new Maybe<U>(converter(Value));
        }

        /// <summary>
        /// FlatMap maps over the value contained in the Maybe, producing a new Maybe value directly
        /// from the provided function. Can be used to flatten mapping nested Maybe-values.
        /// </summary>
        /// <param name="converter">Converter function providing a new Maybe</param>
        /// <returns>New Maybe created by the converter, or empty.</returns>
        public Maybe<U> FlatMap<U>(Func<T, Maybe<U>> converter)
        {
            return (!HasSome) ? default(Maybe<U>) : converter(Value);
        }

        /// <summary>
        /// FlatMap overload that returns the next value if the previous one exists.
        /// Can be used to check multiple Maybes containing values when you only care about the last one.
        /// </summary>
        /// <param name="nextValue">Next Maybe to return if the previous value exists.</param>
        /// <returns>nextValue or empty.</returns>
        public Maybe<U> FlatMap<U>(Maybe<U> nextValue)
        {
            return (!HasSome) ? default(Maybe<U>) : nextValue;
        }

        /// <summary>
        /// Filters the Maybe, returning a new Maybe with the contained value removed if it doesn't
        /// match the predicate.
        /// </summary>
        /// <param name="pred">Condition to test the contained value with.</param>
        /// <returns>Filtered Maybe</returns>
        public Maybe<T> Filter(Predicate<T> pred)
        {
            return (!HasSome || !pred(Value)) ? default(Maybe<T>) : new Maybe<T>(Value);
        }

        /// <summary>
        /// Calls the action on the contained value if it exists.
        /// </summary>
        /// <param name="action">Action called with the contained value.</param>
        public void ForEach(Action<T> action)
        {
            if (!HasSome) return;
            action(Value);
        }

        /// <summary>
        /// Gets the contained value, or a default value if the Maybe is empty.
        /// </summary>
        /// <param name="defaultValue">Value to return if Maybe is empty.</param>
        /// <returns>Value contained in Maybe or the default value.</returns>
        public T GetOrElse(T defaultValue)
        {
            return (!HasSome) ? defaultValue : Value;
        }

        /// <summary>
        /// Returns the Maybe if it contains a value, or the elseValue if it is empty.
        /// </summary>
        /// <param name="elseValue">Value to return if the caller is empty.</param>
        /// <returns>Caller or the elseValue if caller is empty.</returns>
        public Maybe<T> OrElse(Maybe<T> elseValue)
        {
            return (!HasSome) ? elseValue : new Maybe<T>(Value);
        }

        /// <summary>
        /// Gets the contained value or throws an exception if caller is empty.
        /// </summary>
        /// <typeparam name="E">Thrown exceptions type</typeparam>
        /// <param name="ex">Exception to throw if the caller is empty.</param>
        /// <returns>Contained value.</returns>
        public T GetOrThrow<E>(E ex) where E : Exception
        {
            if (!HasSome) throw ex;
            else return Value;
        }

        /// <summary>
        /// Gets the contained value or throws a lazily constructed exception if caller is empty.
        /// </summary>
        /// <typeparam name="E">Thrown exceptions type</typeparam>
        /// <param name="newException">Lazily creates a new exception if the Maybe is empty.</param>
        /// <returns>Contained value.</returns>
        public T GetOrThrow<E>(Func<E> newException) where E : Exception
        {
            if (!HasSome) throw newException();
            else return Value;
        }

        /// <summary>
        /// Matches on the caller, calling onSome if it contains a value and onNone if it's empty.
        /// </summary>
        /// <param name="onSome">Function to call when the caller contains a value.</param>
        /// <param name="onNone">Function to call when the caller is empty.</param>
        /// <returns>Created value from one of the branches.</returns>
        public U Match<U>(Func<T, U> onSome, Func<U> onNone)
        {
            return (!HasSome) ? onNone() : onSome(Value);
        }

        /// <summary>
        /// Matches on the caller, calling onSome if it contains a value and onNone if it's empty.
        /// </summary>
        /// <param name="onSome">Function to call when the caller contains a value.</param>
        /// <param name="onNone">Function to call when the caller is empty.</param>
        public void MatchAct(Action<T> onSome, Action onNone)
        {
            if (!HasSome) onNone();
            else onSome(Value);
        }

        /// <summary>
        /// Converts the Maybe to an Array containing 0 or 1 values.
        /// </summary>
        public T[] ToArray()
        {
            return (!HasSome) ? new T[]{} : new T[] { Value };
        }

        /// <summary>
        /// Converts the Maybe to a List containing 0 or 1 values.
        /// </summary>
        public List<T> ToList()
        {
            var list = new List<T>();
            if (HasSome) list.Add(Value);
            return list;
        }

        /// <summary>
        /// Gets an enumerator for the Maybe, providing 0 or 1 values.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            if (!HasSome)
            {
                yield break;
            }
            else
            {
                yield return Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            if (!HasSome)
            {
                yield break;
            }
            else
            {
                yield return Value;
            }
        }

        /// <summary>
        /// Returns a string representation of the potentially contained value.
        /// </summary>
        public override string ToString()
        {
            if (!HasSome)
            {
                return "None";
            } else {
                return String.Format("Some({0})", value.ToString());
            }
        }
    }
}
