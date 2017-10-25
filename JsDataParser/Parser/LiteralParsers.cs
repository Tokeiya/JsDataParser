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
using System.Runtime.CompilerServices;
using System.Text;
using Parseq;
using Parseq.Combinators;

[assembly: InternalsVisibleTo("JsDataParserTest")]
[assembly: InternalsVisibleTo("Playground")]


namespace JsDataParser.Parser
{
	using TypedCharEnumerableParser = Parser<char, (IEnumerable<char> captured, TokenTypes type)>;
	using CharEnumerableParser = Parser<char, IEnumerable<char>>;

	internal static class LiteralParsers
	{
		private static readonly CharacterCache Cache = new CharacterCache(1000, 1250);

		public static readonly CharEnumerableParser UnarySign = BuildUnarySign();

		public static readonly TypedCharEnumerableParser IntegerNumber = BuildIntegerNumber();

		public static readonly TypedCharEnumerableParser RealNumber = BuildRealNumber();

		public static readonly TypedCharEnumerableParser Text = BuildText();

		public static readonly TypedCharEnumerableParser Bool = BuildBool();

		public static readonly TypedCharEnumerableParser TinyFunction = BuildTinyFunction();

		public static readonly Parser<char, (IReadOnlyList<IEnumerable<char>> captured, TokenTypes tokenType)> ArrayParser =
			BuildArray();

		public static readonly TypedCharEnumerableParser Comment = BuildComment();

		public static readonly CharEnumerableParser IdentifierName =
			Combinator.Choice(Chars.Letter(), Chars.Char('_')).Many1();


		private static TypedCharEnumerableParser BuildComment()
		{
			var nl = Combinator.Choice(
				Chars.Sequence("\r\n").Ignore(),
				Chars.Char('\n').Ignore(),
				Chars.Char('\r').Ignore());

			var text = (from _ in nl.Not()
				from c in Chars.Any()
				select c).Many0();

			return from _ in Combinator.Sequence(Chars.WhiteSpace().Many0(), Chars.Sequence("//"))
				from cmnt in text
				from __ in nl
				select (cmnt, TokenTypes.Comment);
		}


		private static CharEnumerableParser BuildUnarySign()
		{
			var unaryExpr = Chars.Char('+').Or(Chars.Char('-')).Select(c => Cache.Get(c));

			return unaryExpr;
		}

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
				select (tmp.Concat(digit), TokenTypes.IntegerNumber);
		}

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


			return Combinator.Choice(pattern0, pattern1).Select(cap => (cap, TokenTypes.RealNumber));
		}

		private static TypedCharEnumerableParser BuildText()
		{
			var dict = new Dictionary<char, char[]>
			{
				['b'] = new[] {'\b'},
				['t'] = new[] {'\t'},
				['v'] = new[] {'\v'},
				['n'] = new[] {'\n'},
				['r'] = new[] {'\r'},
				['f'] = new[] {'\f'},
				['\''] = new[] {'\''},
				['\"'] = new[] {'\"'},
				['\\'] = new[] {'\\'},
				['0'] = new[] {'\0'}
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
				let chr = (char) byte.Parse(hexaValue, NumberStyles.HexNumber)
				select Cache.Get(chr);

			var utf16 =
				from quote in Chars.Char('\\')
				from _ in Chars.Char('u')
				from hexaValue in Combinator.Sequence(Chars.Hex(), Chars.Hex(), Chars.Hex(), Chars.Hex())
					.Select(seq => seq.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), b => b.ToString()))
				let chr = (char) uint.Parse(hexaValue, NumberStyles.HexNumber)
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

		private static TypedCharEnumerableParser BuildBool()
		{
			return Chars.Sequence("true").Or(Chars.Sequence("false")).Select(cap => (cap, TokenTypes.Boolean));
		}

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

			return function.Select(cap => (cap, TokenTypes.Function));
		}

		private static Parser<char, (IReadOnlyList<IEnumerable<char>> captured, TokenTypes tokenType)> BuildArray()
		{
			var whiteSpace = Chars.WhiteSpace().Many0().Select(_ => Cache.Get(' '));

			var array =
				from _ in Combinator.Sequence(Chars.Char('[').Ignore(), whiteSpace.Ignore())
				from values in Combinator.Sequence(whiteSpace, IntegerNumber
						.Select(x => x.captured), whiteSpace, Chars.Char(',').Select(c => Cache.Get(c)))
					.Select(cap => cap.Skip(1).Take(1)).Many0()
				from lastValue in Combinator.Sequence(whiteSpace, IntegerNumber.Select(x => x.captured), whiteSpace)
					.Select(cap => cap.Skip(1).Take(1))
				from __ in Chars.Char(']').Ignore()
				let fwd = values.SelectMany(x => x)
				select (IReadOnlyList<IEnumerable<char>>) fwd.Concat(lastValue).ToArray();

			var emptyArray = Combinator.Sequence(Chars.Char('[').Ignore(), whiteSpace.Ignore(), Chars.Char(']').Ignore())
				.Select(_ => (IReadOnlyList<IEnumerable<char>>) Array.Empty<IEnumerable<char>>());


			return emptyArray.Or(array).Select(cap => (cap, TokenTypes.IntegerArray));
		}
	}
}