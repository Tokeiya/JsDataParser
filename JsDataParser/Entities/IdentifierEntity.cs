/*
 * Copyright (C) 2017 Hikotaro Abe <net_seed@hotmail.com> All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace JsDataParser.Entities
{
	public class IdentifierEntity : IEquatable<IdentifierEntity>
	{
		private readonly bool _boolValue;
		private readonly int _intValue;
		private readonly double _realValue;
		private readonly string _stringValue;


		public IdentifierEntity(int integer)
		{
			_intValue = integer;
			IdentityType = IdentifierTypes.Integer;
		}

		public IdentifierEntity(double real)
		{
			_realValue = real;
			IdentityType = IdentifierTypes.Real;
		}

		public IdentifierEntity(bool boolean)
		{
			_boolValue = boolean;
			IdentityType = IdentifierTypes.Boolean;
		}

		public IdentifierEntity(string value, bool isConstant)
		{
			_stringValue = value ?? throw new ArgumentNullException(nameof(value));

			IdentityType = isConstant ? IdentifierTypes.Identity : IdentifierTypes.String;
		}

		public IdentifierEntity(IEnumerable<char> source, IdentifierTypes identityType)
		{
			if (!identityType.Verify()) throw new ArgumentException($"{nameof(identityType)} is unexpected value.");
			if (source == null) throw new ArgumentNullException(nameof(source));


			IdentityType = identityType;

			switch (identityType)
			{
				case IdentifierTypes.Boolean:
					_boolValue = bool.Parse(source.BuildString());
					break;

				case IdentifierTypes.Integer:
					_intValue = int.Parse(source.BuildString());
					break;

				case IdentifierTypes.Real:
					_realValue = double.Parse(source.BuildString(), NumberStyles.Float);
					break;

				case IdentifierTypes.String:
				case IdentifierTypes.Identity:
					_stringValue = source.BuildString();
					break;


				default:
					throw new ArgumentException($"{identityType} is unrecognized.");
			}
		}

		public int Integer
		{
			get
			{
				if (IdentityType != IdentifierTypes.Integer)
					throw new InvalidOperationException($"This instance isn't a Integer identity.");

				return _intValue;
			}
		}

		public double Real
		{
			get
			{
				if (IdentityType != IdentifierTypes.Real)
					throw new InvalidOperationException($"This instance isn't a Real identity.");

				return _realValue;
			}
		}

		public bool Boolean
		{
			get
			{
				if (IdentityType != IdentifierTypes.Boolean)
					throw new InvalidOperationException($"This instance isn't Boolean identity.");

				return _boolValue;
			}
		}

		public string String
		{
			get
			{
				if (IdentityType != IdentifierTypes.String)
					throw new InvalidOperationException("This instance is't String identity");

				return _stringValue;
			}
		}

		public string Identity
		{
			get
			{
				if (IdentityType != IdentifierTypes.Identity)
					throw new InvalidOperationException("This instance isn't identity.");

				return _stringValue;
			}
		}

		public dynamic Dynamic => Object;

		public object Object
		{
			get
			{
				switch (IdentityType)
				{
					case IdentifierTypes.Identity:
					case IdentifierTypes.String:
						return _stringValue;

					case IdentifierTypes.Boolean:
						return _boolValue;

					case IdentifierTypes.Integer:
						return _intValue;

					case IdentifierTypes.Real:
						return _realValue;

					default:
						break;
				}

				Trace.Assert(false);
				return default;
			}
		}


		public IdentifierTypes IdentityType { get; }


		public bool Equals(IdentifierEntity other)
		{
			if (other.IsNull()) return false;

			if (other.IdentityType != IdentityType) return false;

			switch (IdentityType)
			{
				case IdentifierTypes.Boolean:
					return _boolValue == other._boolValue;

				case IdentifierTypes.String:
				case IdentifierTypes.Identity:
					return _stringValue == other._stringValue;

				case IdentifierTypes.Integer:
					return _intValue == other._intValue;

				case IdentifierTypes.Real:
					return Math.Abs(_realValue - other._realValue) < double.Epsilon;

				default:
					Trace.Assert(false, $"{IdentityType} is unexpected.");
					break;
			}

			Trace.Assert(false);
			return false;
		}

		public override bool Equals(object obj)
		{
			var other = obj as IdentifierEntity;
			if (other.IsNull()) return false;

			return Equals(other);
		}

		public override int GetHashCode()
		{
			var tmp = 0;
			unchecked
			{
				switch (IdentityType)
				{
					case IdentifierTypes.Boolean:
						tmp = _boolValue.GetHashCode();
						break;

					case IdentifierTypes.Identity:
					case IdentifierTypes.String:
						tmp = _stringValue.GetHashCode();
						break;

					case IdentifierTypes.Integer:
						tmp = _intValue.GetHashCode();
						break;

					case IdentifierTypes.Real:
						tmp = _realValue.GetHashCode();
						break;

					default:
						Trace.Assert(false);
						break;
				}

				return (tmp * 397) ^ (int) IdentityType;
			}
		}

		public static bool operator ==(IdentifierEntity x, IdentifierEntity y)
		{
			var xNull = x.IsNull();
			var yNull = y.IsNull();

			if (xNull && yNull) return true;
			if (xNull || yNull) return false;

			return x.Equals(y);
		}

		public static bool operator !=(IdentifierEntity x, IdentifierEntity y)
		{
			return !(x == y);
		}
	}
}