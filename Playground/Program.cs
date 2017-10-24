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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JsDataParser;
using Parseq;
using Parseq.Combinators;

namespace Playground
{
	using CharEnumerable = IEnumerable<char>;
	using ChainFunc = Func<string, string, string>;

	using static Parseq.Combinator;
	using static JsDataParser.DataParser;




	internal class Program
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


		private static void Main()
		{
			TotalTest();

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




			var sample = @"1628: {
name: 'name609',
nameJP: 'name610Jp',
image: 'name610.png',
	type: 'DD',
	//hasBuiltInFD: true,
	HP: 255,
	FP: 130,
	TP: 85,
	AA: 300,
	AR: 273,
	EV: 80,
	ASW: 0,
	LOS: 70,
	LUK: 80,
	RNG: 1,
	SPD: 10,
	TACC: 60,
	SLOTS: [0, 0, 0],
	EQUIPS: [553, 553, 531],
	fuel: 0,
	ammo: 0,
  }".AsStream();

			parser.Run(sample).Case(
				(str, txt) =>
				{
					Console.WriteLine(txt);
					Console.WriteLine("Line:" + str.Current.Value.Item1.Line);
					Console.WriteLine("Column:" + str.Current.Value.Item1.Column);
				},
				(_, cap) =>
				{
					Console.WriteLine("sucesss");
				}
			);

			Console.WriteLine("Press enter to exit.");
			Console.ReadLine();



		}


		private static void TotalTest()
		{
			string scr;

			using (var rdr = new StreamReader(".//Samples//HugeSample.txt"))
			{
				scr = rdr.ReadToEnd();
			}

			var reply = JsDataParser.DataParser.Data.Run(scr.AsStream());

			reply.Case(
				(str, txt) =>
				{
					Console.WriteLine(txt);
					Console.WriteLine("Line:" + str.Current.Value.Item1.Line);
					Console.WriteLine("Column:" + str.Current.Value.Item1.Column);
				},
				(_, cap) => Console.WriteLine("success"));

			Console.WriteLine("Press enter to exit.");
			Console.ReadLine();
		}
	}
}