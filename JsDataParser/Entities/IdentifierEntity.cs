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
	[DebuggerStepThrough]
	public class IdentifierEntity : IEquatable<IdentifierEntity>
	{
		private readonly object _entity;


		public IdentifierEntity(int integer)
		{
			_entity = integer;
			IdentityType = IdentifierTypes.Integer;
		}

		public IdentifierEntity(double real)
		{
			_entity = real;
			IdentityType = IdentifierTypes.Real;
		}

		public IdentifierEntity(bool boolean)
		{
			_entity = boolean;
			IdentityType = IdentifierTypes.Boolean;
		}

		public IdentifierEntity(string value, bool isConstant)
		{
			_entity = value ?? throw new ArgumentNullException(nameof(value));

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
					_entity = bool.Parse(source.BuildString());
					break;

				case IdentifierTypes.Integer:
					_entity = int.Parse(source.BuildString());
					break;

				case IdentifierTypes.Real:
					_entity = double.Parse(source.BuildString(), NumberStyles.Float);
					break;

				case IdentifierTypes.String:
				case IdentifierTypes.Identity:
					_entity = source.BuildString();
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

				return (int)_entity;
			}
		}

		public double Real
		{
			get
			{
				if (IdentityType != IdentifierTypes.Real)
					throw new InvalidOperationException($"This instance isn't a Real identity.");

				return (double)_entity;
			}
		}

		public bool Boolean
		{
			get
			{
				if (IdentityType != IdentifierTypes.Boolean)
					throw new InvalidOperationException($"This instance isn't Boolean identity.");

				return (bool)_entity;
			}
		}

		public string String
		{
			get
			{
				if (IdentityType != IdentifierTypes.String)
					throw new InvalidOperationException("This instance is't String identity");

				return (string)_entity;
			}
		}

		public string Identity
		{
			get
			{
				if (IdentityType != IdentifierTypes.Identity)
					throw new InvalidOperationException("This instance isn't identity.");

				return (string) _entity;
			}
		}

		public object Object => _entity;


		public IdentifierTypes IdentityType { get; }


		public bool Equals(IdentifierEntity other)
		{
			if (other.IsNull()) return false;

			if (other.IdentityType != IdentityType) return false;

			switch (IdentityType)
			{
				case IdentifierTypes.Boolean:
					return Boolean == other.Boolean;

				case IdentifierTypes.String:
					return String == other.String;

				case IdentifierTypes.Identity:
					return Identity == other.Identity;

				case IdentifierTypes.Integer:
					return Integer == other.Integer;

				case IdentifierTypes.Real:
					return Math.Abs(Real - other.Real) < double.Epsilon;

				default:
					throw new InconsistencyException($"{IdentityType} is unexpected.");
			}
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
						tmp = Boolean.GetHashCode();
						break;

					case IdentifierTypes.Identity:
						tmp = Identity.GetHashCode();
						break;

					case IdentifierTypes.String:
						tmp = String.GetHashCode();
						break;

					case IdentifierTypes.Integer:
						tmp = Integer.GetHashCode();
						break;

					case IdentifierTypes.Real:
						tmp = Real.GetHashCode();
						break;

					default:
						throw new InconsistencyException($"{IdentityType} is unexpected.");
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