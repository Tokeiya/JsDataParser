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

namespace JsDataParser.Parser
{
	public class DatumExpression
	{
		private readonly Dictionary<string, FieldValueExpression> _dict = new Dictionary<string, FieldValueExpression>();

		public DatumExpression(int id, IEnumerable<FieldExpression> fields)
		{
			if (fields == null) throw new ArgumentNullException(nameof(fields));

			Id = id;

			try
			{
				foreach (var field in fields)
					_dict.Add(field.Name, field);
			}
			catch (NullReferenceException ex)
			{
				throw new ArgumentException($"{nameof(fields)} contains null value.", ex);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException("Detect field name duplication!", ex);
			}
		}

		public int Id { get; }
		public IReadOnlyDictionary<string, FieldValueExpression> Fields => _dict;
	}
}