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
using System.Runtime.Serialization;

namespace JsDataParser.DataLoader
{
	[Serializable]
	public sealed class LoadException : Exception
	{
		private const string lineNumberId = "LoadExceptionLineNumber";
		private const string columnId = "LoadExceptionColumn";
		private const string nearTextId = "LoadExceptionNearText";


		public LoadException(int lineNumber, int column, string nearText)
		{
			LineNumber = lineNumber;
			Column = column;
			NearText = nearText ?? "";
		}

		private LoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			LineNumber = info.GetInt32(lineNumberId);
			Column = info.GetInt32(columnId);
			NearText = info.GetString(nearTextId);
		}


		public int LineNumber { get; }
		public int Column { get; }
		public string NearText { get; }

		public override string Message => $"Line:{LineNumber} Column:{Column} NearText:\"{NearText}\"";

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(lineNumberId, LineNumber);
			info.AddValue(columnId, Column);
			info.AddValue(nearTextId, NearText);

			base.GetObjectData(info, context);
		}
	}
}