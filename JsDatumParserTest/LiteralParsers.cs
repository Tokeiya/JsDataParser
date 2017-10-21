using System;
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
			UnarySign.Run("+".AsStream()).AreSuccess("+");
			UnarySign.Run("-".AsStream()).AreSuccess("-");

			UnarySign.Run("".AsStream()).AreFail();
			UnarySign.Run("1".AsStream()).AreFail();
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
		public void NormalTextTest()
		{

			Text.Run("\"hello\"".AsStream()).AreSuccess("hello");
			Text.Run("\'hello\'".AsStream()).AreSuccess("hello");
			Text.Run("\'hello\"".AsStream()).AreFail();
			Text.Run("\"hello\'".AsStream()).AreFail();


			//successRunner("\\b");
			Text.Execute("''").AreSuccess("");

		}

		[Fact]
		public void EscapeSequenceTest()
		{
			Text.Execute("'\\b\\t\\v'").AreSuccess("\b\t\v");
			Text.Execute("'\\n\\r\\f'").AreSuccess("\n\r\f");

			Text.Execute("'\\'\\\"'").AreSuccess("'\"");
			Text.Execute("'\\\\'").AreSuccess("\\");
			Text.Execute("'\\0'").AreSuccess("\0");

			Text.Execute("'\\h'").AreFail();
		}

		[Fact]
		public void Latain1Test()
		{
			Text.Execute("'\\x41'").AreSuccess("A");
			Text.Execute("'\\xD1'").AreSuccess("Ñ");
		}

		[Fact]
		public void Utf16Test()
		{
			Text.Execute("'\\u304A'").AreSuccess("お");

		}

		[Fact]
		public void BooleanTest()
		{
			Bool.Execute("true").AreSuccess("true");
			Bool.Execute("false").AreSuccess("false");

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
	}");
		}


	}
}
