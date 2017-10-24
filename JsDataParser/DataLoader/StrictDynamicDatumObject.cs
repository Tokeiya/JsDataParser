using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using JsDataParser.Parser;

namespace JsDataParser.DataLoader
{
	internal class StrictDynamicDatumObject : DynamicObject
	{

		private bool TryParse(FieldValueExpression datum, Type targetType,out object result)
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


			int[] buildArray(IReadOnlyList<IEnumerable<char>> data)
			{
				if (data.Count == 0) return Array.Empty<int>();

				var ret = new int[data.Count];

				for (int i = 0; i < ret.Length; i++)
				{
					ret[i] = int.Parse(buildString(data[i]));
				}

				return ret;
			}

			object parse()
			{
				switch (datum.FieldType)
				{
					case TokenTypes.Boolean:
						return bool.Parse(buildString(datum.Source));

					case TokenTypes.Text:
					case TokenTypes.Function:
						return buildString(datum.Source);

					case TokenTypes.IntegerNumber:
						return int.Parse(buildString(datum.Source));

					case TokenTypes.RealNumber:
						return double.Parse(buildString(datum.Source));

					case TokenTypes.IntegerArray:
						return buildArray(datum.ArraySource);

					default:
						throw new InvalidOperationException($"{datum.FieldType} is unexpected.");
				}
			}

			Type datumType = null;

			switch (datum.FieldType)
			{
				case TokenTypes.Boolean:
					datumType = typeof(bool);
					break;

				case TokenTypes.Function:
				case TokenTypes.Text:
					datumType = typeof(string);
					break;

				case TokenTypes.IntegerArray:
					datumType = typeof(int[]);
					break;

				case TokenTypes.IntegerNumber:
					datumType = typeof(int);
					break;

				case TokenTypes.RealNumber:
					datumType = typeof(double);
					break;

				default:
					throw new InvalidOperationException($"{datum.FieldType} is unexpected.");
			}


			if (targetType.IsAssignableFrom(datumType))
			{
				result = parse();
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		private readonly DatumExpression _datum;
		public StrictDynamicDatumObject(DatumExpression datum)=>_datum = datum ?? throw new ArgumentNullException(nameof(datum));

		private bool TryGetMemberIgnoreCaseSensitive(GetMemberBinder binder, out object result)
		{
			throw new NotSupportedException();
		}

		private bool TryGetMemberSensitive(GetMemberBinder binder, out object result)
		{
			if (binder.Name == "Id" && binder.ReturnType.IsAssignableFrom(typeof(int)))
			{
				result = _datum.Id;
				return true;
			}

			if (_datum.Fields.ContainsKey(binder.Name))
			{
				var tmp = _datum.Fields[binder.Name];
				return TryParse(tmp, binder.ReturnType, out result);
			}
			else
			{
				result = default;
				return false;
			}
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (binder.IgnoreCase)
			{
				return TryGetMemberIgnoreCaseSensitive(binder, out result);
			}

			return TryGetMemberSensitive(binder, out result);
		}
	}
}
