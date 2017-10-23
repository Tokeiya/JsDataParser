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

namespace Parseq
{
	public interface IOption<out T>
	{
		bool HasValue { get; }

		T Value { get; }
	}

	public static partial class Option
	{
		public static IOption<T> Some<T>(T value)
		{
			return new SomeImpl<T>(value);
		}

		public static IOption<T> None<T>()
		{
			return SingletonClassHelper<NoneImpl<T>>.Instance;
		}
	}

	public static partial class Option
	{
		private class SomeImpl<T>
			: IOption<T>
		{
			public SomeImpl(T value)
			{
				Value = value;
			}

			public bool HasValue => true;

			public T Value { get; }
		}

		private class NoneImpl<T>
			: IOption<T>
		{
			public bool HasValue => false;

			public T Value => throw new InvalidOperationException();
		}
	}
}