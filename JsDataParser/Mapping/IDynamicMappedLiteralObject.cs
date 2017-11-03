using System.Collections.Generic;

namespace JsDataParser.Mapping
{
	public interface IDynamicMappedLiteralObject : IEnumerable<(object key, object value)>
	{
		bool TryGetField(string identity, out dynamic value);
	}
}