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
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Parseq;
using Parseq.Combinators;

[assembly: InternalsVisibleTo("JsDatumParserTest")]
[assembly: InternalsVisibleTo("Playground")]


namespace JsDatumParser
{
	using TypedCharEnumerableParser = Parser<char, (IEnumerable<char> captured, TokenTypes type)>;
	using CharEnumerableParser=Parser<char,IEnumerable<char>>;

	internal static class LiteralParsers
	{
		private static readonly CharacterCache Cache = new CharacterCache(1000, 1250);


		private static CharEnumerableParser BuildUnarySign()
		{
			var unaryExpr = Chars.Char('+').Or(Chars.Char('-')).Select(c=>Cache.Get(c));

			return unaryExpr;
		}

		public static readonly CharEnumerableParser UnarySign = BuildUnarySign();

		private static TypedCharEnumerableParser BuildIntegerNumber()
		{
			//+10
			//10
			//-114514

			var digits = Chars.Digit().Many1();

			return
				from sign in UnarySign.Optional()
				let tmp = sign.HasValue ? sign.Value : Array.Empty<char>()
				from digit in digits
				select (tmp.Concat(digit),TokenTypes.IntegerNumber);

		}

		public static readonly TypedCharEnumerableParser IntegerNumber=BuildIntegerNumber();

		private static TypedCharEnumerableParser BuildRealNumber()
		{
			//1.0
			//1.
			//.1

			var pattern0 =
				from sign in UnarySign.Optional()
				let signChar = sign.HasValue ? sign.Value : Array.Empty<char>()
				from integerPart in Chars.Digit().Many1()
				from decimalDot in Chars.Char('.').Select(c => Cache.Get('.'))
				from fractionPart in Chars.Digit().Many0()
				select signChar.Concat(integerPart).Concat(decimalDot).Concat(fractionPart);


			var pattern1 =
				from sign in UnarySign.Optional()
				let signChar = sign.HasValue ? sign.Value : Array.Empty<char>()
				from integerPart in Chars.Digit().Many0()
				from decimalDot in Chars.Char('.').Select(c => Cache.Get('.'))
				from fractionPart in Chars.Digit().Many1()
				select signChar.Concat(integerPart).Concat(decimalDot).Concat(fractionPart);


			return  Combinator.Choice(pattern0, pattern1).Select(cap=>(cap,TokenTypes.RealNumber));
		}

		public static readonly TypedCharEnumerableParser RealNumber = BuildRealNumber();

		private static TypedCharEnumerableParser BuildText()
		{
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
				['0'] = new[] { '\0' },
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

			return text.Select(cap => (cap, TokenTypes.Text));
		}

		public static readonly TypedCharEnumerableParser Text = BuildText();

		private static TypedCharEnumerableParser BuildBool()
		{
			return Chars.Sequence("true").Or(Chars.Sequence("false")).Select(cap=>(cap,TokenTypes.Boolean));
		}

		public static readonly TypedCharEnumerableParser Bool = BuildBool();

		private static TypedCharEnumerableParser BuildTinyFunction()
		{
			var function =
				from functionIdent in Chars.Sequence("function").Ignore()
				from space0 in Chars.WhiteSpace().Many0().Ignore()
				from lp in Chars.Char('(').Ignore()
				from args in Chars.NoneOf(')').Many0()
				from rp in Chars.Char(')').Ignore()
				from space1 in Chars.WhiteSpace().Many0()
				from lcb in Chars.Char('{').Select(c => Cache.Get(c))
				from contents in Chars.NoneOf('}').Many0()
				from rcb in Chars.Char('}').Select(c => Cache.Get(c))
				let header = (IEnumerable<char>) "function ("
				select header.Concat(args).Concat(") {").Concat(contents).Concat("}");

			return function.Select(cap=>(cap,TokenTypes.Function));
		}

		public static readonly TypedCharEnumerableParser TinyFunction=BuildTinyFunction();

	}
}
