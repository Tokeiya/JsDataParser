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
using System.IO;
using System.Linq;
using JsDataParser.Dynamic;
using JsDataParser.Entities;
using JsDataParser.Parser;
using Parseq;

namespace Playground
{
	internal class Program
	{
		private static void Main()
		{
			var obj = GetValue();

			dynamic d = new DynamicLiteralObject(obj);
			d = d[0];
			string s = d.name;

			IReadOnlyList<dynamic> ary = d.Array;

			foreach (var i in ary)
			{
				Console.WriteLine(i);
			}

		Console.WriteLine(s);

			Console.ReadLine();
		}

		private static ObjectEntity GetValue()
		{
			ObjectEntity ret=default;
			using (var rdr = new StreamReader(".\\Samples\\sample.txt"))
			{

				ObjectParser.LiteralObject.Run(rdr.AsStream()).Case(
					(stream, __) =>
					{
						try
						{
							Console.WriteLine(
								$"Line:{stream.Current.Value.Item1.Line} Column:{stream.Current.Value.Item1.Column} Token:{stream.Current.Value.Item0}");
						}
						catch (Exception)
						{
							Console.WriteLine("empty");
						}
					},
					(_, cap) =>
					{
						ret = cap;
						Console.WriteLine("success");
					});
			}

			return ret;
		}
	}
}