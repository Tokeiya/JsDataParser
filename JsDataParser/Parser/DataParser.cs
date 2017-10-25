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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parseq;
using Parseq.Combinators;

namespace JsDataParser.Parser
{
	using CharEnumerableParser = Parser<char, IEnumerable<char>>;

	internal static class DataParser
	{
		public static readonly Parser<char, FieldExpression> Field =
			BuildField();

		public static readonly Parser<char, DatumExpression> Datum = BuildDatum();

		public static readonly Parser<char, IEnumerable<DatumExpression>> Data = BuildData();

		private static Envelope<T> CreateEnvelope<T>(T value)
		{
			return new Envelope<T>(value);
		}


		public static Parser<char, FieldExpression> BuildField()
		{
			var fieldValues = Combinator.Choice(
				LiteralParsers.Bool.Select(cap => new FieldValueExpression(cap.captured, TokenTypes.Boolean)),
				LiteralParsers.ArrayParser.Select(cap => new FieldValueExpression(cap.captured)),
				LiteralParsers.RealNumber.Select(cap => new FieldValueExpression(cap.captured, cap.type)),
				LiteralParsers.IntegerNumber.Select(cap => new FieldValueExpression(cap.captured, cap.type)),
				LiteralParsers.Text.Select(cap => new FieldValueExpression(cap.captured, cap.type)),
				LiteralParsers.TinyFunction.Select(cap => new FieldValueExpression(cap.captured, TokenTypes.Function))
			);

			var whiteSpace = Chars.WhiteSpace().Many0().Ignore();

			var ret = from name in LiteralParsers.IdentifierName
				from _ in Combinator.Sequence(whiteSpace, Chars.Char(':').Ignore(), whiteSpace)
				from value in fieldValues
				select FieldExpression.Create(name, value);

			return ret;
		}

		public static Parser<char, DatumExpression> BuildDatum()
		{
			var whiteSpace = Chars.WhiteSpace().Many0().Ignore();

			var index = Chars.Digit().Many1().Select(x =>
				int.Parse(x.Aggregate(new StringBuilder(), (bld, c) => bld.Append(c), bld => bld.ToString())));

			var aField =
				from _ in whiteSpace
				from fld in Field
				from __ in Combinator.Sequence(whiteSpace, Chars.Char(',').Ignore())
				select fld;

			var tmp = Combinator.Choice(
				aField.Select(CreateEnvelope),
				LiteralParsers.Comment.Select(_ => Enumerable.Empty<FieldExpression>())
			);


			var lastField =
				(from _ in whiteSpace
					from fld in Field
					from __ in Combinator.Sequence(whiteSpace, Chars.Char('}').Ignore()).And()
					select fld).Optional()
				.Select(opt => opt.HasValue ? CreateEnvelope(opt.Value) : Enumerable.Empty<FieldExpression>());

			var parser =
				from idx in index
				from _ in Combinator.Sequence(whiteSpace, Chars.Char(':').Ignore(), whiteSpace, Chars.Char('{').Ignore())
				from fld in tmp.Many0()
				from lastFld in lastField
				let f = fld.SelectMany(x => x).Concat(lastFld)
				from __ in Combinator.Sequence(whiteSpace, Chars.Char('}').Ignore())
				select new DatumExpression(idx, f);

			return parser;
		}

		public static Parser<char, IEnumerable<DatumExpression>> BuildData()
		{
			var whiteSpace = Chars.WhiteSpace().Many0().Ignore();

			var datumParser =
				from _ in whiteSpace
				from datum in Datum
				from __ in Combinator.Sequence(whiteSpace, Chars.Char(',').Ignore())
				select datum;

			var parser =
				from _ in Combinator.Sequence(whiteSpace, Chars.Char('{').Ignore(), whiteSpace)
				from data in datumParser.Many0()
				from __ in Combinator.Sequence(whiteSpace, Chars.Char('}').Ignore())
				select data;


			return parser;
		}

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

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}