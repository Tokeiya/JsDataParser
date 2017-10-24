using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parseq;
using Parseq.Combinators;
using static Parseq.Combinator;

namespace JsDataParser
{
	using CharEnumerableParser = Parser<char, IEnumerable<char>>;

	internal static class DataParser
	{

		private class Envelope<T> : IEnumerable<T>
		{
			
			private readonly T _value;


			public Envelope(T value)
			{
				_value = value;
			}


			public IEnumerator<T> GetEnumerator()
			{
				yield return _value;
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		private static Envelope<T> CreateEnvelope<T>(T value) => new Envelope<T>(value);

		public static readonly CharEnumerableParser FieldName = Chars.Letter().Many1();

		public static readonly Parser<char, FieldExpression> Field =
			BuildField();


		public static Parser<char, FieldExpression> BuildField()
		{
			var fieldValues = Choice(
				LiteralParsers.Bool.Select(cap => new FieldValueExpression(cap.captured, TokenTypes.Boolean)),
				LiteralParsers.ArrayParser.Select(cap => new FieldValueExpression(cap.captured)),
				LiteralParsers.RealNumber.Select(cap => new FieldValueExpression(cap.captured, cap.type)),
				LiteralParsers.IntegerNumber.Select(cap => new FieldValueExpression(cap.captured, cap.type)),
				LiteralParsers.Text.Select(cap => new FieldValueExpression(cap.captured, cap.type)),
				LiteralParsers.TinyFunction.Select(cap => new FieldValueExpression(cap.captured, TokenTypes.Function))
			);

			var whiteSpace = Chars.WhiteSpace().Many0().Ignore();

			var ret = from name in FieldName
				from _ in Sequence(whiteSpace, Chars.Char(':').Ignore(), whiteSpace)
				from value in fieldValues
				select FieldExpression.Create(name, value);

			return ret;
		}

		public static Parser<char, ObjectExpression> BuildDatum()
		{
			var whiteSpace = Chars.WhiteSpace().Many0().Ignore();

			var index = Chars.Digit().Many1().Select(x =>
				int.Parse(x.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString())));

			var aField =
				from _ in whiteSpace
				from fld in Field
				from __ in Sequence(whiteSpace, Chars.Char(',').Ignore())
				select fld;

			var tmp = Choice(
				aField.Select(CreateEnvelope),
				LiteralParsers.Comment.Select(_ => Enumerable.Empty<FieldExpression>())
			);


			var lastField =
				(from _ in whiteSpace
					from fld in Field
					from __ in Sequence(whiteSpace, Chars.Char('}').Ignore()).And()
					select fld).Optional()
				.Select(opt => opt.HasValue ? CreateEnvelope(opt.Value) : Enumerable.Empty<FieldExpression>());

			var parser =
				from idx in index
				from _ in Sequence(whiteSpace, Chars.Char(':').Ignore(), whiteSpace, Chars.Char('{').Ignore())
				from fld in tmp.Many0()
				from lastFld in lastField
				let f = fld.SelectMany(x => x).Concat(lastFld)
				from __ in Sequence(whiteSpace, Chars.Char('}').Ignore())
				select new ObjectExpression(idx, f);

			return parser;
		}

		public static readonly Parser<char, ObjectExpression> Datum = BuildDatum();

		public static Parser<char,IEnumerable<ObjectExpression>> BuildData()
		{
			var whiteSpace = Chars.WhiteSpace().Many0().Ignore();

			var datumParser =
				from _ in whiteSpace
				from datum in Datum
				from __ in Sequence(whiteSpace, Chars.Char(',').Ignore())
				select datum;

			var parser =
				from _ in Sequence(whiteSpace, Chars.Char('{').Ignore(), whiteSpace)
				from data in datumParser.Many0()
				from __ in Sequence(whiteSpace, Chars.Char('}').Ignore())
				select data;


			return parser;
		}

		public static readonly Parser<char, IEnumerable<ObjectExpression>> Data = BuildData();

	}
}