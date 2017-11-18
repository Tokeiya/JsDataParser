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

namespace JsDataParser.Entities
{
	public class IndexedIdentiferEntity : IEquatable<IndexedIdentiferEntity>
	{
		public IndexedIdentiferEntity(IEnumerable<char> identity, IdentifierEntity index)
		{
			if (identity == null) throw new ArgumentNullException(nameof(identity));
			Identity = identity.BuildString();
			Index = index ?? throw new ArgumentNullException(nameof(index));


		}

		public string Identity { get; }
		public IdentifierEntity Index { get; }


		public bool Equals(IndexedIdentiferEntity other)
		{
			if (other.IsNull()) return false;

			return other.Identity == Identity && other.Index == Index;
		}

		public override bool Equals(object obj)
		{
			if (obj is IndexedIdentiferEntity other)
			{
				return Equals(other);
			}
			return false;
		}

		public static bool operator ==(IndexedIdentiferEntity x, IndexedIdentiferEntity y)
		{
			if (x.IsNull() && y.IsNull()) return true;
			if (x.IsNull() || y.IsNull()) return false;
			return x.Equals(y);
		}


		public static bool operator !=(IndexedIdentiferEntity x, IndexedIdentiferEntity y)
			=> !(x == y);

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Identity != null ? Identity.GetHashCode() : 0) * 397) ^ (Index != null ? Index.GetHashCode() : 0);
			}
		}
	}
}
