using System;
using System.Collections.Generic;
using System.Text;
using Parseq;
using Parseq.Combinators;
using System.Linq;

namespace JsDatumParser
{
	using CharEnumerableParser = Parseq.Parser<char, IEnumerable<char>>;
	using static LiteralParsers;
	internal static class FieldParser
	{
		private static readonly CharacterCache Cache = new CharacterCache(1000, 1250);

		private static CharEnumerableParser BuildArray()
		{
			var whiteSpace = Chars.WhiteSpace().Many0().Select(_ => Cache.Get(' '));

			var array =
				from _ in Combinator.Sequence(Chars.Char('[').Ignore(), whiteSpace.Ignore())
				from values in Combinator.Sequence(whiteSpace, IntegerNumber, whiteSpace, Chars.Char(',').Select(c => Cache.Get(c))).Many0()
				from lastValue in Combinator.Sequence(whiteSpace, IntegerNumber, whiteSpace)
				from __ in Chars.Char(']').Ignore()
				let first = values.SelectMany(x => x.SelectMany(y => y))
				let second = lastValue.SelectMany(x => x)
				select first.Concat(second);

			var emptyArray = Combinator.Sequence(Chars.Char('[').Ignore(), whiteSpace.Ignore(), Chars.Char(']').Ignore())
				.Select(_ => Array.Empty<char>());


			return Combinator.Choice(emptyArray, array);
		}

		public static readonly CharEnumerableParser ArrayParser=BuildArray();

	}
}
