using System;
using System.Collections.Generic;
using FnTools.Exceptions;
using FnTools.Types.Interfaces;

namespace FnTools.Types
{
    /// <summary>
    /// Represents a value of one of two possible types (a disjoint union). An instance of Either is either Left or Right.
    /// </summary>
    /// <typeparam name="TL">Type of Left.</typeparam>
    /// <typeparam name="TR">Type of Right.</typeparam>
    public readonly struct Either<TL, TR> : IEquatable<Either<TL, TR>>
    {
        private enum EitherState : byte
        {
            Left = 1,
            Right
        }

        // ReSharper disable once InconsistentNaming
        internal readonly TL _left;

        // ReSharper disable once InconsistentNaming
        internal readonly TR _right;

        private EitherState State { get; }

        /// <summary>
        /// Returns true if this is a Left, false otherwise.
        /// </summary>
        public bool IsLeft => State == EitherState.Left;

        /// <summary>
        /// Returns true if this is a Right, false otherwise.
        /// </summary>
        public bool IsRight => State == EitherState.Right;

        /// <summary>
        /// Instantiates Left with the value of <typeparamref name="TL"/>
        /// </summary>
        /// <param name="left"></param>
        public Either(TL left)
        {
            _left = left;
            State = EitherState.Left;

            _right = default;
        }

        /// <summary>
        /// Instantiates Right with the value of <typeparamref name="TR"/>
        /// </summary>
        /// <param name="right"></param>
        public Either(TR right)
        {
            _right = right;
            State = EitherState.Right;

            _left = default;
        }

        /// <summary>
        /// Projects this Either as a Left.
        /// </summary>
        public EitherLeftProjection<TL, TR> Left => new EitherLeftProjection<TL, TR>(this);

        /// <summary>
        /// Projects this Either as a Right.
        /// </summary>
        public EitherRightProjection<TL, TR> Right => new EitherRightProjection<TL, TR>(this);

        /// <summary>
        /// Maps over both Left and Right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <typeparam name="TLResult"></typeparam>
        /// <typeparam name="TRResult"></typeparam>
        /// <returns></returns>
        public Either<TLResult, TRResult> BiMap<TLResult, TRResult>(Func<TL, TLResult> left, Func<TR, TRResult> right)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));
            _ = right ?? throw new ArgumentNullException(nameof(right));

            return State switch
            {
                EitherState.Right => right(_right),
                EitherState.Left => left(_left),
                _ => throw new InvalidOperationException(ExceptionMessages.EitherIsBottom)
            };
        }

        /// <summary>
        /// Applies <paramref name="left"/> if this is a Left or <paramref name="right"/> if this is a Right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Fold<T>(Func<TL, T> left, Func<TR, T> right)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));
            _ = right ?? throw new ArgumentNullException(nameof(right));

            return State switch
            {
                EitherState.Right => right(_right),
                EitherState.Left => left(_left),
                _ => throw new InvalidOperationException(ExceptionMessages.EitherIsBottom)
            };
        }

        /// <summary>
        /// Executes <paramref name="left"/> if this is a Left or <paramref name="right"/> if this is a Right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Match(Action<TL> left, Action<TR> right)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));
            _ = right ?? throw new ArgumentNullException(nameof(right));

            switch (State)
            {
                case EitherState.Right:
                    right(_right);
                    break;
                case EitherState.Left:
                    left(_left);
                    break;
                default:
                    throw new InvalidOperationException(ExceptionMessages.EitherIsBottom);
            }
        }

        /// <summary>
        /// If this is a Left, then return the left value in Right or vice versa.
        /// </summary>
        /// <returns></returns>
        public Either<TR, TL> Swap() =>
            State switch
            {
                EitherState.Right => _right,
                EitherState.Left => (Either<TR, TL>) _left,
                _ => throw new InvalidOperationException(ExceptionMessages.EitherIsBottom)
            };

        public override string ToString() =>
            State switch
            {
                EitherState.Right => $"Right({_right})",
                EitherState.Left => $"Left({_left})",
                _ => throw new InvalidOperationException(ExceptionMessages.EitherIsBottom)
            };

        public bool Equals(Either<TL, TR> other) =>
            State == other.State && State switch
            {
                EitherState.Right => EqualityComparer<TR>.Default.Equals(_right, other._right),
                EitherState.Left => EqualityComparer<TL>.Default.Equals(_left, other._left),
                _ => throw new InvalidOperationException(ExceptionMessages.EitherIsBottom)
            };

        public override bool Equals(object obj) =>
            obj is Either<TL, TR> other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsLeft
                    ? EqualityComparer<TL>.Default.GetHashCode(_left)
                    : EqualityComparer<TR>.Default.GetHashCode(_right);

                return (hashCode * 397) ^ (int) State;
            }
        }

        public static bool operator ==(Either<TL, TR> left, Either<TL, TR> right) => left.Equals(right);
        public static bool operator !=(Either<TL, TR> left, Either<TL, TR> right) => !left.Equals(right);

        public static bool operator true(Either<TL, TR> either) => either.IsLeft;
        public static bool operator false(Either<TL, TR> either) => either.IsRight;

        public static implicit operator Either<TL, TR>(TL left) => new Either<TL, TR>(left);
        public static implicit operator Either<TL, TR>(TR right) => new Either<TL, TR>(right);

        public static implicit operator Either<TL, TR>(Either<TL, Nothing> left) => new Either<TL, TR>(left._left);
        public static implicit operator Either<TL, TR>(Either<Nothing, TR> right) => new Either<TL, TR>(right._right);

        public static explicit operator TL(Either<TL, TR> either) =>
            either.IsLeft
                ? either._left
                : throw new InvalidCastException(ExceptionMessages.EitherIsNotLeft);

        public static explicit operator TR(Either<TL, TR> either) =>
            either.IsRight
                ? either._right
                : throw new InvalidCastException(ExceptionMessages.EitherIsNotRight);

        /// <summary>
        /// Deconstructs Either.
        /// </summary>
        /// <param name="isRight">Returns true if this is Right, otherwise false.</param>
        /// <param name="left">Returns value if this is Left. Otherwise default value of <typeparamref name="TL"/></param>
        /// <param name="right">Returns value if this is Right. Otherwise default value of <typeparamref name="TR"/></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Deconstruct(out bool isRight, out TL left, out TR right)
        {
            switch (State)
            {
                case EitherState.Right:
                    isRight = true;
                    left = default;
                    right = _right;
                    break;
                case EitherState.Left:
                    isRight = false;
                    left = _left;
                    right = default;
                    break;
                default:
                    throw new InvalidOperationException(ExceptionMessages.EitherIsBottom);
            }
        }
    }

    public readonly struct EitherLeftProjection<TL, TR> : IGettable<TL>, IToOption<TL>
    {
        private readonly Either<TL, TR> _either;

        internal EitherLeftProjection(Either<TL, TR> either)
        {
            _either = either;
        }

        /// <summary>
        /// Returns the value from this Left or throws InvalidOperationException if this is a Right.
        /// </summary>
        /// <returns></returns>
        public TL Get() => _either.IsLeft
            ? _either._left
            : throw new InvalidOperationException(ExceptionMessages.EitherIsNotLeft);

        /// <summary>
        /// Returns the value from this Left or the value of <paramref name="or"/> if this is a Right.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public TL GetOrElse(TL or) => _either.IsLeft ? _either._left : or;

        /// <summary>
        /// Returns the value from this Left or the result of <paramref name="or"/> if this is a Right.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public TL GetOrElse(Func<TL> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return _either.IsLeft ? _either._left : or();
        }

        /// <summary>
        /// Returns the value from this Left or the default value of <typeparamref name="TL"/> if this is a Right.
        /// </summary>
        /// <returns></returns>
        public TL GetOrDefault() => _either.IsLeft ? _either._left : default;

        /// <summary>
        /// Maps the given function through Left.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Either<T, TR> Map<T>(Func<TL, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsLeft ? (Either<T, TR>) map(_either._left) : _either._right;
        }

        /// <summary>
        /// Binds the given function across Left.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Either<T, TR> FlatMap<T>(Func<TL, Either<T, TR>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsLeft ? map(_either._left) : _either._right;
        }

        /// <summary>
        /// Returns None if this is a Right or if the given <paramref name="condition"/> does not hold for the left value,
        /// otherwise, returns a Some of Left.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Option<Either<TL, TR>> FilterToOption(Func<TL, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsLeft && condition(_either._left) ? _either : new Option<Either<TL, TR>>();
        }

        /// <summary>
        /// Returns None if this is a Right or if the given <paramref name="condition"/> does not hold for the left value,
        /// otherwise, returns a Some of Left.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Option<Either<TL, TR>> FilterToOption(bool condition) =>
            _either.IsLeft && condition ? _either : new Option<Either<TL, TR>>();

        /// <summary>
        /// Returns false if Right or returns the result of the application of the given function to the Left value.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Exists(Func<TL, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsLeft && condition(Get());
        }

        /// <summary>
        /// Returns a Some containing the Left value if it exists or a None if this is a Right.
        /// </summary>
        /// <returns></returns>
        public Option<TL> ToOption() => _either.IsLeft ? new Option<TL>(_either._left) : new Option<TL>();
    }

    public readonly struct EitherRightProjection<TL, TR> : IGettable<TR>, IToOption<TR>
    {
        private readonly Either<TL, TR> _either;

        internal EitherRightProjection(Either<TL, TR> either)
        {
            _either = either;
        }

        /// <summary>
        /// Returns the value from this Right or throws InvalidOperationException if this is a Left.
        /// </summary>
        /// <returns></returns>
        public TR Get() => _either.IsRight
            ? _either._right
            : throw new InvalidOperationException(ExceptionMessages.EitherIsNotRight);

        /// <summary>
        /// Returns the value from this Right or the value of <paramref name="or"/> if this is a Left.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public TR GetOrElse(TR or) => _either.IsRight ? _either._right : or;

        /// <summary>
        /// Returns the value from this Right or the result of <paramref name="or"/> if this is a Left.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public TR GetOrElse(Func<TR> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return _either.IsRight ? _either._right : or();
        }

        /// <summary>
        /// Returns the value from this Right or the default value of <typeparamref name="TR"/> if this is a Left.
        /// </summary>
        /// <returns></returns>
        public TR GetOrDefault() => _either.IsRight ? _either._right : default;

        /// <summary>
        /// Maps the given function through Right.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Either<TL, T> Map<T>(Func<TR, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsRight ? (Either<TL, T>) map(_either._right) : _either._left;
        }

        /// <summary>
        /// Binds the given function across Right.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Either<TL, T> FlatMap<T>(Func<TR, Either<TL, T>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsRight ? map(_either._right) : _either._left;
        }

        /// <summary>
        /// Returns None if this is a Left or if the given <paramref name="condition"/> does not hold for the right value,
        /// otherwise, returns a Some of Right.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Option<Either<TL, TR>> FilterToOption(Func<TR, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsRight && condition(_either._right) ? _either : new Option<Either<TL, TR>>();
        }

        /// <summary>
        /// Returns None if this is a Left or if the <paramref name="condition"/> is false,
        /// otherwise, returns a Some of Right.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Option<Either<TL, TR>> FilterToOption(bool condition) =>
            _either.IsRight && condition ? _either : new Option<Either<TL, TR>>();

        /// <summary>
        /// Returns false if Left or returns the result of the application of the given function to the Right value.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Exists(Func<TR, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsRight && condition(Get());
        }

        /// <summary>
        /// Returns a Some containing the Right value if it exists or a None if this is a Left.
        /// </summary>
        /// <returns></returns>
        public Option<TR> ToOption() => _either.IsRight ? new Option<TR>(_either._right) : new Option<TR>();
    }

    public static class Either
    {
        /// <summary>
        /// Joins an Either through Left. This method requires that the left side of this Either is itself an Either type.
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TL"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public static Either<TL, TR> JoinLeft<TL, TR>(this Either<Either<TL, TR>, TR> self) =>
            self.IsLeft ? self._left : new Either<TL, TR>(self._right);

        /// <summary>
        /// Joins an Either through Right. This method requires that the right side of this Either is itself an Either type.
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TL"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public static Either<TL, TR> JoinRight<TL, TR>(this Either<TL, Either<TL, TR>> self) =>
            self.IsRight ? self._right : new Either<TL, TR>(self._left);

        public static Either<TResult, TR> Select<TL, TR, TResult>(
            this EitherLeftProjection<TL, TR> self, Func<TL, TResult> map) => self.Map(map);

        public static Either<TL, TResult> Select<TL, TR, TResult>(
            this EitherRightProjection<TL, TR> self, Func<TR, TResult> map) => self.Map(map);
    }
}