using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace JsDataParser.Entities
{
    internal abstract class DynamicEntity:DynamicObject
    {
	    protected DynamicEntity(DynamicTypes type)
	    {
		    throw new NotImplementedException();
	    }

		public DynamicTypes Type { get; }
    }
}
