using System;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Global;
using DevExpress.XtraSplashScreen;

namespace Apzon.Controls;

public class WinCommons
{
	public static void _OpenProcessing()
	{
		try
		{
			if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
			{
				_CloseProcessing();
			}
			SplashScreenManager.ShowForm(typeof(Processing), useFadeIn: true, useFadeOut: true);
		}
		catch
		{
		}
	}

	public static void _CloseProcessing()
	{
		try
		{
			SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false);
		}
		catch
		{
		}
	}

	public static void writeStatus(Enums.MessageCode mesgCode, Enums.MessageType msgType, int msgTime, string msgText = "", params string[] additionalInfo)
	{
		string message = GetMessage(mesgCode, msgText, additionalInfo);
		MainMessage.MsgColor = msgType;
		MainMessage.MsgCountTimer = msgTime;
		MainMessage.MsgMessage = message;
		MainMessage.MsgCount = 0;
		MainMessage.MsgEnable = true;
		MainMessage.ListMessage.Add(new LogMessage
		{
			Message = message,
			MsgColor = msgType,
			Time = DateTime.Now
		});
		RetailMessage.MsgColor = msgType;
		RetailMessage.MsgCountTimer = msgTime;
		RetailMessage.MsgMessage = message;
		RetailMessage.MsgCount = 0;
		RetailMessage.MsgEnable = true;
		_CloseProcessing();
	}

	public static void writeStatus(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		string message = GetMessage(mesgCode, msg, additionalInfo);
		MainMessage.MsgColor = Enums.MessageType.Error;
		MainMessage.MsgCountTimer = Msg_Time._TimeMedium;
		MainMessage.MsgMessage = message;
		MainMessage.MsgCount = 0;
		MainMessage.MsgEnable = true;
		MainMessage.ListMessage.Add(new LogMessage
		{
			Message = message,
			MsgColor = Enums.MessageType.Error,
			Time = DateTime.Now
		});
		RetailMessage.MsgColor = Enums.MessageType.Error;
		RetailMessage.MsgCountTimer = Msg_Time._TimeMedium;
		RetailMessage.MsgMessage = message;
		RetailMessage.MsgCount = 0;
		RetailMessage.MsgEnable = true;
		_CloseProcessing();
	}

	public static void Error(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		writeStatus(mesgCode, msg, additionalInfo);
	}

	public static void Error(Enums.MessageCode mesgCode, int time, string msg = "", params string[] additionalInfo)
	{
		writeStatus(mesgCode, Enums.MessageType.Error, time, msg, additionalInfo);
	}

	public static void ErrorLineNumber(Exception ex)
	{
		try
		{
			int num = Function.ParseInt(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
			string text = $"({num}) - {ex.Message}";
			Logging.Write("ERROR", text);
			if (!ex.Message.Contains("Object reference not set to an instance of an object"))
			{
				writeStatus(Enums.MessageCode.ExceptionMessage, text);
			}
		}
		catch (Exception)
		{
		}
	}

	public static void Success(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		writeStatus(mesgCode, Enums.MessageType.Success, Msg_Time._TimeMedium, msg, additionalInfo);
	}

	public static void Success(Enums.MessageCode mesgCode, int time, string msg = "", params string[] additionalInfo)
	{
		writeStatus(mesgCode, Enums.MessageType.Success, time, msg, additionalInfo);
	}

	public static void Warning(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		writeStatus(mesgCode, Enums.MessageType.Warning, Msg_Time._TimeMedium, msg, additionalInfo);
	}

	public static void Warning(Enums.MessageCode mesgCode, int time, string msg = "", params string[] additionalInfo)
	{
		writeStatus(mesgCode, Enums.MessageType.Warning, time, msg, additionalInfo);
	}

	public static void Info(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		writeStatus(mesgCode, Enums.MessageType.Info, Msg_Time._TimeMedium, msg, additionalInfo);
	}

	public static void InfoForm(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		string message = GetMessage(mesgCode, msg, additionalInfo);
		MainMessage.MsgColor = Enums.MessageType.Info;
		MainMessage.MsgCountTimer = Msg_Time._TimeMedium;
		MainMessage.MsgMessage = message;
		MainMessage.MsgCount = 0;
		MainMessage.MsgEnable = true;
		RetailMessage.MsgColor = Enums.MessageType.Info;
		RetailMessage.MsgCountTimer = Msg_Time._TimeMedium;
		RetailMessage.MsgMessage = message;
		RetailMessage.MsgCount = 0;
		RetailMessage.MsgEnable = true;
	}

	public static void Info(Enums.MessageCode mesgCode, int time, string msg = "", params string[] additionalInfo)
	{
		string message = GetMessage(mesgCode, msg, additionalInfo);
		MainMessage.MsgColor = Enums.MessageType.Info;
		MainMessage.MsgCountTimer = time;
		MainMessage.MsgMessage = message;
		MainMessage.MsgCount = 0;
		MainMessage.MsgEnable = true;
		RetailMessage.MsgColor = Enums.MessageType.Info;
		RetailMessage.MsgCountTimer = time;
		RetailMessage.MsgMessage = message;
		RetailMessage.MsgCount = 0;
		RetailMessage.MsgEnable = true;
		_CloseProcessing();
	}

	public static void Transparent(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		string message = GetMessage(mesgCode, msg, additionalInfo);
		MainMessage.MsgColor = Enums.MessageType.Transparent;
		MainMessage.MsgCountTimer = Msg_Time._TimeMin;
		MainMessage.MsgMessage = message;
		MainMessage.MsgCount = 0;
		MainMessage.MsgEnable = true;
		RetailMessage.MsgColor = Enums.MessageType.Transparent;
		RetailMessage.MsgCountTimer = Msg_Time._TimeMin;
		RetailMessage.MsgMessage = message;
		RetailMessage.MsgCount = 0;
		RetailMessage.MsgEnable = true;
	}

	public static string GetMessage(Enums.MessageCode mesgCode, string msg = "", params string[] additionalInfo)
	{
		try
		{
			string text = "";
			if (mesgCode == Enums.MessageCode.None && string.IsNullOrEmpty(msg))
			{
				mesgCode = Enums.MessageCode.UnknownMessage;
			}
			text = ((mesgCode == Enums.MessageCode.None) ? msg : GlobalData.MessageDataCenter[mesgCode]);
			if (additionalInfo != null)
			{
				for (int i = 0; i < additionalInfo.Length; i++)
				{
					text = text.Replace($"{{{i}}}", additionalInfo[i]);
				}
			}
			if (mesgCode == Enums.MessageCode.ExceptionMessage)
			{
				text = $"{text}-[{msg}]";
			}
			return text;
		}
		catch (Exception)
		{
			return msg;
		}
	}
}
