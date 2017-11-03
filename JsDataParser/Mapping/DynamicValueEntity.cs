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
using System.Dynamic;
using System.Linq;
using JsDataParser.Entities;

namespace JsDataParser.Mapping
{
	internal class DynamicValueMapping : DynamicMappedObject
	{
		private readonly ValueEntity _value;

		public DynamicValueMapping(ValueEntity value) : base(GetType(value), DynamicMappedTypes.Value)
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

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (binder.Name.ToLower() == "function" && _value.ValueType == ValueTypes.Function)
			{
				result = _value.Function;
				return true;
			}

			if (binder.Name.ToLower() == "identity" && _value.ValueType == ValueTypes.Identity)
			{
				result = _value.Identity;
				return true;
			}
			throw new InvalidOperationException();
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			result = default;

			if (indexes.Length != 1 || !(indexes[0] is int) || _value.ValueType != ValueTypes.Array) return false;
			// ReSharper disable once PossibleInvalidCastException
			var idx = (int) indexes[0];

			if (idx < 0 || idx > _value.Array.Count) return false;

			result = new DynamicValueMapping(_value.Array[idx]);
			return true;
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			//Strict

			//Object should be always on top.
			if (binder.Type == typeof(object))
			{
				result = _value.Object;
				return true;
			}

			if (binder.Type.IsAssignableFrom(typeof(ValueEntity)))
			{
				result = _value;
				return true;
			}


			if (binder.Type == typeof(int) && _value.ValueType == ValueTypes.Integer)
			{
				result = _value.Integer;
				return true;
			}

			if (binder.Type == typeof(double) && _value.ValueType == ValueTypes.Real)
			{
				result = _value.Real;
				return true;
			}

			if (binder.Type == typeof(bool) && _value.ValueType == ValueTypes.Boolean)
			{
				result = _value.Boolean;
				return true;
			}

			if (binder.Type == typeof(string) && _value.ValueType == ValueTypes.String)
			{
				result = _value.String;
				return true;
			}

			if (binder.Type == typeof(IEnumerable) && _value.ValueType == ValueTypes.Array)
			{
				result =(IReadOnlyList<DynamicValueMapping>) _value.Array.Select(x => new DynamicValueMapping(x)).ToArray();
				return true;
			}

			//Implicit
			if (binder.Type == typeof(double) && _value.ValueType == ValueTypes.Integer)
			{
				result = (double) _value.Integer;
				return true;
			}

			if (binder.Type == typeof(int) && _value.ValueType == ValueTypes.Real)
			{
				result = (int) _value.Real;
				return true;
			}


			result = default;
			return false;
		}

		public override string ToString()
		{
			return _value.Object.ToString();
		}
	}
}