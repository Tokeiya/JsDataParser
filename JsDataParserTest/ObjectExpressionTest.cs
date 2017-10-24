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
using System.Linq;
using JsDataParser;
using JsDataParser.Parser;
using Xunit;

namespace JsDataParserTest
{
	public class ObjectExpressionTest
	{
		[Fact]
		public void CtorTest()
		{
			Assert.Throws<ArgumentNullException>(
				() => new DatumExpression(42, null));

			Assert.Throws<ArgumentException>(
				() => new DatumExpression(114514, new FieldExpression[] {null}));


			Assert.Throws<ArgumentException>(
				() => new DatumExpression(42, new[]
				{
					new FieldExpression("name0", "hello", TokenTypes.Text),
					new FieldExpression("name0", "hello", TokenTypes.Text)
				})
			);
		}

		[Fact]
		public void GetFieldValueTest()
		{
			var target = new DatumExpression(42, new[]
			{
				new FieldExpression("name0", "hello", TokenTypes.Text),
				new FieldExpression("name1", "42", TokenTypes.IntegerNumber)
			});


			target.Fields.Count.Is(2);

			var actual = target.Fields["name0"];
			actual.FieldType.Is(TokenTypes.Text);
			actual.Source.SequenceEqual("hello").IsTrue();

			actual = target.Fields["name1"];
			actual.FieldType.Is(TokenTypes.IntegerNumber);
			actual.Source.SequenceEqual("42").IsTrue();
		}

		[Fact]
		public void IdTest()
		{
			var target = new DatumExpression(42, new[]
			{
				new FieldExpression("name0", "hello", TokenTypes.Text),
				new FieldExpression("name1", "world", TokenTypes.Text)
			});


			target.Id.Is(42);
		}
	}
}