using System;
using System.Collections.Generic;
using FnTools.Exceptions;
using FnTools.Types.Interfaces;

namespace FnTools.Types
{
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

        public bool IsLeft => State == EitherState.Left;

        public bool IsRight => State == EitherState.Right;

        public Either(TL left)
        {
            _left = left;
            State = EitherState.Left;

            _right = default;
        }

        public Either(TR right)
        {
            _right = right;
            State = EitherState.Right;

            _left = default;
        }

        public EitherLeftProjection<TL, TR> Left => new EitherLeftProjection<TL, TR>(this);

        public EitherRightProjection<TL, TR> Right => new EitherRightProjection<TL, TR>(this);

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

        public EitherLeftProjection(Either<TL, TR> either)
        {
            _either = either;
        }

        public TL Get() => _either.IsLeft
            ? _either._left
            : throw new InvalidOperationException(ExceptionMessages.EitherIsNotLeft);

        public TL GetOrElse(TL or) => _either.IsLeft ? _either._left : or;

        public TL GetOrElse(Func<TL> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return _either.IsLeft ? _either._left : or();
        }

        public TL GetOrDefault() => _either.IsLeft ? _either._left : default;

        public Either<T, TR> Map<T>(Func<TL, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsLeft ? (Either<T, TR>) map(_either._left) : _either._right;
        }

        public Either<T, TR> FlatMap<T>(Func<TL, Either<T, TR>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsLeft ? map(_either._left) : _either._right;
        }

        public Option<Either<TL, TR>> Filter(Func<TL, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsLeft && condition(_either._left) ? _either : new Option<Either<TL, TR>>();
        }

        public bool Exists(Func<TL, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsLeft && condition(Get());
        }

        public Option<TL> ToOption() => _either.IsLeft ? new Option<TL>(_either._left) : new Option<TL>();
    }

    public readonly struct EitherRightProjection<TL, TR> : IGettable<TR>, IToOption<TR>
    {
        private readonly Either<TL, TR> _either;

        public EitherRightProjection(Either<TL, TR> either)
        {
            _either = either;
        }

        public TR Get() => _either.IsRight
            ? _either._right
            : throw new InvalidOperationException(ExceptionMessages.EitherIsNotRight);

        public TR GetOrElse(TR or) => _either.IsRight ? _either._right : or;

        public TR GetOrElse(Func<TR> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return _either.IsRight ? _either._right : or();
        }

        public TR GetOrDefault() => _either.IsRight ? _either._right : default;

        public Either<TL, T> Map<T>(Func<TR, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsRight ? (Either<TL, T>) map(_either._right) : _either._left;
        }

        public Either<TL, T> FlatMap<T>(Func<TR, Either<TL, T>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return _either.IsRight ? map(_either._right) : _either._left;
        }

        public Option<Either<TL, TR>> Filter(Func<TR, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsRight && condition(_either._right) ? _either : new Option<Either<TL, TR>>();
        }

        public bool Exists(Func<TR, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return _either.IsRight && condition(Get());
        }

        public Option<TR> ToOption() => _either.IsRight ? new Option<TR>(_either._right) : new Option<TR>();
    }

    public static class Either
    {
        public static Either<TL, TR> JoinLeft<TL, TR>(this Either<Either<TL, TR>, TR> self) =>
            self.IsLeft ? self._left : new Either<TL, TR>(self._right);

        public static Either<TL, TR> JoinRight<TL, TR>(this Either<TL, Either<TL, TR>> self) =>
            self.IsRight ? self._right : new Either<TL, TR>(self._left);

        public static Either<TResult, TR> Select<TL, TR, TResult>(
            this EitherLeftProjection<TL, TR> self, Func<TL, TResult> map) => self.Map(map);

        public static Either<TL, TResult> Select<TL, TR, TResult>(
            this EitherRightProjection<TL, TR> self, Func<TR, TResult> map) => self.Map(map);
    }
}