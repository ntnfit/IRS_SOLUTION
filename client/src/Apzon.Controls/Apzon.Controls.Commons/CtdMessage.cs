using System.Collections.Generic;

namespace Apzon.Controls.Commons;

public class CtdMessage
{
	private string _message;

	public MessageType Type { get; set; }

	public string Message
	{
		get
		{
			if (LineNumber != 0)
			{
				return $"Line {LineNumber}. {_message}";
			}
			return _message;
		}
		set
		{
			_message = value;
		}
	}

	public int LineNumber { get; set; }

	public DataTypeCtd Obj { get; set; }

	public ActionStatment Action { get; set; }

	public Dictionary<string, object> OutVariable { get; set; }

	public CtdMessage()
	{
		Message = "";
		LineNumber = 0;
	}
}
