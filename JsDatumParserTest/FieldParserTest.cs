using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsDatumParserTest
{
	using static JsDatumParser.FieldParser;


	public class FieldParserTest
	{
		[Fact]
		public void ArrayTest()
		{
			ArrayParser.Execute("[565, 540, 539, 567]").AreSuccess(" 565 , 540 , 539 , 567 ");
			ArrayParser.Execute("[ 56 ]").AreSuccess(" 56 ");

			ArrayParser.Execute("[]").AreSuccess("");
			ArrayParser.Execute("[		]").AreSuccess("");
		}
	}
}
