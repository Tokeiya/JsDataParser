﻿/*
 * Copyright (C) 2017 Hikotaro Abe <net_seed@hotmail.com> All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */

using System.Linq;
using JsDataParser.Parser;
using Parseq;
using Xunit;

namespace JsDataParserTest
{
	using static LiteralParser;


	public class LiteralParserTest
	{
		[Fact]
		public void BooleanTest()
		{
			Bool.Execute("true").AreSuccess("true");
			Bool.Execute("false").AreSuccess("false");

			Bool.Execute("True").AreFail();
			Bool.Execute("False").AreFail();
		}

		[Fact]
		public void CommentTest()
		{
			Comment.Run("//hello world\n".AsStream()).Case(
				(_, __) => Assert.False(true),
				(_, cap) => { cap.SequenceEqual("hello world").IsTrue(); });
		}

		[Fact]
		public void EscapeSequenceTest()
		{
			String.Execute("'\\b\\t\\v'").AreSuccess("\b\t\v");
			String.Execute("'\\n\\r\\f'").AreSuccess("\n\r\f");

			String.Execute("'\\'\\\"'").AreSuccess("'\"");
			String.Execute("'\\\\'").AreSuccess("\\");
			String.Execute("'\\0'").AreSuccess("\0");

			String.Execute("'\\h'").AreFail();
		}

		[Fact]
		public void FieldnameTeset()
		{
			var target = IdentifierName;

			target.Run("field".AsStream())
				.Case((_, __) => Assert.False(true),
					(_, seq) => seq.SequenceEqual("field").IsTrue());

			target.Run("Field".AsStream())
				.Case((_, __) => Assert.False(true),
					(_, seq) => seq.SequenceEqual("Field").IsTrue());

			target.Run("1field".AsStream())
				.Case((_, __) => Assert.False(false), (_, __) => Assert.True(false));

			target.Run("_field_".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) => cap.SequenceEqual("_field_").IsTrue());
		}

		[Fact]
		public void IntegerNumberTest()
		{
			IntegerNumber.Run("114514".AsStream()).AreSuccess("114514");
			IntegerNumber.Run("-42".AsStream()).AreSuccess("-42");
			IntegerNumber.Run("+11".AsStream()).AreSuccess("+11");

			IntegerNumber.Run("++10".AsStream()).AreFail();
		}

		[Fact]
		public void Latain1Test()
		{
			String.Execute("'\\x41'").AreSuccess("A");
			String.Execute("'\\xD1'").AreSuccess("Ñ");
		}

		[Fact]
		public void NormalTextTest()
		{
			String.Run("\"hello\"".AsStream()).AreSuccess("hello");
			String.Run("\'hello\'".AsStream()).AreSuccess("hello");
			String.Run("\'hello\"".AsStream()).AreFail();
			String.Run("\"hello\'".AsStream()).AreFail();


			//successRunner("\\b");
			String.Execute("''").AreSuccess("");
		}

		[Fact]
		public void RealNumberTest()
		{
			RealNumber.Run("-.0".AsStream()).AreSuccess("-.0");
			RealNumber.Run("-0.".AsStream()).AreSuccess("-0.");
			RealNumber.Run("0.".AsStream()).AreSuccess("0.");
			RealNumber.Run(".0".AsStream()).AreSuccess(".0");
			RealNumber.Run("+0.".AsStream()).AreSuccess("+0.");
			RealNumber.Run("+.0".AsStream()).AreSuccess("+.0");

			RealNumber.Run(".".AsStream()).AreFail();
			RealNumber.Run("-.".AsStream()).AreFail();
			RealNumber.Run("+.".AsStream()).AreFail();
		}

		[Fact]
		public void TinyFunctionTest()
		{
			TinyFunction.Execute(@"function () {
	  return (this.HP / this.maxHP > .5);
	}").AreSuccess(@"function () {
	  return (this.HP / this.maxHP > .5);
	}");

			TinyFunction.Execute("function(){}").AreSuccess("function () {}");
		}

		[Fact]
		public void UnarySignTest()
		{
			UnarySign.Run("+".AsStream()).Case((_, __) => Assert.True(false), (_, seq) => seq.SequenceEqual("+").IsTrue());
			UnarySign.Run("-".AsStream()).Case((_, __) => Assert.True(false), (_, seq) => seq.SequenceEqual("-").IsTrue());

			UnarySign.Run("".AsStream()).Case((_, __) => Assert.True(true), (_, seq) => Assert.True(false));
			UnarySign.Run("1".AsStream()).Case((_, __) => Assert.True(true), (_, seq) => Assert.True(false));
		}

		[Fact]
		public void Utf16Test()
		{
			String.Execute("'\\u304A'").AreSuccess("お");
		}
	}
}