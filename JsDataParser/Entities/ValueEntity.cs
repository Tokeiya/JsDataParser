using System;
using System.Collections.Generic;
using JsDataParser.Parser;

namespace JsDataParser.Entities
{



	public class ValueEntity
	{
		private readonly ObjectEntity _nestedObject;
		private readonly ValueEntity[] _array;
		private readonly object _value;

		public ValueEntity(ObjectEntity nestedObject)
		{
			_nestedObject = nestedObject ?? throw new ArgumentNullException(nameof(nestedObject));
			ValueType = ValueTypes.Object;
		}

		public ValueEntity(IEnumerable<char> value, ValueTypes valueType)
		{
#warning ValueEntity_Is_NotImpl
			throw new NotImplementedException("ValueEntity is not implemented");
		}

		public ValueEntity(IEnumerable<ValueEntity> arrayValue)
		{
#warning ValueEntity_Is_NotImpl
			throw new NotImplementedException("ValueEntity is not implemented");
		}

		public ValueTypes ValueType { get; }

		public object Value
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");
			}
		}

		public IReadOnlyList<ValueEntity> ArrayValue { get; }

		public ObjectEntity NestedObject => _nestedObject ?? throw new InvalidOperationException();
	}
}