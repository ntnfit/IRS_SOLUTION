using System;
using System.IO;
using System.Text;

namespace Apzon.Api;

public class SessionHandle
{
	public static string SaveSession(string json, string fileName)
	{
		try
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SESSION");
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			string path = text + "\\" + fileName;
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			using FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			fileStream.Write(Encoding.UTF8.GetBytes(json), 0, Encoding.UTF8.GetByteCount(json));
			fileStream.Close();
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		return string.Empty;
	}

	public static string GetSession(string fileName)
	{
		try
		{
			return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SESSION") + "\\" + fileName, Encoding.UTF8);
		}
		catch (Exception)
		{
		}
		return string.Empty;
	}
}
