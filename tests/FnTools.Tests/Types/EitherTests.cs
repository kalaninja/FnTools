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
            Should.Throw<InvalidOperationException>(() => { var (_, _, _) = bottom; });
            Should.Throw<InvalidOperationException>(() => bottom.Match(_ => { }, _ => { }));
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
            Should.Throw<InvalidOperationException>(() => new Either<int, int>().Swap());
        }

        [Fact]
        public void TestMatch()
        {
            Left("left").Match(x => x.ShouldBe("left"), _ => true.ShouldNotBe(true));
            Right("right").Match(_ => true.ShouldNotBe(true), x => x.ShouldBe("right"));
            Should.Throw<InvalidOperationException>(() => new Either<int, int>().Match(_ => { }, _ => { }));
        }

        [Fact]
        public void TestFold()
        {
            Left("left").Fold(x => x, _ => "error").ShouldBe("left");
            Right("right").Fold(x => "error", x => x).ShouldBe("right");
            Should.Throw<InvalidOperationException>(() => new Either<int, int>().Fold(Combinators.I, Combinators.I));
        }

        [Fact]
        public void BiMapMapsBothSides()
        {
            Left("left").BiMap(x => x.Length, _ => "right").ShouldBe(Left(4));
            Right("right").BiMap(_ => "left", x => x.Length).ShouldBe(Right(5));
            Should.Throw<InvalidOperationException>(() => new Either<int, int>().BiMap(Combinators.I, Combinators.I));
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
        public void TestOperators()
        {
            (Left(true) == Left(false)).ShouldBe(false);
            (Left(true) == (Either<bool, bool>) Right(true)).ShouldBe(false);
            (Right("right") == Right("right")).ShouldBe(true);

            (Left(true) != Left(false)).ShouldBe(true);
            (Left(true) != (Either<bool, bool>) Right(true)).ShouldBe(true);
            (Right("right") != Right("right")).ShouldBe(false);

            Left("left").Equals((object) Left("left")).ShouldBe(true);
            Left("left").Equals((object) Left(1)).ShouldBe(false);

            (Left(true) ? "left" : "right").ShouldBe("left");
            (Right(true) ? "left" : "right").ShouldBe("right");

            ((string) Left("left")).ShouldBe("left");
            Should.Throw<InvalidCastException>(() => (Nothing) Left("left"));
            ((string) Right("right")).ShouldBe("right");
            Should.Throw<InvalidCastException>(() => (Nothing) Right("right"));
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
            Right<string, bool>(true).Left.GetOrElse("else").ShouldBe("else");
            Either.Right<string, bool>(true).Left.GetOrElse(() => "else").ShouldBe("else");

            Right("right").Right.GetOrElse("else").ShouldBe("right");
            Right("right").Right.GetOrElse(() => throw new Exception()).ShouldBe("right");
            Left<bool, string>(true).Right.GetOrElse("else").ShouldBe("else");
            Either.Left<bool, string>(true).Right.GetOrElse(() => "else").ShouldBe("else");
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
            Either.Left("le").Left.Map(x => x + "ft").ShouldBe(Left("left"));
            Either.Right("right").Left.Map(x => x + "ft").ShouldBe(Right("right"));

            Right("ri").Right.Map(x => x + "ght").ShouldBe(Right("right"));
            Left("left").Right.Map(x => x + "ght").ShouldBe(Left("left"));
        }

        [Fact]
        public void FlatMap()
        {
            Left(true).Left.FlatMap(x => Left("left")).ShouldBe(Left("left"));
            Right(true).Left.FlatMap(x => Left("left")).ShouldBe(Right(true));
            ((Either<string, string>) Left("left")).Left.FlatMap(_ => Right("right")).ShouldBe(Right("right"));

            Right(true).Right.FlatMap(x => Right("right")).ShouldBe(Right("right"));
            Left(true).Right.FlatMap(x => Right("right")).ShouldBe(Left(true));
            ((Either<string, string>) Right("right")).Right.FlatMap(_ => Left("left")).ShouldBe(Left("left"));
        }

        [Fact]
        public void Filter()
        {
            Left(12).Left.FilterToOption(x => x > 10).ShouldBe(Some(Left(12)));
            Left(12).Left.FilterToOption(x => x < 10).ShouldBe(None);
            Right(12).Left.FilterToOption(x => true).ShouldBe(None);
            Left(12).Left.FilterToOption(true).ShouldBe(Some(Left(12)));
            Left(12).Left.FilterToOption(false).ShouldBe(None);
            Right(12).Left.FilterToOption(true).ShouldBe(None);

            Right(12).Right.FilterToOption(x => x > 10).ShouldBe(Some(Right(12)));
            Right(12).Right.FilterToOption(x => x < 10).ShouldBe(None);
            Left(12).Right.FilterToOption(x => true).ShouldBe(None);
            Right(12).Right.FilterToOption(true).ShouldBe(Some(Right(12)));
            Right(12).Right.FilterToOption(false).ShouldBe(None);
            Left(12).Right.FilterToOption(true).ShouldBe(None);
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
        public void ToResult()
        {
            Left("left").Left.ToResult().ShouldBe(Ok("left"));
            Right("right").Left.ToResult().ShouldBe(Error("right"));

            Right("right").Right.ToResult().ShouldBe(Ok("right"));
            Left("left").Right.ToResult().ShouldBe(Error("left"));
        }

        [Fact]
        public void FromComprehension()
        {
            (from x in Left("left").Left select x.Length).ShouldBe(Left(4));
            (from x in Right("right").Right select x.Length).ShouldBe(Right(5));
        }

        [Fact]
        public void TestPatternMatching()
        {
            (Left(1) switch
            {
                var (isRight, left, _) when !isRight => left,
                _ => 0
            }).ShouldBe(1);

            (Right(1) switch
            {
                var (isRight, _, right) when isRight => right,
                _ => 0
            }).ShouldBe(1);
        }
    }
}