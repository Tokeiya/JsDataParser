using System;
using JsDataParser;
using Xunit;

namespace JsDataParserTest
{
	public class FieldExpressionTest
	{
		[Fact]
		public void CtorTest()
		{
			Assert.Throws<ArgumentNullException>(() => new FieldExpression("name", null, TokenTypes.Boolean));
			Assert.Throws<ArgumentNullException>(() => new FieldExpression(null, "hello", TokenTypes.Text));
			Assert.Throws<ArgumentException>(() => new FieldExpression("", "hello", TokenTypes.Text));


			Assert.Throws<ArgumentException>(() => new FieldExpression("name", "hello", TokenTypes.FieldName));
		}

		[Fact]
		public void NameTest()
		{
			var target = new FieldExpression("name", "42", TokenTypes.Text);
			target.Name.Is("name");
		}
	}
}