using System;
using JsDataParser.DataLoader;

namespace Sample
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			dynamic data = DataLoader.LoadAsDynamic("SampleData.txt");
			dynamic datum = data[1];

			string function = datum.canTorp.Function;
			Console.WriteLine(function);

			string identity = datum.type.Identity;
			Console.WriteLine(identity);

		}
	}
}