using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apzon.Commons;

public class Logging
{
	public const string ERROR = "ERROR";

	public const string WATCH = "WATCH";

	public const string TRACE = "TRACE";

	public const string CHARGE = "CHARGE";

	private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

	public static void Write(string errorType, params string[] arrMess)
	{
		DateTime timeNow = DateTime.Now;
		Task.Run(async () =>
		{
			_ = 1;
			try
			{
				string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", timeNow.ToString("yyyy"), timeNow.ToString("MM"));
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				string fileLog = Path.Combine(text, "Log-" + timeNow.ToString("yyyyMMdd") + ".txt");
				string strInfor = $"[{errorType.ToUpper()}]\t{timeNow:yyyy-MM-dd}\t{timeNow:HH:mm:ss:fff}";
				string[] array = arrMess;
				foreach (string text2 in array)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						strInfor = strInfor + "\t" + text2.Replace("\n", "").Replace("\r", "");
					}
				}
				strInfor += Environment.NewLine;
				await semaphore.WaitAsync();
				using FileStream fileStream = new FileStream(fileLog, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096);
				await fileStream.WriteAsync(Encoding.UTF8.GetBytes(strInfor), 0, Encoding.UTF8.GetByteCount(strInfor));
			}
			catch
			{
			}
			finally
			{
				semaphore.Release();
			}
		});
	}

	public static void Write(string errorType, Exception ex)
	{
		DateTime timeNow = DateTime.Now;
		Task.Run(async () =>
		{
			_ = 1;
			try
			{
				string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", timeNow.ToString("yyyy"), timeNow.ToString("MM"));
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				string fileLog = Path.Combine(text, "Log-" + timeNow.ToString("yyyyMMdd") + ".txt");
				string strInfor = $"[{errorType.ToUpper()}]\t{timeNow:yyyy-MM-dd}\t{timeNow:HH:mm:ss:fff}";
				StackTrace stackTrace = new StackTrace(ex, fNeedFileInfo: true);
				StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
				string fullName = frame.GetMethod().DeclaringType.FullName;
				int fileLineNumber = frame.GetFileLineNumber();
				strInfor = strInfor + "\t" + ex.Message + "\t" + fullName + ">\t" + fileLineNumber;
				if (ex.InnerException != null)
				{
					strInfor = strInfor + "\t" + ex.InnerException.Message;
				}
				strInfor += Environment.NewLine;
				await semaphore.WaitAsync();
				using FileStream fileStream = new FileStream(fileLog, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096);
				await fileStream.WriteAsync(Encoding.UTF8.GetBytes(strInfor), 0, Encoding.UTF8.GetByteCount(strInfor));
			}
			catch
			{
			}
			finally
			{
				semaphore.Release();
			}
		});
	}

	private static string AutoRenameFile(string folderPath, string fileName)
	{
		try
		{
			string[] array = Directory.GetFiles(folderPath).Select(Path.GetFileNameWithoutExtension).ToArray();
			if (array.Length == 0)
			{
				return fileName;
			}
			FileInfo fileInfo = null;
			fileInfo = ((array.Length != 1) ? new FileInfo(folderPath + "\\" + $"{fileName} ({array.Length - 1})" + ".txt") : new FileInfo(folderPath + "\\" + fileName + ".html"));
			if (fileInfo.Length >= 1048576)
			{
				fileName = $"{fileName} ({array.Length})";
			}
			else if (array.Length != 1)
			{
				fileName = $"{fileName} ({array.Length - 1})";
			}
			return fileName;
		}
		catch (Exception)
		{
			return fileName;
		}
	}
}
