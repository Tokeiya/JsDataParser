using System;
using JsDatumParser;
using Parseq;
using Xunit;

namespace JsDatumParserTest
{
    public class ParserDefinationTest
    {
        [Fact]
        public void UnarySignTest()
		{ 
			ParserDefinations.UnarySign.Run("+".AsStream()).AreSuccess("+");
			ParserDefinations.UnarySign.Run("-".AsStream()).AreSuccess("-");
			ParserDefinations.UnarySign.Run("".AsStream()).AreSuccess("");

			ParserDefinations.UnarySign.Run("1".AsStream()).AreFail();

		}
    }
}
