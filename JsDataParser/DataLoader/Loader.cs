using System;
using System.Collections.Generic;
using System.IO;
using JsDataParser.Parser;
using Parseq;

namespace JsDataParser.DataLoader
{
	public class Loader
	{
		public static IEnumerable<DatumExpression> LoadDatumExpression(TextReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			IEnumerable<DatumExpression> data = null;

			DataParser.Data.Run(reader.AsStream())
				.Case(
					(str, _) =>
					{
						if (str.Current.HasValue)
						{
							throw new LoadException(str.Current.Value.Item1.Line, str.Current.Value.Item1.Line, "");
						}
						else
						{
							throw new LoadException(-1, -1, "");

						}
					}
					,
					(_, cap) =>
					{
						data = cap;
					});

			return data;

		}
		public static IEnumerable<DatumExpression> LoadDatumExpression(string path)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));

			using (var rdr = new StreamReader(path))
			{
				return LoadDatumExpression(rdr);
			}
		}


	}
}
