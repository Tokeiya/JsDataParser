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

			var filtered = iterator.Where(x => ((int?) x.value.RNG ?? 0) != 0);


			foreach (var item in filtered)
			{
				Console.WriteLine($"key:{item.key} value:{item.value.name} RNG:{item.value.RNG}");
			}


		}
	}
}