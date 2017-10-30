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
using System.IO;
using JsDataParser.DataLoader;
using JsDataParser.Entities;
using JsDataParser.Parser;
using Parseq;

namespace Playground
{
	internal class Program
	{
		private static void Main()
		{
			var sampl = @"{
				35: {
					nameA: 'Type 3 Shell',
					nameJP: '三式弾',
					added: '2013-04-17',
					type: TYPESHELL,
					btype: B_TYPESHELL,
					atype: A_TYPESHELL,
					AA: 5,
					'piyo':10.5,
					piyo:21.55,
					Array:[0, 2.0, 'aa',{Hoge:'hello',Piyo:'world'}],
					10:20,
					F:function(){},
				},
			}";


			var root = DataLoader.LoadAsDynamic(".\\Samples\\itemdata.txt");


			int s = root[1].Hoge;

			Console.WriteLine(s);


			Console.ReadLine();
		}

		private static ObjectLiteralEntity GetValue()
		{
			ObjectLiteralEntity ret = default;
			using (var rdr = new StreamReader(".\\Samples\\sample.txt"))
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
					(_, cap) =>
					{
						ret = cap;
						Console.WriteLine("success");
					});
			}

			return ret;
		}
	}
}