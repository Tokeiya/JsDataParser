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

		private static readonly RepresentTypes[] _dynamicTypes =
			(RepresentTypes[]) Enum.GetValues(typeof(RepresentTypes));

		private static readonly DynamicEntityTypes[] _dynamicEntityTypes =
			(DynamicEntityTypes[]) Enum.GetValues(typeof(DynamicEntityTypes));

		public static bool Verify(this IdentifierTypes type) => _identifierTypeses.Any(x => x == type);

		public static bool Verify(this ValueTypes type) => _valueTypes.Any(x => x == type);

		public static bool Verify(this RepresentTypes type) => _dynamicTypes.Any(x => x == type);

		public static bool Verify(this DynamicEntityTypes type) => _dynamicEntityTypes.Any(x => x == type);


		public static string BuildString(this IEnumerable<char> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var bld = new StringBuilder();

			foreach (var c in source)
				bld.Append(c);

			return bld.ToString();
		}


		public static bool IsNull<T>(this T value) => NullChecker<T>.IsNull(value);

		public static bool IsNotNull<T>(this T value) => !IsNull(value);

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