/*
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
using JsDataParser;
using JsDataParser.Parser;
using Parseq;
using Xunit;

namespace JsDataParserTest
{
	using static JsDataParser.Parser.LiteralParsers;

	public class LiteralParsers
	{
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
		public void ArrayTest()
		{
			ArrayParser.Run("[1,   2,3,4	,5]".AsStream())
				.Case(
					(_, __) => Assert.False(true),
					(_, tuple) =>
					{
						tuple.tokenType.Is(TokenTypes.IntegerArray);
						tuple.captured.Count.Is(5);
						for (var i = 1; i <= 5; i++) tuple.captured[i - 1].Is(i.ToString());
					});

			ArrayParser.Run("[1]".AsStream())
				.Case(
					(_, __) => Assert.False(true),
					(_, tuple) =>
					{
						tuple.tokenType.Is(TokenTypes.IntegerArray);
						tuple.captured.Count.Is(1);
						tuple.captured[0].Is("1");
					});


			ArrayParser.Run("[]".AsStream())
				.Case(
					(_, __) => Assert.False(true),
					(_, tuple) =>
					{
						tuple.tokenType.Is(TokenTypes.IntegerArray);
						tuple.captured.Count.Is(0);
					});

			ArrayParser.Run("[      ]".AsStream())
				.Case(
					(_, __) => Assert.False(true),
					(_, tuple) =>
					{
						tuple.tokenType.Is(TokenTypes.IntegerArray);
						tuple.captured.Count.Is(0);
					});
		}

		[Fact]
		public void BooleanTest()
		{
			Bool.Execute("true").AreSuccess("true", TokenTypes.Boolean);
			Bool.Execute("false").AreSuccess("false", TokenTypes.Boolean);

			Bool.Execute("True").AreFail();
			Bool.Execute("False").AreFail();
		}

		[Fact]
		public void EscapeSequenceTest()
		{
			Text.Execute("'\\b\\t\\v'").AreSuccess("\b\t\v", TokenTypes.Text);
			Text.Execute("'\\n\\r\\f'").AreSuccess("\n\r\f", TokenTypes.Text);

			Text.Execute("'\\'\\\"'").AreSuccess("'\"", TokenTypes.Text);
			Text.Execute("'\\\\'").AreSuccess("\\", TokenTypes.Text);
			Text.Execute("'\\0'").AreSuccess("\0", TokenTypes.Text);

			Text.Execute("'\\h'").AreFail();
		}

		[Fact]
		public void IntegerNumberTest()
		{
			IntegerNumber.Run("114514".AsStream()).AreSuccess("114514", TokenTypes.IntegerNumber);
			IntegerNumber.Run("-42".AsStream()).AreSuccess("-42", TokenTypes.IntegerNumber);
			IntegerNumber.Run("+11".AsStream()).AreSuccess("+11", TokenTypes.IntegerNumber);

			IntegerNumber.Run("++10".AsStream()).AreFail();
		}

		[Fact]
		public void Latain1Test()
		{
			Text.Execute("'\\x41'").AreSuccess("A", TokenTypes.Text);
			Text.Execute("'\\xD1'").AreSuccess("Ñ", TokenTypes.Text);
		}

		[Fact]
		public void NormalTextTest()
		{
			Text.Run("\"hello\"".AsStream()).AreSuccess("hello", TokenTypes.Text);
			Text.Run("\'hello\'".AsStream()).AreSuccess("hello", TokenTypes.Text);
			Text.Run("\'hello\"".AsStream()).AreFail();
			Text.Run("\"hello\'".AsStream()).AreFail();


			//successRunner("\\b");
			Text.Execute("''").AreSuccess("", TokenTypes.Text);
		}

		[Fact]
		public void RealNumberTest()
		{
			RealNumber.Run("-.0".AsStream()).AreSuccess("-.0", TokenTypes.RealNumber);
			RealNumber.Run("-0.".AsStream()).AreSuccess("-0.", TokenTypes.RealNumber);
			RealNumber.Run("0.".AsStream()).AreSuccess("0.", TokenTypes.RealNumber);
			RealNumber.Run(".0".AsStream()).AreSuccess(".0", TokenTypes.RealNumber);
			RealNumber.Run("+0.".AsStream()).AreSuccess("+0.", TokenTypes.RealNumber);
			RealNumber.Run("+.0".AsStream()).AreSuccess("+.0", TokenTypes.RealNumber);

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
	}", TokenTypes.Function);
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
			Text.Execute("'\\u304A'").AreSuccess("お", TokenTypes.Text);
		}

		[Fact]
		public void CommentTest()
		{
			Comment.Run("//hello world\n".AsStream()).Case(
				(_, __) => Assert.False(true),
				(_, cap) =>
				{
					cap.type.Is(TokenTypes.Comment);
					cap.captured.SequenceEqual("hello world").IsTrue();
				});
		}
	}
}