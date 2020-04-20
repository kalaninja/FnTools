using System;
using System.Collections.Generic;
using FnTools.Exceptions;
using FnTools.Types.Interfaces;
using static FnTools.Prelude;

namespace FnTools.Types
{
    public readonly struct Result<TOk, TError> : IGettable<TOk>, IEquatable<Result<TOk, TError>>, IToOption<TOk>
    {
        private readonly TError _error;

        private readonly TOk _ok;

        public bool IsOk { get; }

        public bool IsError => !IsOk;

        public Option<TError> Error => IsError ? Some(_error) : None;

        public Option<TOk> Ok => IsOk ? Some(_ok) : None;

        private Result(bool isOk, TOk ok, TError error)
        {
            IsOk = isOk;
            _ok = ok;
            _error = error;
        }

        public Result(TOk ok)
        {
            _ok = ok;
            _error = default;
            IsOk = true;
        }

        public Result(TError error)
        {
            _error = error;
            _ok = default;
            IsOk = false;
        }

        public bool Exists(Func<TOk, bool> condition)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsOk && condition(_ok);
        }

        public Result<T, TError> Map<T>(Func<TOk, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsOk ? new Result<T, TError>(map(_ok)) : new Result<T, TError>(_error);
        }

        public Result<TTOk, TTError> BiMap<TTOk, TTError>(Func<TOk, TTOk> map, Func<TError, TTError> errorMap)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));
            _ = errorMap ?? throw new ArgumentNullException(nameof(errorMap));

            return IsOk ? Ok<TTOk, TTError>(map(_ok)) : Error<TTOk, TTError>(errorMap(_error));
        }

        public Result<TOk, T> ErrorMap<T>(Func<TError, T> map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            return IsOk ? new Result<TOk, T>(_ok) : new Result<TOk, T>(map(_error));
        }

        public Result<T, TError> FlatMap<T>(Func<TOk, Result<T, TError>> flatMap)
        {
            _ = flatMap ?? throw new ArgumentNullException(nameof(flatMap));

            return IsOk ? flatMap(_ok) : new Result<T, TError>(_error);
        }

        public Result<TOk, TError> FlatTap<T>(Func<TOk, Result<T, TError>> flatTap)
        {
            _ = flatTap ?? throw new ArgumentNullException(nameof(flatTap));

            return FlatMap(x => flatTap(x).Map(_ => x));
        }

        public Result<TOk, TError> Recover(Func<TError, TOk> recover)
        {
            _ = recover ?? throw new ArgumentNullException(nameof(recover));

            return IsOk ? this : new Result<TOk, TError>(recover(_error));
        }

        public Result<TOk, TError> RecoverWith(Func<TError, Result<TOk, TError>> recover)
        {
            _ = recover ?? throw new ArgumentNullException(nameof(recover));

            return IsOk ? this : recover(_error);
        }

        public TOk Get() => IsOk ? _ok : throw new InvalidOperationException(ExceptionMessages.ResultIsError);

        public TOk GetOrElse(TOk or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return IsOk ? _ok : or;
        }

        public TOk GetOrElse(Func<TOk> or)
        {
            _ = or ?? throw new ArgumentNullException(nameof(or));

            return IsOk ? _ok : or();
        }

        public TOk GetOrDefault() => IsOk ? _ok : default;

        public void Deconstruct(out bool isOk, out TOk ok, out TError error)
        {
            isOk = IsOk;
            ok = _ok;
            error = _error;
        }

        public void Match(Action<TOk> ok, Action<TError> error)
        {
            _ = ok ?? throw new ArgumentNullException(nameof(ok));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            if (IsOk)
                ok(_ok);
            else
                error(_error);
        }

        public TResult Fold<TResult>(Func<TOk, TResult> ok, Func<TError, TResult> error)
        {
            _ = ok ?? throw new ArgumentNullException(nameof(ok));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return IsOk ? ok(_ok) : error(_error);
        }

        public Result<TOk, TError> Filter(Func<TOk, bool> condition, TError error = default)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));

            return IsOk && condition(_ok) ? this : Error(IsOk ? error : _error);
        }

        public Result<TOk, TError> Filter(Func<TOk, bool> condition, Func<TError> error)
        {
            _ = condition ?? throw new ArgumentNullException(nameof(condition));
            _ = error ?? throw new ArgumentNullException(nameof(error));

            return IsOk && condition(_ok) ? this : Error(IsOk ? error() : _error);
        }

        public Result<TOk, TError> Filter(bool condition, TError error = default) =>
            IsOk && condition ? this : Error(IsOk ? error : _error);

        public override string ToString() =>
            IsOk
                ? $"Ok({_ok})"
                : $"Error({_error})";

        public bool Equals(Result<TOk, TError> other)
        {
            return EqualityComparer<TError>.Default.Equals(_error, other._error)
                   && EqualityComparer<TOk>.Default.Equals(_ok, other._ok) && IsOk == other.IsOk;
        }

        public Option<TOk> ToOption() => IsOk ? Some(_ok) : None;

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

        public static implicit operator Result<TOk, TError>(Result<TOk, Nothing> result) => CreateOk(result._ok);

        public static implicit operator Result<TOk, TError>(Result<Nothing, TError> result) =>
            CreateError(result._error);

        public static explicit operator TOk(Result<TOk, TError> result) =>
            result.IsOk
                ? result._ok
                : throw new InvalidCastException(ExceptionMessages.ResultIsError);

        public static explicit operator TError(Result<TOk, TError> result) =>
            result.IsError
                ? result._error
                : throw new InvalidCastException(ExceptionMessages.ResultIsOk);


        internal static Result<TOk, TError> CreateOk(TOk ok) => new Result<TOk, TError>(ok);
        internal static Result<TOk, TError> CreateError(TError error) => new Result<TOk, TError>(error);
    }


    public static class Result
    {
        public static Result<TOk, TError> Flatten<TOk, TError>(this Result<Result<TOk, TError>, TError> self) =>
            self.FlatMap(Combinators.I);

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