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
using System.Diagnostics;

namespace JsDataParser
{
	internal class CharacterCache
	{
		private readonly Dictionary<char, char[]> _cache = new Dictionary<char, char[]>();
		private readonly Queue<char> _sizeCoordinator = new Queue<char>();
		public readonly int CacheSize;

		public readonly int Threshold;

		public CharacterCache(int cacheSize, int threshold)
		{
			if (cacheSize <= 0) throw new ArgumentOutOfRangeException(nameof(cacheSize));
			if (threshold <= 0) throw new ArgumentOutOfRangeException(nameof(threshold));

			if (threshold < cacheSize) throw new ArgumentException();

			Threshold = threshold;
			CacheSize = cacheSize;
		}

		public int CurrentCachSize
		{
			get
			{
				Debug.Assert(_cache.Count == _sizeCoordinator.Count);
				return _sizeCoordinator.Count;
			}
		}

		public IEnumerable<char> Get(char c)
		{
			Debug.Assert(_sizeCoordinator.Count == _cache.Count);

			if (_cache.TryGetValue(c, out var ary)) return ary;

			if (_cache.Count > Threshold)
				while (_sizeCoordinator.Count == CacheSize)
				{
					var tmp = _sizeCoordinator.Dequeue();
					_cache.Remove(tmp);
				}

			var ret = new[] {c};
			_sizeCoordinator.Enqueue(c);
			_cache.Add(c, ret);

			return ret;
		}
	}
}