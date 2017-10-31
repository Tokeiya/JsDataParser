using System;
using System.Collections.Generic;
using System.Text;
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
			foreach ((dynamic key,dynamic value) elem in actual)
			{
				cnt++;
			}

			cnt.Is(333);
		}
	}
}
