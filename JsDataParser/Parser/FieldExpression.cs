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
	public class FieldExpression : FieldValueExpression
	{
		public FieldExpression(string name, IEnumerable<char> source, TokenTypes fieldType)
			: base(source, fieldType)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

			Name = name;
		}

		public FieldExpression(string name, IReadOnlyCollection<IEnumerable<char>> source)
			: base(source)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

			Name = name;
		}

		public string Name { get; }

		public static FieldExpression Create(IEnumerable<char> name, FieldValueExpression value)
		{
			if (value == null) throw new ArgumentException(nameof(value));
			if (name == null) throw new ArgumentNullException(nameof(name));

			var tmp = name.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString());


			if (value.FieldType == TokenTypes.IntegerArray)
				return new FieldExpression(tmp, value.ArraySource);

			return new FieldExpression(tmp, value.Source, value.FieldType);
		}
	}
}