using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parseq;
using Xunit;

namespace JsDatumParserTest
{
	public static class Helper
	{
		public static string BuildString(this IEnumerable<char> source)
		{
			if (source == null) Assert.True(false, $"{nameof(BuildString)}({nameof(source)})is null");
			return source.Aggregate(new StringBuilder(), (b, c) => b.Append(c), b => b.ToString());
		}

		public static void AreSuccess(this IReply<char, IEnumerable<char>> reply, string expected)
		{
			reply.IsNotNull();
			expected.IsNotNull();
			AreSuccess(reply, act => expected.SequenceEqual(act));
		}

		public static void AreSuccess(this IReply<char, IEnumerable<char>> reply, Func<IEnumerable<char>, bool> predicate)
		{
			reply.IsNotNull();
			predicate.IsNotNull();

			reply.Case(
				_ =>
				{
					false.IsTrue();
					return 0;
				},
				seq =>
				{
					Assert.True(predicate(seq),seq.BuildString());
					return 0;
				});
		}

		public static void AreFail(this IReply<char, IEnumerable<char>> reply)
		{
			reply.IsNotNull();

			reply.Case(_ =>
			{
				true.IsTrue();
				return 0;
			}, seq =>
			{
				Assert.False(true, seq.BuildString());
				return 0;
			});
		}

		public static IReply<char, IEnumerable<char>> Execute(this Parser<char, IEnumerable<char>> parser, string input)
		{
			parser.IsNotNull();
			return parser.Run(input.AsStream());
		}
	}
}
