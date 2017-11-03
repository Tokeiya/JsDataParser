using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsDataParser.DataLoader;
using JsDataParser.Mapping;

namespace Sample
{
	//全部書く必要は無いです｡
	public class NameOnly
	{
		public string Name { get; set; }
		public string NameJp { get; set; }
	}

	//配列の例
	public class Ship
	{
		public string Name { get; set; }
	
		public int[] Slots { get; set; }
	}

	class TinyMapperSample
	{
		/// <summary>
		/// すべてのフィールドが無くてもマップ可能です｡
		/// また､大文字､小文字は無視します｡
		/// </summary>
		public static void NameOnly()
		{
			//最初にRawデータを読みます｡
			var rawData = DataLoader.LoadRaw(".\\DataFolder\\itemdata.txt");

			//一括マッピング
			var itemData = TinyMapper<NameOnly>.MultiMap(rawData).ToArray();

			//Key/Valueになってる｡
			foreach (var elem in itemData)
			{
				Console.WriteLine($"Key:{elem.Key} Name:{elem.Value.Name} Jp:{elem.Value.NameJp}");
			}
		}


		public static void ArrayMapping()
		{
			var rawData = DataLoader.LoadRaw(".\\DataFolder\\shipdata.txt");

			var ships = TinyMapper<Ship>.MultiMap(rawData).Where(x => x.Value.Slots != null).ToArray();

			foreach (var ship in ships)
			{
				Console.WriteLine($"id:{ship.Key} name:{ship.Value.Name}");
				Console.WriteLine(
					$"Slots:[{ship.Value.Slots.Aggregate(new StringBuilder(" "), (bld, i) => bld.Append(i).Append(','), bld => bld.Remove(bld.Length - 1, 1).ToString())}]");
				Console.WriteLine();
			}


		}


	}
}
