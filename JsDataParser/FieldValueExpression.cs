using System;
using System.Collections.Generic;
using System.Linq;

namespace JsDataParser
{
	internal class FieldValueExpression
	{
		private readonly object _source;

		public FieldValueExpression(IEnumerable<char> source, TokenTypes fieldType)
		{
			_source = source ?? throw new ArgumentNullException(nameof(source));

			switch (fieldType)
			{
				case TokenTypes.Boolean:
				case TokenTypes.Function:
				case TokenTypes.IntegerNumber:
				case TokenTypes.RealNumber:
				case TokenTypes.Text:
					break;

				default:
					throw new ArgumentException(nameof(fieldType));
			}

			FieldType = fieldType;
		}

		public FieldValueExpression(IReadOnlyCollection<IEnumerable<char>> arraySource)
		{
			if (arraySource.Any(x => x == null)) throw new ArgumentException($"{nameof(arraySource)} contains null");
			_source = arraySource ?? throw new ArgumentNullException(nameof(arraySource));

			FieldType = TokenTypes.IntegerArray;
		}


		public IEnumerable<char> Source
		{
			get
			{
				if (TokenTypes.IntegerArray == FieldType) throw new InvalidOperationException();

				return (IEnumerable<char>) _source;
			}
		}

		public IReadOnlyList<IEnumerable<char>> ArraySource
		{
			get
			{
				if (TokenTypes.IntegerArray != FieldType) throw new InvalidOperationException();

				return (IReadOnlyList<IEnumerable<char>>) _source;
			}
		}

		public TokenTypes FieldType { get; }
	}
}