using System;
using System.Collections.Generic;
using System.Text;
using Parseq;

namespace JsDatumParserTest
{
    public static class Helper
    {
	    public static void AreSuccsess(this IReply<char, IEnumerable<char>> reply, string expected)
	    {
		    throw new NotImplementedException();
	    }

	    public static void AreSuccess(this IReply<char, IEnumerable<char>> reply, Func<IEnumerable<char>, bool> predicate)
	    {
		    throw new NotImplementedException();
	    }

	    public static void AreFail(this IReply<char, IEnumerable<char>> reply)
	    {
		    throw new NotImplementedException();
	    }
    }
}
