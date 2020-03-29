using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Tests
{
    public class PreludeTests
    {
        [Fact]
        public void RunExecutesAction()
        {
            var flag = false;
            var action = Def(() => { flag = true; });
            Run(action);
            flag.ShouldBe(true);
        }
    }
}