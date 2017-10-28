using System;
using Parseq;

namespace Playground
{
	public static class Helper
	{
		public static Parser<TToken, T> Wrap<TToken, T>(this Parser<TToken, T> parser)
		{
			return stream =>
			{
				if (stream.Current.HasValue)
					Console.WriteLine(
						$"Line:{stream.Current.Value.Item1.Line} " +
						$"Col:{stream.Current.Value.Item1.Column} " +
						$"Token:{stream.Current.Value.Item0}");
				else
					Console.WriteLine("Stream.Current don't have value.");

				return parser(stream);
			};
		}

		public static void CaseSuucess<T>(this Parser<char, T> parser, string source
			, Action<ITokenStream<char>, T> success = null)
		{
			void proc(ITokenStream<char> stream, T result)
			{
				if (stream.Current.HasValue)
					Console.WriteLine(
						$"Line:{stream.Current.Value.Item1.Line} " +
						$"Col:{stream.Current.Value.Item1.Column} " +
						$"Token:{stream.Current.Value.Item0}");
				else
					Console.WriteLine("Stream.Current don't have value.");

				Console.WriteLine();
				success?.Invoke(stream, result);
			}


			parser.Run(source.AsStream())
				.Case((stream, _) =>
				{
					if (stream.Current.HasValue)
						Console.WriteLine(
							$"Line:{stream.Current.Value.Item1.Line} " +
							$"Col:{stream.Current.Value.Item1.Column} " +
							$"Token:{stream.Current.Value.Item0}");
					else
						Console.WriteLine("Stream.Current don't have value.");
				}, success);
		}
	}
}