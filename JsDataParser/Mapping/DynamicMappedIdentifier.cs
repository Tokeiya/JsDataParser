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
using System.Dynamic;
using JsDataParser.Entities;

namespace JsDataParser.Mapping
{
	internal class DynamicIdentifierMapping : DynamicMappedObject
	{
		private readonly IdentifierEntity _identity;

		public DynamicIdentifierMapping(IdentifierEntity identity)
			: base(GetType(identity), DynamicMappedTypes.Identity)
		{
			_identity = identity ?? throw new ArgumentNullException(nameof(identity));
		}


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

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (binder.Name.ToLower() == "identity" && RepresentType == RepresentTypes.Identity)
			{
				result = _identity.Identity;
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

			if (binder.Type.IsAssignableFrom(typeof(int)) && _identity.IdentityType == IdentifierTypes.Integer)
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