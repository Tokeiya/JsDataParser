/*
 * Copyright (C) 2017 Hikotaro Abe <net_seed@hotmail.com> All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsDataParser.Entities;

namespace JsDataParser.DataLoader
{


	public static class TinyMapper<T> where T:new()
	{
		private static readonly Action<T, ValueEntity> _empty = (_, __) => { };
		private static readonly Dictionary<(string name, ValueTypes type), Action<T, ValueEntity>> _cache =
			new Dictionary<(string name, ValueTypes type), Action<T, ValueEntity>>();

		private static readonly PropertyInfo[] _properties = typeof(T).GetProperties().Where(x => x.CanWrite).ToArray();
		private static readonly FieldInfo[] _fields = typeof(T).GetFields().Where(x => !x.IsInitOnly).ToArray();

		private static Action<T,ValueEntity> Build(KeyValuePair<IdentifierEntity, ValueEntity> pair)
		{

#warning Build_Is_NotImpl
			throw new NotImplementedException("Build is not implemented");
		}

		public static T SingleMap(ObjectLiteralEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			var ret = new T();

			foreach (var pair in entity.Where(x=>x.Key.IdentityType== IdentifierTypes.Identity))
			{
				if (_cache.TryGetValue((pair.Key.Constant.ToLower(), pair.Value.ValueType), out var act))
					act(ret, pair.Value);
				else
				{
					var proc = Build(pair);
					proc(ret, pair.Value);
					_cache.Add((pair.Key.Constant.ToLower(), pair.Value.ValueType), proc);
				}
			}

			return ret;
		}

		public static IEnumerable<T> MultiMap(ObjectLiteralEntity rootEntity)
			=> rootEntity.Values.Where(x => x.ValueType == ValueTypes.Object).Select(elem => SingleMap(elem.NestedObject));
	}
}
