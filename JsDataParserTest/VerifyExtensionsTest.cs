using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using JsDataParser;
using JsDataParser.Entities;


namespace JsDataParserTest
{

    public class VerifyExtensionsTest
    {
	    [Fact]
	    public void ValueTypesTest()
	    {
		    VerifyExtensions.Verify((ValueTypes)0).IsFalse();


	    }
    }
}
