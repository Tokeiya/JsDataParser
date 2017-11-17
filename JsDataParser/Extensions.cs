/*
 * Copyright (C) 2017 Hikotaro Abe <net_seed@hotmail.com> All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsDataParser.Entities;
using JsDataParser.Mapping;

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

		private static readonly DynamicMappedTypes[] _dynamicEntityTypes =
			(DynamicMappedTypes[]) Enum.GetValues(typeof(DynamicMappedTypes));

		public static bool Verify(this IdentifierTypes type)
		{
			return _identifierTypeses.Any(x => x == type);
		}

		public static bool Verify(this ValueTypes type)
		{
			return _valueTypes.Any(x => x == type);
		}

		public static bool Verify(this RepresentTypes type)
		{
			return _dynamicTypes.Any(x => x == type);
		}

		public static bool Verify(this DynamicMappedTypes type)
		{
			return _dynamicEntityTypes.Any(x => x == type);
		}


		public static string BuildString(this IEnumerable<char> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var bld = new StringBuilder();

			foreach (var c in source)
				bld.Append(c);

			return bld.ToString();
		}


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