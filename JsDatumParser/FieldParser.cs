using System;
using System.Collections.Generic;
using System.Text;
using Parseq;
using Parseq.Combinators;
using System.Linq;

namespace JsDatumParser
{
	using CharEnumerableParser = Parser<char,(IEnumerable<char> captured,TokenTypes type)>;
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

		private static CharEnumerableParser BuildIdentifier()
		{
#warning BuildIdentifier_Is_NotImpl
			throw new NotImplementedException("BuildIdentifier is not implemented");
		}

		public static readonly CharEnumerableParser Identifier;



	}



}
