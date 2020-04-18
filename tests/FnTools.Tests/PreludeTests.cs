using System;
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
            var flag = Run(() => false);
            var action = Def(() => { flag = true; });
            Run(action);

            flag.ShouldBe(true);
        }

        [Fact]
        public void DefInfers1ArgAction()
        {
            var n = 0;
            var a = Def((int x) => { n = x; });
            a(5);

            n.ShouldBe(5);
        }
    }
}