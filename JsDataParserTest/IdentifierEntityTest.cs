﻿/*
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
using JsDataParser.Entities;
using Xunit;

namespace JsDataParserTest
{
	public class IdentifierEntityTest
	{
		[Fact]
		public void CtorTest()
		{
			Assert.Throws<ArgumentNullException>(() => new IdentifierEntity(null, IdentifierTypes.Boolean));
			Assert.Throws<ArgumentException>(() => new IdentifierEntity("hello", 0));
		}

		[Fact]
		public void IntegerValue()
		{
			var target = new IdentifierEntity("11", IdentifierTypes.Integer);
			target.IdentityType.Is(IdentifierTypes.Integer);

			target.Integer.Is(11);

			Assert.Throws<InvalidOperationException>(() => target.Boolean);
			Assert.Throws<InvalidOperationException>(() => target.Identity);
			Assert.Throws<InvalidOperationException>(() => target.Real);
			Assert.Throws<InvalidOperationException>(() => target.String);

			var tmp = new IdentifierEntity("11", IdentifierTypes.Integer);


			(target == tmp).IsTrue();
			(target != tmp).IsFalse();

			target.Equals(tmp).IsTrue();
			tmp.Equals(target).IsTrue();

			target.GetHashCode().Is(tmp.GetHashCode());

			tmp = new IdentifierEntity("11", IdentifierTypes.Real);

			(target == tmp).IsFalse();
			(target != tmp).IsTrue();


			target.Equals(tmp).IsFalse();
			tmp.Equals(target).IsFalse();

			target.Equals(null).IsFalse();
		}

		[Fact]
		public void StringValue()
		{
			var target = new IdentifierEntity("Hoge", IdentifierTypes.String);
			var constant = new IdentifierEntity("Hoge", IdentifierTypes.Identity);


			target.Equals(constant).IsFalse();
		}
	}
}