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
using System.IO;
using JsDataParser.Entities;
using JsDataParser.Mapping;
using JsDataParser.Parser;
using Parseq;

namespace JsDataParser.DataLoader
{
	public static class DataLoader
	{
		public static ObjectLiteralEntity LoadRaw(string path)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));

			using (var rdr = new StreamReader(path))
			{
				return LoadRaw(rdr);
			}
		}

		public static ObjectLiteralEntity LoadRaw(TextReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			ObjectLiteralEntity ret = null;

			ObjectParser.LiteralObject.Run(reader.AsStream())
				.Case(
					(stream, message) =>
					{
						var lineNum = stream.Current.HasValue ? stream.Current.Value.Item1.Line : -1;
						var col = stream.Current.HasValue ? stream.Current.Value.Item1.Column : -1;
						throw new LoadException(lineNum, col, message);
					}
					,
					(_, cap) => ret = cap
				);

			return ret;
		}

		public static dynamic LoadAsDynamic(string path)
		{
			using (var rdr = new StreamReader(path))
			{
				return LoadAsDynamic(rdr);
			}
		}

		public static dynamic LoadAsDynamic(TextReader reader)
		{
			var obj = LoadRaw(reader);

			return new DynamicMappedLiteralObject(obj);
		}
	}
}