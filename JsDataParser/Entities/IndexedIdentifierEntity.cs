using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace JsDataParser.Entities
{
	// Token: 0x0200005A RID: 90
	public class IndexedIdentiferEntity : IEquatable<IndexedIdentiferEntity>
	{
		// Token: 0x0600022B RID: 555 RVA: 0x00009CBC File Offset: 0x00007EBC
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
