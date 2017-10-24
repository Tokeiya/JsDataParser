using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JsDataParser.DataLoader
{
	[Serializable]
	public sealed class LoadException:Exception
	{
		private const string lineNumberId = "LoadExceptionLineNumber";
		private const string columnId = "LoadExceptionColumn";
		private const string nearTextId = "LoadExceptionNearText";


		public LoadException(int lineNumber, int column,string nearText)
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
			info.AddValue(nearTextId,NearText);

			base.GetObjectData(info, context);
		}

	}
}
