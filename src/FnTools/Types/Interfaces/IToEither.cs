using System;

namespace FnTools.Types.Interfaces
{
    public interface IToEither<T>
    {
        Either<T, TR> ToLeft<TR>(TR right);

        Either<T, TR> ToLeft<TR>(Func<TR> right);

        Either<TL, T> ToRight<TL>(TL left);

        Either<TL, T> ToRight<TL>(Func<TL> left);
    }
}