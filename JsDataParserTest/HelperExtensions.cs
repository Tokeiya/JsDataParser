using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parseq;
using Xunit;

namespace JsDataParserTest
{
	internal static class HelperExtensions
	{
		public static string BuildString(this IEnumerable<char> source)
		{
			var bld = new StringBuilder();

			foreach (var c in source)
				bld.Append(c);

			return bld.ToString();
		}

		public static IReply<char, IEnumerable<char>> Execute(this Parser<char, IEnumerable<char>> parser, string value)
		{
			return parser.Run(value.AsStream());
		}

		public static void AreSuccess(this IReply<char, IEnumerable<char>> actual, string expected)
		{
			actual.Case(
				(_, __) => Assert.False(true),
				(_, cap) =>
				{
					cap.SequenceEqual(expected).IsTrue();
				});
		}

		public static void AreFail(this IReply<char, IEnumerable<char>> actual)
		{
			actual.Case(
				(_, __) => Assert.False(false),
				(_, __) => Assert.True(false)
			);
		}
	}
}