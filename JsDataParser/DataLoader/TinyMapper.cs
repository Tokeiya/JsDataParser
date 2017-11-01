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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JsDataParser.Entities;
// ReSharper disable InconsistentNaming

namespace JsDataParser.DataLoader
{


	public static class TinyMapper<T> where T:new()
	{
		private static readonly Action<T, ValueEntity> _empty = (_, __) => { };
		private static readonly Dictionary<(string name, ValueTypes type), Action<T, ValueEntity>> _cache =
			new Dictionary<(string name, ValueTypes type), Action<T, ValueEntity>>();

		private static readonly PropertyInfo[] _properties = typeof(T).GetProperties().Where(x => x.CanWrite).ToArray();
		private static readonly FieldInfo[] _fields = typeof(T).GetFields().Where(x => !x.IsInitOnly).ToArray();


		private static bool Verify(Type mapTo, ValueTypes mapFrom)
		{
			switch (mapFrom)
			{
				case ValueTypes.Array:
					return mapTo.IsAssignableFrom(typeof(int[])) || mapTo.IsAssignableFrom(typeof(string[])) ||
					       mapTo.IsAssignableFrom(typeof(object[])) || mapTo.IsAssignableFrom(typeof(double[])) ||
					       mapTo.IsAssignableFrom(typeof(bool[]));

				case ValueTypes.Boolean:
					return mapTo.IsAssignableFrom(typeof(bool));

				case ValueTypes.Function:
				case ValueTypes.Identity:
				case ValueTypes.String:
					return mapTo.IsAssignableFrom(typeof(string));

				case ValueTypes.Integer:
					return mapTo.IsAssignableFrom(typeof(int));

				case ValueTypes.Real:
					return mapTo.IsAssignableFrom(typeof(double));

				default:
					throw new InvalidOperationException($"{mapFrom} is unexpected value");
			}
		}

		private static bool TryBuildIntArray(IReadOnlyList<ValueEntity> mapFrom, out int[] mapTo)
		{
			mapTo = new int[mapFrom.Count];


			for (var i = 0; i < mapTo.Length; i++)
			{
				switch (mapFrom[i].ValueType)
				{
					case ValueTypes.Integer:
						mapTo[i] = mapFrom[i].Integer;
						break;

					default:
						return false;
				}
			}

			return true;
		}

		private static bool TryBuildRealArray(IReadOnlyList<ValueEntity> mapFrom, out double[] mapTo)
		{
			mapTo = new double[mapFrom.Count];

			for (int i = 0; i < mapTo.Length; i++)
			{
				switch (mapFrom[i].ValueType)
				{
					case ValueTypes.Real:
						mapTo[i] = mapFrom[i].Real;
						break;

					default:
						return false;
				}
			}

			return true;
		}


		private static bool TryBuildBoolArray(IReadOnlyList<ValueEntity> mapFrom, out bool[] mapTo)
		{
			mapTo = new bool[mapFrom.Count];

			for (int i = 0; i < mapTo.Length; i++)
			{
				switch (mapFrom[i].ValueType)
				{
					case ValueTypes.Boolean:
						mapTo[i] = mapFrom[i].Boolean;
						break;

					default:
						return false;
				}
			}

			return true;
		}


		private static bool TryBuildStringArray(IReadOnlyList<ValueEntity> mapFrom, out string[] mapTo)
		{
			mapTo = new string[mapFrom.Count];


			for (int i = 0; i < mapTo.Length; i++)
			{
				switch (mapFrom[i].ValueType)
				{
					case ValueTypes.Function:
						mapTo[i] = mapFrom[i].Function;
						break;
						
					case ValueTypes.String:
						mapTo[i] = mapFrom[i].String;
						break;
						
					case ValueTypes.Identity:
						mapTo[i] = mapFrom[i].Identity;
						break;

					default:
						return false;
				}
			}

			return true;

		}

		private static bool TryBuildObjectArray(IReadOnlyList<ValueEntity> mapFrom, out object[] mapTo)
		{
			mapTo = new object[mapFrom.Count];

			for (int i = 0; i < mapTo.Length; i++)
			{
				switch (mapFrom[i].ValueType)
				{
					case ValueTypes.Array:
					case ValueTypes.Object:
						return false;

					default:
						mapTo[i] = mapFrom[i].Object;
						break;
				}
			}

			return true;
		}

		private static Action<T, ValueEntity> BuildField(FieldInfo info, ValueTypes valueType)
		{
			switch (valueType)
			{
				case ValueTypes.Array:
					if (info.FieldType.IsAssignableFrom(typeof(int[])))
					{
						return (to, from) =>
						{
							if (TryBuildIntArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}

					if (info.FieldType.IsAssignableFrom(typeof(double[])))
					{
						return (to, from) =>
						{
							if (TryBuildRealArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}

					if (info.FieldType.IsAssignableFrom(typeof(bool[])))
					{
						return (to, from) =>
						{
							if (TryBuildBoolArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}


					//statement order is important.
					if (info.FieldType.IsAssignableFrom(typeof(object[])))
					{
						return (to, from) =>
						{
							if (TryBuildObjectArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}




					if (info.FieldType.IsAssignableFrom(typeof(string[])))
					{
						return (to, from) =>
						{
							if (TryBuildStringArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}


					return _empty;

				case ValueTypes.Boolean:
					return (to, from) => info.SetValue(to, from.Boolean);

				case ValueTypes.Function:
					return (to, from) => info.SetValue(to, from.Function);

				case ValueTypes.Identity:
					return (to, from) => info.SetValue(to, from.Identity);

				case ValueTypes.Integer:
					return (to, from) => info.SetValue(to, from.Integer);

				case ValueTypes.String:
					return (to, from) => info.SetValue(to, from.String);

				case ValueTypes.Real:
					return (to, from) => info.SetValue(to, from.Real);

				default:
					return _empty;
			}
		}

		private static Action<T, ValueEntity> BuildProperty(PropertyInfo info, ValueTypes valueType)
		{
			switch (valueType)
			{
				case ValueTypes.Array:
					if (info.PropertyType.IsAssignableFrom(typeof(int[])))
					{
						return (to, from) =>
						{
							if (TryBuildIntArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}

					if (info.PropertyType.IsAssignableFrom(typeof(double[])))
					{
						return (to, from) =>
						{
							if (TryBuildRealArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}

					if (info.PropertyType.IsAssignableFrom(typeof(bool[])))
					{
						return (to, from) =>
						{
							if (TryBuildBoolArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}


					//statement order is important.
					if (info.PropertyType.IsAssignableFrom(typeof(object[])))
					{
						return (to, from) =>
						{
							if (TryBuildObjectArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}


					if (info.PropertyType.IsAssignableFrom(typeof(string[])))
					{
						return (to, from) =>
						{
							if (TryBuildStringArray(from.Array, out var array))
							{
								info.SetValue(to, array);
							}
						};
					}


					return _empty;

				case ValueTypes.Boolean:
					return (to, from) =>info.SetValue(to, from.Boolean);

				case ValueTypes.Function:
					return (to, from) =>info.SetValue(to, from.Function);

				case ValueTypes.Identity:
					return (to, from) => info.SetValue(to, from.Identity);

				case ValueTypes.Integer:
					return (to, from) => info.SetValue(to, from.Integer);

				case ValueTypes.String:
					return (to, from) => info.SetValue(to, from.String);

				case ValueTypes.Real:
					return (to, from) => info.SetValue(to, from.Real);

				default:
					return _empty;
			}
		}

		private static Action<T,ValueEntity> Build(KeyValuePair<IdentifierEntity, ValueEntity> pair)
		{
			if (pair.Value == null || pair.Key == null) throw new ArgumentException($"{nameof(pair)} contains null value.");

			var propCandidates = _properties.Where(x => x.Name.ToLower() == pair.Key.Identity.ToLower())
				.Where(x => Verify(x.PropertyType, pair.Value.ValueType));

			var fldCandidates = _fields.Where(x => x.Name.ToLower() == pair.Key.Identity.ToLower())
				.Where(x => Verify(x.FieldType, pair.Value.ValueType));

			var cnt = propCandidates.Count() + fldCandidates.Count();

			if (cnt > 1) throw new InvalidOperationException($"Detect many candidates!");

			if (cnt == 0) return _empty;


			var prop = propCandidates.FirstOrDefault();

			if (prop != null)
			{
				return BuildProperty(prop, pair.Value.ValueType);
			}

			return BuildField(fldCandidates.FirstOrDefault(), pair.Value.ValueType);
		}

		public static T SingleMap(ObjectLiteralEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			var ret = new T();

			foreach (var pair in entity.Where(x=>x.Key.IdentityType== IdentifierTypes.Identity))
			{
				if (_cache.TryGetValue((pair.Key.Identity.ToLower(), pair.Value.ValueType), out var act))
					act(ret, pair.Value);
				else
				{
					var proc = Build(pair);
					proc(ret, pair.Value);
					_cache.Add((pair.Key.Identity.ToLower(), pair.Value.ValueType), proc);
				}
			}

			return ret;
		}

		public static IEnumerable<T> MultiMap(ObjectLiteralEntity rootEntity)
			=> rootEntity.Values.Where(x => x.ValueType == ValueTypes.Object).Select(elem => SingleMap(elem.NestedObject));
	}
}
