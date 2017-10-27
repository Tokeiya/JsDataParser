using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsDataParser.Entities;
// ReSharper disable InconsistentNaming

namespace JsDataParser
{
    internal static class VerifyExtensions
    {
	    private static readonly ValueTypes[] _valueTypes = (ValueTypes[]) Enum.GetValues(typeof(ValueTypes));

	    private static readonly IdentifierTypes[] _identifierTypes =
		    (IdentifierTypes[]) Enum.GetValues(typeof(IdentifierTypes));

	    public static bool Verify(this ValueTypes value)
		    => _valueTypes.Any(x => value == x);

	    public static bool Verify(this IdentifierTypes value)
		    => _identifierTypes.Any(x=>x==value);
    }
}
