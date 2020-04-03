using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static FnTools.Prelude;

namespace FnTools.Types
{
    public readonly struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T _value;

        public bool IsSome { get; }

        public bool IsNone => !IsSome;

        public Option(T value)
        {
            IsSome = true;
            _value = value;
        }

        public T Get() => IsSome ? _value : throw new InvalidOperationException();

        public T GetOrElse(T or) => IsSome ? _value : or;

        public T GetOrElse(Func<T> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return IsSome ? _value : or();
        }

        [return: MaybeNull]
        public T GetOrDefault() => IsSome ? _value : default;

        public Option<TResult> Map<TResult>(Func<T, TResult> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsSome ? map(_value) : new Option<TResult>();
        }

        public Option<TResult> FlatMap<TResult>(Func<T, Option<TResult>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsSome ? map(_value) : new Option<TResult>();
        }

        public Option<T> Filter(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsSome && condition(_value) ? this : new Option<T>();
        }

        public bool Exists(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsSome && condition(_value);
        }

        public TResult Fold<TResult>(Func<T, TResult> some, TResult none)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));

            return IsSome ? some(_value) : none;
        }

        public TResult Fold<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));
            _ = none ?? throw new ArgumentNullException(nameof(none));

            return IsSome ? some(_value) : none();
        }

        public void Match(Action<T> some, Action none)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));
            _ = none ?? throw new ArgumentNullException(nameof(none));

            if (IsSome)
                some(_value);
            else
                none();
        }

        public Either<T, TR> ToLeft<TR>(TR right) => IsSome ? (Either<T, TR>) Get() : right;

        public Either<T, TR> ToLeft<TR>(Func<TR> right)
        {
            _ = right ?? throw new ArgumentNullException(nameof(right));

            return IsSome ? (Either<T, TR>) Get() : right();
        }

        public Either<TL, T> ToRight<TL>(TL left) => IsSome ? (Either<TL, T>) Get() : left;

        public Either<TL, T> ToRight<TL>(Func<TL> left)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));

            return IsSome ? (Either<TL, T>) Get() : left();
        }

        public override string ToString() => IsSome ? $"Some({_value?.ToString() ?? "null"})" : "None";

        public static implicit operator Option<T>(Option<Nothing> _) => new Option<T>();

        public static implicit operator Option<T>(T value) => value is null ? new Option<T>() : new Option<T>(value);

        public static explicit operator T(Option<T> option) =>
            option.IsSome
                ? option._value
                : throw new InvalidCastException("Option is None");

        public static Option<T> operator &(Option<T> left, Option<T> right) => left ? right : left;

        public static Option<T> operator |(Option<T> left, Option<T> right) => left ? left : right;

        public static bool operator true(Option<T> option) => option.IsSome;

        public static bool operator false(Option<T> option) => option.IsNone;

        public bool Equals(Option<T> other) =>
            IsSome == other.IsSome && (!IsSome || EqualityComparer<T>.Default.Equals(_value, other._value));

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) && obj is Option<T> other && Equals(other);

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

        public static Option<T> Some(T value) => new Option<T>(value);

        public static Option<T> None => new Option<T>();
    }

    public static class Option
    {
        public static Option<T> ToOption<T>(this T value) => value;

        public static Option<T> ToSome<T>(this T value) => Some(value);

        public static Option<T> Flatten<T>(this Option<Option<T>> self) => self.FlatMap(Combinators.I);

        public static Option<TB> Select<TA, TB>(this Option<TA> self, Func<TA, TB> map) => self.Map(map);

        public static Option<TB> SelectMany<TA, TB>(this Option<TA> self, Func<TA, Option<TB>> map) =>
            self.FlatMap(map);

        public static Option<TC> SelectMany<TA, TB, TC>(this Option<TA> self, Func<TA, Option<TB>> map,
            Func<TA, TB, TC> project)
        {
            _ = project ?? throw new ArgumentNullException(nameof(project));

            return self.FlatMap(map).FlatMap(x => Some(project(self.Get(), x)));
        }

        public static Option<T> Where<T>(this Option<T> self, Func<T, bool> condition) => self.Filter(condition);
    }
}