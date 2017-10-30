using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace JsDataParser.Entities
{
    internal class DynamicIdentifierEntity:DynamicEntity
    {
	    private static DynamicTypes GetType(IdentifierEntity identity)
	    {
#warning GetType_Is_NotImpl
		    throw new NotImplementedException("GetType is not implemented");
	    }

	    public DynamicIdentifierEntity(IdentifierEntity identity)
			:base(GetType(identity))
	    {
#warning DynamicIdentifierEntity_Is_NotImpl
		    throw new NotImplementedException("DynamicIdentifierEntity is not implemented");
	    }

	    public object this[object key]
	    {
		    get
		    {
#warning this_Is_NotImpl
			    throw new NotImplementedException("this is not implemented");
		    }
	    }

	    public override bool TryGetMember(GetMemberBinder binder, out object result)
	    {
#warning TryGetMember_Is_NotImpl
		    throw new NotImplementedException("TryGetMember is not implemented");
	    }

	    public override bool TryConvert(ConvertBinder binder, out object result)
	    {
#warning TryConvert_Is_NotImpl
		    throw new NotImplementedException("TryConvert is not implemented");
	    }
    }
}
