using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsDatumParser;
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

		public static void AreSuccess(this IReply<char, (IEnumerable<char> captured,TokenTypes tokenType)> reply,
			string expectedCapture,TokenTypes expectedTokenType)
		{
			reply.IsNotNull();
			expectedCapture.IsNotNull();
			AreSuccess(reply, act => expectedCapture.SequenceEqual(act),expectedTokenType);
		}

		public static void AreSuccess(this IReply<char, (IEnumerable<char> captured, TokenTypes tokenType)> reply,
			Func<IEnumerable<char>, bool> predicate,TokenTypes expectedTokenType)
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
					Assert.True(predicate(seq.captured),seq.captured.BuildString());

					return 0;
				});
		}

		public static void AreFail(this IReply<char, (IEnumerable<char> captured, TokenTypes tokenType)> reply)
		{
			reply.IsNotNull();

			reply.Case(_ =>
			{
				true.IsTrue();
				return 0;
			}, seq =>
			{
				Assert.False(true, seq.captured.BuildString());
				return 0;
			});
		}

		public static IReply<char, (IEnumerable<char> captured, TokenTypes tokenType)> Execute(this Parser<char, (IEnumerable<char> captured, TokenTypes type)> parser, string input)
		{
			parser.IsNotNull();
			return parser.Run(input.AsStream());
		}
	}
}
