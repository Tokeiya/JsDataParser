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

namespace JsDataParser.Parser
{
	public class PropertyEntity
	{
		private readonly IEnumerable<char> _value;
		private readonly ObjectEntity _nestedObject;
		private readonly IReadOnlyList<IEnumerable<char>> _arrayValue;

		public PropertyEntity(string name, IEnumerable<char> value)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException($"name is empty");

			Name = name;
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}

		public PropertyEntity(string name, ObjectEntity nestedObject)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException($"name is empty");

			Name = name;
			_nestedObject = nestedObject ?? throw new ArgumentNullException(nameof(nestedObject));
		}

		public PropertyEntity(string name, IReadOnlyList<IEnumerable<char>> arrayValue)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException($"name is empty");

			if (arrayValue == null) throw new ArgumentNullException(nameof(arrayValue));
			if (arrayValue.Any(x => x == null)) throw new ArgumentException($"{nameof(arrayValue)} contains null.");

			Name = name;
			_arrayValue = arrayValue;

		}

		public string Name { get; }

		public IEnumerable<char> Value => _value ?? throw new InvalidOperationException();

		public string ValueToString() =>
			Value.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString());

		public ObjectEntity NestedObject => _nestedObject ?? throw new InvalidOperationException();

	}
}
