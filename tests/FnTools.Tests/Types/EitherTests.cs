using System;
using FnTools.Types;
using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Tests.Types
{
    public class EitherTests
    {
        [Fact]
        public void DefaultCtorInitsBottom()
        {
            var bottom = new Either<int, int>();

            bottom.IsLeft.ShouldBe(false);
            bottom.IsRight.ShouldBe(false);

            Should.Throw<InvalidOperationException>(() => bottom.Left.Get());
            Should.Throw<InvalidOperationException>(() => bottom.Right.Get());
        }

        [Fact]
        public void LeftInitsStateCorrectly()
        {
            const string value = "left";
            var left = Left(value);

            left.IsLeft.ShouldBe(true);
            left.IsRight.ShouldBe(false);

            left.Left.Get().ShouldBeSameAs(value);
            Should.Throw<InvalidOperationException>(() => left.Right.Get());
        }

        [Fact]
        public void RightInitsStateCorrectly()
        {
            const string value = "right";
            var right = Right(value);

            right.IsLeft.ShouldBe(false);
            right.IsRight.ShouldBe(true);

            right.Right.Get().ShouldBeSameAs(value);
            Should.Throw<InvalidOperationException>(() => right.Left.Get());
        }

        [Fact]
        public void TestSwap()
        {
            Left("left").Swap().ShouldBe(Right("left"));
            Right("right").Swap().ShouldBe(Left("right"));
        }

        [Fact]
        public void TestMatch()
        {
            Left("left").Match(x => x.ShouldBe("left"), _ => true.ShouldNotBe(true));
            Right("right").Match(_ => true.ShouldNotBe(true), x => x.ShouldBe("right"));
        }

        [Fact]
        public void TestFold()
        {
            Left("left").Fold(x => x, _ => "error").ShouldBe("left");
            Right("right").Fold(x => "error", x => x).ShouldBe("right");
        }

        [Fact]
        public void TestJoinLeft()
        {
            ((Either<Either<int, string>, string>) Left((Either<int, string>) Right("right"))).JoinLeft()
                .ShouldBe(Right("right"));

            Left(Left(10)).JoinLeft().ShouldBe(Left(10));

            ((Either<Either<int, string>, string>) Right("right")).JoinLeft().ShouldBe(Right("right"));
        }

        [Fact]
        public void TestJoinRight()
        {
            Right(Right(10)).JoinRight().ShouldBe(Right(10));

            ((Either<string, Either<string, int>>) Right((Either<string, int>) Left("left"))).JoinRight()
                .ShouldBe(Left("left"));

            ((Either<string, Either<string, int>>) Left("left")).JoinRight().ShouldBe(Left("left"));
        }

        [Fact]
        public void ToStringReturnsStateAndValue()
        {
            Left(true).ToString().ShouldBe("Left(True)");
            Right("right").ToString().ShouldBe("Right(right)");
        }

        [Fact]
        public void Operators()
        {
            (Left(true) == Left(false)).ShouldBe(false);
            (Left(true) == (Either<bool, bool>) Right(true)).ShouldBe(false);
            (Right("right") == Right("right")).ShouldBe(true);

            (Left(true) != Left(false)).ShouldBe(true);
            (Left(true) != (Either<bool, bool>) Right(true)).ShouldBe(true);
            (Right("right") != Right("right")).ShouldBe(false);

            (Left(true) ? "left" : "right").ShouldBe("left");
            (Right(true) ? "left" : "right").ShouldBe("right");
        }

        [Fact]
        public void TestGetHashCode()
        {
            Left(1).GetHashCode().ShouldBe(Left(1).GetHashCode());
            Left(1).GetHashCode().ShouldNotBe(Left(2).GetHashCode());
            Left(1).GetHashCode().ShouldNotBe(Right(1).GetHashCode());
            Right(1).GetHashCode().ShouldBe(Right(1).GetHashCode());
            Right(1).GetHashCode().ShouldNotBe(Right(2).GetHashCode());
        }

        [Fact]
        public void GetOrElse()
        {
            Left("left").Left.GetOrElse("else").ShouldBe("left");
            Left("left").Left.GetOrElse(() => throw new Exception()).ShouldBe("left");
            ((Either<string, bool>) Right(true)).Left.GetOrElse("else").ShouldBe("else");
            ((Either<string, bool>) Right(true)).Left.GetOrElse(() => "else").ShouldBe("else");

            Right("right").Right.GetOrElse("else").ShouldBe("right");
            Right("right").Right.GetOrElse(() => throw new Exception()).ShouldBe("right");
            ((Either<bool, string>) Left(true)).Right.GetOrElse("else").ShouldBe("else");
            ((Either<bool, string>) Left(true)).Right.GetOrElse(() => "else").ShouldBe("else");
        }

        [Fact]
        public void GetOrDefault()
        {
            Left("left").Left.GetOrDefault().ShouldBe("left");
            ((Either<string, int>) Right(0)).Left.GetOrDefault().ShouldBe(null);

            Right("right").Right.GetOrDefault().ShouldBe("right");
            ((Either<int, string>) Left(0)).Right.GetOrDefault().ShouldBe(null);
        }

        [Fact]
        public void Map()
        {
            Left("le").Left.Map(x => x + "ft").ShouldBe(Left("left"));
            Right("right").Left.Map(x => x + "ft").ShouldBe(Right("right"));

            Right("ri").Right.Map(x => x + "ght").ShouldBe(Right("right"));
            Left("left").Right.Map(x => x + "ght").ShouldBe(Left("left"));
        }

        [Fact]
        public void FlatMap()
        {
            Left(true).Left.FlatMap(x => Left("left")).ShouldBe(Left("left"));
            Right(true).Left.FlatMap(x => Left("left")).ShouldBe(Right(true));

            Right(true).Right.FlatMap(x => Right("right")).ShouldBe(Right("right"));
            Left(true).Right.FlatMap(x => Right("right")).ShouldBe(Left(true));
        }

        [Fact]
        public void Filter()
        {
            Left(12).Left.Filter(x => x > 10).ShouldBe(Some(Left(12)));
            Left(12).Left.Filter(x => x < 10).ShouldBe(None);
            Right(12).Left.Filter(x => true).ShouldBe(None);

            Right(12).Right.Filter(x => x > 10).ShouldBe(Some(Right(12)));
            Right(12).Right.Filter(x => x < 10).ShouldBe(None);
            Left(12).Right.Filter(x => true).ShouldBe(None);
        }

        [Fact]
        public void Exists()
        {
            Left(12).Left.Exists(x => x > 10).ShouldBe(true);
            Left(7).Left.Exists(x => x > 10).ShouldBe(false);
            ((Either<int, string>) Right("right")).Left.Exists(x => x > 10).ShouldBe(false);

            Right(12).Right.Exists(x => x > 10).ShouldBe(true);
            Right(7).Right.Exists(x => x > 10).ShouldBe(false);
            ((Either<string, int>) Left("left")).Right.Exists(x => x > 10).ShouldBe(false);
        }

        [Fact]
        public void ToOption()
        {
            Left("left").Left.ToOption().ShouldBe(Some("left"));
            Right("right").Left.ToOption().ShouldBe(None);

            Right("right").Right.ToOption().ShouldBe(Some("right"));
            Left("left").Right.ToOption().ShouldBe(None);
        }

        [Fact]
        public void FromComprehension()
        {
            (from x in Left("left").Left select x.Length).ShouldBe(Left(4));
            (from x in Right("right").Right select x.Length).ShouldBe(Right(5));
        }
    }
}