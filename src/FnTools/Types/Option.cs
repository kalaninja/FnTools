using System;
using System.Collections.Generic;
using FnTools.Exceptions;
using FnTools.Types.Interfaces;
using static FnTools.Prelude;

namespace FnTools.Types
{
    /// <summary>
    /// Represents optional values. Instances of Option are either Some or None.
    /// The most idiomatic way to use an Option instance is to treat it as a monad and use Map(), FlatMap(), Filter().
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public readonly struct Option<T> : IGettable<T>, IToEither<T>, IEquatable<Option<T>>
    {
        private readonly T _value;

        /// <summary>
        /// Returns true if the option is Some, false otherwise.
        /// </summary>
        public bool IsSome { get; }

        /// <summary>
        /// Returns true if the option is None, false otherwise.
        /// </summary>
        public bool IsNone => !IsSome;

        /// <summary>
        /// Instantiates Some representing an existing value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        public Option(T value)
        {
            IsSome = true;
            _value = value;
        }

        /// <summary>
        /// Returns the option's value.
        /// </summary>
        /// <returns></returns>
        public T Get() => IsSome ? _value : throw new InvalidOperationException(ExceptionMessages.OptionIsNone);

        /// <summary>
        /// Returns the option's value if the option is Some, otherwise returns <paramref name="or"/>.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public T GetOrElse(T or) => IsSome ? _value : or;

        /// <summary>
        /// Returns the option's value if the option is Some, otherwise returns the result of evaluating <paramref name="or"/>.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public T GetOrElse(Func<T> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return IsSome ? _value : or();
        }

        /// <summary>
        /// Returns the option's value if the option is Some, otherwise returns default value of <typeparamref name="T"/>.
        /// </summary>
        /// <returns></returns>
        public T GetOrDefault() => IsSome ? _value : default;

        /// <summary>
        /// Applies a function on the optional value.
        /// Returns a Some containing the result of applying <paramref name="map"/> to this Option's value
        /// if this Option is Some. Otherwise return None.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public Option<TResult> Map<TResult>(Func<T, TResult> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsSome ? map(_value) : new Option<TResult>();
        }

        /// <summary>
        /// Returns the result of applying <paramref name="map"/> to this Option's value
        /// if this Option is Some. Returns None if this Option is None.
        /// Slightly different from Map() in that <paramref name="map"/> is expected to return an Option (which could be None).
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public Option<TResult> FlatMap<TResult>(Func<T, Option<TResult>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsSome ? map(_value) : new Option<TResult>();
        }

        /// <summary>
        /// Returns this Option if it is Some and applying the predicate <paramref name="condition"/> to this Option's value returns true.
        /// Otherwise, return None.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Option<T> Filter(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsSome && condition(_value) ? this : new Option<T>();
        }

        /// <summary>
        /// Returns this Option if it is Some and <paramref name="condition"/> is true. Otherwise, return None.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Option<T> Filter(bool condition) =>
            IsSome && condition ? this : new Option<T>();

        /// <summary>
        /// Returns true if this option is Some and the predicate <paramref name="condition"/> returns true
        /// when applied to this Option's value. Otherwise, returns false.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Exists(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsSome && condition(_value);
        }

        /// <summary>
        /// Returns the result of applying <paramref name="some"/> to this Option's value if the Option is Some.
        /// Otherwise, returns <paramref name="none"/>.
        /// </summary>
        /// <param name="some"></param>
        /// <param name="none"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult Fold<TResult>(Func<T, TResult> some, TResult none)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));

            return IsSome ? some(_value) : none;
        }

        /// <summary>
        /// Returns the result of applying <paramref name="some"/> to this Option's value if the Option is Some.
        /// Otherwise, evaluates <paramref name="none"/>.
        /// </summary>
        /// <param name="some"></param>
        /// <param name="none"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult Fold<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));
            _ = none ?? throw new ArgumentNullException(nameof(none));

            return IsSome ? some(_value) : none();
        }

        /// <summary>
        /// Executes <paramref name="some"/> to this Option's value if the Option is Some.
        /// Otherwise, executes <paramref name="none"/>.
        /// </summary>
        /// <param name="some"></param>
        /// <param name="none"></param>
        /// <returns></returns>
        public void Match(Action<T> some, Action none)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));
            _ = none ?? throw new ArgumentNullException(nameof(none));

            if (IsSome)
                some(_value);
            else
                none();
        }

        /// <summary>
        /// Returns a Right containing the given argument <paramref name="right"/> if this Option is None,
        /// or a Left containing this Option's value if Option is Some.
        /// </summary>
        /// <param name="right"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public Either<T, TR> ToLeft<TR>(TR right) => IsSome ? (Either<T, TR>) _value : right;

        /// <summary>
        /// Returns a Right containing the result of <paramref name="right"/> if this Option is None,
        /// or a Left containing this Option's value if Option is Some.
        /// </summary>
        /// <param name="right"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public Either<T, TR> ToLeft<TR>(Func<TR> right)
        {
            _ = right ?? throw new ArgumentNullException(nameof(right));

            return IsSome ? (Either<T, TR>) _value : right();
        }

        /// <summary>
        /// Returns a Left containing the the given argument <paramref name="left"/> if this Option is None,
        /// or a Right containing this Option's value if Option is Some.
        /// </summary>
        /// <param name="left"></param>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public Either<TL, T> ToRight<TL>(TL left) => IsSome ? (Either<TL, T>) _value : left;

        /// <summary>
        /// Returns a Left containing the result of <paramref name="left"/> if this Option is None,
        /// or a Right containing this Option's value if Option is Some.
        /// </summary>
        /// <param name="left"></param>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public Either<TL, T> ToRight<TL>(Func<TL> left)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));

            return IsSome ? (Either<TL, T>) _value : left();
        }

        public override string ToString() => IsSome ? $"Some({_value})" : "None";

        public static implicit operator Option<T>(Option<Nothing> _) => new Option<T>();

        public static implicit operator Option<T>(T value) => value is null ? new Option<T>() : new Option<T>(value);

        public static explicit operator T(Option<T> option) =>
            option.IsSome
                ? option._value
                : throw new InvalidCastException(ExceptionMessages.OptionIsNone);

        public static Option<T> operator &(Option<T> left, Option<T> right) => left ? right : left;

        public static Option<T> operator |(Option<T> left, Option<T> right) => left ? left : right;

        public static bool operator true(Option<T> option) => option.IsSome;

        public static bool operator false(Option<T> option) => option.IsNone;

        public bool Equals(Option<T> other) =>
            IsSome == other.IsSome && (!IsSome || EqualityComparer<T>.Default.Equals(_value, other._value));

        public override bool Equals(object obj) =>
            obj is Option<T> other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return IsSome
                    ? (EqualityComparer<T>.Default.GetHashCode(_value) * 397) ^ IsSome.GetHashCode()
                    : IsSome.GetHashCode();
            }
        }

        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

        /// <summary>
        /// Instantiates Some representing an existing value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Option<T> Some(T value) => new Option<T>(value);

        /// <summary>
        /// Instantiates None representing no value.
        /// </summary>
        public static Option<T> None => new Option<T>();

        /// <summary>
        /// Deconstructs Option.
        /// </summary>
        /// <param name="isSome">Returns true if this Option is Some. Otherwise, false.</param>
        /// <param name="value">Returns value if this Option is Some. Otherwise, default value of <typeparamref name="T"/></param>
        public void Deconstruct(out bool isSome, out T value)
        {
            isSome = IsSome;
            value = _value;
        }
    }

    public static class Option
    {
        /// <summary>
        /// Converts caller to Some if caller is not null. Otherwise, to None.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Option<T> ToOption<T>(this T value) => value;

        /// <summary>
        /// Converts caller to Some, even if it is null.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Option<T> ToSome<T>(this T value) => Some(value);

        /// <summary>
        /// Returns the nested Option value if it is Some. Otherwise, returns None.
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Option<T> Flatten<T>(this Option<Option<T>> self) => self.FlatMap(Combinators.I);

        public static Option<TB> Select<TA, TB>(this Option<TA> self, Func<TA, TB> map) => self.Map(map);

        public static Option<TB> SelectMany<TA, TB>(this Option<TA> self, Func<TA, Option<TB>> map) =>
            self.FlatMap(map);

        public static Option<TC> SelectMany<TA, TB, TC>(this Option<TA> self, Func<TA, Option<TB>> map,
            Func<TA, TB, TC> project)
        {
            _ = project ?? throw new ArgumentNullException(nameof(project));

            return self.FlatMap(a => map(a).Map(b => project(a, b)));
        }

        public static Option<T> Where<T>(this Option<T> self, Func<T, bool> condition) => self.Filter(condition);
    }
}