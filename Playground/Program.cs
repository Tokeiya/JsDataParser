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
using System.Linq.Expressions;
using JsDataParser.DataLoader;
using JsDataParser.Entities;
using JsDataParser.Mapping;

namespace Playground
{
	class Ship
	{
		public int Hp;
		public int HpMax;
		public int[] Slots = Array.Empty<int>();
	}

	static class Extension
	{
		public static void Dump(this object value)
			=> Console.WriteLine(value);

		public static void DumpAndQuery(this object value, string message = "Press enter to continue.")
		{
			Console.WriteLine(value);
			Console.WriteLine(message);
			Console.ReadLine();
		}
	}

	internal class Program
	{
		private const string Path = ".\\Samples\\shipdata.txt";
		private static void Main()
		{
			var raw = DataLoader.LoadRaw(Path);
			var ships = TinyMapper<Ship>.MultiMap(raw);




			IEnumerable<(dynamic key, dynamic value)> dynamIterator = DataLoader.LoadAsDynamic(Path);

			var hoge = dynamIterator
				.SelectMany(x => (IEnumerable<dynamic>) x.value.SLOTS??Enumerable.Empty<object>())
				.Select(x => (int?) x).Sum();

			hoge.DumpAndQuery();

		}
	}
}