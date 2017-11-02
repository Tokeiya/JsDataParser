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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JsDataParser.Entities;

namespace JsDataParser.Mapping
{
	internal class MappingImpl
	{
		private static readonly Action<ValueEntity, object> Empty = (_, __) => { };

		private static bool Verify(Type mapTo, ValueTypes mapFrom)
		{
			switch (mapFrom)
			{
				case ValueTypes.Array:
					return mapTo.GetInterfaces().Any(x => x == typeof(IEnumerable));

				case ValueTypes.Boolean:
					return mapTo.IsAssignableFrom(typeof(bool));

				case ValueTypes.Identity:
				case ValueTypes.String:
				case ValueTypes.Function:
					return mapTo.IsAssignableFrom(typeof(string));

				case ValueTypes.Integer:
					return mapTo.IsAssignableFrom(typeof(int));

				case ValueTypes.Real:
					return mapTo.IsAssignableFrom(typeof(double));

				case ValueTypes.Object:
					return true;


				default:
					throw new InvalidOperationException($"{mapFrom} is unexpected.");
			}
		}




		private readonly bool _hasDefaultCtor;

		private readonly Dictionary<Type, MappingImpl> _nestedMapper;

		private readonly Dictionary<(string name, ValueTypes type), Action<ValueEntity, object>> _cache
			= new Dictionary<(string name, ValueTypes type), Action<ValueEntity, object>>();

		private readonly PropertyInfo[] _properties;
		private readonly FieldInfo[] _fields;


		//CandidatesはTaypeじゃ話にならん｡Mapper食えるようにしないと
		public MappingImpl(Type mapToType, params Type[] candidates)
		{
			MapToType = mapToType ?? throw new ArgumentNullException(nameof(mapToType));

			_nestedMapper = (candidates ?? throw new ArgumentNullException(nameof(candidates)))
				.ToDictionary(x => x, x => new MappingImpl(x));

			_hasDefaultCtor = mapToType.GetConstructor(Array.Empty<Type>()) == null;

			_properties = MapToType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_fields = mapToType.GetFields(BindingFlags.Public | BindingFlags.Instance);
		}

		public Type MapToType { get; }


		private Action<ValueEntity, object> BuildNestedObject(PropertyInfo info)
		{
			if (!_nestedMapper.ContainsKey(info.PropertyType))
			{
				_nestedMapper.Add(info.PropertyType,new MappingImpl(info.PropertyType));
			}

			void ret(ValueEntity from, object to)
			{
				var tmp = info.GetValue(to);
				_nestedMapper[info.PropertyType].Map(from.NestedObject, ref tmp);

				info.SetValue(to, tmp);
			}

			return ret;
		}

		private Action<ValueEntity, object> BuildNestedObject(FieldInfo info)
		{
			if (!_nestedMapper.ContainsKey(info.FieldType))
			{
				_nestedMapper.Add(info.FieldType, new MappingImpl(info.FieldType));
			}

			void ret(ValueEntity from, object to)
			{
				var tmp = info.GetValue(to);
				_nestedMapper[info.FieldType].Map(from.NestedObject,ref tmp);

				info.SetValue(to, tmp);
			}

			return ret;
		}

		private Action<ValueEntity, object> BuildArray(PropertyInfo info)
		{
#warning BuildArray_Is_NotImpl
			throw new NotImplementedException("BuildArray is not implemented");
		}

		private Action<ValueEntity, object> BuildArray(FieldInfo info)
		{
#warning BuildArray_Is_NotImpl
			throw new NotImplementedException("BuildArray is not implemented");
		}



		private Action<ValueEntity, object> BuildProperty(PropertyInfo info, ValueTypes valueType)
		{
			switch (valueType)
			{
				case ValueTypes.Array:
					return BuildArray(info);

				case ValueTypes.Boolean:
					return(from, to) =>info.SetValue(to, from.Boolean);

				case ValueTypes.Function:
					return (from, to) => info.SetValue(to, from.Function);

				case ValueTypes.Identity:
					return (from, to) => info.SetValue(to, from.Identity);

				case ValueTypes.Integer:
					return (from, to) => info.SetValue(to, from.Integer);

				case ValueTypes.Real:
					return (from, to) => info.SetValue(to, from.Real);

				case ValueTypes.String:
					return (from, to) => info.SetValue(to, from.String);

				case ValueTypes.Object:
					return BuildNestedObject(info);

				default:
					throw new InvalidOperationException($"{valueType} is unexpected.");


			}
		}

		private Action<ValueEntity, object> BuildFields(FieldInfo info, ValueTypes valueType)
		{
			switch (valueType)
			{
				case ValueTypes.Array:
					return BuildArray(info);

				case ValueTypes.Boolean:
					return (from, to) => info.SetValue(to, from.Boolean);

				case ValueTypes.Function:
					return (from, to) => info.SetValue(to, from.Function);

				case ValueTypes.Identity:
					return (from, to) => info.SetValue(to, from.Identity);

				case ValueTypes.Integer:
					return (from, to) => info.SetValue(to, from.Integer);

				case ValueTypes.Real:
					return (from, to) => info.SetValue(to, from.Real);

				case ValueTypes.String:
					return (from, to) => info.SetValue(to, from.String);

				case ValueTypes.Object:
					return BuildNestedObject(info);

				default:
					throw new InvalidOperationException($"{valueType} is unexpected.");


			}
		}


		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		private Action<ValueEntity, object> Build(string identity, ValueTypes valueType)
		{
			var propCandidate = _properties.Where(x => x.Name.ToLower() == identity && x.CanRead && x.CanWrite)
				.Where(x => Verify(x.PropertyType, valueType));

			var fldCandidate = _fields.Where(x => x.Name.ToLower() == identity)
				.Where(x => Verify(x.FieldType, valueType));

			var cnt = propCandidate.Count() + fldCandidate.Count();

			if (cnt > 1) throw new MappingException($"Too many candidates.");
			if (cnt == 0) return Empty;

			var prop = propCandidate.FirstOrDefault();

			return prop != null ? BuildProperty(prop, valueType) : BuildFields(fldCandidate.First(), valueType);
		}


		public void Map(ObjectLiteralEntity entity,ref object mapTo)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			if (mapTo == null)
			{
				if (!_hasDefaultCtor) throw new MappingException($"{MapToType.Name} don't have a default ctor.");
				mapTo = Activator.CreateInstance(MapToType);
			}
			else if (!MapToType.IsInstanceOfType(mapTo))
			{
				 throw new ArgumentException($"{nameof(mapTo)} is unexpected type.");
			}


			foreach (var elem in entity)
			{
				if(!_cache.TryGetValue((elem.Key.Identity.ToLower(),elem.Value.ValueType),out var act))
				{
					act = Build(elem.Key.Identity.ToLower(), elem.Value.ValueType);
					_cache.Add((elem.Key.Identity.ToLower(), elem.Value.ValueType), act);
				}

				act(elem.Value, mapTo);
			}
		}
	}
}

