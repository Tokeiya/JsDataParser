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
using JsDataParser.Entities;

namespace Playground
{
	internal static class Extension
	{
		public static void Dump(this object value)
		{
			Console.WriteLine(value);
		}

		public static void DumpAndQuery(this object value, string message = "Press enter to continue.")
		{
			Console.WriteLine(value);
			Console.WriteLine(message);
			Console.ReadLine();
		}
	}

	internal class Program
	{
		private const string Path = ".\\Samples\\shipdata.txt";

		private static void Main()
		{
			List<(IdentifierEntity, ValueEntity)> list = new List<(IdentifierEntity, ValueEntity)>();

			//12cm Single Cannon
			var contents = new[]
			{
				(new IdentifierEntity("name", true), new ValueEntity("12cm Single Cannon", ValueTypes.String)),
				(new IdentifierEntity("atype", true), new ValueEntity("A_GUN", ValueTypes.Identity) ),
				(new IdentifierEntity("FP", true), new ValueEntity("2", ValueTypes.Integer) )
			};

			var nestedObject = new ObjectLiteralEntity(contents);

			(IdentifierEntity id, ValueEntity value) tmp = (null, null);
			tmp.id = new IdentifierEntity(1);
			tmp.value = new ValueEntity(nestedObject);

			list.Add(tmp);

			//12.7cm Twin Cannon
			contents = new[]
			{
				(new IdentifierEntity("name", true), new ValueEntity("12.7cm Twin Cannon", ValueTypes.String)),
				(new IdentifierEntity("atype", true), new ValueEntity("A_GUN", ValueTypes.Identity) ),
				(new IdentifierEntity("FP", true), new ValueEntity("2", ValueTypes.Integer) )
			};

			nestedObject = new ObjectLiteralEntity(contents);

			tmp.id = new IdentifierEntity(2);
			tmp.value = new ValueEntity(nestedObject);

			list.Add(tmp);

			//10cm Twin High-Angle Cannon
			contents = new[]
			{
				(new IdentifierEntity("name", true), new ValueEntity("10cm Twin High-Angle Cannon", ValueTypes.String)),
				(new IdentifierEntity("atype", true), new ValueEntity("A_HAGUN", ValueTypes.Identity) ),
				(new IdentifierEntity("FP", true), new ValueEntity("2", ValueTypes.Integer) )
			};

			nestedObject = new ObjectLiteralEntity(contents);

			tmp.id = new IdentifierEntity(2);
			tmp.value = new ValueEntity(nestedObject);

			list.Add(tmp);

			//こいつがLoadRawと同じ。
			var rootEntity = new ObjectLiteralEntity(list);
		}
	}
}