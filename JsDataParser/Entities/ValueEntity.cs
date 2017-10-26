using System;
using System.Collections.Generic;
using JsDataParser.Parser;

namespace JsDataParser.Entities
{



	public class ValueEntity
	{
		private readonly ObjectEntity _nestedObject;
		private readonly IEnumerable<char> _value;


		public ValueEntity(ObjectEntity nestedObject)
		{
			_nestedObject = nestedObject ?? throw new ArgumentNullException(nameof(nestedObject));
			ValueType = ValueTypes.Object;
		}

		public ValueEntity(IEnumerable<char> value, ValueTypes valueType)
		{
			_value = value ?? throw new ArgumentNullException(nameof(value));
			ValueType = valueType;
		}

		public ValueEntity(IEnumerable<ValueEntity> arrayValue)
		{
			if (arrayValue == null) throw new ArgumentNullException(nameof(arrayValue));

			var list = new List<ValueEntity>();

			foreach (var elem in arrayValue)
			{
				if (elem == null) throw new ArgumentException($"{nameof(arrayValue)} contains null");
				list.Add(elem);
			}

			list.TrimExcess();
			ArrayValue = list;

			ValueType = ValueTypes.Array;
		}

		public ValueTypes ValueType { get; }

		public IEnumerable<char> Value => _value ?? throw new InvalidOperationException();

		public IReadOnlyList<ValueEntity> ArrayValue { get; }

		public ObjectEntity NestedObject => _nestedObject ?? throw new InvalidOperationException();
	}
}