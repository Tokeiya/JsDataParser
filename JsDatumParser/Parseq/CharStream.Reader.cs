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
using System.IO;

namespace Parseq
{
	public partial class CharStream
	{
		private class Reader
			: IDisposable
		{
			public const int EOF = -1;
			private TextReader baseReader;

			public Reader(TextReader baseReader)
			{
				if (baseReader == null)
					throw new ArgumentNullException("baseReader");

				CurrentPosition = Position.Zero;
				this.baseReader = baseReader;
			}

			public Position CurrentPosition { get; private set; }

			public void Dispose()
			{
				if (baseReader != null)
				{
					baseReader.Dispose();
					baseReader = null;
				}
			}

			public int Read()
			{
				if (baseReader == null)
					throw new ObjectDisposedException("baseReader");

				var c = baseReader.Read();

				CurrentPosition = c == '\n' || c == '\r' && baseReader.Peek() == '\r'
					? new Position(CurrentPosition.Line + 1, 1)
					: new Position(CurrentPosition.Line, CurrentPosition.Column + 1);
				return c;
			}

			public int Peek()
			{
				if (baseReader == null)
					throw new ObjectDisposedException("baseReader");

				return baseReader.Peek();
			}
		}
	}
}