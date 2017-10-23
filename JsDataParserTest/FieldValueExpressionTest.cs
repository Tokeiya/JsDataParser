using System;
using System.Linq;
using JsDataParser;
using Xunit;

namespace JsDataParserTest
{
	public class FieldValueExpressionTest
	{
		[Fact]
		public void CtorTest()
		{
			Assert.Throws<ArgumentNullException>(() => new FieldValueExpression(null, TokenTypes.Boolean));
			Assert.Throws<ArgumentException>(() => new FieldValueExpression("hello", TokenTypes.FieldName));
		}

		[Fact]
		public void FieldTypeTest()
		{
			var target = new FieldValueExpression("hello", TokenTypes.Boolean);
			target.FieldType.Is(TokenTypes.Boolean);

			target = new FieldValueExpression(new[] {"1", "2"});
			target.FieldType.Is(TokenTypes.IntegerArray);
		}

		[Fact]
		public void SourceArrayTest()
		{
			var target = new FieldValueExpression(new[] {"1", "2"});

			target.ArraySource.SequenceEqual(new[] {"1", "2"}).IsTrue();
			Assert.Throws<InvalidOperationException>(() => target.Source);
		}

		[Fact]
		public void SourceTest()
		{
			var target = new FieldValueExpression("hello", TokenTypes.Text);

			target.Source.SequenceEqual("hello").IsTrue();
			Assert.Throws<InvalidOperationException>(() => target.ArraySource);
		}
	}
}