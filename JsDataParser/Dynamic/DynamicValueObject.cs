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
using System.Dynamic;
using System.Text;
using JsDataParser.Entities;

namespace JsDataParser.Dynamic
{
	internal class DynamicValueObject:DynamicObject
	{
		private readonly ValueEntity _value;

		public DynamicValueObject(ValueEntity value)
			=> _value = value ?? throw new ArgumentNullException(nameof(value));

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (binder.Name.ToLower() == "function"&&_value.ValueType== ValueTypes.Function)
			{
				result = _value.Function;
				return true;
			}

			if (binder.Name.ToLower()=="constant"&&_value.ValueType== ValueTypes.ConstantName)
			{
				result = _value.Constant;
				return true;
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			//Strict
			if (binder.Type == typeof(object))
			{
				result = _value.Object;
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

			result = default;
			return false;
		}

		public override string ToString() => _value.Object.ToString();
	}
}
