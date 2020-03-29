using FnTools.Types;
using Shouldly;
using Xunit;

namespace FnTools.Tests.Types
{
    public class NothingTests
    {
        [Fact]
        public void NothingToStringReturnsNothing() =>
            default(Nothing).ToString().ShouldBe("Nothing");
    }
}