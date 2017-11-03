using System.Collections;
using System.Collections.Generic;
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

			int hoge = actual.SLOTS[0];

			Assert.True(true);
		}
	}
}