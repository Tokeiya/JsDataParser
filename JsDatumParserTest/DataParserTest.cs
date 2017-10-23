using System.Linq;
using JsDatumParser;
using Parseq;
using Xunit;

namespace JsDatumParserTest
{
	public class FieldParserTest
	{
		[Fact]
		public void FieldnameTeset()
		{
			var target = DataParser.FieldName;

			target.Run("field".AsStream())
				.Case((_, __) => Assert.False(true),
					(_, seq) => seq.SequenceEqual("field").IsTrue());

			target.Run("Field".AsStream())
				.Case((_, __) => Assert.False(true),
					(_, seq) => seq.SequenceEqual("Field").IsTrue());

			target.Run("1field".AsStream())
				.Case((_, __) => Assert.False(false), (_, __) => Assert.True(false));


		}
	}
}