using System;
using System.Collections.Generic;
using System.Linq;
using JsDataParser.Entities;
using JsDataParser.Parser;
using Parseq;
using Parseq.Combinators;
using Xunit;
using Xunit.Abstractions;

namespace JsDataParserTest
{
	using static JsDataParser.Parser.ObjectParser;

	//Currently "true" and "false" value can't recognize as a boolean.

	internal static class ObjectParserTestHelper
	{


		public static void AreSuccess(this Parser<char, IdentifierEntity> parser, string value, IdentifierEntity expected)
		{
			expected.IsNotNull();
			parser.IsNotNull();
			value.IsNotNull();

			parser.Run(value.AsStream()).Case(
				(_, __) => Assert.False(true),
				(_, cap) =>
				{
					cap.Equals(expected).IsTrue();
				});
		}

		public static void AreSuucess(this Parser<char, IReadOnlyList<ValueEntity>> parser, string value,params ValueEntity[] expected)
		{
			parser.IsNotNull();
			value.IsNotNull();
			expected.IsNotNull();

			parser.Run(value.AsStream())
				.Case(
					(_, __) => Assert.False(true),
					(_, cap) =>
					{
						cap.Count.Is(expected.Length);

						foreach (var elem in expected.Zip(cap,(exp,act)=>(exp,act)))
						{
							elem.exp.ValueType.Is(elem.act.ValueType);
							elem.exp.Object.Equals(elem.act.Object).IsTrue();
						}

					});

		}

		public static void AreSuccess(this Parser<char, ValueEntity> parser, string value, ValueEntity expected,ITestOutputHelper output)
		{
			parser.IsNotNull();
			value.IsNotNull();
			expected.IsNotNull();

			parser.Run(value.AsStream()).Case(
				(stream, msg) => Assert.False(true, $"{msg}"),
				(stream, cap) =>
				{
					output.WriteLine($"ValueType:{cap.ValueType.ToString()} Object:{cap.Object.ToString()}\n");

					cap.ValueType.Is(expected.ValueType);
					cap.Object.Equals(expected.Object).IsTrue();
				}
			);


		}

	}



	public class ObjectParserTest
	{

		private readonly ITestOutputHelper _output;

		public ObjectParserTest(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void IdentifierTest()
		{
			Identifier.AreSuccess("11", new IdentifierEntity("11", IdentifierTypes.Integer));
			Identifier.AreSuccess("true", new IdentifierEntity("true", IdentifierTypes.Boolean));
			Identifier.AreSuccess("false", new IdentifierEntity("false", IdentifierTypes.Boolean));

			Identifier.AreSuccess("'hello'", new IdentifierEntity("hello", IdentifierTypes.String));
			Identifier.AreSuccess("CONST", new IdentifierEntity("CONST", IdentifierTypes.Constant));
			Identifier.AreSuccess("11.:", new IdentifierEntity("11",IdentifierTypes.Real));
		}

		[Fact]
		public void NaiveValueTest()
		{
			Value.AreSuccess("function(){hoge}", new ValueEntity("function () {hoge}", ValueTypes.Function), _output);
			Value.AreSuccess("42", new ValueEntity("42", ValueTypes.Integer),_output);
			Value.AreSuccess("42.", new ValueEntity("42", ValueTypes.Real),_output);
			Value.AreSuccess("HE_LLO",new ValueEntity("HE_LLO", ValueTypes.ConstantName),_output);
			Value.AreSuccess("'HE_LLO'", new ValueEntity("HE_LLO", ValueTypes.String),_output);
			Value.AreSuccess("true", new ValueEntity("true", ValueTypes.Boolean), _output);

			Value.AreSuccess("Av98Ingram", new ValueEntity("Av98Ingram", ValueTypes.ConstantName), _output);

		}



		[Fact]
		public void ArrayValueTest()
		{
			var actual = Value.Run("[1,2,3]".AsStream());

			actual.Case(
				(_, __) => Assert.False(true, "FAIL!"),
				(_, cap) =>
				{
					cap.ValueType.Is(ValueTypes.Array);
					cap.Array.Count.Is(3);
					cap.Array.Select(x=>x.Integer).SequenceEqual(new []{1,2,3}).IsTrue();
				}
			);


		}


		[Fact]
		public void ArrayLiteralTest()
		{
			ArrayLiteral.AreSuucess("[]", System.Array.Empty<ValueEntity>());
			ArrayLiteral.AreSuucess("[42]", new ValueEntity("42", ValueTypes.Integer));
			ArrayLiteral.AreSuucess("[1,2,3]",
				new ValueEntity("1", ValueTypes.Integer),
				new ValueEntity("2", ValueTypes.Integer),
				new ValueEntity("3", ValueTypes.Integer));

			ArrayLiteral.AreSuucess("['a','b']",
				new ValueEntity("a", ValueTypes.String),
				new ValueEntity("b", ValueTypes.String));

		}

		[Fact]
		public void NaivePropertyTest()
		{
			Property.Run("AA:2".AsStream()).Case(
				(_, __) => Assert.False(true, "FAIL!"),
				(_, cap) =>
				{
					cap.identifier.Constant.Is("AA");
					cap.value.Integer.Is(2);
				}
			);
		}

		[Fact]
		public void ArrayPropertyTest()
		{
			Property.Run("AA	:           [1,2,3]".AsStream()).Case(
				(_, __) => Assert.False(true, "FAIL!"),
				(_, cap) =>
				{
					cap.identifier.Constant.Is("AA");
					var array = cap.value.Array;
					array.Count.Is(3);
					array.Select(x => x.Integer).SequenceEqual(new[] {1, 2, 3}).IsTrue();
				}
			);
		}

		[Fact]
		public void ObjectTest()
		{
			LiteralObject.Run("{Value:42,Hoge:43}".AsStream()).Case(
				(_, __) => Assert.False(true, "FAIL!"),
				(_, cap) =>
				{
					cap.Count.Is(2);
				});

			LiteralObject.Run("{123:{foo:42,bar:{piyo:43}}}".AsStream()).Case(
				(_, __) => Assert.False(true, "FAIL!"),
				(_, cap) =>
				{
					var piv = cap;

					piv.Count.Is(1);

					var val = cap[new IdentifierEntity("123", IdentifierTypes.Integer)];
					val.ValueType.Is(ValueTypes.Object);
				}
			);
		}


	}
}
