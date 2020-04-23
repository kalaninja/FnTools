using System;
using System.Threading.Tasks;
using FnTools.Exceptions;
using FnTools.Types;
using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Tests.Types
{
    public class TryTests
    {
        [Fact]
        public void DefaultCtorInitsBottom()
        {
            var bottom = new Try<Nothing>();

            bottom.IsSuccess.ShouldBe(false);
            bottom.IsFailure.ShouldBe(false);
            Should.Throw<InvalidOperationException>(() => bottom.Get());
        }

        [Fact]
        public void SuccessSetsStateCorrectly()
        {
            var success = new Try<string>("ok");

            success.IsSuccess.ShouldBe(true);
            success.IsFailure.ShouldBe(false);
            success.Get().ShouldBe("ok");
            success.GetOrElse("else").ShouldBe("ok");
            success.GetOrElse(() => "else").ShouldBe("ok");
            success.GetOrDefault().ShouldBe("ok");
            ((string) success).ShouldBe("ok");
        }

        [Fact]
        public void FailureSetsStateCorrectly()
        {
            var failure = new Try<string>(new Exception("fail"));

            failure.IsSuccess.ShouldBe(false);
            failure.IsFailure.ShouldBe(true);
            Should.Throw<Exception>(() => failure.Get()).Message.ShouldBe("fail");
            failure.GetOrElse("fail").ShouldBe("fail");
            failure.GetOrElse(() => "fail").ShouldBe("fail");
            failure.GetOrDefault().ShouldBe(null);
            ((Exception) failure).Message.ShouldBe("fail");
        }

        [Fact]
        public void MapThrowsOnBottom() =>
            Should.Throw<InvalidOperationException>(() => new Try<int>().Map(Combinators.I));

        [Fact]
        public void MapAppliesFuncToSuccess() =>
            FnTools.Types.Try<string>.Success("ok").Map(x => x.Length).ShouldBe(new Try<int>(2));

        [Fact]
        public void MapDoesNotApplyFuncToFailure() =>
            Should.Throw<Exception>(() =>
                FnTools.Types.Try<string>.Failure(new Exception("fail")).Map(x => x.Length).Get()
            ).Message.ShouldBe("fail");

        [Fact]
        public void MapReturnsFailureIfFuncThrows() =>
            Should.Throw<Exception>(() =>
                FnTools.Types.Try<string>.Success("ok").Map<string>(x => throw new Exception("fail")).Get()
            ).Message.ShouldBe("fail");

        [Fact]
        public void FlatMapBindsSuccess() =>
            Try(() => int.Parse("10"))
                .FlatMap(x => Try(() => int.Parse("1") + x))
                .ShouldBe(11);

        [Fact]
        public void FlatMapStopsOnFirstException()
        {
            var failure = Try(() => int.Parse("10"))
                .FlatMap(x => Try(() => int.Parse("not a number") + x))
                .FlatMap(x => Try(() => int.Parse("1") + x));

            (failure ? true : false).ShouldBe(false);
            Should.Throw<FormatException>(() => failure.Get());
        }

        [Fact]
        public void FlatMapReturnsFailureIfFuncThrows() =>
            Should.Throw<Exception>(() =>
                FnTools.Types.Try<string>.Success("ok")
                    .FlatMap<string>(x => throw new Exception("fail")).Get()
            ).Message.ShouldBe("fail");

        [Fact]
        public void FlatTapDiscardsResult()
        {
            Try(() => int.Parse("not a number")).FlatTap(x => Try(() => int.Parse("1"))).IsFailure.ShouldBe(true);
            Try(() => int.Parse("1")).FlatTap(x => Try(() => x + 1)).ShouldBe(1);
        }

        [Fact]
        public void FlatTapKeepsEffect()
        {
            Try(() => 1)
                .FlatTap(x => Try(() => false ? Try(() => "ok") : throw new Exception("fail")))
                .IsFailure.ShouldBe(true);

            Should.Throw<Exception>(() =>
                FnTools.Types.Try<string>.Success("ok")
                    .FlatTap<string>(x => throw new Exception("fail")).Get()
            ).Message.ShouldBe("fail");
        }

        [Fact]
        public void FlattenFlatsNestedTry()
        {
            Try(() => Try(() => "ok")).Flatten().ShouldBe("ok");
            Try(() => new Try<string>(new Exception("fail"))).Flatten().IsSuccess.ShouldBe(false);
            Try(() => false ? Try(() => "ok") : throw new Exception("fail")).Flatten().IsSuccess.ShouldBe(false);
        }

        [Fact]
        public void FilterReturnsFailureWhenConditionDoesNotHold()
        {
            Should.Throw<NoSuchElementException>(() =>
                Try(() => 3).Filter(x => x < 0).Rethrow());

            Should.Throw<NoSuchElementException>(() =>
                Try(() => 3).Filter(false).Rethrow());
        }

        [Fact]
        public void FilterReturnsFailureWhenFuncThrows()
        {
            var (isSuccess, _, exception) = Try(() => 3).Filter(x => throw new Exception("fail"));
            isSuccess.ShouldBe(false);
            exception!.Message.ShouldBe("fail");
        }

        [Fact]
        public void ExistsReturnsTrueWhenConditionHolds() =>
            Try(() => 3).Exists(x => x == 3).ShouldBe(true);

        [Fact]
        public async Task FoldAppliesSuccessHandlerToSuccess() =>
            (await Try(() => Task.FromResult("ok"))).Fold(Combinators.I, _ => "fail").ShouldBe("ok");

        [Fact]
        public async Task FoldAppliesFailureHandlerToFailure() =>
            (await Try(() => false ? Task.FromResult("ok") : throw new Exception("fail")))
            .Fold(Combinators.I, e => e.Message)
            .ShouldBe("fail");

        [Fact]
        public void FoldAppliesFailureHandlerIfSuccessHandlerThrows() =>
            Try(() => "ok")
                .Fold(x => throw new Exception("fail"), e => e.Message)
                .ShouldBe("fail");

        [Fact]
        public void MatchAppliesSuccessHandlerToSuccess() =>
            Try(() => "ok").Match(x => x.ShouldBe("ok"), e => throw e);

        [Fact]
        public void MatchAppliesFailureHandlerToFailure()
        {
            var result = "ok";
            Try(() => false ? result : throw new Exception("fail"))
                .Match(x => result = x, e => result = e.Message);
            result.ShouldBe("fail");
        }

        [Fact]
        public void MatchAppliesFailureHandlerIfSuccessHandlerThrows()
        {
            var result = "ok";
            Try(() => result)
                .Match(x => throw new Exception("fail"), e => result = e.Message);
            result.ShouldBe("fail");
        }

        [Fact]
        public void RecoverTurnsFailureIntoSuccess() =>
            ((Try<string>) new Exception()).Recover(e => "recovered").ShouldBe("recovered");

        [Fact]
        public void RecoverDoesNotAffectSuccess() =>
            Try(() => "ok")
                .Recover(e => "recovered")
                .Recover<Exception>(e => "recovered")
                .ShouldBe("ok");

        [Fact]
        public void RecoverReturnsNewFailureIfFuncThrows() =>
            ((Exception)
                ((Try<string>) new Exception())
                .Recover(e => throw new Exception("fail"))
                .Recover<Exception>(e => throw new Exception("fail"))
            ).Message.ShouldBe("fail");

        [Fact]
        public void RecoverTurnsMatchingTypeFailureIntoSuccess() =>
            ((Try<string>) new NoSuchElementException("fail"))
            .Recover<InvalidOperationException>(e => "recovered")
            .ShouldBe("recovered");

        [Fact]
        public void RecoverDoesNotTurnNonMatchingTypeFailureIntoSuccess() =>
            Should.Throw<NoSuchElementException>(() =>
                ((Try<string>) new NoSuchElementException("fail"))
                .Recover<ArgumentNullException>(e => "recovered")
                .Get()
            ).Message.ShouldBe("fail");

        [Fact]
        public void RecoverWithTurnsFailureIntoSuccess() =>
            ((Try<string>) new Exception()).RecoverWith(e => "recovered").ShouldBe("recovered");

        [Fact]
        public void RecoverWithDoesNotAffectSuccess() =>
            Try(() => "ok")
                .RecoverWith(e => "recovered")
                .RecoverWith<Exception>(e => "recovered")
                .ShouldBe("ok");

        [Fact]
        public void RecoverWithReturnsNewFailureIfFuncThrows() =>
            ((Exception)
                ((Try<string>) new Exception())
                .RecoverWith(e => throw new Exception("fail"))
                .RecoverWith<Exception>(e => throw new Exception("fail"))
            ).Message.ShouldBe("fail");

        [Fact]
        public void RecoverWithTurnsMatchingTypeFailureIntoSuccess() =>
            ((Try<string>) new NoSuchElementException("fail"))
            .RecoverWith<InvalidOperationException>(e => "recovered")
            .ShouldBe("recovered");

        [Fact]
        public void RecoverWithDoesNotTurnNonMatchingTypeFailureIntoSuccess() =>
            Should.Throw<NoSuchElementException>(() =>
                ((Try<string>) new NoSuchElementException("fail"))
                .RecoverWith<ArgumentNullException>(e => "recovered")
                .Get()
            ).Message.ShouldBe("fail");

        [Fact]
        public void ToStringReturnsStateAndValue()
        {
            Try(() => "ok").ToString().ShouldBe("Success(ok)");
            new Try<Exception>(new Exception("fail")).ToString().ShouldBe("Failure(System.Exception: fail)");
        }

        [Fact]
        public void TestOperators()
        {
            (Try(() => 1) && Try(() => 2) ? true : false).ShouldBe(true);
            (
                new Try<string>(new Exception("fail")) || Try(() => "ok") || new Try<string>(new Exception("fail"))
                    ? true
                    : false
            ).ShouldBe(true);

            (Try(() => 1) == Try(() => 2)).ShouldBe(false);
            (Try(() => 1) == Try(() => 1)).ShouldBe(true);
            (Try(() => 1) == new Try<int>(new Exception())).ShouldBe(false);
            (Try(() => 1) != Try(() => 2)).ShouldBe(true);
            (Try(() => 1) != Try(() => 1)).ShouldBe(false);
            (Try(() => 1) != new Try<int>(new Exception())).ShouldBe(true);
            Try(() => 1).Equals((object) Try(() => 1)).ShouldBe(true);
            Try(() => 1).Equals("str").ShouldBe(false);
        }

        [Fact]
        public void ToOptionReturnsSomeIfSuccess() =>
            Try(() => "ok").ToOption().ShouldBe(Some("ok"));

        [Fact]
        public void ToOptionReturnsNoneIfFailure() =>
            new Try<string>(new Exception("fail")).ToOption().ShouldBe(None);

        [Fact]
        public void GetHashCodeIsEqualIfTryIsEqual() =>
            Try(() => "ok").GetHashCode().ShouldBe(Try(() => "ok").GetHashCode());

        [Fact]
        public void ToLeftReturnsLeftIfSuccess()
        {
            Try(() => "ok").ToLeft("fail").ShouldBe(Left("ok"));
            Try(() => "ok").ToLeft(() => "fail").ShouldBe(Left("ok"));
        }

        [Fact]
        public void ToLeftReturnsRightIfFailure()
        {
            ((Try<string>) new Exception()).ToLeft("fail").ShouldBe(Right("fail"));
            ((Try<string>) new Exception()).ToLeft(() => "fail").ShouldBe(Right("fail"));
        }

        [Fact]
        public void ToRightReturnsRightIfSuccess()
        {
            Try(() => "ok").ToRight("fail").ShouldBe(Right("ok"));
            Try(() => "ok").ToRight(() => "fail").ShouldBe(Right("ok"));
        }

        [Fact]
        public void ToRightReturnsLeftIfFailure()
        {
            ((Try<string>) new Exception()).ToRight("fail").ShouldBe(Left("fail"));
            ((Try<string>) new Exception()).ToRight(() => "fail").ShouldBe(Left("fail"));
        }

        [Fact]
        public void ToResultReturnsOkIfSuccess()
        {
            Try(() => "ok").ToResult().ShouldBe(Ok("ok"));
        }

        [Fact]
        public void ToResultReturnsOkIfFailure()
        {
            var result = ((Try<string>) new Exception()).ToResult();
            result.IsOk.ShouldBe(false);
            result.Error.IsSome.ShouldBe(true);
        }

        [Fact]
        public void TestForComprehension()
        {
            (from x in Try(() => "ok") select x).ShouldBe("ok");

            Should.Throw<NoSuchElementException>(() => (
                from x in
                    from a in Try(() => 10)
                    from b in Try(() => 20)
                    select Math.Max(a, b)
                let bound = 100
                where x > bound
                select x).Rethrow());
        }
    }
}