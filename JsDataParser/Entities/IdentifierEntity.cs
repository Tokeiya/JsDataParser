using System;
using System.Collections.Generic;
using System.Diagnostics;
using JsDataParser;

namespace JsDataParser.Entities
{
	public class IdentifierEntity : IEquatable<IdentifierEntity>
	{
		private readonly int _intValue;
		private readonly double _realValue;
		private readonly bool _boolValue;
		private readonly string _stringValue;


		public IdentifierEntity(IEnumerable<char> source, IdentifierTypes identityType)
		{
			if (!identityType.Verify()) throw new ArgumentException($"{nameof(identityType)} is unexpected value.");
			if (source == null) throw new ArgumentNullException(nameof(source));

			var value = source.BuildString();


			switch (identityType)
			{
				case IdentifierTypes.Boolean:
#warning IdentifierEntity_Is_NotImpl
					throw new NotImplementedException("IdentifierEntity is not implemented");

				case IdentifierTypes.Integer:
#warning IdentifierEntity_Is_NotImpl
					throw new NotImplementedException("IdentifierEntity is not implemented");

				case IdentifierTypes.Real:
#warning IdentifierEntity_Is_NotImpl
					throw new NotImplementedException("IdentifierEntity is not implemented");


				case IdentifierTypes.String:
#warning IdentifierEntity_Is_NotImpl
					throw new NotImplementedException("IdentifierEntity is not implemented");

				case IdentifierTypes.Constant:
#warning IdentifierEntity_Is_NotImpl
					throw new NotImplementedException("IdentifierEntity is not implemented");

				default:
					throw new ArgumentException($"{identityType} is unrecognized.");
			}

			IdentityType = identityType;

#warning IdentifierEntity_Is_NotImpl
	throw

	new NotImplementedException("IdentifierEntity is not implemented");
		}

		public object Identity { get; }

		public int IntIdentity
		{
			get
			{
				if (IdentityType != IdentifierTypes.Integer)
					throw new InvalidOperationException($"This instance isn't a Integer identity.");

				return _intValue;
			}
		}

		public double RealIdentity
		{
			get
			{
				if (IdentityType != IdentifierTypes.Real)
					throw new InvalidOperationException($"This instance isn't a Real identity.");

				return _realValue;
			}
		}

		public bool BooleanIdentity
		{
			get
			{
				if (IdentityType != IdentifierTypes.Boolean)
					throw new InvalidOperationException($"This instance isn't Boolean identity.");

				return _boolValue;
			}
		}

		public string StringIdentity
		{
			get
			{
#warning StringIdentity_Is_NotImpl
				throw new NotImplementedException("StringIdentity is not implemented");
			}
		}

		public string ConstantIdentity
		{
			get
			{
#warning ConstantIdentity_Is_NotImpl
				throw new NotImplementedException("ConstantIdentity is not implemented");
			}
		}


		public IdentifierTypes IdentityType { get; }


		public bool Equals(IdentifierEntity other)
		{
#warning Equals_Is_NotImpl
			throw new NotImplementedException("Equals is not implemented");
		}

		public override bool Equals(object obj)
		{
#warning Equals_Is_NotImpl
			throw new NotImplementedException("Equals is not implemented");
		}

		public override int GetHashCode()
		{
#warning GetHashCode_Is_NotImpl
			throw new NotImplementedException("GetHashCode is not implemented");
			unchecked
			{
				return ((Identity != null ? Identity.GetHashCode() : 0) * 397) ^ (int) IdentityType;
			}
		}

		public static bool operator ==(IdentifierEntity x, IdentifierEntity y)
		{
#warning ==_Is_NotImpl
			throw new NotImplementedException("== is not implemented");
		}

		public static bool operator !=(IdentifierEntity x, IdentifierEntity y)
		{
#warning !=_Is_NotImpl
			throw new NotImplementedException("!= is not implemented");
		}

	}
}
