using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Apzon.Commons;
using FtpLib;

namespace Commons.Helper;

public class FtpHelper
{
	private struct RM_UNIQUE_PROCESS
	{
		public int dwProcessId;

		public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct RM_PROCESS_INFO
	{
		public RM_UNIQUE_PROCESS Process;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string strAppName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string strServiceShortName;

		public RM_APP_TYPE ApplicationType;

		public uint AppStatus;

		public uint TSSessionId;

		[MarshalAs(UnmanagedType.Bool)]
		public bool bRestartable;
	}

	private enum RM_APP_TYPE
	{
		RmUnknownApp = 0,
		RmMainWindow = 1,
		RmOtherWindow = 2,
		RmService = 3,
		RmExplorer = 4,
		RmConsole = 5,
		RmCritical = 1000
	}

	private const int RmRebootReasonNone = 0;

	private const int CCH_RM_MAX_APP_NAME = 255;

	private const int CCH_RM_MAX_SVC_NAME = 63;

	public string TransferToFtp(Function.ConfigFTPServer ftpCfg, string sourcePath, string destPath, string fileName)
	{
		try
		{
			using FtpConnection ftpConnection = new FtpConnection(ftpCfg.Address.Remove(0, 6), ftpCfg.User, ftpCfg.PassWord);
			ftpConnection.Open();
			ftpConnection.Login();
			ftpConnection.SetCurrentDirectory("/" + ftpCfg.Folder + destPath);
			ftpConnection.GetCurrentDirectory();
			try
			{
				ftpConnection.PutFile(sourcePath, fileName);
			}
			catch (FtpException ex)
			{
				return $"FTP Error: {ex.ErrorCode} {ex.Message}";
			}
			return string.Empty;
		}
		catch (Exception ex2)
		{
			return ex2.ToString();
		}
	}

	public string DowloadFile(Function.ConfigFTPServer ftpCfg, string path, string fileName)
	{
		try
		{
			string text = Path.GetTempPath() + "APZ-IRS-SalesOpportunity" + fileName;
			path = "/" + ftpCfg.Folder + path + "/";
			if (File.Exists(text))
			{
				return text;
			}
			using (FtpConnection ftpConnection = new FtpConnection(ftpCfg.Address.Remove(0, 6), ftpCfg.User, ftpCfg.PassWord))
			{
				ftpConnection.Open();
				ftpConnection.Login();
				if (ftpConnection.FileExists(path + fileName))
				{
					ftpConnection.GetFile($"{path}{fileName}", text, failIfExists: true);
				}
			}
			return text;
		}
		catch (FtpException ex)
		{
			return $"FTP Error: {ex.ErrorCode} {ex.Message}";
		}
	}

	public string WriteLogViaFtp(string ftpServer, string userName, string passWord, string path, string fileName, string text)
	{
		try
		{
			using (FtpConnection ftpConnection = new FtpConnection(ftpServer, userName, passWord))
			{
				ftpConnection.Open();
				ftpConnection.Login();
				if (string.IsNullOrEmpty(path))
				{
					path = "\\";
				}
				if (ftpConnection.DirectoryExists(path))
				{
					ftpConnection.SetCurrentDirectory(path);
				}
				if (ftpConnection.FileExists($"{path}{fileName}"))
				{
					ftpConnection.GetFile($"{path}{fileName}", failIfExists: false);
				}
				using (FileStream fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
				{
					fileStream.Write(Encoding.UTF8.GetBytes(text), 0, Encoding.UTF8.GetByteCount(text));
				}
				try
				{
					ftpConnection.SetCurrentDirectory(path);
					ftpConnection.PutFile(fileName, fileName);
				}
				catch (FtpException ex)
				{
					return $"FTP Error: {ex.ErrorCode} {ex.Message}";
				}
			}
			return string.Empty;
		}
		catch (Exception ex2)
		{
			return ex2.Message;
		}
	}

	public static string PutLogToFtp(string ftpServer, string userName, string password, string localPath, string fileName, string ftpPath)
	{
		try
		{
			using FtpConnection ftpConnection = new FtpConnection(ftpServer, userName, password);
			ftpConnection.Open();
			ftpConnection.Login();
			if (ftpConnection.DirectoryExists(ftpPath))
			{
				ftpConnection.SetCurrentDirectory(ftpPath);
			}
			DateTime now = DateTime.Now;
			string text = Path.Combine(ftpPath, now.ToString("yyyy") + "\\" + now.ToString("MM"));
			try
			{
				if (!ftpConnection.DirectoryExists(text))
				{
					ftpConnection.CreateDirectory(text);
				}
			}
			catch
			{
			}
			try
			{
				ftpConnection.SetCurrentDirectory(text);
				ftpConnection.PutFile(localPath + "\\" + fileName, fileName);
			}
			catch (FtpException ex)
			{
				return $"FTP Error: {ex.ErrorCode} {ex.Message}";
			}
			return string.Empty;
		}
		catch (Exception ex2)
		{
			return ex2.Message;
		}
	}

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

	public string CopyFileSharing(string domain, string sourcePath, string username, string password, string destPath, bool copyForOpen = false)
	{
		try
		{
			if (File.Exists(destPath))
			{
				if (copyForOpen)
				{
					return destPath;
				}
				return "1000002 - File exists in destination folder, please delete old file or rename new file";
			}
			File.Copy(sourcePath, destPath, overwrite: true);
			return destPath;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteFileSharing(string domain, string sourcePath, string username, string password)
	{
		try
		{
			if (File.Exists(sourcePath))
			{
				if (!IsFileLocked(sourcePath))
				{
					File.Delete(sourcePath);
				}
				else
				{
					bool flag = false;
					try
					{
						File.Delete(sourcePath);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						flag = true;
					}
					if (flag)
					{
						try
						{
							File.Delete(sourcePath);
						}
						catch (Exception)
						{
							List<Process> list = LockFinder(sourcePath);
							if (list != null && list.Count > 0)
							{
								return "Can not delete file. File being used by process:[" + list[0].ProcessName + "], SessionId:[" + list[0].SessionId + "]], MachineName:[" + list[0].MachineName + "]";
							}
						}
					}
				}
			}
			return "";
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			return ex3.Message;
		}
	}

	private bool IsFileLocked(string path)
	{
		LockFinder(path);
		FileInfo fileInfo = new FileInfo(path);
		try
		{
			using (fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
			{
			}
		}
		catch (IOException)
		{
			return true;
		}
		return false;
	}

	private bool IsLocalIPAddress(string domain)
	{
		try
		{
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork && iPAddress.MapToIPv4().ToString() == domain)
				{
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public static List<Process> LockFinder(string path)
	{
		string strSessionKey = Guid.NewGuid().ToString();
		List<Process> list = new List<Process>();
		if (RmStartSession(out var pSessionHandle, 0, strSessionKey) != 0)
		{
			throw new Exception("Could not begin restart session. Unable to determine file locker.");
		}
		try
		{
			uint pnProcInfoNeeded = 0u;
			uint pnProcInfo = 0u;
			uint lpdwRebootReasons = 0u;
			string[] array = new string[1] { path };
			if (RmRegisterResources(pSessionHandle, (uint)array.Length, array, 0u, null, 0u, null) != 0)
			{
				throw new Exception("Could not register resource.");
			}
			switch (RmGetList(pSessionHandle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons))
			{
			case 234:
			{
				RM_PROCESS_INFO[] array2 = new RM_PROCESS_INFO[pnProcInfoNeeded];
				pnProcInfo = pnProcInfoNeeded;
				if (RmGetList(pSessionHandle, out pnProcInfoNeeded, ref pnProcInfo, array2, ref lpdwRebootReasons) == 0)
				{
					list = new List<Process>((int)pnProcInfo);
					for (int i = 0; i < pnProcInfo; i++)
					{
						try
						{
							list.Add(Process.GetProcessById(array2[i].Process.dwProcessId));
						}
						catch (ArgumentException)
						{
						}
					}
					break;
				}
				throw new Exception("Could not list processes locking resource");
			}
			default:
				throw new Exception("Could not list processes locking resource.Failed to get size of result.");
			case 0:
				break;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		finally
		{
			RmEndSession(pSessionHandle);
		}
		return list;
	}

	[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

	[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames, uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices, string[] rgsServiceNames);

	[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int RmEndSession(uint pSessionHandle);

	[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In][Out] RM_PROCESS_INFO[] rgAffectedApps, ref uint lpdwRebootReasons);
}
