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
using System.Xml.Serialization;
using Parseq;
using Parseq.Combinators;

namespace JsDataParser.Parser
{
	using static Chars;

	internal static class ObjectParser
	{
		static ObjectParser()
		{
			IntegerArray = BuildIntegerArray();
			Object = Combinator.Lazy(BuildObject);
			Property = Combinator.Lazy(BuildProperty);
		}

		private static readonly Parser<char, Unit> WhiteSpace = Chars.WhiteSpace().Many0().Ignore();

		public static readonly Parser<char, ValueEntity> IntegerArray;

		public static readonly Parser<char, PropertyEntity> Property;
		public static readonly Parser<char, ObjectEntity> Object;




		private static Parser<char, PropertyEntity> BuildProperty()
		{
			var scalarField = Combinator.Choice(
				LiteralParser.IntegerNumber.Select(cap => new ValueEntity(cap, ValueTypes.Integer)),
				LiteralParser.RealNumber.Select(cap => new ValueEntity(cap, ValueTypes.Real)),
				LiteralParser.String.Select(cap => new ValueEntity(cap, ValueTypes.String)),
				LiteralParser.Bool.Select(cap => new ValueEntity(cap, ValueTypes.Boolean)),
				LiteralParser.IdentifierName.Select(cap => new ValueEntity(cap, ValueTypes.ConstantName)),
				LiteralParser.TinyFunction.Select(cap => new ValueEntity(cap, ValueTypes.Function))
			);

			var value = Combinator.Choice(
				scalarField,
				IntegerArray,
				Object.Select(cap => new ValueEntity(cap))
			);

			var prop =
				from name in LiteralParser.IdentifierName
				from _ in Combinator.Sequence(WhiteSpace, Char(':').Ignore(), WhiteSpace)
				from v in value
				select new PropertyEntity(name, v);

			return prop;
		}

		private static Parser<char, ObjectEntity> BuildObject()
		{
			IEnumerable<PropertyEntity> build(IEnumerable<PropertyEntity> fwd, IOption<PropertyEntity> last)
			{
				foreach (var elem in fwd)
				{
					yield return elem;
				}

				if (last.HasValue) yield return last.Value;
			}

			var identity =
				(from idx in LiteralParser.IntegerNumber
					from _ in Combinator.Sequence(WhiteSpace, Char(':').Ignore(), WhiteSpace)
					select new ValueEntity(idx, ValueTypes.Integer)).Optional()
				.Select(x => x.HasValue ? x.Value : new ValueEntity(Enumerable.Empty<char>(), ValueTypes.Unit));


			var fwdProp =
				(from _ in WhiteSpace
				from p in Property
				from __ in Combinator.Choice(WhiteSpace, Char(',').Ignore())
				select p).Many0();

			var lastProp =
			(from _ in WhiteSpace
				from p in Property
				from __ in WhiteSpace
				select p).Optional();


			var tmp =
				from _ in Combinator.Sequence(WhiteSpace, Char('{').Ignore(), WhiteSpace)
				from id in identity
				from __ in Combinator.Sequence(WhiteSpace, Char('{').Ignore())
				from fwd in fwdProp
				from lst in lastProp
				select new ObjectEntity(id, build(fwd, lst));

			return tmp;
		}



		private static Parser<char, ValueEntity> BuildIntegerArray()
		{
			IEnumerable<ValueEntity> build(IEnumerable<ValueEntity> fwd, ValueEntity last)
			{
				foreach (var elem in fwd)
					yield return elem;
				yield return last;
			}

			var delimiter = Char(',').Ignore();

			var element =
				from _ in WhiteSpace
				from num in LiteralParser.IntegerNumber
				from __ in Combinator.Sequence(WhiteSpace, delimiter)
				select new ValueEntity(num, ValueTypes.Integer);

			var lastElement =
				from _ in WhiteSpace
				from num in LiteralParser.IntegerNumber
				from __ in WhiteSpace
				select new ValueEntity(num, ValueTypes.Integer);

			var emptyArray =
				from _ in Char('[')
				from __ in WhiteSpace
				from ___ in Char(']')
				select new ValueEntity(Array.Empty<ValueEntity>());

			var array =
				from _ in Char('[')
				from elems in element.Many0()
				from lastElem in lastElement
				select new ValueEntity(build(elems, lastElem));

			return emptyArray.Or(array);
		}
	}
}