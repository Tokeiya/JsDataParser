using System;
using System.Collections.Generic;
using System.Text;
using JsDataParser.DataLoader;
using JsDataParser.Entities;
using JsDataParser.Mapping;
using Xunit;
using Xunit.Abstractions;

namespace JsDataParserTest
{
	public class TxtLiteral
	{
		[MappingFrom("txtLiteral",false)]
		public string txtLiteral { get; set; }

		[MappingFrom(42)]
		public int intLiteral { get; set; }

		[MappingFrom(42)]
		public string intLiteralAsString { get; set; }

		[MappingFrom(42.195)]
		public string realValue { get; set; }
	}

	class OrderSample
	{
		public string nameJp { get; set; }

		[MappingFrom("nameJp",true)]
		public string name { get; set; }

	}

	public class PeripheralMapperTest
	{
		private readonly ITestOutputHelper _output;

		public PeripheralMapperTest(ITestOutputHelper output) => _output = output;

		private ObjectLiteralEntity Load()
			=> DataLoader.LoadRaw(".\\Samples\\PeripheralSample.txt");


		[Fact]
		public void MapAttributeTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<TxtLiteral>();

			var actual = target.Map(raw[new IdentifierEntity(1)].NestedObject);

			actual.txtLiteral.Is("hello");
			actual.intLiteral.Is(114514);
			actual.realValue.Is("marathon");
		}

		[Fact]
		public void MappingIntLiteral()
		{
			var raw = Load();
			var target = new PeripheralMapper<TxtLiteral>();

			var actual = target.Map(raw[new IdentifierEntity(1)].NestedObject);

			actual.intLiteral.Is(114514);
			actual.intLiteralAsString.IsNull();


			actual = target.Map(raw[new IdentifierEntity(2)].NestedObject);

			actual.intLiteral.Is(0);
			actual.intLiteralAsString.Is("114514");
		}

		[Fact]
		public void MappingOrderTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<OrderSample>();

			var actual = target.Map(raw[new IdentifierEntity(1)].NestedObject);

			actual.name.Is("10cm連装高角砲");
			actual.nameJp.IsNull();
			
		}




	}
}
