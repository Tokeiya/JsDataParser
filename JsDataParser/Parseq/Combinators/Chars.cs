/*
 * Copyright (C) 2012 - 2015 Takahisa Watanabe <linerlock@outlook.com> All rights reserved.
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

namespace Parseq.Combinators
{
	public static class Chars
	{
		public static Parser<char, Unit> EndOfInput()
		{
			return stream => stream.Current.HasValue
				? Reply.Failure<char, Unit>(stream, "Failure: Chars.EndOfInput")
				: Reply.Success(stream, Unit.Instance);
		}

		public static Parser<char, char> Any()
		{
			return stream => stream.Current.HasValue
				? Reply.Success(stream.MoveNext(), stream.Current.Value.Item0)
				: Reply.Failure<char, char>(stream, "Failure: Chars.Any");
		}

		public static Parser<char, char> Char(char c)
		{
			return stream => stream.Current.HasValue && stream.Current.Value.Item0 == c
				? Reply.Success(stream.MoveNext(), c)
				: Reply.Failure<char, char>(stream, "Failure: Chars.Char");
		}

		public static Parser<char, char> Char(Func<char, bool> predicate)
		{
			return stream => stream.Current.HasValue && predicate(stream.Current.Value.Item0)
				? Reply.Success(stream.MoveNext(), stream.Current.Value.Item0)
				: Reply.Failure<char, char>(stream, "Failure: Chars.Char");
		}

		public static Parser<char, IEnumerable<char>> Sequence(IEnumerable<char> enumerble)
		{
			return enumerble.Select(Char).Sequence().Attempt();
		}

		public static Parser<char, IEnumerable<char>> Sequence(params char[] charArray)
		{
			return Sequence(charArray.AsEnumerable());
		}

		public static Parser<char, IEnumerable<char>> Sequence(string s)
		{
			return Sequence(s.ToCharArray());
		}

		public static Parser<char, char> OneOf(IEnumerable<char> candidates)
		{
			return candidates.Select(Char).Choice();
		}

		public static Parser<char, char> OneOf(params char[] candidates)
		{
			return OneOf(candidates.AsEnumerable());
		}

		public static Parser<char, char> NoneOf(IEnumerable<char> candidates)
		{
			return candidates.Select(c => Char(c).Not()).Sequence().Bindr(Any());
		}

		public static Parser<char, char> NoneOf(params char[] candidates)
		{
			return NoneOf(candidates.AsEnumerable());
		}

		public static Parser<char, char> Digit()
		{
			return Char(char.IsDigit);
		}

		public static Parser<char, char> Letter()
		{
			return Char(char.IsLetter);
		}

		public static Parser<char, char> LetterOrDigit()
		{
			return Char(char.IsLetterOrDigit);
		}

		public static Parser<char, char> Lower()
		{
			return Char(char.IsLower);
		}

		public static Parser<char, char> Upper()
		{
			return Char(char.IsUpper);
		}

		public static Parser<char, char> WhiteSpace()
		{
			return Char(char.IsWhiteSpace);
		}

		public static Parser<char, char> Oct()
		{
			return OneOf('0', '1', '2', '3', '4', '5', '6', '7');
		}

		public static Parser<char, char> Hex()
		{
			return OneOf(
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
				'a', 'b', 'c', 'd', 'e', 'f',
				'A', 'B', 'C', 'D', 'E', 'F');
		}

		public static Parser<char, char> Satisfy(char c)
		{
			return Char(c);
		}

		public static Parser<char, char> Satisfy(Func<char, bool> predicate)
		{
			return Char(predicate);
		}
	}
}