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
using System.Linq;
using JsDataParser.DataLoader;
using JsDataParser.Mapping;

namespace Playground
{
	internal static class Extends
	{
		public static void Dump(this object source)
		{
			if (string.IsNullOrWhiteSpace(source?.ToString() ?? ""))
				Console.WriteLine("Empty");
			else
				Console.WriteLine(source);
		}
	}

	internal class Dumper
	{
		public void Dump()
		{
			Console.WriteLine("Dump called.");
		}
	}

	class ItemType
	{
		public string Name { get; set; }
		public int Image { get; set; }
		public dynamic Improve { get; set; }
		public string[] CanEquip { get; set; }
	}


	internal class Program
	{
		private const string Path = ".\\Samples\\itemtype.txt";


		private static void Main()
		{
			var raw = DataLoader.LoadCollectionRaw(Path);

			Console.WriteLine(raw[0].index.Identity);
			Console.WriteLine(raw[0].index.Index.Identity);

			var mapper = new PeripheralMapper<ItemType>();

			var ret = raw.Select(x => (index:x.index.Identity, value:mapper.Map(x.value))).ToArray();

			Console.WriteLine(ret[0].value.Improve.ACCnb);

		}

		
	}
}