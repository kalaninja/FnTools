using System;
using System.Threading.Tasks;
using FnTools.Types;

namespace FnTools
{
    public static class Prelude
    {
        public static Nothing __ => new Nothing();

        public static Option<T> Some<T>(T value) => new Option<T>(value);

        public static Option<Nothing> None => new Option<Nothing>();

        public static Either<TL, Nothing> Left<TL>(TL left) => left;

        public static Either<Nothing, TR> Right<TR>(TR right) => right;

        public static Result<TOk, Nothing> Ok<TOk>(TOk ok) => ok;

        public static Result<TOk, TError> Ok<TOk, TError>(TOk ok) => Result<TOk, TError>.CreateOk(ok);

        public static Result<Nothing, TError> Error<TError>(TError error) => error;
        
        public static Result<TOk, TError> Error<TOk, TError>(TError error) => Result<TOk, TError>.CreateError(error);

        public static Try<T> Try<T>(Func<T> f)
        {
            try
            {
                return new Try<T>(f());
            }
            catch (Exception e)
            {
                return new Try<T>(e);
            }
        }

        public static async Task<Try<T>> Try<T>(Func<Task<T>> f)
        {
            try
            {
                return new Try<T>(await f().ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return new Try<T>(e);
            }
        }

        public static Func<T, string> ToString<T>(bool @throw = false) =>
            self => self?.ToString() ?? (@throw ? throw new ArgumentNullException(nameof(self)) : "null");

        public static void Run(Action a) => a();
        public static T Run<T>(Func<T> f) => f();

        // @formatter:off
        public static Func<TResult> Def<TResult>(Func<TResult> f) => f;
        public static Func<T1, TResult> Def<T1, TResult>(Func<T1, TResult> f) => f;
        public static Func<T1, T2, TResult> Def<T1, T2, TResult>(Func<T1, T2, TResult> f) => f;
        public static Func<T1, T2, T3, TResult> Def<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> f) => f;
        public static Func<T1, T2, T3, T4, TResult> Def<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, TResult> Def<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, TResult> Def<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Def<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Def<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Def<T1, T2, T3, T4, T5, T6, T7, T8, T9,TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Def<T1, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Def<T1, T2, T3, T4, T5, T6, T7, T8,T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Def<T1, T2, T3, T4, T5, T6, T7,T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> f) =>f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Def<T1, T2, T3, T4, T5, T6,T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Def<T1, T2, T3, T4,T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Def<T1, T2, T3,T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Def<T1, T2,T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> f) => f;
        // @formatter:on

        // @formatter:off
        public static Action Def(Action a) => a;
        public static Action<T1> Def<T1>(Action<T1> a) => a;
        public static Action<T1, T2> Def<T1, T2>(Action<T1, T2> a) => a;
        public static Action<T1, T2, T3> Def<T1, T2, T3>(Action<T1, T2, T3> a) => a;
        public static Action<T1, T2, T3, T4> Def<T1, T2, T3, T4>(Action<T1, T2, T3, T4> a) => a;
        public static Action<T1, T2, T3, T4, T5> Def<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6> Def<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7> Def<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Def<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Def<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Def<T1, T2, T3, T4, T5, T6, T7, T8, T9,T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Def<T1, T2, T3, T4, T5, T6, T7, T8,T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Def<T1, T2, T3, T4, T5, T6, T7,T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> a) =>a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Def<T1, T2, T3, T4, T5, T6,T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Def<T1, T2, T3, T4,T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Def<T1, T2, T3,T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> a) => a;
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Def<T1, T2,T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> a) => a;
        // @formatter:on
    }
}