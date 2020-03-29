using FnTools.Func;
using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Tests.Func
{
    public class PartialApplicationTests
    {
        [Fact]
        public void TestPartialApplication()
        {
            var f = Def((int x, int y, int z) => x + y + z);

            f.Partial(1)(2, 3).ShouldBe(6);
            f.Partial(1, 2)(3).ShouldBe(6);
            f.Partial(__, 2)(1, 3).ShouldBe(6);
            f.Partial(__, __, 3)(1, 2).ShouldBe(6);
            f.Partial(__, 2, 3)(1).ShouldBe(6);
            f.Partial(1, __, 3)(2).ShouldBe(6);
        }
    }
}