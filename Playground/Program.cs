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
using System.IO;
using System.Linq;
using JsDataParser.DataLoader;
using JsDataParser.Parser;
using Parseq;

namespace Playground
{
	using CharEnumerable = IEnumerable<char>;
	using ChainFunc = Func<string, string, string>;

	using static Parseq.Combinator;
	using static DataParser;




	internal class Program
	{
		private static void Main()
		{
			var data = Loader.LoadDatumExpression(".\\Samples\\HugeSample.txt");

			var dict = new Dictionary<string, int>();


			foreach (var elem in data.Select(x=>x.Fields).Select(x=>x.Keys))
			{
				foreach (var name in elem)
				{
					if (dict.ContainsKey(name))
					{
						dict[name]++;
					}
					else
					{
						dict.Add(name, 1);
					}
				}
			}


			using (var wtr = new StreamWriter("E:\\Fields.txt"))
			{
				wtr.WriteLine("Name\tCount");
				foreach (var elem in dict)
				{
					wtr.WriteLine($"{elem.Key}\t{elem.Value}");
				}
			}



		}


		private static void TotalTest()
		{
			string scr;

			using (var rdr = new StreamReader(".//Samples//HugeSample.txt"))
			{
				scr = rdr.ReadToEnd();
			}

			var reply = DataParser.Data.Run(scr.AsStream());

			reply.Case(
				(str, txt) =>
				{
					Console.WriteLine(txt);
					Console.WriteLine("Line:" + str.Current.Value.Item1.Line);
					Console.WriteLine("Column:" + str.Current.Value.Item1.Column);
				},
				(_, cap) => Console.WriteLine("success"));

			Console.WriteLine("Press enter to exit.");
			Console.ReadLine();
		}
	}
}