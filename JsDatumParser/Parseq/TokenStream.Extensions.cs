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
using System.Diagnostics;
using System.IO;

namespace Parseq
{
	[DebuggerStepThrough]
	public static partial class TokenStream
	{
		public static CharStream AsStream(this string inputString)
		{
			return new CharStream(inputString);
		}

		public static CharStream AsStream(this TextReader inputStringReader)
		{
			return new CharStream(inputStringReader);
		}

		public static CharStream AsStream(this IEnumerable<char> enumerable)
		{
			return new TextReaderAdapter(enumerable).AsStream();
		}

		public static ITokenStream<T> AsStream<T>(this IEnumerable<T> enumerable)
		{
			return new TokenStreamImpl<T>(enumerable);
		}
	}

	public static partial class TokenStream
	{
		private class TokenStreamImpl<T>
			: ITokenStream<T>
		{
			private readonly IEnumerator<T> enumerator;
			private readonly IDelayed<ITokenStream<T>> restStream;

			public TokenStreamImpl(IEnumerable<T> enumerable)
				: this(enumerable.GetEnumerator())
			{
			}

			public TokenStreamImpl(IEnumerator<T> enumerator)
				: this(enumerator, Position.Zero)
			{
			}

			private TokenStreamImpl(IEnumerator<T> enumerator, Position currentPosition)
			{
				this.enumerator = enumerator;
				Current = this.enumerator.MoveNext()
					? Option.Some(Pair.Return(this.enumerator.Current, currentPosition))
					: Option.None<IPair<T, Position>>();
				restStream = Delayed.Return(() =>
					new TokenStreamImpl<T>(enumerator, new Position(currentPosition.Line, currentPosition.Column + 1)));
			}

			public IOption<IPair<T, Position>> Current { get; }

			public ITokenStream<T> MoveNext()
			{
				return restStream.Force();
			}
		}

		private class TextReaderAdapter
			: TextReader
		{
			public const int EOF = -1;
			private IOption<char> current;

			private IEnumerator<char> enumerator;

			public TextReaderAdapter(IEnumerable<char> enumerable)
				: this(enumerable.GetEnumerator())
			{
			}

			public TextReaderAdapter(IEnumerator<char> enumerator)
			{
				this.enumerator = enumerator;
				current = this.enumerator.MoveNext()
					? Option.Some(this.enumerator.Current)
					: Option.None<char>();
			}

			public override int Peek()
			{
				if (enumerator == null)
					throw new ObjectDisposedException("enumerator");

				return current.Case(
					() => EOF,
					value => (int) value);
			}

			public override int Read()
			{
				if (enumerator == null)
					throw new ObjectDisposedException("enumerator");

				var c = Peek();

				current = enumerator.MoveNext()
					? Option.Some(enumerator.Current)
					: Option.None<char>();
				return c;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && enumerator != null)
				{
					enumerator.Dispose();
					enumerator = null;
				}
			}
		}
	}
}