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
using System.Diagnostics;

namespace JsDataParser.Entities
{
	[DebuggerStepThrough]
	public class ObjectLiteralEntity : IReadOnlyDictionary<IdentifierEntity, ValueEntity>
	{
		private readonly Dictionary<IdentifierEntity, ValueEntity> _values = new Dictionary<IdentifierEntity, ValueEntity>();

		public ObjectLiteralEntity(IEnumerable<(IdentifierEntity identifier, ValueEntity value)> properties)
		{
			if (properties == null) throw new ArgumentNullException(nameof(properties));

			foreach (var elem in properties)
			{
				if (elem.value == null) throw new ArgumentException($"Contains null value.", nameof(properties));
				if (elem.identifier == null) throw new ArgumentException("Contains null identifier", nameof(properties));

				if (_values.ContainsKey(elem.identifier))
					_values[elem.identifier] = elem.value;
				else
					_values.Add(elem.identifier, elem.value);
			}
		}

		public IEnumerator<KeyValuePair<IdentifierEntity, ValueEntity>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count => _values.Count;

		public bool ContainsKey(IdentifierEntity key)
		{
			return _values.ContainsKey(key);
		}

		public bool TryGetValue(IdentifierEntity key, out ValueEntity value)
		{
			return _values.TryGetValue(key, out value);
		}

		public ValueEntity this[IdentifierEntity key] => _values[key];

		public IEnumerable<IdentifierEntity> Keys => _values.Keys;
		public IEnumerable<ValueEntity> Values => _values.Values;
	}
}