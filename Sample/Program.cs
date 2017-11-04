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


			foreach (var elem in iterator)
			{
				int? v = elem.value.RNG;
			}

			//int?にキャストしないとRNGがそもそも無い時にこけてしまうのでその対策
			var filter=iterator.Where(x => (((int?) x.value.RNG) ?? 0) == 2);

			foreach (var elem in filter)
			{
				Console.WriteLine(elem.key);
				Console.WriteLine(elem.value.name);
			}


		}
	}
}