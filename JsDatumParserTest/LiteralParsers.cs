using System;
using System.Linq;
using JsDatumParser;
using Parseq;
using Xunit;

namespace JsDatumParserTest
{
	using static JsDatumParser.LiteralParsers;

	public class LiteralParsers
	{
		[Fact]
		public void UnarySignTest()
		{ 
			UnarySign.Run("+".AsStream()).Case((_, __) => Assert.True(false), (_, seq) => seq.SequenceEqual("+").IsTrue());
			UnarySign.Run("-".AsStream()).Case((_, __) => Assert.True(false), (_, seq) => seq.SequenceEqual("-").IsTrue());

			UnarySign.Run("".AsStream()).Case((_, __) => Assert.True(true), (_, seq) => Assert.True(false));
			UnarySign.Run("1".AsStream()).Case((_, __) => Assert.True(true), (_, seq) => Assert.True(false));

		}

		[Fact]
		public void IntegerNumberTest()
		{
			IntegerNumber.Run("114514".AsStream()).AreSuccess("114514",TokenTypes.IntegerNumber);
			IntegerNumber.Run("-42".AsStream()).AreSuccess("-42",TokenTypes.IntegerNumber);
			IntegerNumber.Run("+11".AsStream()).AreSuccess("+11",TokenTypes.IntegerNumber);

			IntegerNumber.Run("++10".AsStream()).AreFail();

		}

		[Fact]
		public void RealNumberTest()
		{
			RealNumber.Run("-.0".AsStream()).AreSuccess("-.0",TokenTypes.RealNumber);
			RealNumber.Run("-0.".AsStream()).AreSuccess("-0.",TokenTypes.RealNumber);
			RealNumber.Run("0.".AsStream()).AreSuccess("0.",TokenTypes.RealNumber);
			RealNumber.Run(".0".AsStream()).AreSuccess(".0",TokenTypes.RealNumber);
			RealNumber.Run("+0.".AsStream()).AreSuccess("+0.",TokenTypes.RealNumber);
			RealNumber.Run("+.0".AsStream()).AreSuccess("+.0",TokenTypes.RealNumber);

			RealNumber.Run(".".AsStream()).AreFail();
			RealNumber.Run("-.".AsStream()).AreFail();
			RealNumber.Run("+.".AsStream()).AreFail();

		}

		[Fact]
		public void NormalTextTest()
		{

			Text.Run("\"hello\"".AsStream()).AreSuccess("hello",TokenTypes.Text);
			Text.Run("\'hello\'".AsStream()).AreSuccess("hello",TokenTypes.Text);
			Text.Run("\'hello\"".AsStream()).AreFail();
			Text.Run("\"hello\'".AsStream()).AreFail();


			//successRunner("\\b");
			Text.Execute("''").AreSuccess("",TokenTypes.Text);

		}

		[Fact]
		public void EscapeSequenceTest()
		{
			Text.Execute("'\\b\\t\\v'").AreSuccess("\b\t\v",TokenTypes.Text);
			Text.Execute("'\\n\\r\\f'").AreSuccess("\n\r\f",TokenTypes.Text);

			Text.Execute("'\\'\\\"'").AreSuccess("'\"",TokenTypes.Text);
			Text.Execute("'\\\\'").AreSuccess("\\",TokenTypes.Text);
			Text.Execute("'\\0'").AreSuccess("\0",TokenTypes.Text);

			Text.Execute("'\\h'").AreFail();
		}

		[Fact]
		public void Latain1Test()
		{
			Text.Execute("'\\x41'").AreSuccess("A",TokenTypes.Text);
			Text.Execute("'\\xD1'").AreSuccess("Ñ",TokenTypes.Text);
		}

		[Fact]
		public void Utf16Test()
		{
			Text.Execute("'\\u304A'").AreSuccess("お",TokenTypes.Text);

		}

		[Fact]
		public void BooleanTest()
		{
			Bool.Execute("true").AreSuccess("true",TokenTypes.Boolean);
			Bool.Execute("false").AreSuccess("false",TokenTypes.Boolean);

			Bool.Execute("True").AreFail();
			Bool.Execute("False").AreFail();
		}

		[Fact]
		public void TinyFunctionTest()
		{
			TinyFunction.Execute(@"function () {
	  return (this.HP / this.maxHP > .5);
	}").AreSuccess(@"function () {
	  return (this.HP / this.maxHP > .5);
	}",TokenTypes.Function);
		}


		[Fact]
		public void ArrayTest()
		{
			ArrayParser.Execute("[565, 540, 539, 567]").AreSuccess(" 565 , 540 , 539 , 567 ",TokenTypes.Array);
			ArrayParser.Execute("[ 56 ]").AreSuccess(" 56 ",TokenTypes.Array);

			ArrayParser.Execute("[]").AreSuccess("",TokenTypes.Array);
			ArrayParser.Execute("[		]").AreSuccess("",TokenTypes.Array);
		}


	}
}
