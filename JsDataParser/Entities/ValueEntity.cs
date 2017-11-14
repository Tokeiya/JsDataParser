using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JsDataParser.Entities
{
	[DebuggerStepThrough]
	public class ValueEntity
	{
		private readonly ValueEntity[] _array;
		private readonly bool _boolValue;
		private readonly int _integerValue;
		private readonly ObjectLiteralEntity _nestedObject;
		private readonly double _realValue;
		private readonly string _textValue;


		public ValueEntity(ObjectLiteralEntity nestedObject)
		{
			_nestedObject = nestedObject ?? throw new ArgumentNullException(nameof(nestedObject));
			ValueType = ValueTypes.Object;
			_nestedObject = nestedObject;
		}

		public ValueEntity(IEnumerable<char> value, ValueTypes valueType)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (!valueType.Verify()) throw new ArgumentException($"{nameof(valueType)} is unexpected");

			switch (valueType)
			{
				case ValueTypes.Boolean:
					_boolValue = bool.Parse(value.BuildString());
					break;

				case ValueTypes.Identity:
				case ValueTypes.String:
				case ValueTypes.Function:
					_textValue = value.BuildString();
					break;

				case ValueTypes.Integer:
					_integerValue = int.Parse(value.BuildString());
					break;

				case ValueTypes.Real:
					_realValue = double.Parse(value.BuildString());
					break;

				default:
					throw new ArgumentException("unexpected value.", nameof(valueType));
			}
			ValueType = valueType;
		}

		public ValueEntity(IEnumerable<ValueEntity> arrayValue)
		{
			if (arrayValue == null) throw new ArgumentNullException(nameof(arrayValue));

			var buffer = new List<ValueEntity>();

			foreach (var elem in arrayValue)
			{
				if (elem == null) throw new ArgumentException($"Contains null value.", nameof(arrayValue));

				buffer.Add(elem);
			}

			_array = buffer.ToArray();

			ValueType = ValueTypes.Array;
		}

		public ValueTypes ValueType { get; }

		public object Object
		{
			get
			{
				switch (ValueType)
				{
					case ValueTypes.Array:
						return _array;

					case ValueTypes.Boolean:
						return _boolValue;

					case ValueTypes.Identity:
					case ValueTypes.Function:
					case ValueTypes.String:
						return _textValue;

					case ValueTypes.Real:
						return _realValue;

					case ValueTypes.Integer:
						return _integerValue;

					case ValueTypes.Object:
						return _nestedObject;

					default:
						throw new InvalidOperationException();
				}
			}
		}

		public bool Boolean
		{
			get
			{
				if (ValueType != ValueTypes.Boolean) throw new InvalidOperationException("This instance isn't boolean entity.");
				return _boolValue;
			}
		}


		public string String
		{
			get
			{
				if (ValueType != ValueTypes.String) throw new InvalidOperationException("This instance isn't string entity.");

				return _textValue;
			}
		}

		public string Function
		{
			get
			{
				if (ValueType != ValueTypes.Function) throw new InvalidOperationException("This instance isn't function entity.");
				return _textValue;
			}
		}

		public string Identity
		{
			get
			{
				if (ValueType != ValueTypes.Identity)
					throw new InvalidOperationException("This instance isn't constant entity.");

				return _textValue;
			}
		}


		public int Integer
		{
			get
			{
				if (ValueType != ValueTypes.Integer) throw new InvalidOperationException("This instance isn't integer entity.");
				return _integerValue;
			}
		}

		public double Real
		{
			get
			{
				if (ValueType != ValueTypes.Real) throw new InvalidOperationException("Tis instance isn't real entity.");
				return _realValue;
			}
		}

		public IReadOnlyList<ValueEntity> Array
		{
			get
			{
				if (ValueType != ValueTypes.Array) throw new InvalidOperationException("This instance isn't array entity.");
				return _array;
			}
		}

		public ObjectLiteralEntity NestedObject
		{
			get
			{
				if (ValueType != ValueTypes.Object)
					throw new InvalidOperationException("This instance isn't nested object entity.");

				return _nestedObject;
			}
		}
	}
}