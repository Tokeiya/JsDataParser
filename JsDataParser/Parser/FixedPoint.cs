using System;
using System.Collections.Generic;
using System.Text;
using Parseq;

namespace JsDataParser.Parser
{
	internal class FixedPoint<TToken, T>
	{
		private Parser<TToken, T> _fixedParser;
		public readonly string Name;

		public FixedPoint(string name)
		{
			Name = name ?? "";
		}

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


			var line = stream.Current.HasValue ? stream.Current.Value.Item1.Line : -1;
			var column = stream.Current.HasValue ? stream.Current.Value.Item1.Column : -1;

			//Console.WriteLine($"{Name} is Called. Line:{line} Col:{column} Token:{stream.Current.Value.Item0}");
			




			return _fixedParser(stream);
		}
	}
}
