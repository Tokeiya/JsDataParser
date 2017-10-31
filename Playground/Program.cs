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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using JsDataParser.DataLoader;
using JsDataParser.Entities;

namespace Playground
{
	public class Sample
	{
		public string Name { get; set; }
		public string NameJP { get; set; }
		public string Image { get; set; }

		//[DestinationName("type")]
		public object ShipType { get; set; }

		public IReadOnlyList<int> Slots { get; set; }


	}


	//1: {
	//	name: '12cm Single Cannon',
	//	nameJP: '12cm単装砲',
	//	added: '2013-04-17',
	//	type: MAINGUNS,
	//	btype: B_MAINGUN,
	//	atype: A_GUN,
	//	FP: 1,
	//	AA: 1,
	//	RNG: 1
	//},

	class TaskProcedure
	{
		private readonly CancellationTokenSource _source = new CancellationTokenSource();

		public void Stop()
		{
			_source.Cancel();
		}

		public void Proc()
		{
			for (int i = 0;; i++)
			{
				Task.Delay(500);
				Console.WriteLine(i);
			}

		}
	}

	internal class Program
	{


		private static void Main()
		{
			var tmp=new TaskProcedure();

			var task = new Task(tmp.Proc);

			task.Start();


			Console.ReadLine();

			tmp.Stop();


		}
	}
}
