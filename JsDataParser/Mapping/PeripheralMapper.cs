using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsDataParser.Entities;

namespace JsDataParser.Mapping
{
	internal enum TypeMatchResults
	{
		NoConvertible = 1,
		Equivalent,
		ImplicitConversion,
		ExplicitConversion,
		ReferenceConversion,
		Array,
		Enumerable
	}

	public class PeripheralMapper<T> where T : new()
	{
		private readonly Dictionary<(IdentifierEntity identity, ValueTypes valueType), Action<T, ValueEntity>> _cache =
			new Dictionary<(IdentifierEntity identity, ValueTypes valueType), Action<T, ValueEntity>>();

		private readonly Action<T, ValueEntity> _defaultAssignment = (_, __) => { };

		private readonly FieldInfo[] _fields;
		private readonly PropertyInfo[] _properties;

		public PeripheralMapper()
		{
			_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
		}

		private TypeMatchResults CheckArrayMatch(ValueTypes mapFrom, Type mapTo)
		{
			if (mapTo == typeof(int[]) || mapTo == typeof(double[])
			    || mapTo == typeof(bool[]) || mapTo == typeof(string[])
			    || mapTo == typeof(object[])) return TypeMatchResults.Array;

			if (typeof(IEnumerable).IsAssignableFrom(mapTo)) return TypeMatchResults.Enumerable;

			return TypeMatchResults.NoConvertible;
		}

		private TypeMatchResults CheckMatch(ValueTypes mapFrom, Type mapTo)
		{
			if (mapTo == typeof(object) && mapFrom == ValueTypes.Object) return TypeMatchResults.Equivalent;
			if (mapTo == typeof(object)) return TypeMatchResults.ReferenceConversion;

			switch (mapFrom)
			{
				case ValueTypes.Integer:
					if (mapTo == typeof(int)) return TypeMatchResults.Equivalent;
					if (mapTo == typeof(long)) return TypeMatchResults.ImplicitConversion;
					if (mapTo == typeof(double)) return TypeMatchResults.ImplicitConversion;
					break;

				case ValueTypes.Real:
					if (mapTo == typeof(double)) return TypeMatchResults.Equivalent;
					if (mapTo == typeof(int) || mapTo == typeof(long)) return TypeMatchResults.ExplicitConversion;
					break;

				case ValueTypes.Boolean:
					if (mapTo == typeof(bool)) return TypeMatchResults.Equivalent;
					break;

				case ValueTypes.Function:
				case ValueTypes.Identity:
				case ValueTypes.String:
					if (mapTo == typeof(string)) return TypeMatchResults.Equivalent;
					break;

				case ValueTypes.Array:
					return CheckArrayMatch(mapFrom, mapTo);

				default:
					throw new ArgumentOutOfRangeException(nameof(mapFrom), mapFrom, null);
			}

			return TypeMatchResults.NoConvertible;
		}

		private object[] ArrayAssignmentProc(IReadOnlyList<ValueEntity> list)
		{
			var ret = new object[list.Count];

			for (int i = 0; i < ret.Length; i++)
			{
				if (list[i].ValueType == ValueTypes.Array) ret[i] = ArrayAssignmentProc(list[i].Array);
				else if (list[i].ValueType == ValueTypes.Object) ret[i] = new DynamicMappedLiteralObject(list[i].NestedObject);
				else ret[i] = list[i].Object;
			}

			return ret;
		}


		public Action<T, ValueEntity> BuildObjectArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			PropertyInfo mapTo)
		{
			return (to, from) =>
			{
				var ret = ArrayAssignmentProc(from.Array);
				mapTo.SetValue(to, ret);
			};
		}


		public Action<T, ValueEntity> BuildObjectArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			FieldInfo mapTo)
		{
			return (to, from) =>
			{
				var ret = ArrayAssignmentProc(from.Array);
				mapTo.SetValue(to, ret);
			};
		}

		public Action<T, ValueEntity> BuildBooleanArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			FieldInfo mapTo)
		{
			return (to, from) =>
			{
				var tmp = from.Array;
				var ret = new bool[tmp.Count];

				for (int i = 0; i < ret.Length; i++)
				{
					if (tmp[i].ValueType == ValueTypes.Boolean) ret[i] = tmp[i].Boolean;
					else return;
				}

				mapTo.SetValue(to, ret);
			};

		}



		public Action<T, ValueEntity> BuildBooleanArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			PropertyInfo mapTo)
		{
			return (to, from) =>
			{
				var tmp = from.Array;
				var ret = new bool[tmp.Count];

				for (int i = 0; i < ret.Length; i++)
				{
					if (tmp[i].ValueType == ValueTypes.Boolean) ret[i] = tmp[i].Boolean;
					else return;
				}

				mapTo.SetValue(to, ret);
			};

		}

		public Action<T, ValueEntity> BuildStringArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			PropertyInfo mapTo)
		{
			return (to, from) =>
			{
				var tmp = from.Array;
				var ret = new string[tmp.Count];

				for (int i = 0; i < ret.Length; i++)
				{
					if (tmp[i].ValueType == ValueTypes.String) ret[i] = tmp[i].String;
					else if (tmp[i].ValueType == ValueTypes.Identity) ret[i] = tmp[i].Identity;
					else if (tmp[i].ValueType == ValueTypes.Function) ret[i] = tmp[i].Function;

					else return;
				}

				mapTo.SetValue(to, ret);
			};
		}


		public Action<T, ValueEntity> BuildStringArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			FieldInfo mapTo)
		{
			return (to, from) =>
			{
				var tmp = from.Array;
				var ret = new string[tmp.Count];

				for (int i = 0; i < ret.Length; i++)
				{
					if (tmp[i].ValueType == ValueTypes.String) ret[i] = tmp[i].String;
					else if (tmp[i].ValueType == ValueTypes.Identity) ret[i] = tmp[i].Identity;
					else if (tmp[i].ValueType == ValueTypes.Function) ret[i] = tmp[i].Function;

					else return;
				}

				mapTo.SetValue(to, ret);
			};
		}

		public Action<T, ValueEntity> BuildRealArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			FieldInfo mapTo)
		{
			return (to, from) =>
			{
				var ret = new double[from.Array.Count];
				var tmp = from.Array;

				for (int i = 0; i < ret.Length; i++)
				{
					if (tmp[i].ValueType == ValueTypes.Real) ret[i] = tmp[i].Real;
					else if (tmp[i].ValueType == ValueTypes.Integer) ret[i] = tmp[i].Integer;
					else return;
				}

				mapTo.SetValue(to, ret);
			};
		}


		public Action<T, ValueEntity> BuildRealArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			PropertyInfo mapTo)
		{
			return (to, from) =>
			{
				var ret = new double[from.Array.Count];
				var tmp = from.Array;

				for (int i = 0; i < ret.Length; i++)
				{
					if (tmp[i].ValueType == ValueTypes.Real) ret[i] = tmp[i].Real;
					else if (tmp[i].ValueType == ValueTypes.Integer) ret[i] = tmp[i].Integer;
					else return;
				}

				mapTo.SetValue(to, ret);
			};
		}

		public Action<T, ValueEntity> BuildIntArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			PropertyInfo mapTo)
		{
			return (to, from) =>
			{
				var ret = new int[from.Array.Count];
				var fromArray = from.Array;
				for (int i = 0; i < ret.Length; i++)
				{
					if (fromArray[i].ValueType == ValueTypes.Integer) ret[i] = fromArray[i].Integer;
					else if (fromArray[i].ValueType == ValueTypes.Real) ret[i] = (int)fromArray[i].Real;
					else return;
				}

				mapTo.SetValue(to, ret);
			};
		}

		public Action<T, ValueEntity> BuildIntArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			FieldInfo mapTo)
		{
			return (to, from) =>
			{
				var ret = new int[from.Array.Count];
				var fromArray = from.Array;
				for (int i = 0; i < ret.Length; i++)
				{
					if (fromArray[i].ValueType == ValueTypes.Integer) ret[i] = fromArray[i].Integer;
					else if (fromArray[i].ValueType == ValueTypes.Real) ret[i] = (int) fromArray[i].Real;
					else return;
				}

				mapTo.SetValue(to, ret);
			};
		}



		private Action<T, ValueEntity> BuildArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			FieldInfo mapTo)
		{
			if (typeof(IEnumerable<int>).IsAssignableFrom(mapTo.FieldType))
				return BuildIntArraySetter(mapFrom, mapTo);

			if (typeof(IEnumerable<double>).IsAssignableFrom(mapTo.FieldType))
				return BuildRealArraySetter(mapFrom, mapTo);

			if (typeof(IEnumerable<string>).IsAssignableFrom(mapTo.FieldType))
				return BuildStringArraySetter(mapFrom, mapTo);

			if (typeof(IEnumerable<bool>).IsAssignableFrom(mapTo.FieldType))
				return BuildBooleanArraySetter(mapFrom, mapTo);

			return BuildObjectArraySetter(mapFrom, mapTo);
		}

		private Action<T, ValueEntity> BuildArraySetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			PropertyInfo mapTo)
		{
			if (typeof(IEnumerable<int>).IsAssignableFrom(mapTo.PropertyType))
				return BuildIntArraySetter(mapFrom, mapTo);

			if (typeof(IEnumerable<double>).IsAssignableFrom(mapTo.PropertyType))
				return BuildRealArraySetter(mapFrom, mapTo);

			if (typeof(IEnumerable<string>).IsAssignableFrom(mapTo.PropertyType))
				return BuildStringArraySetter(mapFrom, mapTo);

			if (typeof(IEnumerable<bool>).IsAssignableFrom(mapTo.PropertyType))
				return BuildBooleanArraySetter(mapFrom, mapTo);

			return BuildObjectArraySetter(mapFrom, mapTo);
		}

		private Action<T, ValueEntity> BuildSetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			PropertyInfo mapTo)
		{
			switch (mapFrom.Value.ValueType)
			{
				case ValueTypes.String:
					return (to, from) => mapTo.SetValue(to, from.String);

				case ValueTypes.Function:
					return (to, from) => mapTo.SetValue(to, from.Function);

				case ValueTypes.Identity:
					return (to, from) => mapTo.SetValue(to, from.Identity);

				case ValueTypes.Integer:
					return (to, from) =>
					{
						if (from.ValueType == ValueTypes.Integer)
						{
							mapTo.SetValue(to, from.Integer);
						}
						else
						{
							mapTo.SetValue(to, (int) from.Real);
						}
					};

				case ValueTypes.Real:
					return (to, from) =>
					{
						if (from.ValueType == ValueTypes.Real)
						{
							mapTo.SetValue(to, from.Real);
						}
						else
						{
							mapTo.SetValue(to, from.Integer);
						}
					};

				case ValueTypes.Boolean:
					return (to, from) => mapTo.SetValue(to, from.Boolean);

				case ValueTypes.Object:
					return (to, from) => mapTo.SetValue(to, new DynamicMappedLiteralObject(from.NestedObject));
					
				case ValueTypes.Array:
					return BuildArraySetter(mapFrom, mapTo);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}



		private Action<T, ValueEntity> BuildSetter(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			FieldInfo mapTo)
		{
			switch (mapFrom.Value.ValueType)
			{
				case ValueTypes.String:
					return (to, from) => mapTo.SetValue(to, from.String);

				case ValueTypes.Function:
					return (to, from) => mapTo.SetValue(to, from.Function);

				case ValueTypes.Identity:
					return (to, from) => mapTo.SetValue(to, from.Identity);

				case ValueTypes.Integer:
					return (to, from) =>
					{
						if (from.ValueType == ValueTypes.Integer)
						{
							mapTo.SetValue(to, from.Integer);
						}
						else
						{
							mapTo.SetValue(to, (int)from.Real);
						}
					};

				case ValueTypes.Real:
					return (to, from) =>
					{
						if (from.ValueType == ValueTypes.Real)
						{
							mapTo.SetValue(to, from.Real);
						}
						else
						{
							mapTo.SetValue(to, from.Integer);
						}
					};

				case ValueTypes.Boolean:
					return (to, from) => mapTo.SetValue(to, from.Boolean);

				case ValueTypes.Object:
					return (to, from) => mapTo.SetValue(to, new DynamicMappedLiteralObject(from.NestedObject));

				case ValueTypes.Array:
					return BuildArraySetter(mapFrom, mapTo);

				default:
					throw new ArgumentOutOfRangeException();
			}

		}

		private bool TryBuildProperty(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			(PropertyInfo prop, IdentifierEntity id, TypeMatchResults check)[] candidates, TypeMatchResults result,
			out Action<T, ValueEntity> setter)
		{
			var cnt = candidates.Count(rec => rec.check == result);

			if (cnt == 1)
			{
				setter = BuildSetter(mapFrom, candidates.First(rec => rec.check == result).prop);
				return true;
			}

			if (cnt > 1) throw new MappingException($"{mapFrom.Key} has too many candidates.");

			setter = default;
			return false;
		}


		private bool TryBuildField(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			(FieldInfo field, IdentifierEntity id, TypeMatchResults check)[] candidates, TypeMatchResults result,
			out Action<T, ValueEntity> setter)
		{
			var cnt = candidates.Count(rec => rec.check == result);

			if (cnt == 1)
			{
				setter = BuildSetter(mapFrom, candidates.First(rec => rec.check == result).field);
				return true;
			}

			if (cnt > 1) throw new MappingException($"{mapFrom.Key} has too many candidates.");

			setter = default;
			return false;
		}


		private bool TryBuildLazyNameMatchedField(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			out Action<T, ValueEntity> assignment)
		{
			if (mapFrom.Key.IdentityType != IdentifierTypes.Identity)
			{
				assignment = default;
				return false;
			}

			var name = mapFrom.Key.Identity.ToLower();
			var dummy = new IdentifierEntity(42);

			var candidates = _fields.Where(field => field.Name.ToLower() == name)
				.Select(field => (field, dummy, check:CheckMatch(mapFrom.Value.ValueType, field.FieldType))).ToArray();

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.Equivalent, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.Array, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.ImplicitConversion, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.ExplicitConversion, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.ReferenceConversion, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.Enumerable, out assignment))
				return true;

			return false;
		}

		private bool TryBuildLazyNameMatchedProperty(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			out Action<T, ValueEntity> assignment)
		{
			if (mapFrom.Key.IdentityType != IdentifierTypes.Identity)
			{
				assignment = default;
				return false;
			}

			var name = mapFrom.Key.Identity.ToLower();
			var dummy = new IdentifierEntity(114514);

			var candidates = _properties.Where(prop => prop.Name.ToLower() == name)
				.Select(prop => (prop, dummy, check:CheckMatch(mapFrom.Value.ValueType, prop.PropertyType))).ToArray();

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.Equivalent, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.Array, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.ImplicitConversion, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.ExplicitConversion, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.ReferenceConversion, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.Enumerable, out assignment))
				return true;

			return false;
		}


		private bool TryBuildAttributedField(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			out Action<T, ValueEntity> assignment)
		{
			var candidates = _fields
				.Select(field => (field, identity: field.GetCustomAttribute<MappingFromAttribute>()?.MappingFromIdentifier))
				.Where(datum => datum.identity != null)
				.Where(rec => rec.identity == mapFrom.Key)
				.Select(rec => (rec.field, rec.identity, check: CheckMatch(mapFrom.Value.ValueType, rec.field.FieldType)))
				.ToArray();

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.Equivalent, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.Array, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.ImplicitConversion, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.ExplicitConversion, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.ReferenceConversion, out assignment))
				return true;

			if (TryBuildField(mapFrom, candidates, TypeMatchResults.Enumerable, out assignment))
				return true;

			return false;
		}

		private bool TryBuildAttributedProperty(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,
			out Action<T, ValueEntity> assignment)
		{
			var candidates = _properties
				.Select(prop => (prop, identity:prop.GetCustomAttribute<MappingFromAttribute>()?.MappingFromIdentifier))
				.Where(datum => datum.identity != null)
				.Where(rec => rec.identity == mapFrom.Key)
				.Select(rec => (rec.prop, rec.identity, check:CheckMatch(mapFrom.Value.ValueType, rec.prop.PropertyType)))
				.ToArray();

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.Equivalent, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.Array, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.ImplicitConversion, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.ExplicitConversion, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.ReferenceConversion, out assignment))
				return true;

			if (TryBuildProperty(mapFrom, candidates, TypeMatchResults.Enumerable, out assignment))
				return true;

			return false;
		}


		private Action<T, ValueEntity> Build(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom)
		{
			if (TryBuildAttributedProperty(mapFrom, out var assignment)) return assignment;
			if (TryBuildAttributedField(mapFrom, out assignment)) return assignment;
			if (TryBuildLazyNameMatchedProperty(mapFrom, out assignment)) return assignment;
			if (TryBuildLazyNameMatchedField(mapFrom, out assignment)) return assignment;

			return _defaultAssignment;
		}

		public T Map(ObjectLiteralEntity mapFrom)
		{
			if (mapFrom == null) throw new ArgumentNullException(nameof(mapFrom));

			var ret = new T();

			foreach (var elem in mapFrom)
				if (_cache.TryGetValue((elem.Key, elem.Value.ValueType), out var assignment))
				{
					assignment(ret, elem.Value);
				}
				else
				{
					assignment = Build(elem);
					assignment(ret, elem.Value);
					_cache.Add((elem.Key, elem.Value.ValueType), assignment);
				}

			return ret;
		}

		public IEnumerable<(IdentifierEntity identity, T value)> Flatmap(ObjectLiteralEntity root)
		{
			return root == null
				? throw new ArgumentNullException(nameof(root))
				: root.Select(elem => (elem.Key, Map(elem.Value.NestedObject)));
		}
	}
}