using System;
using System.Collections.Generic;
using System.Text;
using Parseq;
using Parseq.Combinators;
using System.Linq;

namespace JsDatumParser
{
	using TypedCharEnumerableParser = Parser<char, (IEnumerable<char> captured, TokenTypes type)>;
	using CharEnumerableParser = Parser<char, IEnumerable<char>>;


	using static LiteralParsers;
	internal static class FieldParser
	{
		private static readonly CharacterCache Cache = new CharacterCache(1000, 1250);


		private static TypedCharEnumerableParser BuildIdentifier()
		{
#warning BuildIdentifier_Is_NotImpl
			throw new NotImplementedException("BuildIdentifier is not implemented");
		}


		public static readonly TypedCharEnumerableParser Identifier;



	}



}
