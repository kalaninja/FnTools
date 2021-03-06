using System;
using System.Collections.Generic;
using FnTools.Exceptions;
using FnTools.Types.Interfaces;
using static FnTools.Prelude;

namespace FnTools.Types
{
    /// <summary>
    /// The Result type represents Ok value of <typeparamref name="TOk"/>  or Error value of <typeparamref name="TError"/>.
    /// An instance of Result is either Ok or Error.
    /// </summary>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public readonly struct Result<TOk, TError>
        : IEquatable<Result<TOk, TError>>, IGettable<TOk>, IToOption<TOk>, IToEither<TOk>
    {
        internal readonly TError _error;

        internal readonly TOk _ok;

        /// <summary>
        /// Returns true if this is Ok, false otherwise.
        /// </summary>
        public bool IsOk { get; }

        /// <summary>
        /// Returns true if this is Error, false otherwise.
        /// </summary>
        public bool IsError => !IsOk;

        /// <summary>
        /// Returns Some if this is Error, None otherwise.
        /// </summary>
        public Option<TError> Error => IsError ? Some(_error) : None;

        /// <summary>
        /// Instantiates Result with the value of <typeparamref name="TOk"/>
        /// </summary>
        /// <param name="ok"></param>
        public Result(TOk ok)
        {
            _ok = ok;
            _error = default;
            IsOk = true;
        }

        /// <summary>
        /// Instantiates Result with the value of <typeparamref name="TError"/>
        /// </summary>
        /// <param name="error"></param>
        public Result(TError error)
        {
            _error = error;
            _ok = default;
            IsOk = false;
        }

        /// <summary>
        /// Returns false if Error or returns the result of the application
        /// of the given <paramref name="condition"/> to the Ok value.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Exists(Func<TOk, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsOk && condition(_ok);
        }

        /// <summary>
        /// Applies a function on the Ok value.
        /// Returns an Ok containing the result of applying <paramref name="map"/> to this Ok value
        /// if this Result is Ok. Otherwise, return Error.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Result<T, TError> Map<T>(Func<TOk, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsOk ? new Result<T, TError>(map(_ok)) : new Result<T, TError>(_error);
        }

        /// <summary>
        /// Applies a function to the Ok or Error
        /// to the Error value. Returns Result containing the result of applying <paramref name="ok"/> to the Ok value
        /// or <paramref name="error"/> to the Error value
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="error"></param>
        /// <typeparam name="TTOk"></typeparam>
        /// <typeparam name="TTError"></typeparam>
        /// <returns></returns>
        public Result<TTOk, TTError> BiMap<TTOk, TTError>(Func<TOk, TTOk> ok, Func<TError, TTError> error)
        {
            _ = ok ?? throw new ArgumentNullException(nameof(ok));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return IsOk ? Ok<TTOk, TTError>(ok(_ok)) : Error<TTOk, TTError>(error(_error));
        }

        /// <summary>
        /// Applies a function to the Error value.
        /// Returns an Error containing the result of applying <paramref name="map"/> to this Errors value
        /// if this Result is Ok. Otherwise, return Error.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Result<TOk, T> ErrorMap<T>(Func<TError, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsOk ? new Result<TOk, T>(_ok) : new Result<TOk, T>(map(_error));
        }

        /// <summary>
        /// Returns the Result of applying <paramref name="map"/> to Result's Ok value,
        /// otherwise, returns Error.
        /// Slightly different from Map() in that <paramref name="map"/>
        /// is expected to return a Result (which could be Error).
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Result<T, TError> FlatMap<T>(Func<TOk, Result<T, TError>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsOk ? map(_ok) : new Result<T, TError>(_error);
        }

        /// <summary>
        /// Applies <paramref name="map"/> to Result's Ok value discarding the result of applying and
        /// returns original Result if result of applying is Ok value. Otherwise, returns Error.
        /// </summary>
        /// <param name="map"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Result<TOk, TError> FlatTap<T>(Func<TOk, Result<T, TError>> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return FlatMap(x => map(x).Map(_ => x));
        }

        /// <summary>
        /// Applies the given function <paramref name="recover"/> if this is an Error,
        /// otherwise, returns this if this is an Ok.
        /// </summary>
        /// <param name="recover"></param>
        /// <returns></returns>
        public Result<TOk, TError> Recover(Func<TError, TOk> recover)
        {
            _ = recover ?? throw new ArgumentNullException(nameof(recover));

            return IsOk ? this : new Result<TOk, TError>(recover(_error));
        }

        /// <summary>
        /// Applies the given function <paramref name="recover"/> if this is Error, otherwise returns this if this is an Ok.
        /// This is like flatMap for the Error.
        /// </summary>
        /// <param name="recover"></param>
        /// <typeparam name="TTError"></typeparam>
        /// <returns></returns>
        public Result<TOk, TTError> RecoverWith<TTError>(Func<TError, Result<TOk, TTError>> recover)
        {
            _ = recover ?? throw new ArgumentNullException(nameof(recover));

            return IsOk ? Ok(_ok) : recover(_error);
        }

        /// <summary>
        /// Returns the value from this Ok or throws the exception if this is an Error.
        /// </summary>
        /// <returns></returns>
        public TOk Get() =>
            IsOk ? _ok : throw new InvalidOperationException(ExceptionMessages.ResultIsError);

        /// <summary>
        /// Returns the value from this Ok or the given value of <paramref name="or"/> if this is an Error.
        /// </summary>
        /// <returns></returns>
        public TOk GetOrElse(TOk or) => IsOk ? _ok : or;

        /// <summary>
        /// Returns the value from this Ok, or the result of evaluating <paramref name="or"/> if this is an Error.
        /// </summary>
        /// <param name="or"></param>
        /// <returns></returns>
        public TOk GetOrElse(Func<TOk> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return IsOk ? _ok : or();
        }

        /// <summary>
        /// Returns the value from this Ok, or default value of Ok type.
        /// </summary>
        /// <returns></returns>
        public TOk GetOrDefault() => IsOk ? _ok : default;

        /// <summary>
        /// Deconstruct Result
        /// </summary>
        /// <param name="isOk"></param>
        /// <param name="ok"></param>
        /// <param name="error"></param>
        public void Deconstruct(out bool isOk, out TOk ok, out TError error)
        {
            isOk = IsOk;
            ok = _ok;
            error = _error;
        }

        /// <summary>
        /// Executes <paramref name="ok"/> if this is an Ok  or <paramref name="error"/> if this is an Error.
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="error"></param>
        public void Match(Action<TOk> ok, Action<TError> error)
        {
            _ = ok ?? throw new ArgumentNullException(nameof(ok));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            if (IsOk)
                ok(_ok);
            else
                error(_error);
        }

        /// <summary>
        /// Returns the result of applying <paramref name="ok"/> to this Result's value if the Result is Ok.
        /// Otherwise, returns result of applying <paramref name="error"/> to the Error.
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="error"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult Fold<TResult>(Func<TOk, TResult> ok, Func<TError, TResult> error)
        {
            _ = ok ?? throw new ArgumentNullException(nameof(ok));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return IsOk ? ok(_ok) : error(_error);
        }

        /// <summary>
        /// Returns an Ok if applying the predicate <paramref name="condition"/> to this Result's Ok value returns true.
        /// Otherwise, return Error containing the given <paramref name="error"/>.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Result<TOk, TError> Filter(Func<TOk, bool> condition, TError error = default)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsError || condition(_ok) ? this : new Result<TOk, TError>(error);
        }

        /// <summary>
        /// Returns this Result if applying the predicate <paramref name="condition"/> to this Result's Ok value returns true.
        /// Otherwise, return Error containing the result of applying <paramref name="error"/>.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Result<TOk, TError> Filter(Func<TOk, bool> condition, Func<TError> error)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return IsError || condition(_ok) ? this : new Result<TOk, TError>(error());
        }

        /// <summary>
        /// Returns this Result if <paramref name="condition"/> is true and Result state is Ok.
        /// Otherwise, return Error containing the given <paramref name="error"/>.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Result<TOk, TError> Filter(bool condition, TError error = default) =>
            IsError || condition ? this : new Result<TOk, TError>(error);

        /// <summary>
        /// Returns this Result if <paramref name="condition"/> is true and Result state is Ok.
        /// Otherwise, return Error containing the result of applying <paramref name="error"/>.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Result<TOk, TError> Filter(bool condition, Func<TError> error)
        {
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return IsError || condition ? this : new Result<TOk, TError>(error());
        }

        public override string ToString() =>
            IsOk
                ? $"Ok({_ok})"
                : $"Error({_error})";

        public bool Equals(Result<TOk, TError> other)
        {
            return EqualityComparer<TError>.Default.Equals(_error, other._error)
                   && EqualityComparer<TOk>.Default.Equals(_ok, other._ok) && IsOk == other.IsOk;
        }

        /// <summary>
        /// Returns a Some containing the Ok value if it exists, or a None if this is an Error.
        /// </summary>
        /// <returns></returns>
        public Option<TOk> ToOption() => IsOk ? Some(_ok) : None;

        /// <summary>
        /// Returns a Right containing the given argument <paramref name="right"/> if this is Error,
        /// or a Left containing the value if this is Ok.
        /// </summary>
        /// <param name="right"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public Either<TOk, TR> ToLeft<TR>(TR right) => IsOk ? Left<TOk, TR>(_ok) : Right<TOk, TR>(right);

        /// <summary>
        /// Returns a Right containing the result of <paramref name="right"/> if this is Error,
        /// or a Left containing the value if this is Ok.
        /// </summary>
        /// <param name="right"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public Either<TOk, TR> ToLeft<TR>(Func<TR> right)
        {
            _ = right ?? throw new ArgumentNullException(nameof(right));

            return IsOk ? Left<TOk, TR>(_ok) : Right<TOk, TR>(right());
        }

        /// <summary>
        /// Returns a Left containing the given argument <paramref name="left"/> if this is Error,
        /// or a Right containing the value if this is Ok.
        /// </summary>
        /// <param name="left"></param>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public Either<TL, TOk> ToRight<TL>(TL left) => IsOk ? Right<TL, TOk>(_ok) : Left<TL, TOk>(left);

        /// <summary>
        /// Returns a Left containing the result of <paramref name="left"/> if this is Error,
        /// or a Right containing the value if this is Ok.
        /// </summary>
        /// <param name="left"></param>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public Either<TL, TOk> ToRight<TL>(Func<TL> left)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));

            return IsOk ? Right<TL, TOk>(_ok) : Left<TL, TOk>(left());
        }

        public override bool Equals(object obj)
        {
            return obj is Result<TOk, TError> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<TError>.Default.GetHashCode(_error);
                hashCode = (hashCode * 397) ^ EqualityComparer<TOk>.Default.GetHashCode(_ok);
                hashCode = (hashCode * 397) ^ IsOk.GetHashCode();
                return hashCode;
            }
        }

        public static Result<TOk, TError> operator &(Result<TOk, TError> left, Result<TOk, TError> right) =>
            left ? right : left;

        public static Result<TOk, TError> operator |(Result<TOk, TError> left, Result<TOk, TError> right) =>
            left ? left : right;

        public static bool operator true(Result<TOk, TError> result) => result.IsOk;

        public static bool operator false(Result<TOk, TError> result) => result.IsError;

        public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);

        public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);


        public static implicit operator Result<TOk, TError>(TOk ok) => new Result<TOk, TError>(ok);

        public static implicit operator Result<TOk, TError>(TError error) => new Result<TOk, TError>(error);

        public static implicit operator Result<TOk, TError>(Result<TOk, Nothing> result) =>
            new Result<TOk, TError>(result._ok);

        public static implicit operator Result<TOk, TError>(Result<Nothing, TError> result) =>
            new Result<TOk, TError>(result._error);

        public static explicit operator TOk(Result<TOk, TError> result) =>
            result.IsOk
                ? result._ok
                : throw new InvalidCastException(ExceptionMessages.ResultIsError);

        public static explicit operator TError(Result<TOk, TError> result) =>
            result.IsError
                ? result._error
                : throw new InvalidCastException(ExceptionMessages.ResultIsOk);
    }


    public static class Result
    {
        /// <summary>
        /// Collapse nested Results into single Result
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TOk"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <returns></returns>
        public static Result<TOk, TError> Flatten<TOk, TError>(this Result<Result<TOk, TError>, TError> self) =>
            self.FlatMap(Combinators.I);

        /// <summary>
        /// Returns an Ok if applying the predicate <paramref name="condition"/> to this Result's Ok value returns true.
        /// Otherwise, return Error containing the given <paramref name="error"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TOk, TError> Filter<TOk, TError>(
            this Result<TOk, Nothing> result, Func<TOk, bool> condition, TError error = default)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return condition(result._ok) ? new Result<TOk, TError>(result._ok) : new Result<TOk, TError>(error);
        }

        /// <summary>
        /// Returns this Result if applying the predicate <paramref name="condition"/> to this Result's Ok value returns true.
        /// Otherwise, return Error containing the result of applying <paramref name="error"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TOk, TError> Filter<TOk, TError>(
            this Result<TOk, Nothing> result, Func<TOk, bool> condition, Func<TError> error)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return condition(result._ok) ? new Result<TOk, TError>(result._ok) : new Result<TOk, TError>(error());
        }

        /// <summary>
        /// Returns this Result if <paramref name="condition"/> is true and Result state is Ok.
        /// Otherwise, return Error containing the given <paramref name="error"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TOk, TError> Filter<TOk, TError>(
            this Result<TOk, Nothing> result, bool condition, TError error = default) =>
            condition ? new Result<TOk, TError>(result._ok) : new Result<TOk, TError>(error);

        /// <summary>
        /// Returns this Result if <paramref name="condition"/> is true and Result state is Ok.
        /// Otherwise, return Error containing the result of applying <paramref name="error"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="condition"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TOk, TError> Filter<TOk, TError>(
            this Result<TOk, Nothing> result, bool condition, Func<TError> error)
        {
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return condition ? new Result<TOk, TError>(result._ok) : new Result<TOk, TError>(error());
        }

        public static Result<TTOk, TError> Select<TOk, TError, TTOk>(this Result<TOk, TError> self, Func<TOk, TTOk> map)
            => self.Map(map);

        public static Result<TB, TError> SelectMany<TA, TError, TB>(this Result<TA, TError> self,
            Func<TA, Result<TB, TError>> map) => self.FlatMap(map);

        public static Result<TC, TError> SelectMany<TA, TError, TB, TC>(this Result<TA, TError> self,
            Func<TA, Result<TB, TError>> map, Func<TA, TB, TC> project)
        {
            _ = project ?? throw new ArgumentNullException(nameof(project));

            return self.FlatMap(a => map(a).Map(b => project(a, b)));
        }

        public static Result<TOk, TError> Where<TOk, TError>(this Result<TOk, TError> self, Func<TOk, bool> condition)
            => self.Filter(condition);
    }
}