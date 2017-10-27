using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Playground.ComplexSample
{
	class TinyIdentifier
	{
		public 
	}


	class TinyValue
	{
		
	}

	class TinyObject:IDictionary<>
	{
		private readonly (string key, TinyValue value)[] _contents;
		public TinyObject(IEnumerable<(string key, TinyValue value)> values)
		{
			
		}



		public IEnumerator<(string key, TinyValue value)> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	class ComplexSample
	{
	}
}
