using System;
using System.Linq;
using FnTools.Func;
using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Tests.Func
{
    public class CurryingTests
    {
        [Fact]
        public void TestCurry16Args()
        {
            dynamic min = Def((int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8, int n9, int n10,
                    int n11, int n12, int n13, int n14, int n15, int n16) =>
                n1.Apply(x => Math.Min(x, n2))
                    .Apply(x => Math.Min(x, n3))
                    .Apply(x => Math.Min(x, n4))
                    .Apply(x => Math.Min(x, n5))
                    .Apply(x => Math.Min(x, n6))
                    .Apply(x => Math.Min(x, n7))
                    .Apply(x => Math.Min(x, n8))
                    .Apply(x => Math.Min(x, n9))
                    .Apply(x => Math.Min(x, n10))
                    .Apply(x => Math.Min(x, n11))
                    .Apply(x => Math.Min(x, n12))
                    .Apply(x => Math.Min(x, n13))
                    .Apply(x => Math.Min(x, n14))
                    .Apply(x => Math.Min(x, n15))
                    .Apply(x => Math.Min(x, n16))
            ).Curry();

            foreach (var x in Enumerable.Range(-10, 16).Select(Math.Abs))
            {
                min = min(x);
            }

            Assert.Equal(min, 0);
        }

        [Fact]
        public void TestCurry2Args()
        {
            Func<int, int, int> f = Math.Max;
            f.Curry()(5)(15).ShouldBe(15);
        }
    }
}