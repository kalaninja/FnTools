using System.Linq;
using System.Text;
using FnTools.Func;
using FnTools.Types;
using MoreLinq.Extensions;
using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Examples
{
    public class TypeExamples
    {
        [Fact]
        public void OptionExample()
        {
            static Option<int> ParseInt(string val) =>
                int.TryParse(val, out var num) ? Some(num) : None;

            var result = new StringBuilder().Apply(sb =>
                new[] {"1", "2", "1.7", "NotANumber", "3"}
                    .Select(ParseInt)
                    .ForEach(o => o.Map(sb.Append))).ToString();

            result.ShouldBe("123");
        }

        [Fact]
        public void EitherExample()
        {
            static Either<string, int> Div(int x, int y)
            {
                if (y == 0)
                    return Left("cannot divide by 0");
                else
                    return Right(x / y);
            }

            static string PrintResult(Either<string, int> result) =>
                result.Fold(
                    left => $"Error: {left}",
                    right => right.ToString()
                );

            Div(10, 1).Apply(PrintResult).ShouldBe("10");
            Div(10, 0).Apply(PrintResult).ShouldBe("Error: cannot divide by 0");
        }

        [Fact]
        public void DefAndRun()
        {
            var value = 0;
            var action = Def(() => { value = 2; });

            Run(() => value = 1);
            value.ShouldBe(1);

            action();
            value.ShouldBe(2);
        }
    }
}