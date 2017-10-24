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
using System.Text;
using JsDataParser;
using JsDataParser.Parser;
using Parseq;
using Xunit;

namespace JsDataParserTest
{
	public static class Helper
	{
		public static string BuildString(this IEnumerable<char> source)
		{
			if (source == null) Assert.True(false, $"{nameof(BuildString)}({nameof(source)})is null");
			return source.Aggregate(new StringBuilder(), (b, c) => b.Append(c), b => b.ToString());
		}

		public static void AreSuccess(this IReply<char, (IEnumerable<char> captured, TokenTypes tokenType)> reply,
			string expectedCapture, TokenTypes expectedTokenType)
		{
			reply.IsNotNull();
			expectedCapture.IsNotNull();
			AreSuccess(reply, act => expectedCapture.SequenceEqual(act), expectedTokenType);
		}

		public static void AreSuccess(this IReply<char, (IEnumerable<char> captured, TokenTypes tokenType)> reply,
			Func<IEnumerable<char>, bool> predicate, TokenTypes expectedTokenType)
		{
			reply.IsNotNull();
			predicate.IsNotNull();

			reply.Case((str, txt) => Assert.False(true, txt),
				(str, cap) =>
				{
					predicate(cap.captured).IsTrue();
					(cap.tokenType == expectedTokenType).IsTrue();
				});
		}

		public static void AreFail(this IReply<char, (IEnumerable<char> captured, TokenTypes tokenType)> reply)
		{
			reply.IsNotNull();

			reply.Case(_ =>
			{
				true.IsTrue();
				return 0;
			}, seq =>
			{
				Assert.False(true, seq.captured.BuildString());
				return 0;
			});
		}

		public static IReply<char, (IEnumerable<char> captured, TokenTypes tokenType)> Execute(
			this Parser<char, (IEnumerable<char> captured, TokenTypes type)> parser, string input)
		{
			parser.IsNotNull();
			return parser.Run(input.AsStream());
		}
	}
}