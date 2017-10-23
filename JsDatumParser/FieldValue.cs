using System;
using System.Collections.Generic;
using System.Text;

namespace JsDatumParser
{
    internal class FieldValue
    {
	    public FieldValue(IEnumerable<char> value, TokenTypes fieldType)
	    {
		    throw new NotImplementedException();
	    }

	    public FieldValue(IReadOnlyCollection<IEnumerable<char>> value)
	    {
		    throw new NotImplementedException();
	    }

	    public IEnumerable<char> Value
	    {
		    get
		    {
			    throw new NotImplementedException();
		    }
	    }

	    public IReadOnlyCollection<IEnumerable<char>> ArrayValue
	    {
		    get
		    {
			    throw new NotImplementedException();
		    }
	    }
    }
}
