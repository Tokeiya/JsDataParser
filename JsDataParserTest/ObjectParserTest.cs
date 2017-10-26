using System.Linq;
using JsDataParser.Entities;
using JsDataParser.Parser;
using Parseq;
using Xunit;

namespace JsDataParserTest
{
	using static JsDataParser.Parser.ObjectParser;

	//Currently "true" and "false" value can't recognize as a boolean.


	public class ObjectParserTest
	{

		[Fact]
		public void IntegerArrayParserTest()
		{
			IntegerArray.Run("[0, 1, 2		, 3		]".AsStream())
				.Case((_, __) => Assert.False(true),
					(_, cap) =>
					{
						cap.ValueType.Is(ValueTypes.Array);
						cap.ArrayValue.Count.Is(4);

						cap.ArrayValue.Any(x => x.ValueType != ValueTypes.Integer).IsFalse();

						for (int i = 0; i < 4; i++)
						{
							cap.ArrayValue[i].Value.SequenceEqual(i.ToString()).IsTrue();
						}
					});

			IntegerArray.Run("[]".AsStream()).Case(
				(_, __) => Assert.False(true),
				(_, cap) =>
				{
					cap.ValueType.Is(ValueTypes.Array);
					cap.ArrayValue.Count.Is(0);
				});
		}

	}
}
