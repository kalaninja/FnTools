using FnTools.Func;
using Shouldly;
using Xunit;
using static FnTools.Prelude;

namespace FnTools.Tests.Func
{
    public partial class PartialApplicationTests
    {
        [Fact]
        public void PartialApplication3ArgAction()
        {
            int result;
            var a = Def((int arg1, int arg2, int arg3) => { result = arg1 + arg2 + arg3; });

            result = 0;
            a.Partial(1)(2, 3);
            result.ShouldBe(6);
            result = 0;
            a.Partial(1, 2)(3);
            result.ShouldBe(6);
        }

        [Fact]
        public void PartialApplication4ArgAction()
        {
            int result;
            var a = Def((int arg1, int arg2, int arg3, int arg4) => { result = arg1 + arg2 + arg3 + arg4; });

            result = 0;
            a.Partial(1)(2, 3, 4);
            result.ShouldBe(10);
            result = 0;
            a.Partial(1, 2)(3, 4);
            result.ShouldBe(10);
            result = 0;
            a.Partial(1, 2, 3)(4);
            result.ShouldBe(10);
        }

        [Fact]
        public void PartialApplication5ArgAction()
        {
            int result;
            var a = Def((int arg1, int arg2, int arg3, int arg4, int arg5) =>
            {
                result = arg1 + arg2 + arg3 + arg4 + arg5;
            });

            result = 0;
            a.Partial(1)(2, 3, 4, 5);
            result.ShouldBe(15);
            result = 0;
            a.Partial(1, 2)(3, 4, 5);
            result.ShouldBe(15);
            result = 0;
            a.Partial(1, 2, 3)(4, 5);
            result.ShouldBe(15);
            result = 0;
            a.Partial(1, 2, 3, 4)(5);
            result.ShouldBe(15);
        }

        [Fact]
        public void PartialApplication6ArgAction()
        {
            int result;
            var a = Def((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6) =>
            {
                result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6;
            });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6);
            result.ShouldBe(21);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6);
            result.ShouldBe(21);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6);
            result.ShouldBe(21);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6);
            result.ShouldBe(21);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6);
            result.ShouldBe(21);
        }

        [Fact]
        public void PartialApplication7ArgAction()
        {
            int result;
            var a = Def((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7) =>
            {
                result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7;
            });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7);
            result.ShouldBe(28);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7);
            result.ShouldBe(28);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7);
            result.ShouldBe(28);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7);
            result.ShouldBe(28);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7);
            result.ShouldBe(28);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7);
            result.ShouldBe(28);
        }

        [Fact]
        public void PartialApplication8ArgAction()
        {
            int result;
            var a = Def((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8) =>
            {
                result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8;
            });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8);
            result.ShouldBe(36);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8);
            result.ShouldBe(36);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8);
            result.ShouldBe(36);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8);
            result.ShouldBe(36);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8);
            result.ShouldBe(36);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8);
            result.ShouldBe(36);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8);
            result.ShouldBe(36);
        }

        [Fact]
        public void PartialApplication9ArgAction()
        {
            int result;
            var a = Def((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9) =>
            {
                result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9;
            });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9);
            result.ShouldBe(45);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9);
            result.ShouldBe(45);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9);
            result.ShouldBe(45);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9);
            result.ShouldBe(45);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9);
            result.ShouldBe(45);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9);
            result.ShouldBe(45);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9);
            result.ShouldBe(45);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9);
            result.ShouldBe(45);
        }

        [Fact]
        public void PartialApplication10ArgAction()
        {
            int result;
            var a = Def(
                (int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10) =>
                {
                    result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10;
                });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9, 10);
            result.ShouldBe(55);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9)(10);
            result.ShouldBe(55);
        }

        [Fact]
        public void PartialApplication11ArgAction()
        {
            int result;
            var a = Def(
                (int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10,
                    int arg11) =>
                {
                    result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11;
                });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9, 10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9)(10, 11);
            result.ShouldBe(66);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)(11);
            result.ShouldBe(66);
        }

        [Fact]
        public void PartialApplication12ArgAction()
        {
            int result;
            var a = Def(
                (int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10,
                    int arg11, int arg12) =>
                {
                    result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12;
                });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9, 10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9)(10, 11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)(11, 12);
            result.ShouldBe(78);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)(12);
            result.ShouldBe(78);
        }

        [Fact]
        public void PartialApplication13ArgAction()
        {
            int result;
            var a = Def(
                (int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10,
                    int arg11, int arg12, int arg13) =>
                {
                    result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 +
                             arg13;
                });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9, 10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9)(10, 11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)(11, 12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)(12, 13);
            result.ShouldBe(91);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)(13);
            result.ShouldBe(91);
        }

        [Fact]
        public void PartialApplication14ArgAction()
        {
            int result;
            var a = Def(
                (int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10,
                    int arg11, int arg12, int arg13, int arg14) =>
                {
                    result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 +
                             arg13 + arg14;
                });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9, 10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9)(10, 11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)(11, 12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)(12, 13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)(13, 14);
            result.ShouldBe(105);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13)(14);
            result.ShouldBe(105);
        }

        [Fact]
        public void PartialApplication15ArgAction()
        {
            int result;
            var a = Def(
                (int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10,
                    int arg11, int arg12, int arg13, int arg14, int arg15) =>
                {
                    result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 +
                             arg13 + arg14 + arg15;
                });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9, 10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9)(10, 11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)(11, 12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)(12, 13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)(13, 14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13)(14, 15);
            result.ShouldBe(120);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14)(15);
            result.ShouldBe(120);
        }

        [Fact]
        public void PartialApplication16ArgAction()
        {
            int result;
            var a = Def(
                (int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10,
                    int arg11, int arg12, int arg13, int arg14, int arg15, int arg16) =>
                {
                    result = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 +
                             arg13 + arg14 + arg15 + arg16;
                });

            result = 0;
            a.Partial(1)(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2)(3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3)(4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4)(5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5)(6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6)(7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7)(8, 9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8)(9, 10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9)(10, 11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)(11, 12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)(12, 13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)(13, 14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13)(14, 15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14)(15, 16);
            result.ShouldBe(136);
            result = 0;
            a.Partial(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)(16);
            result.ShouldBe(136);
        }
    }
}