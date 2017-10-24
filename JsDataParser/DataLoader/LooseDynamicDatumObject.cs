using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using JsDataParser.Parser;

namespace JsDataParser.DataLoader
{
	internal class LooseDynamicDatumObject:DynamicObject
	{
		private readonly int _id;
		private readonly Dictionary<string, FieldValueExpression> _fields = new Dictionary<string, FieldValueExpression>();

		public LooseDynamicDatumObject(DatumExpression datum)
		{
			if (datum == null) throw new ArgumentNullException(nameof(datum));


			_id = datum.Id;

			foreach (var elem in datum.Fields)
			{
				_fields.Add(elem.Key.ToLower(), elem.Value);
			}
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string buildString(IEnumerable<char> source)
			{
				var bld = new StringBuilder();

				foreach (var c in source)
				{
					bld.Append(c);
				}

				return bld.ToString();
			}

			Type convert(FieldValueExpression datum)
			{
				switch (datum.FieldType)
				{
					case TokenTypes.Boolean:
						return typeof(bool);

					case TokenTypes.Function:
					case TokenTypes.Text:
						return typeof(string);

					case TokenTypes.IntegerArray:
						return typeof(int[]);

					case TokenTypes.IntegerNumber:
						return typeof(int);

					case TokenTypes.RealNumber:
						return typeof(double);

					default:
						throw new InvalidOperationException($"{datum.FieldType} is unexpected.");
				}
			}

			object parse(FieldValueExpression datum)
			{
				switch (datum.FieldType)
				{
					case TokenTypes.Boolean:
						return bool.Parse(buildString(datum.Source));

					case TokenTypes.Function:
					case TokenTypes.Text:
						return buildString(datum.Source);

					case TokenTypes.IntegerArray:
						return datum.ArraySource.Count == 0
							? Array.Empty<int>()
							: datum.ArraySource.Select(x => int.Parse(buildString(x))).ToArray();

					case TokenTypes.IntegerNumber:
						return int.Parse(buildString(datum.Source));

					case TokenTypes.RealNumber:
						return double.Parse(buildString(datum.Source));

					default:
						throw new InvalidOperationException($"{datum.FieldType} is unexpected.");
				}
			}

			if (binder.Name.ToLower() == "id")
			{
				result = _id;
				return true;
			}
			else if (_fields.TryGetValue(binder.Name.ToLower(), out var fldDatum))
			{
				var type = convert(fldDatum);

				if (binder.ReturnType.IsAssignableFrom(type))
				{
					result = parse(fldDatum);
					return true;
				}
				else
				{
					result = binder.ReturnType.IsValueType ? Activator.CreateInstance(binder.ReturnType) : null;
					return true;
				}
			}
			else
			{
				result = binder.ReturnType.IsValueType ? Activator.CreateInstance(binder.ReturnType) : null;
				return true;
			}
		}
	}
}


