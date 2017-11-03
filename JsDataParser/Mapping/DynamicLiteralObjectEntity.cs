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
	internal class DynamicLiteralObjectMapping : DynamicMappingObject, IDynamicLiteralObjectEntity
	{
		private readonly ObjectLiteralEntity _entity;

		public DynamicLiteralObjectMapping(ObjectLiteralEntity entity) : base(RepresentTypes.Object, DynamicMappingTypes.Object)
		{
			_entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}


		public bool TryGetField(string identity, out dynamic value)
		{
			if (identity == null) throw new ArgumentNullException(nameof(identity));

			var key = new IdentifierEntity(identity, true);

			if (_entity.TryGetValue(key, out var ret))
			{
				value = new DynamicValueMapping(ret);
				return true;
			}

			value = default;
			return false;
		}

		public IEnumerator<(object key, object value)> GetEnumerator()
		{
			return _entity.Select(x => ((object) new DynamicIdentifierMapping(x.Key), (object) new DynamicValueMapping(x.Value)))
				.GetEnumerator();
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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

			if (_entity.TryGetValue(entity, out var ret))
				switch (ret.ValueType)
				{
					case ValueTypes.Object:
						tmp = new DynamicLiteralObjectMapping(ret.NestedObject);
						break;

					default:
						tmp = new DynamicValueMapping(ret);
						break;
				}
			else
				tmp = default;


			result = tmp;
			return true;
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if (binder.Type.IsAssignableFrom(typeof(ObjectLiteralEntity)))
			{
				result = _entity;
				return true;
			}

			result = default;
			return false;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			object tmp;

			if (_entity.TryGetValue(new IdentifierEntity(binder.Name, true), out var ret))
				if (ret.ValueType == ValueTypes.Object)
					tmp = new DynamicLiteralObjectMapping(ret.NestedObject);
				else
					tmp = new DynamicValueMapping(ret);
			else
				tmp = default;

			result = tmp;
			return true;
		}
	}
}