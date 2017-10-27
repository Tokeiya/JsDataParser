using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using JsDataParser.Entities;
// ReSharper disable InconsistentNaming

namespace JsDataParser
{
	internal static class VerifyExtensions
	{
		private static readonly IdentifierTypes[] _identifierTypeses =
			(IdentifierTypes[]) Enum.GetValues(typeof(IdentifierTypes));

		private static readonly ValueTypes[] _valueTypes =
			(ValueTypes[]) Enum.GetValues(typeof(ValueTypes));

		public static bool Verify(this IdentifierTypes type)
			=> _identifierTypeses.Any(x => x == type);

		public static bool Verify(this ValueTypes type)
			=> _valueTypes.Any(x => x == type);
	}
}
