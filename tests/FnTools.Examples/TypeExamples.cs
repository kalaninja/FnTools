using System;
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
            var input = new[] {"1", "2", "1.7", "Not_A_Number", "3"};

            static Option<int> ParseInt(string val) =>
                int.TryParse(val, out var num) ? Some(num) : None;

            var result = new StringBuilder().Apply(sb =>
                input
                    .Select(ParseInt)
                    .ForEach(o => o.Map(sb.Append))
            ).ToString();

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
        public void TryExample()
        {
            var readLine = Def(() => new[] {"1", "2", "NaN"}.Shuffle().First());
            var tryParse =
                Def((string x) =>
                    Try(() => int.Parse(x))
                        .Recover<FormatException>(_ => 0)
                );

            var trySum =
                from x in tryParse(readLine())
                from y in tryParse(readLine())
                from z in tryParse(readLine())
                select x + y + z;

            trySum.IsSuccess.ShouldBe(true);

            var sum = trySum.Get();

            sum.ShouldBeGreaterThanOrEqualTo(0);
            sum.ShouldBeLessThanOrEqualTo(6);
        }

        enum MathError
        {
            DivisionByZero,
            NonPositiveLogarithm,
            NegativeSquareRoot,
        }

        [Fact]
        public void ResultExample()
        {
            Result<decimal, MathError> Div(decimal x, decimal y) =>
                Try(() => x / y).ToResult().ErrorMap(_ => MathError.DivisionByZero);

            Result<double, MathError> Sqrt(double x) =>
                Ok(Math.Sqrt(x)).Filter(x >= 0, MathError.NegativeSquareRoot);

            Result<double, MathError> Ln(double x)
            {
                if (x > 0)
                    return Ok(Math.Log(x));
                else
                    return Error(MathError.NonPositiveLogarithm);
            }

            // sqrt(ln(x / y))
            Result<double, string> Op(decimal x, decimal y)
            {
                var result =
                    from a in Div(x, y).Map(Convert.ToDouble)
                    from b in Ln(a)
                    from c in Sqrt(b)
                    select c;

                return result.ErrorMap(ToString<MathError>());
            }

            Op(1, 0).ShouldBe("DivisionByZero");
            Op(1, -10).ShouldBe("NonPositiveLogarithm");
            Op(1, 10).ShouldBe("NegativeSquareRoot");
            Op(1, 1).ShouldBe(0);
        }
    }
}