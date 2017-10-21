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
	using static JsDatumParser.LiteralParsers;


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
	}

	class Program
	{
		static void Main(string[] args)
		{
			// ReSharper disable once InconsistentNaming
			var Cache = new CharacterCache(100, 150);


			var dict = new Dictionary<char, char[]>
			{
				['b'] = new[] { '\b' },
				['t'] = new[] { '\t' },
				['v'] = new[] { '\v' },
				['n'] = new[] { '\n' },
				['r'] = new[] { '\r' },
				['f'] = new[] { '\f' },
				['\''] = new[] { '\'' },
				['\"'] = new[] { '\"' },
				['\\'] = new[] { '\\' },
				['\0'] = new[] { '\0' },
			};


			var normalEscape =
				from quote in Chars.Char('\\')
				from character in Chars.Satisfy(c => dict.ContainsKey(c))
				let tmp = dict[character]
				select tmp;


			var latin1 =
				from quote in Chars.Char('\\')
				from _ in Chars.Char('x')
				from hexaValue in Combinator.Sequence(Chars.Hex(), Chars.Hex()).Select(seq =>
					seq.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString()))
				let chr = (char)byte.Parse(hexaValue, NumberStyles.HexNumber)
				select Cache.Get(chr);

			var utf16 =
				from quote in Chars.Char('\\')
				from _ in Chars.Char('u')
				from hexaValue in Combinator.Sequence(Chars.Hex(), Chars.Hex(), Chars.Hex(), Chars.Hex())
					.Select(seq => seq.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), b => b.ToString()))
				let chr = (char)uint.Parse(hexaValue, NumberStyles.HexNumber)
				select Cache.Get(chr);


			var nonEscapeCharacter = Chars.NoneOf('\\', '\'', '\"').Select(c => Cache.Get(c));

			var synth = Combinator.Choice(
				nonEscapeCharacter,
				normalEscape,
				latin1,
				utf16);


			var text = from quote in Chars.Char('\'').Or(Chars.Char('\"'))
				from contents in synth.Many0()
				from _ in Chars.Char(quote)
				select contents.SelectMany(c => c);


			text.Run("\"\\b\"".AsStream()).Dump();



		}
	}
}
