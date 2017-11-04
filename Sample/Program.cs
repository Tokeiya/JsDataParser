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

			var seq = iterator.Select(x => (rng:(int?) x.value.RNG, key:x.key));


			foreach (var elem in seq)
			{
				Console.WriteLine(elem);
			}

			Console.WriteLine(accum);


		}
	}
}