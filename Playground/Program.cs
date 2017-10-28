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
using System.Dynamic;
using System.IO;
using System.Linq;
using JsDataParser.Parser;
using Parseq;

namespace Playground
{
	class DynamicSample : DynamicObject
	{
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{

			return base.TryInvokeMember(binder, args, out result);
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if (binder.Type == typeof(double))
			{
				result = 42.195d;
				return true;
			}

			if (binder.Type == typeof(int) && binder.Explicit)
			{
				result = 42;
				return true;
			}

			else if (binder.Type == typeof(float) && binder.Explicit)
			{
				result = (float) 42.195;
				return true;
			}

			if (binder.Type == typeof(IEnumerable))
			{
				result = Enumerable.Range(0, 100);
				return true;
			}

			result = default;
			return false;

		}

		public IEnumerator<int> GetEnumerator()
		{
			return Enumerable.Range(0, 10).GetEnumerator();
		}

	}

	internal class Program
	{
		private static void Main()
		{


			dynamic d=new DynamicSample();

			foreach (int i in d)
			{
				Console.WriteLine(i);
			}

		}

		private static void GetValue()
		{
			using (var rdr = new StreamReader(".\\Samples\\hugesample.txt"))
			{
				ObjectParser.LiteralObject.Run(rdr.AsStream()).Case(
					(stream, __) =>
					{
						try
						{
							Console.WriteLine(
								$"Line:{stream.Current.Value.Item1.Line} Column:{stream.Current.Value.Item1.Column} Token:{stream.Current.Value.Item0}");
						}
						catch (Exception)
						{
							Console.WriteLine("empty");
						}
					},
					(_, __) => { Console.WriteLine("success"); });
			}

			Console.ReadLine();
		}
	}
}