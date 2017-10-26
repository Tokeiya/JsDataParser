using System;
using System.Collections.Generic;
using System.Text;

namespace JsDataParser.Entities
{
	public class IdentifierEntity
	{
		public IdentifierEntity(IEnumerable<char> souce, IdentifierTypes identityType)
		{
#warning IdentifierEntity_Is_NotImpl
			throw new NotImplementedException("IdentifierEntity is not implemented");
		}

		public object Identity { get; }
		public IdentifierTypes IdentityType { get; }


	}
}
