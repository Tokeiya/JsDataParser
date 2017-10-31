using System;
using System.Collections.Generic;
using System.Text;

namespace JsDataParser.Entities
{
	public interface IDynamicLiteralObjectEntity
	{
		bool TryGetField(string identity, out dynamic value);
	}
}
