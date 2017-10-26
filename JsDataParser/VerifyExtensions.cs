using System;
using System.Collections.Generic;
using System.Text;

namespace JsDataParser
{
    internal static class VerifyExtensions
    {
		private static Dictionary<Type,int[]> _dict=new Dictionary<Type, int[]>();
		public static bool VerifyEnum<TEnum>(this TEnum value) where TEnum:struct 
	    {
		    if (_dict.TryGetValue(typeof(TEnum), out var array))
		    {
				return Array.BinarySearch(array,(int)value)
		    }
	    }
    }
}
