using System;
using JsDataParser.Entities;

namespace JsDataParser.Mapping
{
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field,AllowMultiple = false,Inherited = true)]
	public sealed class MappingFromAttribute:Attribute
	{
		public MappingFromAttribute(string mappingFromidentity, bool isIdentity)
		{
			if (mappingFromidentity == null) throw new ArgumentNullException(nameof(mappingFromidentity));

			MappingFromIdentifier = new IdentifierEntity(mappingFromidentity, isIdentity);
		}

		public MappingFromAttribute(int key)
			=> MappingFromIdentifier = new IdentifierEntity(key);


		public MappingFromAttribute(double key)
			=> MappingFromIdentifier = new IdentifierEntity(key);

		public MappingFromAttribute(bool key)
			=> MappingFromIdentifier = new IdentifierEntity(key);


		public IdentifierEntity MappingFromIdentifier { get; }

	}
}
