using System;
using FnTools.Func;
using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Examples
{
    public class FuncExamples
    {
        private struct Location
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        [Fact]
        public void PartialApplication()
        {
            var location = new Location {X = 50, Y = 23};
            var time = "13:57:59";

            Def<string, object[], string>(string.Format)
                .Partial(__, new object[] {location.X, location.Y, time})
                .Apply(LogLocation);

            void LogLocation(Func<string, string> log)
            {
                log("{2}: {0},{1}").ShouldBe($"{time}: {location.X},{location.Y}");
                log("({0}, {1})").ShouldBe($"({location.X}, {location.Y})");
            }
        }

        [Fact]
        public void PartialAndComposition()
        {
            var substring = Def((int start, int length, string str) => str.Substring(start, length));
            var firstChars = substring.Partial(0);
            var firstChar = firstChars.Partial(1);
            var toLower = Def((string str) => str.ToLower());
            var lowerFirstChar = toLower.Compose(firstChar);

            lowerFirstChar("String").ShouldBe("s");
        }
    }
}