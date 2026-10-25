using System;
using Apzon.Commons.Enums;

namespace Apzon.Global;

public class LogMessage
{
	public string Message { get; set; }

	public Enums.MessageType MsgColor { get; set; }

	public DateTime Time { get; set; }
}
