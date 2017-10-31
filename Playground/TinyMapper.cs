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
using JsDataParser.Entities;

namespace Playground
{
	public static class TinyMapper
	{
		private static bool TryBuildIntArray(ValueEntity entity, out int[] array)
		{
			var source = entity.Array;
			array = new int[source.Count];

			var cnt = 0;
			foreach (var elem in source)
			{
				if (elem.ValueType != ValueTypes.Integer) return false;

				array[++cnt] = elem.Integer;
			}

			return true;
		}

		private static bool TryBuildStringArray(ValueEntity entity, out string[] array)
		{
			var source = entity.Array;
			array = new string[source.Count];

			var cnt = 0;

			foreach (var elem in source)
			{
				if (elem.ValueType != ValueTypes.String) return false;
				array[++cnt] = elem.String;
			}

			return true;
		}


		/*回帰マップはしないよ
		 *	improve: {
			ACCshell: 1.7,
			ACCnb: 1.6
			}
		こーいうやつはマップ不可能*/
		public static T Map<T>(ObjectLiteralEntity objectLiteral) where T : new()
		{
			//DynamicはあくまでもAdhocなアクセスを主目的にしてるので､マップみたいな硬めなやつは､元のデータエンティティ使う方が良い｡
			if (objectLiteral == null) throw new ArgumentNullException(nameof(objectLiteral));

			//大文字小文字は弁別するよ(これ実はしない方が何かと便利かもと思い始めている)

			var t = typeof(T);

			var target = new T();
			//Property setting

			foreach (var prop in t.GetProperties())
			{
				var identity = new IdentifierEntity(prop.Name, true);

				if (!objectLiteral.TryGetValue(identity, out var ret)) continue;

				switch (ret.ValueType)
				{
					case ValueTypes.Array:
						if (prop.PropertyType.IsAssignableFrom(typeof(int[])) && TryBuildIntArray(ret, out var intArray))
							prop.SetValue(target, intArray);
						else if (prop.PropertyType.IsAssignableFrom(typeof(string[])) && TryBuildStringArray(ret, out var strArray))
							prop.SetValue(target, strArray);
						break;

					case ValueTypes.Boolean:
						if (prop.PropertyType.IsAssignableFrom(typeof(bool))) prop.SetValue(target, ret.Boolean);
						break;

					case ValueTypes.Function:
					case ValueTypes.Identity:
					case ValueTypes.String:
						if (prop.PropertyType.IsAssignableFrom(typeof(string))) prop.SetValue(target, ret.Object);
						break;

					case ValueTypes.Integer:
						if (prop.PropertyType.IsAssignableFrom(typeof(int))) prop.SetValue(target, ret.Integer);
						break;

					case ValueTypes.Real:
						if (prop.PropertyType.IsAssignableFrom(typeof(double))) prop.SetValue(target, ret.Real);
						break;

					case ValueTypes.Object:
					default:
						break;
				}
			}


			return target;
		}


		public static IEnumerable<T> MapMany<T>(ObjectLiteralEntity rootObject) where T : new()
		{
			foreach (var elem in rootObject.Values)
				if (elem.ValueType == ValueTypes.Object) yield return Map<T>(elem.NestedObject);
		}
	}
}