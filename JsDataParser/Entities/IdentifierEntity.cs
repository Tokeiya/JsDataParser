using System;
using System.Collections.Generic;

namespace JsDataParser.Entities
{
	public class IdentifierEntity:IEquatable<IdentifierEntity>
	{
		public IdentifierEntity(IEnumerable<char> souce, IdentifierTypes identityType)
		{
#warning IdentifierEntity_Is_NotImpl
			throw new NotImplementedException("IdentifierEntity is not implemented");
		}

		public object Identity { get; }
		public IdentifierTypes IdentityType { get; }


		public bool Equals(IdentifierEntity other)
		{
#warning Equals_Is_NotImpl
			throw new NotImplementedException("Equals is not implemented");
		}

		public override bool Equals(object obj)
		{
#warning Equals_Is_NotImpl
			throw new NotImplementedException("Equals is not implemented");
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Identity != null ? Identity.GetHashCode() : 0) * 397) ^ (int) IdentityType;
			}
		}

		public static bool operator ==(IdentifierEntity x, IdentifierEntity y)
		{
#warning ==_Is_NotImpl
			throw new NotImplementedException("== is not implemented");
		}

		public static bool operator !=(IdentifierEntity x, IdentifierEntity y)
		{
#warning !=_Is_NotImpl
			throw new NotImplementedException("!= is not implemented");
		}

	}
}
