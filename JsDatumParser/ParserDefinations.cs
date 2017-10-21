﻿/*
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
using Parseq;
using Parseq.Combinators;

[assembly: InternalsVisibleTo("JsDatumParserTest")]
[assembly: InternalsVisibleTo("Playground")]


namespace JsDatumParser
{
	using CharEnumerableParser=Parseq.Parser<char,IEnumerable<char>>;

	internal static class ParserDefinations
	{
		private static readonly CharacterCache Cache = new CharacterCache(1000, 1250);


		public static readonly CharEnumerableParser UnarySign = Chars.Char('+').Or(Chars.Char('-')).Optional()
			.Select(opt => opt.HasValue ? Cache.Get(opt.Value) : Array.Empty<char>());



	}
}
