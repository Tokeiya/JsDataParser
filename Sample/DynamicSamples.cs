using System;
using JsDataParser.DataLoader;

namespace Sample
{
	public static class DynamicSamples
	{
		public static void ReadScalarValue()
		{
			var itemData = DataLoader.LoadAsDynamic(".\\DataFolder\\itemdata.txt");

			//識別子じゃ無くて､リテラルの場合は､添え字でアクセスします｡
			var type22Rader = itemData[28];


			string txt = type22Rader.name;
			Console.WriteLine(txt);

			//大文字と小文字はすべて判定してるので､文字通りに書いて下さい｡
			Console.WriteLine(type22Rader.type);
			Console.WriteLine(type22Rader.improveType);
			Console.WriteLine(type22Rader.LOS);

			Console.WriteLine();

			//Objectの中のObjectは以下のように呼びます｡
			var improve = type22Rader.improve;
			Console.WriteLine(improve.ACCshell);
			Console.WriteLine();

			//列挙は､Key/Valueスタイルです｡

			foreach ((dynamic key, dynamic value) elem in type22Rader)
				Console.WriteLine($"key:{elem.key} value:{elem.value}");
		}
	}
}