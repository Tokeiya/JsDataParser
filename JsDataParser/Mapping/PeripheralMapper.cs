using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using JsDataParser.Entities;

namespace JsDataParser.Mapping
{
	public class PeripheralMapper<T> where T : new()
	{
		private readonly Dictionary<(IdentifierEntity identity, ValueTypes valueType), Action<T, ValueEntity>> _cache =
			new Dictionary<(IdentifierEntity identity, ValueTypes valueType), Action<T, ValueEntity>>();

		private readonly FieldInfo[] _fields;
		private readonly PropertyInfo[] _properties;

		public PeripheralMapper()
		{
			_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
		}

		private bool TryBuildLazyNameMatchedField(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom, out Action<T, ValueEntity> assignment)
		{
#warning BuildAttributedProperty_Is_NotImpl
			throw new NotImplementedException("BuildAttributedProperty is not implemented");
		}



		private bool TryBuildLazyNameMatchedProperty(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom, out Action<T, ValueEntity> assignment)
		{
#warning BuildAttributedProperty_Is_NotImpl
			throw new NotImplementedException("BuildAttributedProperty is not implemented");
		}


		private bool TryBuildAttributedField(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom, out Action<T, ValueEntity> assignment)
		{
#warning BuildAttributedProperty_Is_NotImpl
			throw new NotImplementedException("BuildAttributedProperty is not implemented");
		}

		private bool TryBuildAttributedProperty(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom,out Action<T,ValueEntity> assignment)
		{
#warning BuildAttributedProperty_Is_NotImpl
			throw new NotImplementedException("BuildAttributedProperty is not implemented");
		}


		private Action<T, ValueEntity> Build(KeyValuePair<IdentifierEntity, ValueEntity> mapFrom)
		{
#warning Build_Is_NotImpl
			throw new NotImplementedException("Build is not implemented");
		}

		public T Map(ObjectLiteralEntity mapFrom)
		{
			if (mapFrom == null) throw new ArgumentNullException(nameof(mapFrom));

			var ret = new T();

			foreach (var elem in mapFrom)
			{
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
			}

			return ret;
		}

		public IEnumerable<(IdentifierEntity identity, T value)> Flatmap(ObjectLiteralEntity root)
			=> root == null
				? throw new ArgumentNullException(nameof(root))
				: root.Select(elem => (elem.Key, Map(elem.Value.NestedObject)));
	}
}