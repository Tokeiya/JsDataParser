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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsDataParser.Entities;

namespace JsDataParser.Mapping
{
	internal class DynamicMappedValue : DynamicMappedObject
	{
		private readonly ValueEntity _value;

		public DynamicMappedValue(ValueEntity value) : base(GetType(value), DynamicMappedTypes.Value)
		{
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}


		private static RepresentTypes GetType(ValueEntity value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			switch (value.ValueType)
			{
				case ValueTypes.Array:
					return RepresentTypes.Array;

				case ValueTypes.Boolean:
					return RepresentTypes.Boolean;

				case ValueTypes.Identity:
					return RepresentTypes.Identity;

				case ValueTypes.Integer:
					return RepresentTypes.Integer;

				case ValueTypes.Object:
					return RepresentTypes.Object;

				case ValueTypes.Real:
					return RepresentTypes.Real;

				case ValueTypes.String:
					return RepresentTypes.String;

				case ValueTypes.Function:
					return RepresentTypes.Function;

				default:
					throw new InvalidOperationException();
			}
		}

		public string Function => _value.Function;
		public string Identity => _value.Identity;


		public static implicit operator ValueEntity(DynamicMappedValue obj) => obj?._value;

		public static implicit operator int(DynamicMappedValue obj)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			if (obj._value.ValueType == ValueTypes.Real) return (int) obj._value.Real;

			 return obj._value.Integer;
		}



		public static implicit operator double(DynamicMappedValue obj)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			if (obj._value.ValueType == ValueTypes.Integer) return obj._value.Integer;
			return obj._value.Real;
		}

		public static implicit operator bool(DynamicMappedValue obj)
			=> obj?._value.Boolean ?? throw new ArgumentNullException(nameof(obj));

		public static implicit operator string(DynamicMappedValue obj)
			=> obj?._value.String ?? throw new ArgumentNullException(nameof(obj));



		public override string ToString()
		{
			return _value.Object.ToString();
		}
	}
}