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
	internal class DynamicMappedLiteralObject : DynamicMappedObject, IDynamicMappedLiteralObject
	{
		private readonly ObjectLiteralEntity _entity;

		public DynamicMappedLiteralObject(ObjectLiteralEntity entity) : base(RepresentTypes.Object, DynamicMappedTypes.Object)
		{
			_entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}


		public bool TryGetField(string identity, out dynamic value)
		{
			if (identity == null) throw new ArgumentNullException(nameof(identity));

			var key = new IdentifierEntity(identity, true);

			if (_entity.TryGetValue(key, out var ret))
			{
				value = new DynamicMappedValue(ret);
				return true;
			}

			value = default;
			return false;
		}

		public IEnumerator<(object key, object value)> GetEnumerator()
		{
			object select(ValueEntity entity)
			{
				if (entity.ValueType == ValueTypes.Object) return new DynamicMappedLiteralObject(entity.NestedObject);
				return new DynamicMappedValue(entity);
			}

			return _entity
				.Select(x => ((object) new DynamicIdentifierMapping(x.Key), select(x.Value)))
				.GetEnumerator();
		}


		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		public dynamic this[object index]
		{
			get
			{
				if (index == null) throw new NullReferenceException($"{nameof(index)} is null");
				IdentifierEntity entity;

				switch (index)
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
						throw new InconsistencyException();
				}

				if (_entity.TryGetValue(entity, out var ret))
				{
					return ret.ValueType == ValueTypes.Object
						? (object)new DynamicMappedLiteralObject(ret.NestedObject)
						: new DynamicMappedValue(ret);
				}

				return default;
			}
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

		public static implicit operator ObjectLiteralEntity(DynamicMappedLiteralObject obj) => obj?._entity;

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			object selector(ValueEntity entity)
			{
				switch (entity.ValueType)
				{
					case ValueTypes.Array:
						return entity.Array.Select(selector).ToArray();

					case ValueTypes.Object:
						return new DynamicMappedLiteralObject(entity.NestedObject);

					default:
						return new DynamicMappedValue(entity);
				}
			}

			object tmp;

			tmp = _entity.TryGetValue(new IdentifierEntity(binder.Name, true), out var ret)
				? selector(ret)
				: default;

			result = tmp;
			return true;
		}
	}
}