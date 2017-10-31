using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.XPath;

namespace JsDataParser.Entities
{
	internal class DynamicIdentifierEntity:DynamicEntity
	{
		

		private static RepresentTypes GetType(IdentifierEntity identity)
		{
			switch (identity.IdentityType)
			{
				case IdentifierTypes.Boolean:
					return RepresentTypes.Boolean;

				case IdentifierTypes.Identity:
					return RepresentTypes.Identity;

				case IdentifierTypes.Integer:
					return RepresentTypes.Integer;

				case IdentifierTypes.Real:
					return RepresentTypes.Real;

				case IdentifierTypes.String:
					return RepresentTypes.String;


				default:
					throw new InvalidOperationException($"{identity.IdentityType} is unexpted.");
			}

		}

		private readonly IdentifierEntity _identity;

		public DynamicIdentifierEntity(IdentifierEntity identity)
			: base(GetType(identity), DynamicEntityTypes.Identity)
		{
			_identity = identity ?? throw new ArgumentNullException(nameof(identity));
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (binder.Name.ToLower() == "identity"&& RepresentType== RepresentTypes.Identity)
			{
				result = _identity.Constant;
				return true;
			}

			result = default;
			return true;
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if (binder.Type == typeof(object))
			{
				result = _identity.Object;
				return true;
			}

			if (binder.Type.IsAssignableFrom(typeof(IdentifierEntity)))
			{
				result = _identity;
				return true;
			}

			if (binder.Type.IsAssignableFrom(typeof(int))&&_identity.IdentityType== IdentifierTypes.Integer)
			{
				result = _identity.Integer;
				return true;
			}

			if (binder.Type.IsAssignableFrom(typeof(double)) && _identity.IdentityType == IdentifierTypes.Real)
			{
				result = _identity.Real;
				return true;
			}

			if (binder.Type.IsAssignableFrom(typeof(bool)) && _identity.IdentityType == IdentifierTypes.Boolean)
			{
				result = _identity.Boolean;
				return true;
			}

			if (binder.Type.IsAssignableFrom(typeof(string)) && _identity.IdentityType == IdentifierTypes.String)
			{
				result = _identity.String;
				return true;
			}

			//Implicit
			if (binder.Type == typeof(int) && _identity.IdentityType == IdentifierTypes.Real)
			{
				result = _identity.Real;
				return true;
			}


			if (binder.Type == typeof(double) && _identity.IdentityType == IdentifierTypes.Integer)
			{
				result = _identity.Integer;
				return true;
			}


			result = default;
			return true;

		}
	}

}
