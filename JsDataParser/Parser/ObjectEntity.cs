﻿/*
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

namespace JsDataParser.Parser
{
	public class ObjectEntity
	{
		private readonly Dictionary<string, PropertyEntity> _properties = new Dictionary<string, PropertyEntity>();

		public ObjectEntity(ValueEntity id, IEnumerable<PropertyEntity> properties)
		{
			if (properties == null) throw new ArgumentNullException(nameof(properties));
			Id = id;

			foreach (var elem in properties)
			{
				if (elem == null) throw new ArgumentException($"{nameof(properties)} contains null value");

				if (_properties.ContainsKey(elem.Name))
					throw new InvalidOperationException($"property name {elem.Name} is duplicated.");

				_properties.Add(elem.Name, elem);
			}
		}

		public ValueEntity Id { get; }

		public IReadOnlyDictionary<string, PropertyEntity> Properties => _properties;
	}
}