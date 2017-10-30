using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsDataParser.Entities;

// ReSharper disable InconsistentNaming

namespace JsDataParser
{
	internal static class Extensions
	{
		private static readonly IdentifierTypes[] _identifierTypeses =
			(IdentifierTypes[]) Enum.GetValues(typeof(IdentifierTypes));

		private static readonly ValueTypes[] _valueTypes =
			(ValueTypes[]) Enum.GetValues(typeof(ValueTypes));

		private static readonly DynamicTypes[] _dynamicTypes =
			(DynamicTypes[]) Enum.GetValues(typeof(DynamicTypes));

		public static bool Verify(this IdentifierTypes type)
		{
			return _identifierTypeses.Any(x => x == type);
		}

		public static bool Verify(this ValueTypes type)
		{
			return _valueTypes.Any(x => x == type);
		}

		public static bool Verify(this DynamicTypes type)
		{
			return _dynamicTypes.Any(x => x == type);
		}


		public static string BuildString(this IEnumerable<char> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var bld = new StringBuilder();

			foreach (var c in source)
				bld.Append(c);

			return bld.ToString();
		}


		//新型のNull/IsNull
		public static bool IsNull<T>(this T value)
		{
			return NullChecker<T>.IsNull(value);
		}

		public static bool IsNotNull<T>(this T value)
		{
			return !IsNull(value);
		}

		private static class NullChecker<T>
		{
			private static readonly TypeType _type;

			static NullChecker()
			{
				var type = typeof(T);

				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
					_type = TypeType.Nullable;
				else if (type.IsValueType)
					_type = TypeType.Value;
				else
					_type = TypeType.Reference;
			}

			public static bool IsNull(T value)
			{
				switch (_type)
				{
					case TypeType.Value:
						return false;

					case TypeType.Reference:
						return value == null;

					case TypeType.Nullable:
						return value.Equals(null);

					default:
						throw new InvalidOperationException("Unexpected type");
				}
			}

			private enum TypeType
			{
				Value,
				Reference,
				Nullable
			}
		}
	}
}