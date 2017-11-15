using System;
using System.Collections.Generic;
using System.Text;

namespace JsDataParser
{
	public class InconsistencyException : Exception
	{
		public InconsistencyException(string message) : base(message)
		{
		}

		public InconsistencyException()
		{
		}

		public InconsistencyException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
