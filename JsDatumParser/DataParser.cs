using System;
using System.Collections.Generic;
using Parseq;
using Parseq.Combinators;

namespace JsDatumParser
{
	using TypedCharEnumerableParser = Parser<char, (IEnumerable<char> captured, TokenTypes type)>;
	using CharEnumerableParser = Parser<char, IEnumerable<char>>;

	internal static class DataParser
	{
		private static readonly CharacterCache Cache = new CharacterCache(1000, 1250);

		public static readonly CharEnumerableParser FieldName = Chars.Letter().Many1();

		public static Parser<char, IEnumerable<(string fieldName, string value, TokenTypes valueType)>> BuildField()
		{
			var whiteSpace = Chars.WhiteSpace().Many0().Select(_ => Array.Empty<char>());

			//var ret= from name in FieldName 
			//		 from _ in Combinator.Sequence(whiteSpace.Ignore(),Chars.Char(':').Ignore(),whiteSpace.Ignore())
			//		 from value in Combinator.Choice(LiteralParsers)

			return null;
		}

		public static readonly Parser<char, IEnumerable<(string fieldName, string value, TokenTypes valueType)>> Field =
			BuildField();

		public static
			Parser<char, IEnumerable<(int index, IEnumerable<(string fieldName, string value, TokenTypes valueType)> fields)>>
			BuildDataElement()
		{
			return null;

		}

		public static readonly
			Parser<char, IEnumerable<(int index, IEnumerable<(string fieldName, string value, TokenTypes valueType)> fields)>>
			DataElement;




	
	}
}