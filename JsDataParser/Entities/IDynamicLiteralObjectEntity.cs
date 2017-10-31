using System;
using System.Collections.Generic;
using System.Text;

namespace JsDataParser.Entities
{
	public interface IDynamicLiteralObjectEntity:IEnumerable<(object key,object value)>
	{
		bool TryGetField(string identity, out dynamic value);
		
	}
}
