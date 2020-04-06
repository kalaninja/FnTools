using System;

namespace FnTools.Types.Interfaces
{
    public interface IGettable<T>
    {
        T Get();

        T GetOrElse(T or);

        T GetOrElse(Func<T> or);

        T GetOrDefault();
    }
}