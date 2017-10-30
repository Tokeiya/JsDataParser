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
using JsDataParser.Entities;

namespace JsDataParser.Entities
{
	internal class DynamicLiteralObjectEntity : DynamicObject
	{
		private readonly ObjectLiteralEntity _entity;

		public DynamicLiteralObjectEntity(ObjectLiteralEntity entity)
			=> _entity = entity ?? throw new ArgumentNullException(nameof(entity));


		private IReadOnlyList<dynamic> BuildArray(IReadOnlyList<ValueEntity> source)
		{
			var ret = new dynamic[source.Count];

			for (int i = 0; i < ret.Length; i++)
			{
				var piv = source[i];

				if (piv.ValueType == ValueTypes.Array)
				{
					ret[i] = BuildArray(piv.Array);
				}
				else if (piv.ValueType == ValueTypes.Object)
				{
					ret[i]=new DynamicLiteralObjectEntity(piv.NestedObject);
				}
				else
				{
					ret[i] = new DynamicValueEntity(piv);
				}
			}

			return ret;
		}


		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			if (indexes.Length != 1) throw new InvalidOperationException();

			IdentifierEntity entity;

			var idx = indexes[0];

			switch (idx)
			{
				case double d:
					entity = new IdentifierEntity(d);
					break;

				case int i:
					entity = new IdentifierEntity(i);
					break;

				case bool b:
					entity = new IdentifierEntity(b);
					break;

				case string s:
					entity = new IdentifierEntity(s, false);
					break;

				default:
					throw new InvalidOperationException();
			}

			object tmp;

			if(_entity.TryGetValue(entity,out var ret))
			{
				switch (ret.ValueType)
				{
					case ValueTypes.Array:
						tmp = BuildArray(ret.Array);
						break;

					case ValueTypes.Object:
						tmp=new DynamicLiteralObjectEntity(ret.NestedObject);
						break;

					default:
						tmp = new DynamicValueEntity(ret);
						break;
				}
			}
			else
			{
				tmp = default;
			}


			result = tmp;
			return true;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			object tmp;

			if (_entity.TryGetValue(new IdentifierEntity(binder.Name, true), out var ret))
			{
				if (ret.ValueType == ValueTypes.Array)
				{
					tmp = BuildArray(ret.Array);
				}
				else if (ret.ValueType == ValueTypes.Object)
				{
					tmp = new DynamicLiteralObjectEntity(ret.NestedObject);
				}
				else
				{
					tmp = new DynamicValueEntity(ret);
				}
			}
			else
			{
				tmp = default;
			}

			result = tmp;
			return true;
		}
	}



}
