using System.Collections.Generic;
using System.Linq;
using JsDataParser.DataLoader;
using JsDataParser.Entities;
using JsDataParser.Mapping;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable InconsistentNaming

namespace JsDataParserTest
{
	public class SampleClass
	{
		[MappingFrom("txtLiteral", false)]
		public string txtLiteral { get; set; }

		[MappingFrom(42)]
		public int intLiteral { get; set; }

		[MappingFrom(42)]
		public string intLiteralAsString { get; set; }

		[MappingFrom(42.195)]
		public string realValue { get; set; }
	}

	internal class OrderSample
	{
		public string nameJp { get; set; }

		[MappingFrom("nameJP", true)]
		public string name { get; set; }
	}

	internal class ArraySample
	{
		[MappingFrom("intArray", false)] public int[] Array;
		public IEnumerable<bool> BoolArray;
		public dynamic[] ComplexArray;
		public IList<string> StringArray;
		public object[] EmptyArray { get; set; }
	}


	internal class ShipData
	{
		public string Name;
		public string NameJp;
		public int Hp;
		public int[] Slots { get; set; }
	}

	internal class Nullable
	{
		public int? Integer;
		public double? Real;
	}

	internal class LongTest
	{
		public long Integer;
		public long Real;
	}

	public class PeripheralMapperTest
	{
		public PeripheralMapperTest(ITestOutputHelper output)
		{
			_output = output;
		}

		private readonly ITestOutputHelper _output;

		private ObjectLiteralEntity Load()
		{
			return DataLoader.LoadRaw(".\\Samples\\PeripheralSample.txt");
		}

		[Fact]
		public void BooleanArrayTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<ArraySample>();

			var actual = target.Map(raw[new IdentifierEntity(3)].NestedObject);

			actual.BoolArray.IsNotNull();
			actual.BoolArray.SequenceEqual(new[] {true, false}).IsTrue();
		}

		[Fact]
		public void ComplexArrayTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<ArraySample>();

			var actual = target.Map(raw[new IdentifierEntity(3)].NestedObject);

			actual.ComplexArray.Length.Is(2);

			var tmp = (object[]) actual.ComplexArray[0];

			tmp.SequenceEqual(new object[] {1, 2}).IsTrue();

			var element = actual.ComplexArray[1];

			Assert.NotNull(element);

			Assert.Equal(10, (int) element.hoge);
			Assert.Equal(20, (int) element.piyo);
		}

		[Fact]
		public void EmptyArrayTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<ArraySample>();

			var actual = target.Map(raw[new IdentifierEntity(3)].NestedObject);

			actual.EmptyArray.IsNotNull();
			actual.EmptyArray.Length.Is(0);
		}

		[Fact]
		public void IntArrayTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<ArraySample>();

			var actual = target.Map(raw[new IdentifierEntity(3)].NestedObject);

			actual.Array.IsNotNull();
			actual.Array.SequenceEqual(new[] {1, 2, 3}).IsTrue();
		}

		[Fact]
		public void NullableTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<Nullable>();

			var actual = target.Map(raw[new IdentifierEntity(5)].NestedObject);

			actual.Integer.IsNotNull();
			actual.Real.IsNotNull();

			actual.Integer.Is(42);
			actual.Real.Is(42.195);

			actual = target.Map(raw[new IdentifierEntity(4)].NestedObject);

			actual.Integer.IsNull();
			actual.Real.IsNull();

		}


		[Fact]
		public void LongTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<LongTest>();

			var actual = target.Map(raw[new IdentifierEntity(5)].NestedObject);

			actual.Integer.Is(42);
		}

		[Fact]
		public void MapAttributeTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<SampleClass>();

			var actual = target.Map(raw[new IdentifierEntity(1)].NestedObject);

			actual.txtLiteral.Is("hello");
			actual.intLiteral.Is(114514);
			actual.realValue.Is("marathon");
		}

		[Fact]
		public void MappingIntLiteral()
		{
			var raw = Load();
			var target = new PeripheralMapper<SampleClass>();

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

		[Fact]
		public void StringArrayTest()
		{
			var raw = Load();
			var target = new PeripheralMapper<ArraySample>();

			var actual = target.Map(raw[new IdentifierEntity(3)].NestedObject);

			actual.StringArray.IsNotNull();
			actual.StringArray.SequenceEqual(new[] {"hoge", "piyo"}).IsTrue();
		}

		[Fact]
		public void FlatMappingTest()
		{
			var raw = DataLoader.LoadRaw(".\\Samples\\shipdata.txt");

			var target = new PeripheralMapper<ShipData>();


			var actual = target.Flatmap(raw).ToDictionary(x => x.identity.Integer, x => x.value);

			actual.Count.Is(765);

			actual[0].Slots.IsNull();

			var pivot = actual[1];

			pivot.Slots.IsNotNull();
			pivot.Slots.Length.Is(2);
			pivot.Slots.SequenceEqual(new []{0,0}).IsTrue();

			pivot.Name.Is("name3");
		}



	}
}