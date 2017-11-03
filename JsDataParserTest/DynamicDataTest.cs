using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsDataParser.DataLoader;
using Xunit;

namespace JsDataParserTest
{
	public class DynamicDataTest
	{
		[Fact]
		public void EnumTest()
		{
			var actual = DataLoader.LoadAsDynamic(".\\Samples\\itemdata.txt");

			var cnt = 0;
			foreach ((dynamic key, dynamic value) elem in actual)
				cnt++;

			cnt.Is(333);
		}

		[Fact]
		public void ReadScalarTest()
		{
			var loaded = DataLoader.LoadAsDynamic(".\\Samples\\DynamicMappingSamples.txt");


			var actual = loaded[1];
			Assert.NotNull(actual);


			Assert.Equal<string>((string) actual.name, "12cm Single Cannon");
		}

		[Fact]
		public void ReadArrayTest()
		{
			var data = DataLoader.LoadAsDynamic(".\\Samples\\DynamicMappingSamples.txt");

			IReadOnlyList<dynamic> actual = data[1].SLOTS;

			actual.Count.Is(3);

			((int)actual[0]).Is(1);
			((int)actual[1]).Is(2);
			((int)actual[2]).Is(3);


			actual = data[1].REALS;
			actual.Count.Is(2);

			((double) actual[0]).Is(1.2);
			((double)actual[1]).Is(1.5);

			actual = data[1].Misc;
			actual.Count.Is(4);

			((int) actual[0]).Is(1);
			((string) actual[1]).Is("a");
			((double) actual[2]).Is(42.195);
		}

		[Fact]
		public void ReadNestedArrayTest()
		{
			var data = DataLoader.LoadAsDynamic(".\\Samples\\DynamicMappingSamples.txt");

			IReadOnlyList<dynamic> actual = data[1].ArrayArray;
			actual.Count.Is(4);

			actual = actual[2];
			actual.Count.Is(2);

			((int) actual[0]).Is(3);
			((int) actual[1]).Is(4);

		}
	}
}