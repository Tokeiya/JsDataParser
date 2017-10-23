using System;
using System.Linq;
using JsDataParser;
using Xunit;

namespace JsDataParserTest
{
	public class ObjectExpressionTest
	{
		[Fact]
		public void CtorTest()
		{
			Assert.Throws<ArgumentNullException>(
				() => new ObjectExpression(42, null));

			Assert.Throws<ArgumentException>(
				() => new ObjectExpression(114514, new FieldExpression[] {null}));


			Assert.Throws<ArgumentException>(
				() => new ObjectExpression(42, new[]
				{
					new FieldExpression("name0", "hello", TokenTypes.Text),
					new FieldExpression("name0", "hello", TokenTypes.Text)
				})
			);
		}

		[Fact]
		public void GetFieldValueTest()
		{
			var target = new ObjectExpression(42, new[]
			{
				new FieldExpression("name0", "hello", TokenTypes.Text),
				new FieldExpression("name1", "42", TokenTypes.IntegerNumber)
			});


			target.Fields.Count.Is(2);

			var actual = target.Fields["name0"];
			actual.FieldType.Is(TokenTypes.Text);
			actual.Source.SequenceEqual("hello").IsTrue();

			actual = target.Fields["name1"];
			actual.FieldType.Is(TokenTypes.IntegerNumber);
			actual.Source.SequenceEqual("42").IsTrue();
		}

		[Fact]
		public void IdTest()
		{
			var target = new ObjectExpression(42, new[]
			{
				new FieldExpression("name0", "hello", TokenTypes.Text),
				new FieldExpression("name1", "world", TokenTypes.Text)
			});


			target.Id.Is(42);
		}
	}
}