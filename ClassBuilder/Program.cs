using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsDataParser.DataLoader;
using JsDataParser.Entities;
using SeedCreate.CsvManipulator;

namespace ClassBuilder
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var raw = DataLoader.LoadRaw(".\\Samples\\shipdata.txt");

			var dict = new Dictionary<IdentifierEntity, HashSet<ValueTypes>>();

			foreach (var record in raw.Where(x => x.Value.ValueType == ValueTypes.Object))
			foreach (var property in record.Value.NestedObject)
				if (dict.TryGetValue(property.Key, out var set))
				{
					set.Add(property.Value.ValueType);
				}
				else
				{
					set = new HashSet<ValueTypes> {property.Value.ValueType};
					dict.Add(property.Key, set);
				}


			var array = (ValueTypes[]) Enum.GetValues(typeof(ValueTypes));

			using (var wtr = new StreamWriter("E:\\output.txt"))
			using (var tsv = new CsvWriter(wtr, '\t', '"'))
			{
				tsv.WriteField("Name");
				tsv.WriteField("String");
				tsv.WriteField("Integer");
				tsv.WriteField("Real");
				tsv.WriteField("Bool");
				tsv.WriteField("Function");
				tsv.WriteField("Identity");
				tsv.WriteField("Object");
				tsv.WriteField("Array");
				tsv.CreateRecord();


				foreach (var prop in dict)
				{
					tsv.WriteField(prop.Key.Object.ToString());


					foreach (var v in array)
						if (prop.Value.Contains(v))
							tsv.WriteField("○");
						else
							tsv.WriteField("×");

					tsv.CreateRecord();
				}
			}
		}
	}
}