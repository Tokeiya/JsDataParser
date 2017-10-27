using System;
using System.Collections.Generic;
using JsDataParser.Parser;

namespace JsDataParser.Entities
{



	public class ValueEntity
	{
		private readonly ObjectEntity _nestedObject;
		private readonly ValueEntity[] _array;
		private readonly string _textValue;
		private readonly int _integerValue;
		private readonly double _realValue;
		private readonly bool _boolValue;


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

		public string String
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");

			}
		}

		public string Function
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");

			}
		}

		public string Constant
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");

			}
		}


		public int Integer
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");

			}
		}

		public double Real
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");
			}
		}

		public IReadOnlyList<ValueEntity> Array
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");

			}
		}

		public dynamic Dynamic
		{
			get
			{
#warning Value_Is_NotImpl
				throw new NotImplementedException("Value is not implemented");

			}
		}

		public object Object
		{
			get
			{
#warning Equals_Is_NotImpl
				throw new NotImplementedException("Equals is not implemented");

			}
		}

		public ObjectEntity NestedObject => _nestedObject ?? throw new InvalidOperationException();
	}
}