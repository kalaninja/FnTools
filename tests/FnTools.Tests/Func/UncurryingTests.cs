using System;
using FnTools.Func;
using Shouldly;
using Xunit;

namespace FnTools.Tests.Func
{
    public class UncurryingTests
    {
        [Fact]
        public void TestUncurry2Args()
        {
            Func<int, Func<int, int>> f = x => y => x + y;

            f.Uncurry()(5, -6).ShouldBe(-1);
        }

        [Fact]
        public void UncurryCurriedFunc()
        {
            Func<int, int, int> f = Math.Min;
            var g = f.Curry().Uncurry();

            g(4, 5).ShouldBe(4);
        }
    }
}