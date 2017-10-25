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
using System.Linq;
using JsDataParser;
using JsDataParser.Parser;
using Parseq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using static JsDataParser.Parser.DataParser;

namespace JsDataParserTest
{
	public class FieldParserTest
	{
		public FieldParserTest(ITestOutputHelper output)
		{
			this.output = output;
		}

		private readonly ITestOutputHelper output;

		[Fact]
		public void DatumParserTest()
		{
			var sample = @"1657:{
		name: 'foo 2',
		nameJP: 'foo-壊',
		image: 'foo.jpg',
		type: 'Installation',
		installtype: 3,
		divebombWeak: 2.1,
		HP: 430,
		FP: 160,
		TP: 98,
		AA: 80,
		AR: 160,
		EV: 20,
		ASW: 0,
		LOS: 90,
		LUK: 70,
		unknownstats: true,
		RNG: 3,
		SPD: 0,
		TACC: 70,
		SLOTS: [24, 24, 12, 12],
		EQUIPS: [562, 562, 561, 561],
		fuel: 0,
		ammo: 0,
		canTorp: function() { return (this.HP/this.maxHP > .5); },






}".AsStream();

			Datum.Run(sample).Case(
				(_, str) => { Assert.True(false, str); },
				(_, cap) =>
				{
					cap.Id.Is(1657);
					cap.Fields.Count.Is(24);

					cap.Fields["fuel"].FieldType.Is(TokenTypes.IntegerNumber);
					cap.Fields["fuel"].Source.SequenceEqual("0").IsTrue();
				});
		}


		[Fact]
		public void FieldTest()
		{
			Field.Run("TACC: 70".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.IntegerNumber);
						cap.Source.SequenceEqual("70").IsTrue();
					});


			Field.Run("TACC: 7.0".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.RealNumber);
						cap.Source.SequenceEqual("7.0").IsTrue();
					});


			Field.Run("TACC: true".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.Boolean);
						cap.Source.SequenceEqual("true").IsTrue();
					});


			Field.Run("TACC: false".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.Boolean);
						cap.Source.SequenceEqual("false").IsTrue();
					});


			Field.Run("TACC: function() { return (this.HP/this.maxHP > .5); }".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.Function);
						cap.Source.SequenceEqual("function () { return (this.HP/this.maxHP > .5); }").IsTrue();
					});

			Field.Run("TACC: [0,1,2,3,4]".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.IntegerArray);

						cap.ArraySource.Count.Is(5);

						for (var i = 0; i < 5; i++)
							cap.ArraySource[i].SequenceEqual(i.ToString()).IsTrue();
					});
		}

		[Fact]
		public void DataTest()
		{
			#region sample
			var sample = @"
{
1657: {
		name: 'foo 2',
		nameJP: 'foo-壊',
		image: 'foo.jpg',
		type: 'Installation',
		installtype: 3,
		divebombWeak: 2.1,
		HP: 430,
		FP: 160,
		TP: 98,
		AA: 80,
		AR: 160,
		EV: 20,
		ASW: 0,
		LOS: 90,
		LUK: 70,
		unknownstats: true,
		RNG: 3,
		SPD: 0,
		TACC: 70,
		SLOTS: [24, 24, 12, 12],
		EQUIPS: [562, 562, 561, 561],
		fuel: 0,
		ammo: 0,
		canTorp: function() { return (this.HP/this.maxHP > .5); },
	},
	1658: {
		name: 'foo Damaged 3',
		nameJP: 'foo',
		image: 'foo.jpg',
		type: 'Installation',
		installtype: 3,
		divebombWeak: 2.1,
		HP: 480,
		FP: 190,
		TP: 118,
		AA: 80,
		AR: 190,
		EV: 25,
		ASW: 0,
		LOS: 90,
		LUK: 75,
		unknownstats: true,
		RNG: 1,
		SPD: 0,
		TACC: 75,
		SLOTS: [36, 24, 24, 12],
		EQUIPS: [562, 562, 562, 561],
		fuel: 0,
		ammo: 0,
		canTorp: function() { return (this.HP/this.maxHP > .5); },
	},
  }
".AsStream();
			#endregion

			var actual = Data.Run(sample);

			actual.Case(
				(_, __) => Assert.False(true),
				(_, cap) =>
				{
					var ary = cap.ToArray();
					ary.Length.Is(2);

					ary[0].Id.Is(1657);
					ary[1].Id.Is(1658);


				});


		}
	}
}