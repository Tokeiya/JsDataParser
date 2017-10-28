using JsDataParser;
using JsDataParser.Entities;
using Xunit;

namespace JsDataParserTest
{
	public class VerifyExtensionsTest
	{
		[Fact]
		public void ValueTypesTest()
		{
			((ValueTypes) 0).Verify().IsFalse();
		}
	}
}