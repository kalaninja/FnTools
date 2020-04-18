using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using FnTools.Exceptions;
using FnTools.Types.Interfaces;

namespace FnTools.Types
{
    /// <summary>
    /// The Try type represents a computation that may either result in an exception, or return a successfully computed value.
    /// Instances of Try are either Success or Failure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Try<T> : IGettable<T>, IToOption<T>, IToEither<T>, IEquatable<Try<T>>
    {
        private readonly T _value;
        private readonly Exception _exception;

        /// <summary>
        /// Returns true if the Try is a Success, false otherwise.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Returns true if the Try is a Failure, false otherwise.
        /// </summary>
        public bool IsFailure => !(IsSuccess || _exception is null);

        /// <summary>
        /// Instantiates Success containing a successfully computed value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        public Try(T value)
        {
            _value = value;
            _exception = null;
            IsSuccess = true;
        }

        /// <summary>
        /// Instantiates Failure containing an exception.
        /// </summary>
        /// <param name="exception"></param>
        public Try(Exception exception)
        {
            _ = exception ?? throw new ArgumentNullException(nameof(exception));

            _value = default;
            _exception = exception;
            IsSuccess = false;
        }

        /// <summary>
        /// Returns the value from this Success or throws the exception if this is a Failure.
        /// </summary>
        /// <returns></returns>
        public T Get() => IsSuccess ? _value : throw Rethrow();

        /// <summary>
        /// Returns the value from this Success or the given value of <paramref name="or"/> if this is a Failure.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public T GetOrElse(T or) => IsSuccess ? _value : or;

        /// <summary>
        /// Returns the value from this Success or the the result of evaluating <paramref name="or"/> if this is a Failure.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public T GetOrElse(Func<T> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return IsSuccess ? _value : or();
        }

        /// <summary>
        /// Returns the value from this Success or a default value of <typeparamref name="T"/> if this is a Failure.
        /// </summary>
        /// <returns></returns>
        public T GetOrDefault() => IsSuccess ? _value : default;

        /// <summary>
        /// Maps the given function to the value from this Success or returns this if this is a Failure.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the result of the given function applied to the value from this Success
        /// or returns this if this is a Failure.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Applies the given function to the value from this Success, discards the result, but keeps the effect
        /// or returns this if this is a Failure.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public Try<T> FlatTap<TResult>(Func<T, Try<TResult>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            if (!IsSuccess)
                return _exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom);

            try
            {
                return FlatMap(x => map(x).Map(_ => x));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Converts this to a Failure if the predicate <paramref name="condition"/> is not satisfied.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Try<T> Filter(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            try
            {
                return IsSuccess && condition(_value) ? this : new NoSuchElementException();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Converts this to a Failure if <paramref name="condition"/> is false.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Try<T> Filter(bool condition) =>
            IsSuccess && condition ? this : new NoSuchElementException();

        /// <summary>
        /// Returns true if this is Success and the predicate <paramref name="condition"/> returns true
        /// when applied to this Success's value. Otherwise, returns false.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Exists(Func<T, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsSuccess && condition(_value);
        }

        /// <summary>
        /// Applies <paramref name="success"/> if this is a Success or <paramref name="failure"/> if this is a Failure.
        /// If <paramref name="success"/> is initially applied and throws an exception,
        /// then <paramref name="failure"/> is applied with this exception.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Executes <paramref name="success"/> if this is a Success or <paramref name="failure"/> if this is a Failure.
        /// If <paramref name="success"/> is initially executed and throws an exception,
        /// then <paramref name="failure"/> is executed with this exception.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
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

        /// <summary>
        /// Applies the given function <paramref name="recover"/> if this is a Failure, otherwise returns this if this is a Success.
        /// </summary>
        /// <param name="recover"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Applies the given function <paramref name="recover"/> if this is a Failure containing an exception of <typeparamref name="TException"/>.
        /// Otherwise returns this if this is a Success or a Failure containing an exception of different type.
        /// </summary>
        /// <param name="recover"></param>
        /// <typeparam name="TException"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Applies the given function <paramref name="recover"/> if this is a Failure, otherwise returns this if this is a Success.
        /// This is like flatMap for the exception.
        /// </summary>
        /// <param name="recover"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Applies the given function <paramref name="recover"/> if this is a Failure containing an exception of <typeparamref name="TException"/>.
        /// Otherwise returns this if this is a Success or a Failure containing an exception of different type.
        /// This is like flatMap for the exception.
        /// </summary>
        /// <param name="recover"></param>
        /// <typeparam name="TException"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Throws contained exception if this is a Failure. Otherwise, throws InvalidOperationException.
        /// </summary>
        /// <returns></returns>
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

        public override bool Equals(object obj) =>
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

        /// <summary>
        /// Instantiates Success containing a successfully computed value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Try<T> Success(T value) => value;

        /// <summary>
        /// Instantiates Failure containing an exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Try<T> Failure(Exception exception) => exception;

        /// <summary>
        /// Deconstructs Try.
        /// </summary>
        /// <param name="isSuccess">Returns true if this Try is Success. Otherwise, false.</param>
        /// <param name="value">Returns value if this is Success. Otherwise, default value of <typeparamref name="T"/></param>
        /// <param name="exception">Returns exception if this is Failure. Otherwise, null.</param>
        public void Deconstruct(out bool isSuccess, out T value, out Exception exception)
        {
            isSuccess = IsSuccess;
            value = _value;
            exception = isSuccess
                ? default
                : _exception ?? throw new InvalidOperationException(ExceptionMessages.TryIsBottom);
        }

        /// <summary>
        /// Returns a Right containing the given argument <paramref name="right"/> if this is Failure,
        /// or a Left containing the value if this is Success.
        /// </summary>
        /// <param name="right"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public Either<T, TR> ToLeft<TR>(TR right) => IsSuccess ? (Either<T, TR>) _value : right;

        /// <summary>
        /// Returns a Right containing the result of <paramref name="right"/> if this is Failure,
        /// or a Left containing the value if this is Success.
        /// </summary>
        /// <param name="right"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public Either<T, TR> ToLeft<TR>(Func<TR> right)
        {
            _ = right ?? throw new ArgumentNullException(nameof(right));

            return IsSuccess ? (Either<T, TR>) _value : right();
        }

        /// <summary>
        /// Returns a Left containing the given argument <paramref name="left"/> if this is Failure,
        /// or a Right containing the value if this is Success.
        /// </summary>
        /// <param name="left"></param>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public Either<TL, T> ToRight<TL>(TL left) => IsSuccess ? (Either<TL, T>) _value : left;

        /// <summary>
        /// Returns a Left containing the result of <paramref name="left"/> if this is Failure,
        /// or a Right containing the value if this is Success.
        /// </summary>
        /// <param name="left"></param>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public Either<TL, T> ToRight<TL>(Func<TL> left)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));

            return IsSuccess ? (Either<TL, T>) _value : left();
        }
    }

    public static class Try
    {
        /// <summary>
        /// Transforms a nested Try into an un-nested Try.
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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