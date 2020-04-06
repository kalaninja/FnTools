using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using FnTools.Exceptions;
using FnTools.Types.Interfaces;

namespace FnTools.Types
{
    public struct Try<T> : IGettable<T>, IToOption<T>, IToEither<T>, IEquatable<Try<T>>
    {
        [AllowNull] private readonly T _value;
        private readonly Exception? _exception;
        public bool IsSuccess { get; }

        public Try(T value)
        {
            _value = value;
            _exception = null;
            IsSuccess = true;
        }

        public Try(Exception exception)
        {
            _ = exception ?? throw new ArgumentNullException(nameof(exception));

            _value = default;
            _exception = exception;
            IsSuccess = false;
        }

        public T Get() => IsSuccess ? _value : throw Rethrow();

        public T GetOrElse(T or) => IsSuccess ? _value : or;

        public T GetOrElse(Func<T> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return IsSuccess ? _value : or();
        }

        public T GetOrDefault() => IsSuccess ? _value : default;

        public Try<TResult> Map<TResult>(Func<T, TResult> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            if (!IsSuccess)
                return _exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom);

            try
            {
                return map(_value);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public Try<TResult> FlatMap<TResult>(Func<T, Try<TResult>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            if (!IsSuccess)
                return _exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom);

            try
            {
                return map(_value);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public Try<T> Filter(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            try
            {
                return !IsSuccess || condition(_value) ? this : new NoSuchElementException();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public bool Exists(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsSuccess && condition(_value);
        }

        public TResult Fold<TResult>(Func<T, TResult> success, Func<Exception, TResult> failure)
        {
            _ = success ?? throw new ArgumentNullException(nameof(success));
            _ = failure ?? throw new ArgumentNullException(nameof(failure));

            if (!IsSuccess)
                return failure(_exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom));

            try
            {
                return success(_value);
            }
            catch (Exception e)
            {
                return failure(e);
            }
        }

        public void Match(Action<T> success, Action<Exception> failure)
        {
            _ = success ?? throw new ArgumentNullException(nameof(success));
            _ = failure ?? throw new ArgumentNullException(nameof(failure));

            if (IsSuccess)
            {
                try
                {
                    success(_value);
                }
                catch (Exception e)
                {
                    failure(e);
                }
            }
            else
            {
                failure(_exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom));
            }
        }

        public Try<T> Recover(Func<Exception, T> recover)
        {
            if (IsSuccess) return this;

            try
            {
                return recover(_exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public Try<T> Recover<TException>(Func<TException, T> recover) where TException : Exception
        {
            if (IsSuccess) return this;
            if (!(_exception is TException exception)) return this;

            try
            {
                return recover(exception);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public Try<T> RecoverWith(Func<Exception, Try<T>> recover)
        {
            if (IsSuccess) return this;

            try
            {
                return recover(_exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public Try<T> RecoverWith<TException>(Func<TException, Try<T>> recover) where TException : Exception
        {
            if (IsSuccess) return this;
            if (!(_exception is TException exception)) return this;

            try
            {
                return recover(exception);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public Exception Rethrow()
        {
            ExceptionDispatchInfo
                .Capture(_exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsNotFailure))
                .Throw();
            return _exception;
        }

        public override string ToString() =>
            IsSuccess
                ? $"Success({_value})"
                : $"Failure({_exception})";

        public static implicit operator Try<T>(T value) => new Try<T>(value);
        public static implicit operator Try<T>(Exception exception) => new Try<T>(exception);

        public static explicit operator T(Try<T> @try) =>
            @try.IsSuccess
                ? @try._value
                : throw new InvalidCastException(ExceptionMessages.TryIsNotSuccess);

        public static explicit operator Exception(Try<T> @try) =>
            @try.IsSuccess
                ? throw new InvalidCastException(ExceptionMessages.TryIsNotFailure)
                : @try._exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom);

        public static Try<T> operator &(Try<T> left, Try<T> right) => left ? right : left;

        public static Try<T> operator |(Try<T> left, Try<T> right) => left ? left : right;

        public static bool operator true(Try<T> @try) => @try.IsSuccess;

        public static bool operator false(Try<T> @try) => !@try.IsSuccess;

        public bool Equals(Try<T> other) =>
            IsSuccess == other.IsSuccess &&
            (IsSuccess
                ? EqualityComparer<T>.Default.Equals(_value, other._value)
                : Equals(_exception, other._exception));

        public Option<T> ToOption() => IsSuccess ? _value : Option<T>.None;

        public override bool Equals(object? obj) =>
            obj is Try<T> other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return IsSuccess
                    ? EqualityComparer<T>.Default.GetHashCode(_value)
                    : _exception?.GetHashCode() ?? 0;
            }
        }

        public static bool operator ==(Try<T> left, Try<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Try<T> left, Try<T> right)
        {
            return !left.Equals(right);
        }

        public static Try<T> Success(T value) => value;

        public static Try<T> Failure(Exception exception) => exception;

        public void Deconstruct(out bool isSuccess, out T value, out Exception? exception)
        {
            isSuccess = IsSuccess;
            exception = _exception;
            value = _value;
        }

        public Either<T, TR> ToLeft<TR>(TR right) =>
            IsSuccess ? (Either<T, TR>) _value : right;

        public Either<T, TR> ToLeft<TR>(Func<TR> right)
        {
            _ = right ?? throw new ArgumentNullException(nameof(right));

            return IsSuccess ? (Either<T, TR>) _value : right();
        }

        public Either<TL, T> ToRight<TL>(TL left) =>
            IsSuccess ? (Either<TL, T>) _value : left;

        public Either<TL, T> ToRight<TL>(Func<TL> left)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));

            return IsSuccess ? (Either<TL, T>) _value : left();
        }
    }

    public static class Try
    {
        public static Try<T> Flatten<T>(this Try<Try<T>> self) => self.FlatMap(Combinators.I);

        public static Try<TB> Select<TA, TB>(this Try<TA> self, Func<TA, TB> map) => self.Map(map);

        public static Try<TB> SelectMany<TA, TB>(this Try<TA> self, Func<TA, Try<TB>> map) =>
            self.FlatMap(map);

        public static Try<TC> SelectMany<TA, TB, TC>(
            this Try<TA> self, Func<TA, Try<TB>> map, Func<TA, TB, TC> project)
        {
            _ = project ?? throw new ArgumentNullException(nameof(project));

            return self.FlatMap(a => map(a).Map(b => project(a, b)));
        }

        public static Try<T> Where<T>(this Try<T> self, Func<T, bool> condition) => self.Filter(condition);
    }
}