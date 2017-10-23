using System;
using System.Collections.Generic;
using System.Text;

namespace JsDatumParser
{
    internal class FieldValue
    {
	    private readonly object _value;

	    public FieldValue(IEnumerable<char> value, TokenTypes fieldType)
	    {
		    _value = value ?? throw new ArgumentNullException(nameof(value));

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

	    public FieldValue(IReadOnlyCollection<IEnumerable<char>> value)
	    {
		    _value = value ?? throw new ArgumentNullException(nameof(value));
		    FieldType = TokenTypes.IntegerArray;
	    }

	    public IEnumerable<char> Value
	    {
		    get
		    {
			    switch (FieldType)
			    {
					case TokenTypes.Boolean:
					case TokenTypes.Function:
					case TokenTypes.IntegerNumber:
					case TokenTypes.RealNumber:
					case TokenTypes.Text:
						return (IEnumerable<char>) _value;

					default:
						throw new InvalidOperationException();
			    }
		    }
	    }

	    public IReadOnlyCollection<IEnumerable<char>> ArrayValue
		    => FieldType == TokenTypes.IntegerArray
			    ? (IReadOnlyList<IEnumerable<char>>) _value
			    : throw new InvalidOperationException();

	    public TokenTypes FieldType { get; }
    }
}
