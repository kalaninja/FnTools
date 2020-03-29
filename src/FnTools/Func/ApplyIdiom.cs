using System;

namespace FnTools.Func
{
    public delegate void ActionRef<T>(ref T obj);

    public static class ApplyIdiom
    {
        public static T Apply<T>(this T self, Action<T> a)
        {
            a(self);
            return self;
        }

        public static T Apply<T>(this T self, ActionRef<T> a)
        {
            a(ref self);
            return self;
        }

        public static TResult Apply<T, TResult>(this T self, Func<T, TResult> f) =>
            f(self);
    }
}