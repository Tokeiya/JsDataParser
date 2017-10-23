using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsDataParser
{
	internal class FieldExpression : FieldValueExpression
	{
		public FieldExpression(string name, IEnumerable<char> source, TokenTypes fieldType)
			: base(source, fieldType)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

			Name = name;
		}

		public FieldExpression(string name, IReadOnlyCollection<IEnumerable<char>> source)
			: base(source)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

			Name = name;
		}

		public string Name { get; }

		public static FieldExpression Create(IEnumerable<char> name, FieldValueExpression value)
		{
			if (value == null) throw new ArgumentException(nameof(value));
			if (name == null) throw new ArgumentNullException(nameof(name));

			var tmp = name.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString());


			if (value.FieldType == TokenTypes.IntegerArray)
				return new FieldExpression(tmp, value.ArraySource);

			return new FieldExpression(tmp, value.Source, value.FieldType);
		}
	}
}