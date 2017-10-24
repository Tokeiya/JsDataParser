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

namespace JsDataParser.Parser
{
	public class FieldValueExpression
	{
		private readonly object _source;

		public FieldValueExpression(IEnumerable<char> source, TokenTypes fieldType)
		{
			_source = source ?? throw new ArgumentNullException(nameof(source));

			switch (fieldType)
			{
				case TokenTypes.Boolean:
				case TokenTypes.Function:
				case TokenTypes.IntegerNumber:
				case TokenTypes.RealNumber:
				case TokenTypes.Text:
					break;

				default:
					throw new ArgumentException(nameof(fieldType));
			}

			FieldType = fieldType;
		}

		public FieldValueExpression(IReadOnlyCollection<IEnumerable<char>> arraySource)
		{
			if (arraySource.Any(x => x == null)) throw new ArgumentException($"{nameof(arraySource)} contains null");
			_source = arraySource ?? throw new ArgumentNullException(nameof(arraySource));

			FieldType = TokenTypes.IntegerArray;
		}


		public IEnumerable<char> Source
		{
			get
			{
				if (TokenTypes.IntegerArray == FieldType) throw new InvalidOperationException();

				return (IEnumerable<char>) _source;
			}
		}

		public IReadOnlyList<IEnumerable<char>> ArraySource
		{
			get
			{
				if (TokenTypes.IntegerArray != FieldType) throw new InvalidOperationException();

				return (IReadOnlyList<IEnumerable<char>>) _source;
			}
		}

		public TokenTypes FieldType { get; }
	}
}