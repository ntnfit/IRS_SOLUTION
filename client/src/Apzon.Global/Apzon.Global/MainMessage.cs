using System.Collections.Concurrent;
using Apzon.Commons.Enums;

namespace Apzon.Global;

public static class MainMessage
{
	public static bool MsgEnable = false;

	public static string MsgMessage = "";

	public static int MsgCountTimer = 100;

	public static Enums.MessageType MsgColor;

	public static int MsgCount = -1;

	public static int BarMsg = 0;

	public static ConcurrentBag<LogMessage> ListMessage = new ConcurrentBag<LogMessage>();
}
