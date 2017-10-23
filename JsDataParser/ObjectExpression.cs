using System;
using System.Collections.Generic;

namespace JsDataParser
{
	internal class ObjectExpression
	{
		private readonly Dictionary<string, FieldValueExpression> _dict = new Dictionary<string, FieldValueExpression>();

		public ObjectExpression(int id, IEnumerable<FieldExpression> fields)
		{
			if (fields == null) throw new ArgumentNullException(nameof(fields));

			Id = id;

			try
			{
				foreach (var field in fields)
					_dict.Add(field.Name, field);
			}
			catch (NullReferenceException ex)
			{
				throw new ArgumentException($"{nameof(fields)} contains null value.", ex);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException("Detect field name duplication!", ex);
			}
		}

		public int Id { get; }
		public IReadOnlyDictionary<string, FieldValueExpression> Fields => _dict;
	}
}