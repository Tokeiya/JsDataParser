using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parseq;

namespace Playground
{
	using static Parseq.Combinators.Chars;

	internal class Entity
	{
		private readonly Entity[] _nested;
		private readonly int _value;

		public Entity(int value)
		{
			_value = value;
			IsScalar = true;
		}

		public Entity(IEnumerable<Entity> nested, Entity last)
		{
			var tmp = nested.ToList();
			tmp.Add(last);

			_nested = tmp.ToArray();
			IsScalar = false;
		}

		public Entity()
		{
			_nested = Array.Empty<Entity>();
			IsScalar = false;
		}

		public bool IsScalar { get; }

		public int Value => IsScalar ? _value : throw new InvalidOperationException();
		public IReadOnlyList<Entity> Nested => IsScalar ? throw new InvalidOperationException() : _nested;
	}

	class RecursionSample
	{
		public class FixedPoint<TToken, T>
		{
			private Parser<TToken, T> _fixedParser;

			public Parser<TToken, T> FixedParser
			{
				set
				{
					if (_fixedParser != null) throw new InvalidOperationException("Parser is already registered.");
					_fixedParser = value ?? throw new NullReferenceException();
				}
			}

			public IReply<TToken, T> Parse(ITokenStream<TToken> stream)
			{
				if (_fixedParser == null) throw new InvalidOperationException("Parser isn't registered.");
				return _fixedParser(stream);
			}
		}


		public static void Sample()
		{
			//{}
			//{1}
			//{1,2}
			//{1,{1,2,3},4}
			//{1,{2,{3,4},5},6}

			//digit<-[0-9]
			//integer<-digit+
			//element<-integer/entity

			var fix = new FixedPoint<char, Entity>();

			var whiteSpace = WhiteSpace().Many0().Ignore();
			var lb = Combinator.Sequence(Char('{').Ignore(), whiteSpace).Ignore();
			var rb = Combinator.Sequence(whiteSpace, Char('}').Ignore()).Ignore();

			var delimiter = Combinator.Sequence(whiteSpace, Char(',').Ignore()).Ignore();


			var integer =
				from _ in whiteSpace
				from num in Digit().Many1().Select(cap => new string(cap.ToArray()))
				select new Entity(int.Parse(num));

			var empty =
				from _ in lb
				from __ in rb
				select new Entity();

			var field = Combinator.Choice(integer, empty, fix.Parse);


			var forwardField =
				from fld in field
				from _ in delimiter
				select fld;

			var tmp =
				from _ in lb
				from fwd in forwardField.Many0()
				from last in field
				from __ in rb
				select new Entity(fwd, last);

			var entity = Combinator.Choice(
				Combinator.Sequence(lb, rb).Select(_ => new Entity()),
				tmp
			);


			fix.FixedParser = entity;


			var sample = "{1,2,{1,{2},3},5}".AsStream();


			entity.Run(sample)
				.Case(
					(_, __) => Console.WriteLine("fail"),
					(_, __) => Console.WriteLine("Success")
				);


			Console.ReadLine();
		}
	}
}
