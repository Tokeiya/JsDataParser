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
using System.Collections.Generic;
using System.Linq;
using JsDataParser.Entities;
using Parseq;
using Parseq.Combinators;

namespace JsDataParser.Parser
{
	using static Chars;

	internal static class ObjectParser
	{
		public static readonly Parser<char, IdentifierEntity> Identifier;
		public static readonly Parser<char, IReadOnlyList<ValueEntity>> ArrayLiteral;
		public static readonly Parser<char, ValueEntity> Value;
		public static readonly Parser<char, (IdentifierEntity identifier, ValueEntity value)> Property;
		public static readonly Parser<char, ObjectLiteralEntity> LiteralObject;

		public static readonly Parser<char, IEnumerable<(IndexedIdentiferEntity identity, ObjectLiteralEntity value)>>
			CollectionAssignment;


		static ObjectParser()
		{
			var ignore = Combinator.Choice(
				WhiteSpace().Ignore(),
				LiteralParser.Comment.Ignore()
			).Many0().Ignore();


			var objectFixedPoint = new FixedPoint<char, ObjectLiteralEntity>("obj");

			var valueFixedPoint = new FixedPoint<char, ValueEntity>("value");

			var arrayFixedPoint = new FixedPoint<char, IReadOnlyList<ValueEntity>>("Array");


			//Identifier
			{
				Identifier = Combinator.Choice(
					LiteralParser.RealNumber.Select(x => new IdentifierEntity(x, IdentifierTypes.Real)),
					LiteralParser.IntegerNumber.Select(x => new IdentifierEntity(x, IdentifierTypes.Integer)),
					LiteralParser.Bool.Select(x => new IdentifierEntity(x, IdentifierTypes.Boolean)),
					LiteralParser.IdentifierName.Select(x => new IdentifierEntity(x, IdentifierTypes.Identity)),
					LiteralParser.String.Select(x => new IdentifierEntity(x, IdentifierTypes.String))
				);
			}

			//Array
			{
				var foward =
					from _ in ignore
					from value in (Parser<char, ValueEntity>) valueFixedPoint.Parse
					from __ in Combinator.Sequence(ignore, Char(',').Ignore())
					select value;

				var last =
					from _ in ignore
					from value in (Parser<char, ValueEntity>) valueFixedPoint.Parse
					from __ in ignore
					select value;


				var tmp =
					from _ in Char('[')
					from __ in ignore
					from fowards in foward.Many0()
					from l in last
					from ___ in Char(']')
					select (IReadOnlyList<ValueEntity>) Concat(fowards, l).ToArray();


				var empty =
					from _ in Char('[')
					from __ in ignore
					from ___ in Char(']')
					select (IReadOnlyList<ValueEntity>) Array.Empty<ValueEntity>();

				arrayFixedPoint.FixedParser = Combinator.Choice(tmp, empty);

				ArrayLiteral = arrayFixedPoint.Parse;
			}


			//Value
			{
				var naiveValue = Combinator.Choice(
					LiteralParser.TinyFunction.Select(x => new ValueEntity(x, ValueTypes.Function)),
					LiteralParser.RealNumber.Select(x => new ValueEntity(x, ValueTypes.Real)),
					LiteralParser.IntegerNumber.Select(x => new ValueEntity(x, ValueTypes.Integer)),
					LiteralParser.Bool.Select(x => new ValueEntity(x, ValueTypes.Boolean)),
					LiteralParser.String.Select(x => new ValueEntity(x, ValueTypes.String)),
					LiteralParser.IdentifierName.Select(x => new ValueEntity(x, ValueTypes.Identity))
				);

				var arrayValue = ArrayLiteral.Select(x => new ValueEntity(x));
				var nestedObjectValue =
					((Parser<char, ObjectLiteralEntity>) objectFixedPoint.Parse).Select(x => new ValueEntity(x));


				valueFixedPoint.FixedParser = Combinator.Choice(
					arrayValue,
					naiveValue,
					nestedObjectValue
				);

				Value = valueFixedPoint.Parse;
			}

			//Property
			{
				var tmp =
					from identifier in Identifier
					from _ in Combinator.Sequence(ignore, Char(':').Ignore(), ignore)
					from value in Value
					select (identifier, value);

				Property = tmp;
			}

			//LiteralObject
			{
				var fwdProp =
					from _ in ignore
					from prop in Property
					from __ in Combinator.Sequence(ignore.Ignore(), Char(',').Ignore())
					select prop;

				var lstProp =
					from _ in ignore
					from prop in Property.Optional()
					select prop;

				var contents =
					from props in fwdProp.Many0()
					from last in lstProp
					select Concat(props, last);

				var empty = Combinator.Sequence(Char('{').Ignore(), ignore, Char('}').Ignore())
					.Select(_ => new ObjectLiteralEntity(Enumerable.Empty<(IdentifierEntity, ValueEntity)>()));


				var tmp = from _ in Combinator.Sequence(ignore, Char('{').Ignore(), ignore)
					from props in contents
					from __ in Combinator.Sequence(ignore, Char('}').Ignore())
					select new ObjectLiteralEntity(props);

				tmp = Combinator.Choice(tmp, empty);

				objectFixedPoint.FixedParser = tmp;
				LiteralObject = objectFixedPoint.Parse;
			}

			//CollectionAssignment
			{
				var id = from identity in LiteralParser.IdentifierName
					from _ in Combinator.Sequence(ignore, Char('[').Ignore(), ignore)
					from index in Identifier
					from __ in Combinator.Sequence(ignore, Char(']').Ignore(), ignore, Char('=').Ignore(), ignore)
					select new IndexedIdentiferEntity(identity, index);

				var value = from literalObject in LiteralObject
					from _ in Combinator.Sequence(ignore, Char(';').Ignore(), ignore)
					select literalObject;

				var tmp = from idxIdentity in id
					from valueEntity in value
					select (idxIdentity, valueEntity);


				CollectionAssignment = from values in tmp.Many0()
					select values;
			}
		}


		private static IEnumerable<T> Concat<T>(IEnumerable<T> foward, T last)
		{
			foreach (var elem in foward)
				yield return elem;
			yield return last;
		}

		private static IEnumerable<T> Concat<T>(IEnumerable<T> foward, IOption<T> last)
		{
			foreach (var elem in foward)
				yield return elem;

			if (last.HasValue) yield return last.Value;
		}
	}
}