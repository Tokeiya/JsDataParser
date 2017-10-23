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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Parseq;
using Parseq.Combinators;
using JsDatumParser;

namespace Playground
{
	using CharEnumerable=IEnumerable<char>;
	using ChainFunc=Func<string,string,string>;


	static class Extension
	{
		public static bool TryGetValue(this IReply<char, IEnumerable<char>> reply, out string value)
		{
			var result = reply.Case(_ => (result:false, value:default),
				seq => (result:true, value:seq.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString())));

			value = result.value;
			return result.result;
		}

		public static void Dump(this IReply<char, IEnumerable<char>> reply)
		{
			if (reply.TryGetValue(out var ret))
			{
				Console.WriteLine(ret);
			}
			else
			{
				Console.WriteLine("Fail!");
			}
		}

		public static string BuildString(this IEnumerable<char> source)
			=> source.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString());
	}

	class Program
	{
		static void Main()
		{
			// ReSharper disable once InconsistentNaming
			var Cache = new CharacterCache(100, 150);

			var number = Chars.Digit().Many1().Select(c=>c.BuildString());

			var additive = Chars.Char('+')
				.Select(_ => (ChainFunc) ((l, r) => $"({l}+{r})"));

			var subtractive = Chars.Char('-')
				.Select(_ => (ChainFunc) ((l, r) => $"({l}-{r})"));

			var expr = number.Chainl(additive.Or(subtractive)).Or(number);
			var ret = expr.Run("42".AsStream());

			ret.Case((str, txt) => Console.WriteLine(txt),
				(str, txt) => Console.WriteLine(txt));


			ret = expr.Run("42+84-114514".AsStream());

			ret.Case((str, txt) => Console.WriteLine(txt),
				(str, txt) => Console.WriteLine(txt));



		}
	}
}
