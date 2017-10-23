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
	public struct Unit
		: IComparable<Unit>
			, IEquatable<Unit>
	{
		public static Unit Instance { get; }

		static Unit()
		{
			Instance = new Unit();
		}

		public static bool operator ==(Unit lhs, Unit rhs)
		{
			/*
			 * return lhs.Equals(rhs);
			 */
			return true;
		}

		public static bool operator !=(Unit lhs, Unit rhs)
		{
			/*
			 * return !(lhs == rhs);
			 */
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object obj)
		{
			return obj is Unit;
		}

		int IComparable<Unit>.CompareTo(Unit other)
		{
			return 0;
		}

		bool IEquatable<Unit>.Equals(Unit other)
		{
			return true;
		}
	}
}