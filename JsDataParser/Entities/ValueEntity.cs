using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JsDataParser.Entities
{
	[DebuggerStepThrough]
	public class ValueEntity
	{
		private readonly object _value;	

		public ValueEntity(ObjectLiteralEntity nestedObject)
		{
			_value = nestedObject ?? throw new ArgumentNullException(nameof(nestedObject));
			ValueType = ValueTypes.Object;
			_value = nestedObject;
		}

		public ValueEntity(IEnumerable<char> value, ValueTypes valueType)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (!valueType.Verify()) throw new ArgumentException($"{nameof(valueType)} is unexpected");

			switch (valueType)
			{
				case ValueTypes.Boolean:
					_value = bool.Parse(value.BuildString());
					break;

				case ValueTypes.Identity:
				case ValueTypes.String:
				case ValueTypes.Function:
					_value = value.BuildString();
					break;

				case ValueTypes.Integer:
					_value = int.Parse(value.BuildString());
					break;

				case ValueTypes.Real:
					_value = double.Parse(value.BuildString());
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

			_value = buffer.ToArray();

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
					case ValueTypes.Boolean:
					case ValueTypes.Identity:
					case ValueTypes.Function:
					case ValueTypes.String:
					case ValueTypes.Real:
					case ValueTypes.Integer:
					case ValueTypes.Object:
						return _value;

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
				return (bool)_value;
			}
		}


		public string String
		{
			get
			{
				if (ValueType != ValueTypes.String) throw new InvalidOperationException("This instance isn't string entity.");

				return (string)_value;
			}
		}

		public string Function
		{
			get
			{
				if (ValueType != ValueTypes.Function) throw new InvalidOperationException("This instance isn't function entity.");
				return (string)_value;
			}
		}

		public string Identity
		{
			get
			{
				if (ValueType != ValueTypes.Identity)
					throw new InvalidOperationException("This instance isn't constant entity.");

				return (string)_value;
			}
		}


		public int Integer
		{
			get
			{
				if (ValueType != ValueTypes.Integer) throw new InvalidOperationException("This instance isn't integer entity.");
				return (int) _value;
			}
		}

		public double Real
		{
			get
			{
				if (ValueType != ValueTypes.Real) throw new InvalidOperationException("Tis instance isn't real entity.");
				return (double) _value;
			}
		}

		public IReadOnlyList<ValueEntity> Array
		{
			get
			{
				if (ValueType != ValueTypes.Array) throw new InvalidOperationException("This instance isn't array entity.");
				return (ValueEntity[]) _value;
			}
		}

		public ObjectLiteralEntity NestedObject
		{
			get
			{
				if (ValueType != ValueTypes.Object)
					throw new InvalidOperationException("This instance isn't nested object entity.");

				return (ObjectLiteralEntity) _value;
			}
		}
	}
}