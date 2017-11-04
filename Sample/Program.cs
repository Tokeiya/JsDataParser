using System;
using System.Collections.Generic;
using System.Linq;
using JsDataParser.DataLoader;

namespace Sample
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			dynamic data = DataLoader.LoadAsDynamic(".\\DataFolder\\itemdata.txt");

			//これがさっき言ってた型を固めるすなわちゆるふわなdynamicからIEnumerableに確定させる。
			IEnumerable<(dynamic key, dynamic value)> iterator = data;

			int accum = 0;

			foreach (var elem in iterator)
			{
				dynamic d = elem.value.RNG;

				Console.WriteLine($"Key:{elem.key} d:{d}");
			}

			Console.WriteLine(accum);


		}
	}
}