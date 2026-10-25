using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Ionic.Zip;
using Newtonsoft.Json;
using Svg;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Apzon.Commons;

public static class Function
{
	public class ConfigData
	{
		public string Server { get; set; }

		public string Data { get; set; }

		public string User { get; set; }

		public string Pass { get; set; }

		public ConfigData()
		{
		}

		public ConfigData(string Server, string Data, string User, string Pass)
		{
			this.Server = Server;
			this.Data = Data;
			this.User = User;
			this.Pass = Pass;
		}
	}

	public class ConfigFTPServer
	{
		public string Address { get; set; }

		public string Folder { get; set; }

		public string User { get; set; }

		public string PassWord { get; set; }

		public string Method { get; set; }

		public ConfigFTPServer()
		{
		}

		public ConfigFTPServer(string address, string folder, string user, string passWord, string method)
		{
			Address = address;
			Folder = folder;
			User = user;
			PassWord = passWord;
			Method = method;
		}
	}

	public class NetworkShareAccesser : IDisposable
	{
		[StructLayout(LayoutKind.Sequential)]
		private class NETRESOURCE
		{
			public int dwScope;

			public int dwType;

			public int dwDisplayType;

			public int dwUsage;

			public string lpLocalName = "";

			public string lpRemoteName = "";

			public string lpComment = "";

			public string lpProvider = "";
		}

		private string _remoteUncName;

		private string _remoteComputerName;

		private const int RESOURCE_CONNECTED = 1;

		private const int RESOURCE_GLOBALNET = 2;

		private const int RESOURCE_REMEMBERED = 3;

		private const int RESOURCETYPE_ANY = 0;

		private const int RESOURCETYPE_DISK = 1;

		private const int RESOURCETYPE_PRINT = 2;

		private const int RESOURCEDISPLAYTYPE_GENERIC = 0;

		private const int RESOURCEDISPLAYTYPE_DOMAIN = 1;

		private const int RESOURCEDISPLAYTYPE_SERVER = 2;

		private const int RESOURCEDISPLAYTYPE_SHARE = 3;

		private const int RESOURCEDISPLAYTYPE_FILE = 4;

		private const int RESOURCEDISPLAYTYPE_GROUP = 5;

		private const int RESOURCEUSAGE_CONNECTABLE = 1;

		private const int RESOURCEUSAGE_CONTAINER = 2;

		private const int CONNECT_INTERACTIVE = 8;

		private const int CONNECT_PROMPT = 16;

		private const int CONNECT_REDIRECT = 128;

		private const int CONNECT_UPDATE_PROFILE = 1;

		private const int CONNECT_COMMANDLINE = 2048;

		private const int CONNECT_CMD_SAVECRED = 4096;

		private const int CONNECT_LOCALDRIVE = 256;

		private const int NO_ERROR = 0;

		private const int ERROR_ACCESS_DENIED = 5;

		private const int ERROR_ALREADY_ASSIGNED = 85;

		private const int ERROR_BAD_DEVICE = 1200;

		private const int ERROR_BAD_NET_NAME = 67;

		private const int ERROR_BAD_PROVIDER = 1204;

		private const int ERROR_CANCELLED = 1223;

		private const int ERROR_EXTENDED_ERROR = 1208;

		private const int ERROR_INVALID_ADDRESS = 487;

		private const int ERROR_INVALID_PARAMETER = 87;

		private const int ERROR_INVALID_PASSWORD = 1216;

		private const int ERROR_MORE_DATA = 234;

		private const int ERROR_NO_MORE_ITEMS = 259;

		private const int ERROR_NO_NET_OR_BAD_PATH = 1203;

		private const int ERROR_NO_NETWORK = 1222;

		private const int ERROR_BAD_PROFILE = 1206;

		private const int ERROR_CANNOT_OPEN_PROFILE = 1205;

		private const int ERROR_DEVICE_IN_USE = 2404;

		private const int ERROR_NOT_CONNECTED = 2250;

		private const int ERROR_OPEN_FILES = 2401;

		public string RemoteComputerName
		{
			get
			{
				return _remoteComputerName;
			}
			set
			{
				_remoteComputerName = value;
				_remoteUncName = "\\\\" + _remoteComputerName;
			}
		}

		public string UserName { get; set; }

		public string Password { get; set; }

		[DllImport("Mpr.dll")]
		private static extern int WNetUseConnection(IntPtr hwndOwner, NETRESOURCE lpNetResource, string lpPassword, string lpUserID, int dwFlags, string lpAccessName, string lpBufferSize, string lpResult);

		[DllImport("Mpr.dll")]
		private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

		public static NetworkShareAccesser Access(string remoteComputerName)
		{
			return new NetworkShareAccesser(remoteComputerName);
		}

		public static NetworkShareAccesser Access(string remoteComputerName, string domainOrComuterName, string userName, string password)
		{
			return new NetworkShareAccesser(remoteComputerName, domainOrComuterName + "\\" + userName, password);
		}

		public static NetworkShareAccesser Access(string remoteComputerName, string userName, string password)
		{
			return new NetworkShareAccesser(remoteComputerName, userName, password);
		}

		private NetworkShareAccesser(string remoteComputerName)
		{
			RemoteComputerName = remoteComputerName;
			ConnectToShare(_remoteUncName, null, null, promptUser: true);
		}

		private NetworkShareAccesser(string remoteComputerName, string userName, string password)
		{
			RemoteComputerName = remoteComputerName;
			UserName = userName;
			Password = password;
			ConnectToShare(_remoteUncName, UserName, Password, promptUser: false);
		}

		private void ConnectToShare(string remoteUnc, string username, string password, bool promptUser)
		{
			NETRESOURCE lpNetResource = new NETRESOURCE
			{
				dwType = 1,
				lpRemoteName = remoteUnc
			};
			int num = ((!promptUser) ? WNetUseConnection(IntPtr.Zero, lpNetResource, password, username, 0, null, null, null) : WNetUseConnection(IntPtr.Zero, lpNetResource, "", "", 24, null, null, null));
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		private void DisconnectFromShare(string remoteUnc)
		{
			int num = WNetCancelConnection2(remoteUnc, 1, fForce: false);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		public void Dispose()
		{
			DisconnectFromShare(_remoteUncName);
		}
	}

	private static readonly string[] VietnameseSigns = new string[15]
	{
		"aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ",
		"ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ"
	};

	public static DateTime DateMinValue()
	{
		try
		{
			return SqlDateTime.MinValue.Value;
		}
		catch (Exception)
		{
			return DateTime.Now.AddDays(-1.0);
		}
	}

	public static DateTime IntToTime(int time)
	{
		try
		{
			DateTime today = DateTime.Today;
			TimeSpan value = default;
			string text = ToString(time);
			if (time < 60)
			{
				value = new TimeSpan(0, time, 0);
			}
			if (time >= 100 && time < 959)
			{
				value = new TimeSpan(ParseInt(text.Substring(0, 1)), ParseInt(text.Substring(1, 2)), 0);
			}
			if (time >= 1000)
			{
				value = new TimeSpan(ParseInt(text.Substring(0, 2)), ParseInt(text.Substring(2, 2)), 0);
			}
			return today.Add(value);
		}
		catch (Exception)
		{
			return DateTime.Now;
		}
	}

	public static int TimeToInt(TimeSpan time)
	{
		try
		{
			return ParseInt(ToString(time.Hours) + ToString(time.Milliseconds));
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public static int ParseInt(object obj)
	{
		if (obj is int)
		{
			return (int)obj;
		}
		try
		{
			if (obj == null)
			{
				return 0;
			}
			if (int.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public static long ParseLong(object obj)
	{
		try
		{
			if (obj == null)
			{
				return 0L;
			}
			if (long.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return 0L;
		}
		catch (Exception)
		{
			return 0L;
		}
	}

	public static double ParseDouble(object obj)
	{
		if (obj is double)
		{
			return (double)obj;
		}
		if (obj is decimal num)
		{
			return (double)num;
		}
		if (obj is float num2)
		{
			return num2;
		}
		try
		{
			if (obj == null)
			{
				return 0.0;
			}
			if (double.TryParse(obj.ToString(), out var result) && !double.IsInfinity(result) && !double.IsNaN(result))
			{
				return result;
			}
			return 0.0;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	public static double ParseDouble(object obj, NumberFormatInfo provider)
	{
		if (obj is double)
		{
			return (double)obj;
		}
		if (obj is decimal num)
		{
			return (double)num;
		}
		if (obj is float num2)
		{
			return num2;
		}
		try
		{
			if (obj == null)
			{
				return 0.0;
			}
			if (double.TryParse(obj.ToString(), NumberStyles.Any, provider, out var result) && !double.IsInfinity(result) && !double.IsNaN(result))
			{
				return result;
			}
			return 0.0;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	public static decimal ParseDecimal(object obj)
	{
		if (obj is decimal)
		{
			return (decimal)obj;
		}
		if (obj is double num)
		{
			return (decimal)num;
		}
		if (obj is float num2)
		{
			return (decimal)num2;
		}
		try
		{
			if (obj == null)
			{
				return 0m;
			}
			string s = obj.ToString();
			if (decimal.TryParse(s, out var result))
			{
				return result;
			}
			if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return 0m;
		}
		catch (Exception)
		{
			return 0m;
		}
	}

	public static short? ParseShort(object obj)
	{
		try
		{
			if (obj == null)
			{
				return null;
			}
			if (short.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static float ParseFloat(object obj)
	{
		if (obj is float)
		{
			return (float)obj;
		}
		if (obj is double num)
		{
			return (float)num;
		}
		if (obj is decimal num2)
		{
			return (float)num2;
		}
		try
		{
			if (obj == null)
			{
				return 0f;
			}
			if (float.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return 0f;
		}
		catch (Exception)
		{
			return 0f;
		}
	}

	public static string ToString(object obj)
	{
		try
		{
			if (obj == null)
			{
				return string.Empty;
			}
			return obj.ToString().Trim();
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static string ToStringNotTrim(object obj)
	{
		try
		{
			if (obj == null)
			{
				return string.Empty;
			}
			return obj.ToString();
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static bool ParseBool(object obj)
	{
		try
		{
			if (obj == null)
			{
				return false;
			}
			return Convert.ToBoolean(obj);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static byte ParseByte(object obj)
	{
		try
		{
			if (obj == null)
			{
				return Convert.ToByte(0);
			}
			if (byte.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return Convert.ToByte(0);
		}
		catch (Exception)
		{
			return Convert.ToByte(0);
		}
	}

	public static DateTime? ParseDateTime(object obj, string format = "")
	{
		if (obj is DateTime)
		{
			return (DateTime)obj;
		}
		try
		{
			if (obj == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(format))
			{
				return DateTime.ParseExact(obj.ToString(), format, CultureInfo.InvariantCulture);
			}
			if (DateTime.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static DateTime ParseDateTimes(object obj)
	{
		if (obj is DateTime)
		{
			return (DateTime)obj;
		}
		try
		{
			if (obj == null)
			{
				return DateTime.Now;
			}
			if (obj != null && Convert.ToDateTime(obj) == DateTime.MinValue)
			{
				return DateTime.Now;
			}
			if (obj != null && DateTime.Parse(obj.ToString()).Year == 1899)
			{
				return DateTime.Now;
			}
			if (obj != null && DateTime.Parse(obj.ToString()).Year == 1900)
			{
				return DateTime.Now;
			}
			if (DateTime.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return DateTime.Now;
		}
		catch (Exception)
		{
			return DateTime.Now;
		}
	}

	public static DateTime ParseDateTimes(object obj, string format)
	{
		if (obj is DateTime)
		{
			return (DateTime)obj;
		}
		try
		{
			if (!string.IsNullOrEmpty(format))
			{
				return DateTime.ParseExact(obj.ToString(), format, CultureInfo.InvariantCulture);
			}
			if (obj == null)
			{
				return DateTime.Now.Date;
			}
			if (DateTime.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return DateTime.Now.Date;
		}
		catch (Exception)
		{
			return DateTime.Now.Date;
		}
	}

	public static char? ParseChar(object obj)
	{
		try
		{
			if (obj == null)
			{
				return Convert.ToChar(" ");
			}
			if (char.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return null;
		}
		catch (Exception)
		{
			return Convert.ToChar(" ");
		}
	}

	public static TimeSpan IntSAPToTimeSpan(object obj)
	{
		string text = ToString(obj);
		int minutes = 0;
		int hours = 0;
		try
		{
			switch (text.Length)
			{
			case 1:
			case 2:
				minutes = ParseInt(obj);
				break;
			case 3:
				hours = ParseInt(text.Substring(0, 1));
				minutes = ParseInt(text.Substring(1, 2));
				break;
			case 4:
				hours = ParseInt(text.Substring(0, 2));
				minutes = ParseInt(text.Substring(2, 2));
				break;
			}
		}
		catch (Exception)
		{
		}
		return new TimeSpan(hours, minutes, 0);
	}

	public static string CaseSensitive(string source)
	{
		try
		{
			return Regex.Replace(source, "[A-Z]", " $&");
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static string RawText(object obj)
	{
		try
		{
			return (obj == null) ? string.Empty : ToString(SecurityElement.Escape(obj.ToString()));
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static DateTime ParseDateTimeMinToMaxValue(object obj, int time)
	{
		try
		{
			if (obj == null)
			{
				return DateTime.MaxValue;
			}
			if (DateTime.TryParse(obj.ToString(), out var result))
			{
				return result.Date.AddHours(time / 100).AddMinutes(time % 100);
			}
		}
		catch (Exception)
		{
		}
		return DateTime.MaxValue;
	}

	public static string GetApiUri(string apiServer, string controller, string action)
	{
		if (string.IsNullOrEmpty(apiServer))
		{
			throw new Exception("Invalid api server");
		}
		if (string.IsNullOrEmpty(controller))
		{
			throw new Exception("Invalid api controller");
		}
		if (string.IsNullOrEmpty(action))
		{
			throw new Exception("Invalid api action");
		}
		return string.Join("/", apiServer, controller, action);
	}

	public static string GetApiUri(string apiServer, string controller, string action, object pr)
	{
		if (string.IsNullOrEmpty(apiServer))
		{
			throw new Exception("Invalid api server");
		}
		if (string.IsNullOrEmpty(controller))
		{
			throw new Exception("Invalid api controller");
		}
		if (string.IsNullOrEmpty(action))
		{
			throw new Exception("Invalid api action");
		}
		if (pr == null)
		{
			return string.Join("/", apiServer, controller, action);
		}
		Type type = pr.GetType();
		PropertyInfo[] properties = type.GetProperties();
		if (properties != null && properties.Length != 0)
		{
			string text = string.Join("/", apiServer, controller, action) + "?";
			List<string> list = new List<string>();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				object obj = type.GetProperty(propertyInfo.Name).GetValue(pr, null);
				if (obj is double num)
				{
					obj = num.ToString(CultureInfo.InvariantCulture);
				}
				list.Add(propertyInfo.Name + "=" + HttpUtility.UrlEncode(ToString(obj)));
			}
			return text + string.Join("&", list.ToArray());
		}
		return string.Join("/", apiServer, controller, action);
	}

	public static void setConfigData(ConfigData _cd, string strPath)
	{
		try
		{
			using ResourceWriter resourceWriter = new ResourceWriter(strPath);
			resourceWriter.AddResource("Server", Encrypt(Apzon.Commons.Constants.RSA.PublicKey, _cd.Server));
			resourceWriter.AddResource("Data", Encrypt(Apzon.Commons.Constants.RSA.PublicKey, _cd.Data));
			resourceWriter.AddResource("User", Encrypt(Apzon.Commons.Constants.RSA.PublicKey, _cd.User));
			resourceWriter.AddResource("Pass", Encrypt(Apzon.Commons.Constants.RSA.PublicKey, _cd.Pass));
			resourceWriter.Generate();
			resourceWriter.Close();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static ConfigData getConfigData(string strPath)
	{
		ConfigData configData = new ConfigData();
		try
		{
			using ResourceSet resourceSet = new ResourceSet(strPath);
			configData.Server = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, resourceSet.GetString("Server"));
			configData.Data = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, resourceSet.GetString("Data"));
			configData.User = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, resourceSet.GetString("User"));
			configData.Pass = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, resourceSet.GetString("Pass"));
			return configData;
		}
		catch (Exception)
		{
			return configData;
		}
	}

	public static bool testConnection(string _sql)
	{
		try
		{
			using SqlConnection sqlConnection = new SqlConnection(_sql);
			sqlConnection.Open();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void setConfigFTPServer(ConfigFTPServer _ftp, string strPath)
	{
		try
		{
			using ResourceWriter resourceWriter = new ResourceWriter(strPath);
			resourceWriter.AddResource("Address", _ftp.Address);
			resourceWriter.AddResource("Folder", _ftp.Folder);
			resourceWriter.AddResource("User", _ftp.User);
			resourceWriter.AddResource("PassWord", _ftp.PassWord);
			resourceWriter.AddResource("Method", _ftp.Method);
			resourceWriter.Generate();
			resourceWriter.Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static ConfigFTPServer getConfigFTPServer(string strPath)
	{
		ConfigFTPServer configFTPServer = new ConfigFTPServer();
		try
		{
			using ResourceSet resourceSet = new ResourceSet(strPath);
			configFTPServer.Address = resourceSet.GetString("Address");
			configFTPServer.Folder = resourceSet.GetString("Folder");
			configFTPServer.User = resourceSet.GetString("User");
			configFTPServer.PassWord = resourceSet.GetString("PassWord");
			if (null != configFTPServer.GetType().GetProperty("Method"))
			{
				configFTPServer.Method = resourceSet.GetString("Method");
			}
			return configFTPServer;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return configFTPServer;
		}
	}

	public static string CheckConnectFTP(ConfigFTPServer ftp)
	{
		try
		{
			string text = ftp.Address;
			string folder = ftp.Folder;
			string user = ftp.User;
			string passWord = ftp.PassWord;
			string value = "";
			if (null != ftp.GetType().GetProperty("Method"))
			{
				value = ftp.Method;
			}
			string path = "";
			if (!string.IsNullOrEmpty(value) && "net".Equals(value))
			{
				try
				{
					if (!string.IsNullOrEmpty(text) && text.Length > 5 && text.StartsWith("ftp://"))
					{
						text = text.Substring(6, text.Length - 6);
					}
					path = $"\\\\{text}\\{folder}";
					using (NetworkShareAccesser.Access(text, user, passWord))
					{
						if (Directory.Exists(path))
						{
							return "";
						}
						return "Cannot connect to share folder!";
					}
				}
				catch (Exception ex)
				{
					if (ex.Message.StartsWith("Multiple connections") && Directory.Exists(path))
					{
						return "";
					}
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					return ex.Message;
				}
			}
			if (!string.IsNullOrEmpty(value) && "ftp".Equals(value))
			{
				path = $"{text}/{folder}";
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(path);
				ftpWebRequest.Credentials = new NetworkCredential(user, passWord);
				ftpWebRequest.Method = "NLST";
				_ = (FtpWebResponse)ftpWebRequest.GetResponse();
				return string.Empty;
			}
			return "Function not support!";
		}
		catch (WebException ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return ex2.Message;
		}
	}

	public static DateTime ToDate(string x, string kieu)
	{
		try
		{
			int num = x.IndexOf('/');
			int num2 = x.LastIndexOf('/');
			int day;
			int month;
			int year;
			if (kieu.Equals("MM/dd/yyyy"))
			{
				day = int.Parse(x.Substring(num + 1, num2 - num - 1));
				month = int.Parse(x.Substring(0, num));
				year = int.Parse(x.Substring(num2 + 1, 4));
			}
			else if (kieu.Equals("yyyyMMdd"))
			{
				day = int.Parse(x.Substring(6, 2));
				month = int.Parse(x.Substring(4, 2));
				year = int.Parse(x.Substring(0, 4));
			}
			else
			{
				day = int.Parse(x.Substring(0, num));
				month = int.Parse(x.Substring(num + 1, num2 - num - 1));
				year = int.Parse(x.Substring(num2 + 1, 4));
			}
			return new DateTime(year, month, day);
		}
		catch (Exception ex)
		{
			throw new Exception("Phải định dạng kiểu MM/dd/yyyy. Sai kiểu ngày tháng. Chi tiết lỗi: " + ex.Message);
		}
	}

	public static DataTable PasteFromExcel(string clipboardData)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<string> list = new List<string>(clipboardData.Split('\n'));
			bool flag = true;
			if (list.Count > 0 && string.IsNullOrWhiteSpace(list[list.Count - 1]))
			{
				list.RemoveAt(list.Count - 1);
			}
			foreach (string item in list)
			{
				string text = item;
				if (text.EndsWith("\r"))
				{
					text = text.Substring(0, text.Length - "\r".Length);
				}
				string[] array = text.Split('\r', '\t');
				DataRow dataRow = dataTable.NewRow();
				if (flag)
				{
					int num = 0;
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (string.IsNullOrWhiteSpace(text2))
						{
							dataTable.Columns.Add($"[BLANK{num++}]");
						}
						else if (!dataTable.Columns.Contains(text2))
						{
							if (!dataTable.Columns.Contains(text2))
							{
								dataTable.Columns.Add(text2);
							}
						}
						else
						{
							dataTable.Columns.Add($"Column {num++}");
						}
					}
					for (int j = 0; j < array.Length && j < dataTable.Columns.Count; j++)
					{
						dataRow[j] = array[j];
					}
					dataTable.Rows.Add(dataRow);
					flag = false;
				}
				else
				{
					for (int k = 0; k < array.Length && k < dataTable.Columns.Count; k++)
					{
						dataRow[k] = array[k];
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public static List<string> GetParamFromSqlString(string str)
	{
		try
		{
			return (from t in GetListSytax(StandardizedSql(str))
				where t.StartsWith("@")
				select t).ToList();
		}
		catch (Exception)
		{
			return new List<string>();
		}
	}

	private static int GetMaxDataLength()
	{
		if (Apzon.Commons.Constants.RSA.fOAEP)
		{
			return (Apzon.Commons.Constants.RSA.KEY_SIZE - 384) / 8 + 7;
		}
		return (Apzon.Commons.Constants.RSA.KEY_SIZE - 384) / 8 + 37;
	}

	private static bool IsKeySizeValid()
	{
		if (Apzon.Commons.Constants.RSA.KEY_SIZE >= 384 && Apzon.Commons.Constants.RSA.KEY_SIZE <= 16384)
		{
			return Apzon.Commons.Constants.RSA.KEY_SIZE % 8 == 0;
		}
		return false;
	}

	private static CspParameters GetCspParameters()
	{
		return new CspParameters
		{
			ProviderType = 1,
			KeyNumber = 1
		};
	}

	public static string Encrypt(string publickey, string plainText)
	{
		if (string.IsNullOrEmpty(plainText))
		{
			throw new ArgumentException("Could not encrypt data");
		}
		int maxDataLength = GetMaxDataLength();
		if (Encoding.Unicode.GetBytes(plainText).Length > maxDataLength)
		{
			throw new ArgumentException("Could not encrypt data");
		}
		if (!IsKeySizeValid())
		{
			throw new ArgumentException("Could not encrypt data");
		}
		if (string.IsNullOrWhiteSpace(publickey))
		{
			throw new ArgumentException("Could not encrypt data");
		}
		try
		{
			CspParameters cspParameters = GetCspParameters();
			cspParameters.Flags = CspProviderFlags.NoFlags;
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(Apzon.Commons.Constants.RSA.KEY_SIZE, cspParameters);
			rSACryptoServiceProvider.FromXmlString(publickey);
			byte[] bytes = Encoding.Unicode.GetBytes(plainText);
			return Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(bytes, fOAEP: false));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw new Exception("Could not encrypt data!");
		}
	}

	public static string Decrypt(string privatekey, string encryptedText)
	{
		if (string.IsNullOrWhiteSpace(encryptedText))
		{
			throw new ArgumentException("Could not Decrypt data");
		}
		if (!IsKeySizeValid())
		{
			throw new ArgumentException("Could not Decrypt data");
		}
		if (string.IsNullOrWhiteSpace(privatekey))
		{
			throw new ArgumentException("Could not Decrypt data");
		}
		try
		{
			CspParameters cspParameters = GetCspParameters();
			cspParameters.Flags = CspProviderFlags.NoFlags;
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(Apzon.Commons.Constants.RSA.KEY_SIZE, cspParameters);
			rSACryptoServiceProvider.FromXmlString(privatekey);
			byte[] rgb = Convert.FromBase64String(encryptedText);
			byte[] bytes = rSACryptoServiceProvider.Decrypt(rgb, fOAEP: false);
			return Encoding.Unicode.GetString(bytes);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw new Exception("Could not Decrypt data");
		}
	}

	public static void GenerateKeysRsa(out string publicKey, out string privateKey)
	{
		try
		{
			CspParameters cspParameters = GetCspParameters();
			cspParameters.Flags = CspProviderFlags.UseArchivableKey;
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(Apzon.Commons.Constants.RSA.KEY_SIZE, cspParameters);
			publicKey = rSACryptoServiceProvider.ToXmlString(includePrivateParameters: false);
			privateKey = rSACryptoServiceProvider.ToXmlString(includePrivateParameters: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw new Exception(ex.Message);
		}
	}

	public static int DecodeLineTotal(string line)
	{
		if (string.IsNullOrEmpty(line))
		{
			return 0;
		}
		char startIndex = Convert.ToChar(line.Substring(line.Length - 1));
		return ParseInt(line.Substring(startIndex, 2));
	}

	public static void ReadLicense(string file, out string bpCode, out string installNumber, out string hardwareKey, out string fileId, out string sysType, out string dateCreate, out string version, out List<LicenseInfoImpl> lstLicense)
	{
		lstLicense = new List<LicenseInfoImpl>();
		using StringReader stringReader = new StringReader(file);
		stringReader.ReadLine();
		bpCode = stringReader.ReadLine();
		stringReader.ReadLine();
		stringReader.ReadLine();
		stringReader.ReadLine();
		stringReader.ReadLine();
		stringReader.ReadLine();
		stringReader.ReadLine();
		stringReader.ReadLine();
		hardwareKey = ReadLicensePath(stringReader.ReadLine(), stringReader);
		installNumber = ReadLicensePath(stringReader.ReadLine(), stringReader);
		fileId = ReadLicensePath(stringReader.ReadLine(), stringReader);
		sysType = ReadLicensePath(stringReader.ReadLine(), stringReader);
		version = ReadLicensePath(stringReader.ReadLine(), stringReader);
		dateCreate = ReadLicensePath(stringReader.ReadLine(), stringReader);
		string text = ReadLicensePath(stringReader.ReadLine(), stringReader);
		while (!text.Equals("ENDLICENSE"))
		{
			string[] array = text.Split(';');
			LicenseInfoImpl licenseInfoImpl = new LicenseInfoImpl();
			licenseInfoImpl.TypeCode = array[0];
			licenseInfoImpl.TypeName = array[1];
			licenseInfoImpl.Amount = ParseInt(array[2]);
			licenseInfoImpl.ExpiredDate = ParseDateTimes(array[3], "yyyyMMdd");
			licenseInfoImpl.LicenseRule = (array[4].Equals("S") ? LicenseRule.Super : (array[4].Equals("X") ? LicenseRule.Service : LicenseRule.Normal));
			licenseInfoImpl.LstAccessForm = new List<string>();
			string text2 = ReadLicensePath(stringReader.ReadLine(), stringReader);
			while (!text2.Equals("END"))
			{
				licenseInfoImpl.LstAccessForm.AddRange(text2.Split(';'));
				text2 = ReadLicensePath(stringReader.ReadLine(), stringReader);
			}
			lstLicense.Add(licenseInfoImpl);
			text = ReadLicensePath(stringReader.ReadLine(), stringReader);
		}
	}

	private static string ReadLicensePath(string encryptText, StringReader reader)
	{
		int num = Convert.ToChar(encryptText.Substring(15, 1)) % 5;
		for (int i = 0; i < num; i++)
		{
			reader.ReadLine();
		}
		return Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, encryptText + "==");
	}

	public static string RandomSession(string userName, string dataBase)
	{
		string text = "";
		char[] array = $"{userName}{dataBase}".ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = text;
			int num = array[i];
			text = text2 + num;
		}
		text += DateTime.Now.Year;
		text += DateTime.Now.Day;
		text += DateTime.Now.Month;
		text += DateTime.Now.Hour;
		text += DateTime.Now.Minute;
		text += DateTime.Now.Second;
		return text + DateTime.Now.Millisecond;
	}

	public static string GetMd5Hash(string input)
	{
		try
		{
			byte[] array = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	private static bool VerifyMd5Hash(string input, string hash)
	{
		string md5Hash = GetMd5Hash(input);
		StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
		if (ordinalIgnoreCase.Compare(md5Hash, hash) == 0)
		{
			return true;
		}
		return false;
	}

	public static string GetCodePass(string pass)
	{
		string text = "";
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(pass);
			bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
			byte[] array = bytes;
			foreach (byte b in array)
			{
				text += b.ToString("X2");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public static int GetIso8601WeekOfYear(DateTime time, CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek, DayOfWeek dayOfWeek = DayOfWeek.Monday)
	{
		DayOfWeek dayOfWeek2 = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
		if (dayOfWeek2 >= DayOfWeek.Monday && dayOfWeek2 <= DayOfWeek.Wednesday)
		{
			time = time.AddDays(3.0);
		}
		return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, rule, dayOfWeek);
	}

	public static List<DatabaseParamater> ParseSql(string sql)
	{
		List<DatabaseParamater> list = new List<DatabaseParamater>();
		try
		{
			string sql2 = StandardizedSql(sql, removeComment: false);
			List<string> listSytax = GetListSytax(sql2);
			List<string> lstParamater = GetParamater(sql2);
			int i;
			for (i = 0; i < lstParamater.Count; i++)
			{
				for (int num = listSytax.Count - 1; num >= 0; num--)
				{
					DatabaseParamater databaseParamater = new DatabaseParamater();
					if (list.Any((DatabaseParamater p) => p.ParaName == lstParamater[i].Trim()) || (!(listSytax[num] == lstParamater[i]) && !(listSytax[num] == "'" + lstParamater[i] + "'") && !(listSytax[num].ToLower() == "n'" + lstParamater[i] + "'")))
					{
						continue;
					}
					databaseParamater.ParaName = lstParamater[i].Trim();
					if (num > 2 && listSytax[num - 1].ToLower().Equals("between"))
					{
						GetColumAndTableBetween(listSytax, num - 2, out var table, out var columns);
						databaseParamater.ColumnName = columns;
						databaseParamater.TableName = table;
						databaseParamater.ParaType = ParaType.FromBetween;
						list.Add(databaseParamater);
						continue;
					}
					if (num > 4 && listSytax[num - 1].ToLower().Equals("and"))
					{
						if (listSytax[num - 2].Equals(")"))
						{
							int num2 = 1;
							for (int num3 = num - 3; num3 >= 0; num3--)
							{
								if (listSytax[num3].Equals(")"))
								{
									num2++;
								}
								else if (listSytax[num3].Equals("("))
								{
									num2--;
								}
								if (num2 == 0)
								{
									num = num3 + 2;
									break;
								}
							}
						}
						if (listSytax[num - 3].ToLower().Equals("between"))
						{
							GetColumAndTableBetween(listSytax, num - 4, out var table2, out var columns2);
							databaseParamater.ColumnName = columns2;
							databaseParamater.TableName = table2;
							databaseParamater.ParaType = ParaType.ToBetween;
							list.Add(databaseParamater);
							continue;
						}
					}
					if (num > 3 && listSytax[num - 1] == "/" && listSytax[num - 2] == "*")
					{
						List<string> list2 = new List<string>();
						int num4 = num - 3;
						while (num4 >= 1 && listSytax[num4] != "*" && listSytax[num4 - 1] != "/")
						{
							list2.Insert(0, listSytax[num4--]);
						}
						string text = "";
						for (num4 = 0; num4 < list2.Count - 1; num4++)
						{
							if (list2[num4].ToLower() == "select" || list2[num4].ToLower() == "cbbselect")
							{
								text = list2[num4 + 1];
								break;
							}
						}
						if (!string.IsNullOrEmpty(text))
						{
							string[] array = text.Split('.');
							if (array.Length > 1)
							{
								databaseParamater.ColumnName = array[array.Length - 1].Trim();
							}
							else
							{
								databaseParamater.ColumnName = text.Trim();
							}
							if (list2[num4].ToLower() == "cbbselect")
							{
								databaseParamater.Combobox = true;
							}
							string table3 = GetTable(list2, list2.Count - 1, (array.Length > 1) ? array[array.Length - 2] : "", list2.Count - 1);
							databaseParamater.TableName = table3.Trim();
						}
						else
						{
							databaseParamater.ColumnName = lstParamater[i].Trim();
						}
						list.Add(databaseParamater);
						continue;
					}
					if (num > 2 && SQL.ComparisonOperators.Contains(listSytax[num - 1].ToLower()))
					{
						if (listSytax[num - 2] == ")")
						{
							for (int num5 = num - 3; num5 >= 0; num5--)
							{
								if (listSytax[num5] == "(")
								{
									GetTableAndColumns(listSytax, num5, num - 2, out var table4, out var columns3);
									if (!string.IsNullOrEmpty(columns3))
									{
										databaseParamater.ColumnName = columns3.Trim();
										databaseParamater.TableName = table4;
									}
									else
									{
										string[] array2 = listSytax[num - 3].Split('.');
										if (array2.Length > 1)
										{
											databaseParamater.ColumnName = array2[array2.Length - 1].Trim();
										}
										else
										{
											databaseParamater.ColumnName = listSytax[num - 3].Trim();
										}
										table4 = GetTable(listSytax, num - 5, (array2.Length > 1) ? array2[array2.Length - 2] : "", num - 3);
										databaseParamater.TableName = table4.Trim();
									}
									list.Add(databaseParamater);
									break;
								}
							}
						}
						else
						{
							string[] array3 = listSytax[num - 2].Split('.');
							if (array3.Length > 1)
							{
								databaseParamater.ColumnName = array3[array3.Length - 1].Trim();
							}
							else
							{
								databaseParamater.ColumnName = listSytax[num - 2].Trim();
							}
							string table5 = GetTable(listSytax, num - 3, (array3.Length > 1) ? array3[array3.Length - 2] : "", num - 2);
							databaseParamater.TableName = table5.Trim();
							list.Add(databaseParamater);
						}
					}
					else if (num > listSytax.Count - 2)
					{
						list.Add(databaseParamater);
					}
					else if (listSytax[num + 2] == "(")
					{
						for (int num6 = num + 3; num6 <= listSytax.Count; num6++)
						{
							if (!(listSytax[num6] == ")"))
							{
								continue;
							}
							GetTableAndColumns(listSytax, num + 2, num6, out var table6, out var columns4);
							if (!string.IsNullOrEmpty(columns4))
							{
								databaseParamater.ColumnName = columns4.Trim();
								databaseParamater.TableName = table6.Trim();
							}
							else
							{
								string[] array4 = listSytax[num + 3].Split('.');
								if (array4.Length > 1)
								{
									databaseParamater.ColumnName = array4[array4.Length - 1].Trim();
								}
								else
								{
									databaseParamater.ColumnName = listSytax[num + 3].Trim();
								}
								table6 = GetTable(listSytax, num - 1, (array4.Length > 1) ? array4[array4.Length - 2] : "", num + 3);
								databaseParamater.TableName = table6.Trim();
							}
							list.Add(databaseParamater);
							break;
						}
					}
					else
					{
						string[] array5 = listSytax[num + 2].Split('.');
						if (array5.Length > 1)
						{
							databaseParamater.ColumnName = array5[array5.Length - 1].Trim();
						}
						else
						{
							databaseParamater.ColumnName = listSytax[num + 2].Trim();
						}
						string table7 = GetTable(listSytax, num - 1, (array5.Length > 1) ? array5[array5.Length - 2] : "", num + 2);
						databaseParamater.TableName = table7.Trim();
						list.Add(databaseParamater);
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	private static void GetColumAndTableBetween(List<string> code, int index, out string table, out string columns)
	{
		table = "";
		columns = "";
		try
		{
			if (code[index].Equals(")"))
			{
				int end = index;
				int num = 1;
				while (index >= 0 && num > 0)
				{
					index--;
					if (code[index].Equals(")"))
					{
						num++;
					}
					else if (code[index].Equals("("))
					{
						num--;
					}
				}
				GetTableAndColumns(code, index + 1, end, out table, out columns);
			}
			else
			{
				string[] array = code[index].Split('.');
				if (array.Length > 1)
				{
					columns = array[array.Length - 1].Trim();
				}
				else
				{
					columns = code[index].Trim();
				}
				table = GetTable(code, index, (array.Length > 1) ? array[array.Length - 2] : "", index).Trim();
			}
		}
		catch
		{
		}
	}

	private static string StandardizedSql(string sql, bool removeComment = true)
	{
		List<char> list = sql.ToList();
		int num = list.Count;
		string text = "";
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < num; i++)
		{
			if (!flag2 && list[i] == '\'')
			{
				flag = !flag;
			}
			else if (!flag && list[i] == '"')
			{
				flag2 = !flag2;
			}
			else if (!flag && !flag2 && i < num - 1 && list[i] == '-' && list[i + 1] == '-')
			{
				while (i < num - 1 && (list[i] != '\r' || list[i + 1] != '\n'))
				{
					list.RemoveAt(i);
					num--;
				}
				i--;
			}
		}
		if (removeComment)
		{
			for (int j = 0; j < num; j++)
			{
				if (!flag2 && list[j] == '\'')
				{
					flag = !flag;
				}
				else if (!flag && list[j] == '"')
				{
					flag2 = !flag2;
				}
				else if (!flag && !flag2 && j < num - 1 && list[j] == '/' && list[j + 1] == '*')
				{
					while (j < num - 1 && (list[j] != '*' || list[j + 1] != '/'))
					{
						list.RemoveAt(j);
						num--;
					}
					if (j < num - 1)
					{
						list.RemoveRange(j, 2);
						num -= 2;
					}
					j--;
				}
			}
		}
		for (int k = 0; k < num; k++)
		{
			if (list[k] == '\n' || list[k] == '\t' || list[k] == '\r')
			{
				list[k] = ' ';
			}
		}
		for (int l = 0; l < num; l++)
		{
			bool flag3 = false;
			while (l < num - 1 && list[l] == ' ' && list[l + 1] == ' ')
			{
				list.RemoveAt(l);
				num--;
				flag3 = true;
			}
			if (flag3)
			{
				l--;
			}
		}
		for (int m = 0; m < num; m++)
		{
			if ((list[m] == '<' || list[m] == '>' || list[m] == '!') && m < num - 2 && ((list[m + 1] == ' ') & (list[m + 2] == '=' || list[m + 2] == '>' || list[m + 2] == '<')))
			{
				list.RemoveAt(m + 1);
				num--;
			}
		}
		int num2 = 0;
		for (int n = 0; n < num; n++)
		{
			if (list[n] == '\'')
			{
				num2++;
				if (num2 % 2 == 1 && n < num - 1 && list[n + 1] == ' ')
				{
					list.RemoveAt(n + 1);
					num--;
				}
				if (num2 % 2 == 0 && n > 1 && list[n - 1] == ' ')
				{
					list.RemoveAt(n - 1);
					num--;
					n--;
				}
			}
			else if (list[n] == '.')
			{
				if (n > 1 && list[n - 1] == ' ')
				{
					list.RemoveAt(n - 1);
					num--;
					n--;
				}
				if (n < num - 1 && list[n + 1] == ' ')
				{
					list.RemoveAt(n + 1);
					num--;
				}
			}
		}
		for (int num3 = 0; num3 < num; num3++)
		{
			text = new string(list.ToArray());
		}
		return text.Trim();
	}

	private static List<string> GetListSytax(string sql)
	{
		char[] array = sql.ToCharArray();
		int num = array.Length;
		List<string> list = new List<string>();
		string text = "";
		for (int i = 0; i < num; i++)
		{
			if (array[i] == ' ' && !string.IsNullOrWhiteSpace(text))
			{
				list.Add(text);
				text = "";
				continue;
			}
			if (SQL.OperatorSql.Contains(array[i]))
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					list.Add(text);
					list.Add(array[i].ToString() ?? "");
					text = "";
				}
				else
				{
					list.Add(array[i].ToString() ?? "");
				}
			}
			if (array[i] == '%')
			{
				if (i > 0 && array[i - 1] == '[')
				{
					if (array[i] != ' ')
					{
						text += array[i];
					}
				}
				else if (!string.IsNullOrWhiteSpace(text))
				{
					list.Add(text);
					list.Add(array[i].ToString() ?? "");
					text = "";
				}
				else
				{
					list.Add(array[i].ToString() ?? "");
				}
			}
			if (array[i] == '!' || array[i] == '>' || array[i] == '<' || array[i] == '=')
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					list.Add(text);
					text = "";
				}
				text += array[i];
				for (i++; array[i] == '!' || array[i] == '>' || array[i] == '<' || array[i] == '='; i++)
				{
					text += array[i];
				}
				list.Add(text);
				text = "";
				i--;
			}
			if (array[i] != '!' && array[i] != '>' && array[i] != '<' && array[i] != '=' && array[i] != '%' && array[i] != ',' && array[i] != ';' && array[i] != '-' && array[i] != '+' && array[i] != '/' && array[i] != '*' && array[i] != '(' && array[i] != ')' && array[i] != ' ')
			{
				text += array[i];
			}
			if (i == num - 1)
			{
				list.Add(text);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (j < list.Count - 1 && list[j].ToLower() == "not" && list[j + 1].ToLower() == "in")
			{
				list[j] = "not in";
				list.RemoveAt(j + 1);
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (list[k].ToLower() == "join")
			{
				while (k > 0 && SQL.JoinOperator.Contains(list[k - 1].ToLower()))
				{
					List<string> list2 = list;
					int index = k - 1;
					list2[index] = list2[index] + " " + list[k];
					list.RemoveAt(k);
					k--;
				}
			}
		}
		return list;
	}

	private static List<string> GetParamater(string sql)
	{
		List<string> list = new List<string>();
		char[] array = sql.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (i >= array.Length - 1 || array[i] != '[' || array[i + 1] != '%')
			{
				continue;
			}
			string text = "[%";
			for (i += 2; i < array.Length && array[i] != ']'; i++)
			{
				text += array[i];
			}
			if (array[i] == ']')
			{
				text += array[i];
				if (!list.Contains(text))
				{
					list.Add(text);
				}
			}
		}
		return list;
	}

	private static void GetTableAndColumns(List<string> code, int sta, int end, out string table, out string columns)
	{
		try
		{
			columns = "";
			table = "";
			for (int i = sta; i < end; i++)
			{
				if (!(code[i].ToLower() == "select"))
				{
					continue;
				}
				i++;
				while (i <= end && code[i].ToLower() != "from")
				{
					string text = code[i++];
					if (text.ToLower().Equals("top"))
					{
						i++;
					}
					else
					{
						columns = columns + " " + text;
					}
				}
				i++;
				bool flag = true;
				for (int j = i; j < end; j++)
				{
					if (code[j].ToLower().Contains("join"))
					{
						flag = true;
						j++;
					}
					if (code[j].ToLower() == "on")
					{
						flag = false;
					}
					if (code[j].ToLower() == "where")
					{
						break;
					}
					if (flag)
					{
						if (j > 0 && code[j - 1].ToLower().Contains("join"))
						{
							table = table + " ," + code[j];
						}
						else
						{
							table = table + " " + code[j];
						}
					}
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw new Exception("wrong syntax");
		}
	}

	private static string GetTable(List<string> code, int end, string tab = "", int index = 0)
	{
		try
		{
			string text = "";
			if (!string.IsNullOrEmpty(tab))
			{
				int num = code.LastIndexOf(tab, index);
				if (num <= 0)
				{
					return tab;
				}
				if (code[num - 1].ToLower() == "as")
				{
					if (num > 2 && code[num - 2] == ")")
					{
						text = ")";
						int num2 = 1;
						for (int num3 = num - 3; num3 >= 0; num3--)
						{
							if (num2 > 0)
							{
								text = code[num3] + " " + text;
							}
							if (code[num3] == ")")
							{
								num2++;
							}
							if (code[num3] == "(")
							{
								num2--;
							}
						}
						return text;
					}
					return code[num - 2];
				}
				if (code[num - 1] == ")")
				{
					text = ")";
					int num4 = 1;
					for (int num5 = num - 2; num5 >= 0; num5--)
					{
						if (num4 > 0)
						{
							text = code[num5] + " " + text;
						}
						if (code[num5] == ")")
						{
							num4++;
						}
						if (code[num5] == "(")
						{
							num4--;
						}
					}
					return text;
				}
				if (code[num - 1] == "," || SQL.KeyWords.Contains(code[num - 1].ToLower()))
				{
					return tab;
				}
				return code[num - 1];
			}
			bool flag = true;
			for (int num6 = end; num6 >= 0; num6--)
			{
				if (code[num6] == ")")
				{
					flag = false;
				}
				if (code[num6] == "(")
				{
					flag = true;
				}
				if ((code[num6].ToLower() == "from") & flag)
				{
					num6++;
					bool flag2 = true;
					for (int i = num6; i <= end; i++)
					{
						if (code[i].ToLower().Contains("join"))
						{
							flag2 = true;
							i++;
						}
						if (code[i].ToLower() == "on")
						{
							flag2 = false;
						}
						if (code[i].ToLower() == "where")
						{
							break;
						}
						if (flag2)
						{
							text = ((i <= 0 || !code[i - 1].ToLower().Contains("join")) ? (text + " " + code[i]) : (text + " ," + code[i]));
						}
					}
					break;
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw new Exception("wrong syntax");
		}
	}

	public static string GetTableByApzObjectType(ApzObjectTypes objType)
	{
		string result = "";
		try
		{
			switch (objType)
			{
			case ApzObjectTypes.oBusinessPartners:
				result = "OCRD";
				break;
			case ApzObjectTypes.oInvoices:
				result = "OINV";
				break;
			case ApzObjectTypes.oReserveInvoices:
				result = "OINV";
				break;
			case ApzObjectTypes.oCreditNotes:
				result = "ORIN";
				break;
			case ApzObjectTypes.oDeliveryNotes:
				result = "ODLN";
				break;
			case ApzObjectTypes.oReturns:
				result = "ORDN";
				break;
			case ApzObjectTypes.oOrders:
				result = "ORDR";
				break;
			case ApzObjectTypes.oQuotations:
				result = "OQUT";
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				result = "OPCH";
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				result = "ORPC";
				break;
			case ApzObjectTypes.oAPReserveInvoice:
				result = "OPCH";
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				result = "OPDN";
				break;
			case ApzObjectTypes.oPurchaseReturns:
				result = "ORPD";
				break;
			case ApzObjectTypes.oPurchaseOrders:
				result = "OPOR";
				break;
			case ApzObjectTypes.oInventoryGenEntry:
				result = "OIGN";
				break;
			case ApzObjectTypes.oInventoryGenExit:
				result = "OIGE";
				break;
			case ApzObjectTypes.oDrafts:
				result = "ODRF";
				break;
			case ApzObjectTypes.oCorrectionPurchaseInvoice:
				result = "OCPI";
				break;
			case ApzObjectTypes.oCorrectionPurchaseInvoiceReversal:
				result = "OCPV";
				break;
			case ApzObjectTypes.oCorrectionInvoice:
				result = "OCSI";
				break;
			case ApzObjectTypes.oCorrectionInvoiceReversal:
				result = "OCSV";
				break;
			case ApzObjectTypes.oDownPayments:
				result = "ODPI";
				break;
			case ApzObjectTypes.oPurchaseDownPayments:
				result = "ODPO";
				break;
			case ApzObjectTypes.oPurchaseQuotations:
				result = "OPQT";
				break;
			case ApzObjectTypes.oPurchaseRequest:
				result = "OPRQ";
				break;
			case ApzObjectTypes.oKnowledgeBaseSolutions:
				result = "OSLT";
				break;
			case ApzObjectTypes.oStockTransfer:
				result = "OWTR";
				break;
			case ApzObjectTypes.oInventoryTransferRequest:
				result = "OWTQ";
				break;
			case ApzObjectTypes.oStockTransferDraft:
				result = "ORDR";
				break;
			case ApzObjectTypes.oProductionOrders:
				result = "OWOR";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public static string GetColumnKeyByApzObjectType(ApzObjectTypes objType)
	{
		string result = "";
		try
		{
			switch (objType)
			{
			case ApzObjectTypes.oBusinessPartners:
				result = "CardCode";
				break;
			case ApzObjectTypes.oInvoices:
			case ApzObjectTypes.oCreditNotes:
			case ApzObjectTypes.oDeliveryNotes:
			case ApzObjectTypes.oReturns:
			case ApzObjectTypes.oOrders:
			case ApzObjectTypes.oPurchaseInvoices:
			case ApzObjectTypes.oPurchaseCreditNotes:
			case ApzObjectTypes.oPurchaseDeliveryNotes:
			case ApzObjectTypes.oPurchaseReturns:
			case ApzObjectTypes.oPurchaseOrders:
			case ApzObjectTypes.oQuotations:
			case ApzObjectTypes.oAPReserveInvoice:
			case ApzObjectTypes.oInventoryGenEntry:
			case ApzObjectTypes.oInventoryGenExit:
			case ApzObjectTypes.oDrafts:
			case ApzObjectTypes.oCorrectionPurchaseInvoice:
			case ApzObjectTypes.oCorrectionPurchaseInvoiceReversal:
			case ApzObjectTypes.oCorrectionInvoice:
			case ApzObjectTypes.oCorrectionInvoiceReversal:
			case ApzObjectTypes.oProductionOrders:
			case ApzObjectTypes.oDownPayments:
			case ApzObjectTypes.oPurchaseDownPayments:
			case ApzObjectTypes.oReserveInvoices:
			case ApzObjectTypes.oPurchaseQuotations:
			case ApzObjectTypes.oPurchaseRequest:
				result = "DocEntry";
				break;
			case ApzObjectTypes.oKnowledgeBaseSolutions:
				result = "SltCode";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public static string EncryptFile(string fullPathFileFrom, string fullPathFileTo, bool remoExistFile = true)
	{
		try
		{
			if (!File.Exists(fullPathFileFrom))
			{
				return "From File is not exist!";
			}
			if (File.Exists(fullPathFileTo))
			{
				if (!remoExistFile)
				{
					return "to File is exist";
				}
				File.WriteAllText(fullPathFileTo, string.Empty);
			}
			using (StreamWriter streamWriter = new StreamWriter(fullPathFileTo, append: true, Encoding.UTF8))
			{
				using StreamReader streamReader = new StreamReader(fullPathFileFrom, Encoding.UTF8);
				string value = "";
				string text;
				while ((text = streamReader.ReadLine()) != null && string.IsNullOrEmpty(value))
				{
					if (!string.IsNullOrEmpty(text))
					{
						streamWriter.WriteLine(Encrypt(Apzon.Commons.Constants.RSA.PublicKey, "@@@--???"));
						value = WriteLine(streamWriter, text);
					}
					else
					{
						streamWriter.WriteLine(Encrypt(Apzon.Commons.Constants.RSA.PublicKey, "```,,,|||"));
					}
				}
				if (!string.IsNullOrEmpty(value))
				{
					DeleteFolder(fullPathFileTo);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private static string WriteLine(StreamWriter write, string lineString)
	{
		try
		{
			if (string.IsNullOrEmpty(lineString) || write == null)
			{
				return "error";
			}
			if (lineString.Length > 100)
			{
				string text = WriteLine(write, lineString.Substring(0, lineString.Length / 2));
				if (string.IsNullOrEmpty(text))
				{
					text = WriteLine(write, lineString.Substring(lineString.Length / 2));
				}
				return text;
			}
			write.WriteLine(Encrypt(Apzon.Commons.Constants.RSA.PublicKey, lineString));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public static string DecryptFile(string fullPathFileEncrypt, out List<string> stringFile, string fullPathFileTo = "", bool remoExistFile = true)
	{
		stringFile = new List<string>();
		try
		{
			if (!File.Exists(fullPathFileEncrypt))
			{
				return "From File is not exist!";
			}
			if (!string.IsNullOrEmpty(fullPathFileTo))
			{
				if (!File.Exists(fullPathFileEncrypt))
				{
					return "From File is not exist!";
				}
				if (File.Exists(fullPathFileTo))
				{
					if (!remoExistFile)
					{
						return "to File is exist";
					}
					File.WriteAllText(fullPathFileTo, string.Empty);
				}
				using StreamWriter streamWriter = new StreamWriter(fullPathFileTo, append: true, Encoding.UTF8);
				using StreamReader streamReader = new StreamReader(fullPathFileEncrypt, Encoding.UTF8);
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string text2 = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, text);
						if (text2 == "@@@--???")
						{
							streamWriter.WriteLine();
						}
						else if (text2 == "```,,,|||")
						{
							streamWriter.WriteLine();
						}
						else
						{
							streamWriter.Write(text2);
						}
					}
				}
			}
			else
			{
				using StreamReader streamReader2 = new StreamReader(fullPathFileEncrypt, Encoding.UTF8);
				string text3 = "";
				string text4;
				while ((text4 = streamReader2.ReadLine()) != null)
				{
					if (string.IsNullOrEmpty(text4))
					{
						continue;
					}
					string text5 = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, text4);
					if (text5 == "@@@--???")
					{
						text3 += "\r\n";
					}
					else if (text5 == "```,,,|||")
					{
						text3 += "\r\n";
					}
					else if (text5.Trim().ToUpper().Equals("GO") || text5.Trim().ToUpper().Equals("--GO"))
					{
						if (!string.IsNullOrEmpty(text3))
						{
							stringFile.Add(text3);
							text3 = "";
						}
					}
					else
					{
						text3 += text5;
					}
				}
				if (!string.IsNullOrEmpty(text3))
				{
					stringFile.Add(text3);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "erorr";
		}
	}

	public static string DecryptFile(string fullPathFileEncrypt)
	{
		string text = "";
		if (!File.Exists(fullPathFileEncrypt))
		{
			return text;
		}
		using StreamReader streamReader = new StreamReader(fullPathFileEncrypt, Encoding.UTF8);
		string text2;
		while ((text2 = streamReader.ReadLine()) != null)
		{
			if (!string.IsNullOrEmpty(text2))
			{
				string text3 = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, text2);
				if (text3 == "@@@--???")
				{
					text += "\r\n";
				}
				else
				{
					text = ((!(text3 == "```,,,|||")) ? (text + text3) : (text + "\r\n"));
				}
			}
		}
		return text;
	}

	public static string DecryptFile(string fullPathFileEncrypt, string fullPathFileTo, bool remoExistFile = true)
	{
		if (!File.Exists(fullPathFileEncrypt))
		{
			return "From File is not exist!";
		}
		if (!File.Exists(fullPathFileEncrypt))
		{
			return "From File is not exist!";
		}
		if (File.Exists(fullPathFileTo))
		{
			if (!remoExistFile)
			{
				return "to File is exist";
			}
			File.WriteAllText(fullPathFileTo, string.Empty);
		}
		using (StreamWriter streamWriter = new StreamWriter(fullPathFileTo, append: true, Encoding.UTF8))
		{
			using StreamReader streamReader = new StreamReader(fullPathFileEncrypt, Encoding.UTF8);
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = Decrypt(Apzon.Commons.Constants.RSA.PrivateKey, text);
					if (text2 == "@@@--???")
					{
						streamWriter.WriteLine();
					}
					else if (text2 == "```,,,|||")
					{
						streamWriter.WriteLine();
					}
					else
					{
						streamWriter.Write(text2);
					}
				}
			}
		}
		return "";
	}

	public static string DateToStringSap(DateTime date, int dateType, string CharType, bool isDate = true)
	{
		string text = "MM/dd/yyyy";
		string text2 = "MM/yyyy";
		if (string.IsNullOrEmpty(CharType))
		{
			CharType = " ";
		}
		switch (dateType)
		{
		case 0:
			text = string.Join(CharType, "dd", "MM", "yy");
			text2 = string.Join(CharType, "MM", "yy");
			if (isDate)
			{
				return date.ToString(text);
			}
			return date.ToString(text2);
		case 1:
			text = string.Join(CharType, "dd", "MM", "yyyy");
			text2 = string.Join(CharType, "MM", "yyyy");
			if (isDate)
			{
				return date.ToString(text);
			}
			return date.ToString(text2);
		case 2:
			text = string.Join(CharType, "dd", "MM", "yy");
			text2 = string.Join(CharType, "MM", "yy");
			if (isDate)
			{
				return date.ToString(text);
			}
			return date.ToString(text2);
		case 3:
			text = string.Join(CharType, "dd", "MM", "yyyy");
			text2 = string.Join(CharType, "MM", "yyyy");
			if (isDate)
			{
				return date.ToString(text);
			}
			return date.ToString(text2);
		case 4:
			text = string.Join(CharType, "yyyy", "MM", "dd");
			text2 = string.Join(CharType, "yyyy", "MM");
			if (isDate)
			{
				return date.ToString(text);
			}
			return date.ToString(text2);
		case 5:
			text = string.Join(CharType, "dd", "MMMM", "yyyy");
			text2 = string.Join(CharType, "MMMM", "yyyy");
			if (isDate)
			{
				return date.ToString(text);
			}
			return date.ToString(text2);
		case 6:
			text = string.Join(CharType, "yy", "MM", "dd");
			text2 = string.Join(CharType, "yy", "MM");
			if (isDate)
			{
				return date.ToString(text);
			}
			return date.ToString(text2);
		default:
			if (!isDate)
			{
				return date.ToString(text2);
			}
			return date.ToString(text);
		}
	}

	public static string GetDateFormatStringSap(int dateType, string CharType)
	{
		if (string.IsNullOrEmpty(CharType))
		{
			CharType = " ";
		}
		return dateType switch
		{
			0 => string.Join(CharType, "dd", "MM", "yy"), 
			1 => string.Join(CharType, "dd", "MM", "yyyy"), 
			2 => string.Join(CharType, "dd", "MM", "yy"), 
			3 => string.Join(CharType, "dd", "MM", "yyyy"), 
			4 => string.Join(CharType, "yyyy", "MM", "dd"), 
			5 => string.Join(CharType, "dd", "MMMM", "yyyy"), 
			6 => string.Join(CharType, "yy", "MM", "dd"), 
			_ => "dd MM yyyy", 
		};
	}

	public static Type GetColumnDataType(string dataType)
	{
		try
		{
			return dataType.ToLower() switch
			{
				"int16" => typeof(short), 
				"int32" => typeof(int), 
				"int64" => typeof(long), 
				"string" => typeof(string), 
				"datetime" => typeof(DateTime), 
				"decimal" => typeof(decimal), 
				"boolean" => typeof(bool), 
				"double" => typeof(double), 
				"single" => typeof(float), 
				"int" => typeof(int), 
				"timespan" => typeof(TimeSpan), 
				"float" => typeof(float), 
				"byte" => typeof(byte), 
				"char" => typeof(char), 
				"long" => typeof(long), 
				"sbyte" => typeof(sbyte), 
				"short" => typeof(short), 
				"uint" => typeof(uint), 
				"ulong" => typeof(ulong), 
				"ushort" => typeof(ushort), 
				_ => typeof(string), 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return typeof(string);
	}

	public static string GetHardwareKey()
	{
		string text = "";
		try
		{
			foreach (ManagementBaseObject instance in new ManagementClass("Win32_BIOS").GetInstances())
			{
				if (text == "")
				{
					text = ToString(instance["SerialNumber"]).Replace(" ", "");
					if (!string.IsNullOrEmpty(text))
					{
						instance.Dispose();
						break;
					}
				}
				instance.Dispose();
			}
			foreach (ManagementBaseObject instance2 in new ManagementClass("Win32_Processor").GetInstances())
			{
				string text2 = ToString(instance2.Properties["processorID"].Value).Replace(" ", "") + ToString(instance2.Properties["UniqueId"].Value).Replace(" ", "");
				if (!string.IsNullOrEmpty(text2))
				{
					text += text2;
					instance2.Dispose();
					break;
				}
				instance2.Dispose();
			}
			foreach (ManagementBaseObject instance3 in new ManagementClass("Win32_BaseBoard").GetInstances())
			{
				string text3 = ToString(instance3.Properties["SerialNumber"].Value).Replace(" ", "");
				if (!string.IsNullOrEmpty(text3))
				{
					text += text3;
					instance3.Dispose();
					break;
				}
				instance3.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			text = "";
		}
		return text;
	}

	public static string GetTypeName(Type type)
	{
		Type underlyingType = Nullable.GetUnderlyingType(type);
		if (!(underlyingType != null))
		{
			return type.Name;
		}
		return underlyingType.Name;
	}

	public static string ParseSqlToCountRowResult(string sql)
	{
		string sql2 = StandardizedSql(sql);
		List<string> listSytax = GetListSytax(sql2);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < listSytax.Count; i++)
		{
			string text = listSytax[i].ToLower();
			if (!(text == "select"))
			{
				if (!(text == "from"))
				{
					continue;
				}
				num--;
				if (num != 0)
				{
					continue;
				}
				num3 = i - 1;
				num4 = listSytax.Count;
				int num5 = 0;
				for (int j = i; j < listSytax.Count; j++)
				{
					switch (listSytax[j].ToLower())
					{
					case "(":
						num5++;
						break;
					case ")":
						num5--;
						break;
					case "order":
						if (num5 == 0)
						{
							num4 = j;
							j = listSytax.Count;
						}
						break;
					case ";":
						num4 = j;
						j = listSytax.Count;
						break;
					}
				}
				i = listSytax.Count;
			}
			else
			{
				num++;
				if (num == 1)
				{
					num2 = i + 1;
				}
			}
		}
		sql2 = "";
		if (num2 == 0 || num3 == 0)
		{
			return sql;
		}
		for (int k = 0; k < num4; k++)
		{
			if (k >= num2 && k <= num3)
			{
				if (k == num2)
				{
					sql2 += " 1 A";
				}
			}
			else
			{
				sql2 = sql2 + " " + listSytax[k];
			}
		}
		return $"Select Count(1) from ({sql2}) t";
	}

	public static bool CompareVersionLicenseAndVersionIrs(string verLicense, string verIrs)
	{
		try
		{
			if (string.IsNullOrEmpty(verLicense) || string.IsNullOrEmpty(verIrs))
			{
				return false;
			}
			if (verIrs.Substring(0, 4).Equals(verLicense.Substring(0, 4)))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public static string ConvertDoubleToStringEnUs(double numericValue, int numIndecimal, int numInInteger)
	{
		try
		{
			CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-us");
			cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
			string text = numericValue.ToString(cultureInfo);
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			for (int i = 0; i < numInInteger - 1; i++)
			{
				text = "0" + text;
			}
			int num = text.IndexOf('.');
			if (num < 0)
			{
				num = text.Length;
				text += ".";
			}
			for (int j = 0; j < numIndecimal; j++)
			{
				text += "0";
			}
			return string.Format("{0}{1}{2}", text.Substring(num - numInInteger, numInInteger), (numIndecimal == 0) ? "" : ".", text.Substring(num + 1, numIndecimal));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public static TimeSpan ParseIntToTimeSpan(int value)
	{
		try
		{
			value %= 10000;
			return new TimeSpan(value / 100, value % 100, 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new TimeSpan(0, 0, 0);
		}
	}

	public static int ParseTimeSpanToInt(TimeSpan value)
	{
		try
		{
			return value.Hours * 100 + value.Minutes;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public static string ParseSAPHourToString(object value)
	{
		try
		{
			string text = ToString(value);
			int num = 0;
			int num2 = 0;
			switch (text.Length)
			{
			case 1:
				num = ParseInt(text);
				break;
			case 2:
				num = ParseInt(text);
				break;
			case 3:
				num = ParseInt(text.Substring(1, 2));
				num2 = ParseInt(text.Substring(0, 1));
				break;
			case 4:
				num = ParseInt(text.Substring(2, 2));
				num2 = ParseInt(text.Substring(0, 2));
				break;
			}
			return num2.ToString("00") + num.ToString("00");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public static int ParseSAPHour(object value)
	{
		try
		{
			string text = ToString(value);
			int num = 0;
			int num2 = 0;
			if (text.Contains(':'))
			{
				string[] array = text.Split(':');
				if (array != null)
				{
					if (array.Length == 1 || array.Length == 2)
					{
						num2 = ParseInt(array[0]);
					}
					if (array.Length == 2)
					{
						num = ParseInt(array[1]);
					}
					return ParseInt(num2.ToString("00") + num.ToString("00"));
				}
				return 0;
			}
			if (text.Any((char c) => char.IsLetter(c)))
			{
				return 0;
			}
			if (text.Length > 4)
			{
				text = text.Substring(text.Length - 5);
			}
			if (text.Length <= 2)
			{
				num2 = 0;
				num = ParseInt(text);
			}
			else
			{
				num2 = ((text.Length != 4) ? ParseInt(text.Substring(0, 1)) : ParseInt(text.Substring(0, 2)));
				num = ParseInt(text.Substring(3));
			}
			return ParseInt(num2.ToString("00") + num.ToString("00"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public static string ParseIntToTimeSpanString(int value)
	{
		try
		{
			value %= 10000;
			return $"{value / 100}:{value % 100}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "00:00";
		}
	}

	public static string SpaceBeforeCapital(object value)
	{
		try
		{
			return Regex.Replace(ToString(value), "((?<=\\p{Ll})\\p{Lu})|((?!\\A)\\p{Lu}(?>\\p{Ll}))", " $0");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public static string ToTitleCase(object value)
	{
		try
		{
			return new CultureInfo("en-US").TextInfo.ToTitleCase(ToString(value).ToLower());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public static List<string> GetCurrency(string currencyString)
	{
		try
		{
			List<string> list = new List<string>();
			string item = string.Join("", from c in currencyString
				where char.IsDigit(c) || c.Equals('.') || c.Equals('-')
				select (c));
			string item2 = string.Join("", from c in currencyString
				where char.IsLetter(c) && !c.Equals('.') && !c.Equals('-')
				select (c));
			list.Add(item);
			list.Add(item2);
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new List<string>();
		}
	}

	public static object GetValueByType(string type, object sourceObj)
	{
		object obj = DBNull.Value;
		try
		{
			switch (type)
			{
			case "string":
				obj = ToString(sourceObj);
				break;
			case "int16":
				obj = ParseShort(sourceObj);
				obj = obj ?? DBNull.Value;
				break;
			case "int32":
				obj = ParseInt(sourceObj);
				break;
			case "int64":
				obj = ParseLong(sourceObj);
				break;
			case "datetime":
			{
				DateTime? dateTime = ParseDateTime(sourceObj);
				obj = (dateTime.HasValue ? ((object)dateTime.Value) : DBNull.Value);
				break;
			}
			case "decimal":
				obj = ParseDouble(sourceObj);
				break;
			case "double":
				obj = ParseDouble(sourceObj);
				break;
			case "float":
				obj = ParseDouble(sourceObj);
				break;
			default:
				obj = ToString(sourceObj);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return obj;
	}

	public static void ObjectToRow(object obj, DataRow row)
	{
		try
		{
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				string columnName = row.Table.Columns[i].ColumnName;
				PropertyInfo property = obj.GetType().GetProperty(columnName);
				if (!(property == null))
				{
					object obj2 = new object();
					obj2 = property.GetValue(obj, null);
					row[row.Table.Columns[i]] = GetValueByType(row.Table.Columns[i].DataType.Name.ToLower(), obj2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void CopyFolder(string sourceFolder, string destFolder)
	{
		if (Directory.Exists(destFolder))
		{
			DeleteFolder(destFolder);
		}
		Directory.CreateDirectory(destFolder);
		string[] files = Directory.GetFiles(sourceFolder);
		foreach (string text in files)
		{
			string fileName = Path.GetFileName(text);
			if (fileName != null)
			{
				string destFileName = Path.Combine(destFolder, fileName);
				File.Copy(text, destFileName);
			}
		}
		files = Directory.GetDirectories(sourceFolder);
		foreach (string text2 in files)
		{
			string fileName2 = Path.GetFileName(text2);
			if (fileName2 != null)
			{
				string destFolder2 = Path.Combine(destFolder, fileName2);
				CopyFolder(text2, destFolder2);
			}
		}
	}

	public static string GetPlatform()
	{
		if (Marshal.SizeOf(typeof(IntPtr)) == 8)
		{
			return "x64";
		}
		return "x86";
	}

	public static void ZipFolder(string sourceFolder, string destFile)
	{
		if (!Directory.Exists(sourceFolder))
		{
			throw new Exception("Source folder does not exist");
		}
		if (File.Exists(destFile))
		{
			throw new Exception("Dest file exists");
		}
		using ZipFile zipFile = new ZipFile();
		zipFile.AddDirectory(sourceFolder);
		zipFile.Save(destFile);
	}

	public static void ZipListFile(string destFolder, params string[] sourceFile)
	{
		for (int i = 0; i < sourceFile.Length; i++)
		{
			if (!File.Exists(sourceFile[i]))
			{
				throw new Exception($"Source file {sourceFile[i]} does not exist");
			}
		}
		if (File.Exists(destFolder))
		{
			throw new Exception("Dest file exists");
		}
		using ZipFile zipFile = new ZipFile();
		zipFile.AddFiles(sourceFile);
		zipFile.Save(destFolder);
	}

	public static void UnZipFile(string sourceZipFile, string destFolder)
	{
		if (!File.Exists(sourceZipFile))
		{
			throw new Exception("Source file does not exist");
		}
		if (!Directory.Exists(destFolder))
		{
			Directory.CreateDirectory(destFolder);
		}
		using ZipFile zipFile = new ZipFile(sourceZipFile);
		zipFile.ExtractAll(destFolder);
	}

	public static string DeleteFolder(string sourceFolder)
	{
		try
		{
			if (!Directory.Exists(sourceFolder))
			{
				return "";
			}
			string[] files = Directory.GetFiles(sourceFolder);
			for (int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
			files = Directory.GetDirectories(sourceFolder);
			for (int i = 0; i < files.Length; i++)
			{
				DeleteFolder(files[i]);
			}
			Directory.Delete(sourceFolder);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public static string ReplaceSQLInjectCharInColumnName(string inputStr)
	{
		try
		{
			string[] array = new string[8] { "'", ";", "--", "/*", "*/", "xp_", " ", "exec(" };
			for (int i = 0; i < array.Length; i++)
			{
				inputStr = inputStr.Replace(array[i].ToLower(), "");
			}
			return inputStr;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public static string ReplaceSQLInjectCharInCode(string inputStr)
	{
		try
		{
			string[] array = new string[9] { "'", ";", "--", "/*", "*/", "xp_", "exec ", "exec(", "delete " };
			for (int i = 0; i < array.Length; i++)
			{
				inputStr = inputStr.Replace(array[i].ToLower(), "");
			}
			return inputStr;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public static string GetVersionDevexpressDllFile(string file)
	{
		if (string.IsNullOrEmpty(file) || !file.StartsWith("DevExpress.") || !file.EndsWith(".dll"))
		{
			return "";
		}
		string text = "";
		string[] array = file.Split('.');
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (text2.StartsWith("v") && IsInt(text2.Substring(1)))
			{
				text = text2;
				while (IsInt(array[++i]))
				{
					text = string.Join(".", text, array[i]);
				}
				break;
			}
		}
		return text;
	}

	public static bool IsInt(string obj)
	{
		try
		{
			if (string.IsNullOrEmpty(obj))
			{
				return false;
			}
			char[] array = obj.ToCharArray();
			if (array[0] != '-' && array[0] != '+' && (array[0] < '0' || array[0] > '9'))
			{
				return false;
			}
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] < '0' || array[i] > '9')
				{
					return false;
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static int CompareString(string x, string y)
	{
		int num = 0;
		List<int> list = (from z in x.Split(".".ToCharArray())
			select ParseInt(z)).ToList();
		List<int> list2 = (from z in y.Split(".".ToCharArray())
			select ParseInt(z)).ToList();
		for (int num2 = 0; num2 < Math.Max(list.Count, list2.Count); num2++)
		{
			if (list.Count - 1 < num2)
			{
				return -1;
			}
			if (list2.Count - 1 < num2)
			{
				return 1;
			}
			num = list[num2] - list2[num2];
			if (num != 0)
			{
				return num;
			}
		}
		return num;
	}

	public static string FormatDateToView(DateTime sourceDate)
	{
		try
		{
			if (sourceDate == DateTime.MaxValue || sourceDate == DateTime.MinValue)
			{
				return "";
			}
			TimeSpan timeSpan = DateTime.Now - sourceDate;
			double totalSeconds = timeSpan.TotalSeconds;
			if ((int)Math.Abs(timeSpan.TotalDays / 365.0) > 10)
			{
				return "";
			}
			if (totalSeconds < -31104000.0)
			{
				return $"about {(int)Math.Abs(timeSpan.TotalDays / 365.0)} years";
			}
			if (totalSeconds < -2592000.0)
			{
				return $"about {(int)Math.Abs(timeSpan.TotalDays / 30.0)} months";
			}
			if (totalSeconds < -86400.0)
			{
				return $"about {(int)Math.Abs(timeSpan.TotalDays)} days";
			}
			if (totalSeconds < -3600.0)
			{
				return $"about {(int)Math.Abs(timeSpan.TotalHours)} hours";
			}
			if (totalSeconds < -60.0)
			{
				return $"about {(int)Math.Abs(timeSpan.TotalMinutes)} minutes";
			}
			if (totalSeconds < -10.0)
			{
				return $"about {(int)Math.Abs(timeSpan.TotalSeconds)} seconds";
			}
			if (totalSeconds < 0.0)
			{
				return "about few seconds";
			}
			if (totalSeconds < 10.0)
			{
				return "few seconds ago";
			}
			if (totalSeconds < 60.0)
			{
				return $"about {(int)timeSpan.TotalSeconds} seconds ago";
			}
			if (totalSeconds < 3600.0)
			{
				return $"about {(int)timeSpan.TotalMinutes} minutes ago";
			}
			if (totalSeconds < 86400.0)
			{
				if ((int)timeSpan.TotalHours > 1)
				{
					return $"about {(int)timeSpan.TotalHours} hours ago";
				}
				return $"about {(int)timeSpan.TotalHours} hour ago";
			}
			if (totalSeconds < 2592000.0)
			{
				if ((int)timeSpan.TotalDays > 1)
				{
					return $"about {(int)timeSpan.TotalDays} days ago";
				}
				return $"about {(int)timeSpan.TotalDays} day ago";
			}
			if (totalSeconds < 31104000.0)
			{
				if ((int)timeSpan.TotalDays / 30 > 1)
				{
					return $"about {(int)timeSpan.TotalDays / 30} months ago";
				}
				return $"about {(int)timeSpan.TotalDays / 30} month ago";
			}
			if ((int)timeSpan.TotalDays / 365 > 1)
			{
				return $"about {(int)timeSpan.TotalDays / 365} years ago";
			}
			return $"about {(int)timeSpan.TotalDays / 365} year ago";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public static DateTime GetFolderLastModifiedDate(string folder)
	{
		if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
		{
			return DateTime.MinValue;
		}
		DateTime dateTime = (from f in new DirectoryInfo(folder).GetFiles()
			orderby f.LastWriteTime descending
			select f).FirstOrDefault()?.LastWriteTime ?? DateTime.MinValue;
		dateTime = ((dateTime > Directory.GetLastWriteTime(folder)) ? dateTime : Directory.GetLastWriteTime(folder));
		string[] directories = Directory.GetDirectories(folder);
		for (int num = 0; num < directories.Length; num++)
		{
			dateTime = GetFolderLastDateByCurrentDate(directories[num], dateTime);
		}
		return dateTime;
	}

	private static DateTime GetFolderLastDateByCurrentDate(string folder, DateTime lastDate)
	{
		if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
		{
			return DateTime.MinValue;
		}
		DateTime dateTime = (from f in new DirectoryInfo(folder).GetFiles()
			orderby f.LastWriteTime descending
			select f).FirstOrDefault()?.LastWriteTime ?? DateTime.MinValue;
		dateTime = ((dateTime > lastDate) ? dateTime : lastDate);
		string[] directories = Directory.GetDirectories(folder);
		for (int num = 0; num < directories.Length; num++)
		{
			dateTime = GetFolderLastDateByCurrentDate(directories[num], dateTime);
		}
		return dateTime;
	}

	public static string GetExceptionAndLineNum(Exception ex)
	{
		try
		{
			StackTrace stackTrace = new StackTrace(ex, fNeedFileInfo: true);
			return string.Concat(stackTrace.ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), str3: stackTrace.GetFrame(0).GetFileLineNumber().ToString(), str1: " ", str2: ex.Message);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return "";
	}

	public static T DataRowToEntity<T>(DataRow row) where T : new()
	{
		T val = new T();
		for (int i = 0; i < row.Table.Columns.Count; i++)
		{
			string columnName = row.Table.Columns[i].ColumnName;
			PropertyInfo property = val.GetType().GetProperty(columnName);
			if (property == null)
			{
				continue;
			}
			switch (GetTypeName(property.PropertyType).ToLower())
			{
			case "string":
				val.GetType().GetProperty(columnName).SetValue(val, ToString(row[row.Table.Columns[i]]));
				break;
			case "double":
				val.GetType().GetProperty(columnName).SetValue(val, ParseDouble(row[row.Table.Columns[i]]));
				break;
			case "int16":
				val.GetType().GetProperty(columnName).SetValue(val, ParseShort(row[row.Table.Columns[i]]));
				break;
			case "int32":
				val.GetType().GetProperty(columnName).SetValue(val, ParseInt(row[row.Table.Columns[i]]));
				break;
			case "int64":
				val.GetType().GetProperty(columnName).SetValue(val, ParseLong(row[row.Table.Columns[i]]));
				break;
			case "datetime":
			{
				DateTime? dateTime = ((property == null) ? ((DateTime?)null) : ParseDateTime(row[row.Table.Columns[i]]));
				PropertyInfo property2 = val.GetType().GetProperty(columnName);
				object obj = val;
				DateTime dateTime2;
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dateTime2 = dateTime.Value;
				}
				else
				{
					dateTime2 = new DateTime(1900, 1, 1);
				}
				property2.SetValue(obj, dateTime2);
				break;
			}
			}
		}
		return val;
	}

	public static List<string> GetTableName(ApzObjectTypes type)
	{
		List<string> list = new List<string>();
		string item = string.Empty;
		string item2 = string.Empty;
		switch (type)
		{
		case ApzObjectTypes.oQuotations:
			item = "OQUT";
			item2 = "QUT1";
			break;
		case ApzObjectTypes.oOrders:
			item = "ORDR";
			item2 = "RDR1";
			break;
		case ApzObjectTypes.oDeliveryNotes:
			item = "ODLN";
			item2 = "DLN1";
			break;
		case ApzObjectTypes.oReserveInvoices:
			item = "OINV";
			item2 = "INV1";
			break;
		case ApzObjectTypes.oInvoices:
			item = "OINV";
			item2 = "INV1";
			break;
		case ApzObjectTypes.oCreditNotes:
			item = "ORIN";
			item2 = "RIN1";
			break;
		case ApzObjectTypes.oReturnRequest:
			item = "ORRR";
			item2 = "RRR1";
			break;
		case ApzObjectTypes.oReturns:
			item = "ORDN";
			item2 = "RDN1";
			break;
		case ApzObjectTypes.oDrafts:
			item = "ODRF";
			item2 = "DRF1";
			break;
		case ApzObjectTypes.oBlanketAgreement:
			item = "OOAT";
			item2 = "OAT1";
			break;
		case ApzObjectTypes.oDownPayments:
			item = "ODPI";
			item2 = "DPI1";
			break;
		case ApzObjectTypes.oPurchaseQuotations:
			item = "OPQT";
			item2 = "PQT1";
			break;
		case ApzObjectTypes.oPurchaseOrders:
			item = "OPOR";
			item2 = "POR1";
			break;
		case ApzObjectTypes.oPurchaseRequest:
			item = "OPRQ";
			item2 = "PRQ1";
			break;
		case ApzObjectTypes.oPurchaseInvoices:
			item = "OPCH";
			item2 = "PCH1";
			break;
		case ApzObjectTypes.oPurchaseDeliveryNotes:
			item = "OPDN";
			item2 = "PDN1";
			break;
		case ApzObjectTypes.oPurchaseCreditNotes:
			item = "ORPC";
			item2 = "RPC1";
			break;
		case ApzObjectTypes.oGoodsReturnRequest:
			item = "OPRR";
			item2 = "PRR1";
			break;
		case ApzObjectTypes.oPurchaseReturns:
			item = "ORPD";
			item2 = "RPD1";
			break;
		case ApzObjectTypes.oPurchaseLandedCost:
			item = "OIPF";
			item2 = "IPF1";
			break;
		case ApzObjectTypes.oAPReserveInvoice:
			item = "OPCH";
			item2 = "PCH1";
			break;
		case ApzObjectTypes.oPurchaseDownPayments:
			item = "ODPO";
			item2 = "DPO1";
			break;
		case ApzObjectTypes.oPurchaseBlanketAgreement:
			item = "OOAT";
			item2 = "OAT1";
			break;
		case ApzObjectTypes.oStockTransferDraft:
			item = "OTRR";
			item2 = "TRR1";
			break;
		case ApzObjectTypes.oStockTransfer:
			item = "OWTR";
			item2 = "WTR1";
			break;
		case ApzObjectTypes.oInventoryTransferRequest:
			item = "OWTQ";
			item2 = "WTQ1";
			break;
		case ApzObjectTypes.oInventoryGenEntry:
			item = "OIGN";
			item2 = "IGN1";
			break;
		case ApzObjectTypes.oInventoryGenExit:
			item = "OIGE";
			item2 = "IGE1";
			break;
		case ApzObjectTypes.oManufacturers:
			item = "OMRC";
			item2 = "";
			break;
		case ApzObjectTypes.oIndustries:
			item = "OOND";
			item2 = "";
			break;
		case ApzObjectTypes.oRelationships:
			item = "OORL";
			item2 = "";
			break;
		case ApzObjectTypes.oSalesForecast:
			item = "OFCT";
			item2 = "FCT1";
			break;
		case ApzObjectTypes.oTrackingIssue:
			item = "APZ_TGIE";
			item2 = "";
			break;
		case ApzObjectTypes.oEmployeesInfo:
			item = "OHEM";
			item2 = "";
			break;
		case ApzObjectTypes.oOpportunityCompetitor:
			item = "OCMT";
			item2 = "";
			break;
		case ApzObjectTypes.oOpportunityPartners:
			item = "OPRT";
			item2 = "";
			break;
		case ApzObjectTypes.oOpportunityReasons:
			item = "OOFR";
			item2 = "";
			break;
		case ApzObjectTypes.oSalesStages:
			item = "OOST";
			item2 = "";
			break;
		case ApzObjectTypes.oInterest:
			item = "OOIN";
			item2 = "";
			break;
		case ApzObjectTypes.oActivityLocations:
			item = "OCLO";
			item2 = "";
			break;
		case ApzObjectTypes.oProductionOrders:
			item = "OWOR";
			item2 = "";
			break;
		case ApzObjectTypes.oLandedCostsCodes:
			item = "OALC";
			break;
		case ApzObjectTypes.oMaterialRevaluation:
			item = "OMRV";
			item2 = "MRV1";
			break;
		case ApzObjectTypes.oVendorPayments:
			item = "OVPM";
			item2 = "VPM1";
			break;
		case ApzObjectTypes.oIncomingPayments:
			item = "ORCT";
			item2 = "RCT1";
			break;
		}
		list.Add(item);
		list.Add(item2);
		return list;
	}

	public static string GetStartupFolderName()
	{
		return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Name;
	}

	public static string RemoveSign4VietnameseString(string str)
	{
		for (int i = 1; i < VietnameseSigns.Length; i++)
		{
			for (int j = 0; j < VietnameseSigns[i].Length; j++)
			{
				str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
			}
		}
		return str;
	}

	public static List<string> GetFieldsObject(int fatherId, DataTable treeConfig)
	{
		List<string> lstField = new List<string>();
		EnumerableRowCollection<DataRow> source = from t in treeConfig.AsEnumerable()
			where ParseInt(t["FatherNum"]) == fatherId && ToString(t["JsonType"]) == "F"
			select t;
		if (source.Count() > 0)
		{
			lstField = (from t in source
				where !string.IsNullOrEmpty(ToString(t["Value"]))
				select ToString(t["Value"])).Distinct().ToList();
		}
		EnumerableRowCollection<DataRow> enumerableRowCollection = from t in treeConfig.AsEnumerable()
			where ParseInt(t["FatherNum"]) == fatherId && ToString(t["JsonType"]) != "F"
			select t;
		if (enumerableRowCollection.Count() > 0)
		{
			foreach (DataRow item in enumerableRowCollection)
			{
				List<string> fieldsObject = GetFieldsObject(ParseInt(item["ConfigID"]), treeConfig);
				if (!fieldsObject.Any())
				{
					continue;
				}
				IEnumerable<string> enumerable = fieldsObject.Where((string t) => !lstField.Any((string x) => x.Equals(t)));
				if (enumerable.Any())
				{
					lstField.AddRange(enumerable);
				}
			}
		}
		return lstField;
	}

	public static string ConvertTableToJson(DataTable treeConfig, DataTable src, int father, bool isArrayObject, string apiTokenKey = "")
	{
		if (!treeConfig.IsNotNull())
		{
			return "";
		}
		string text = "";
		DataTable dataTable = new DataTable();
		List<string> fieldsObject = GetFieldsObject(father, treeConfig);
		dataTable = ((fieldsObject == null || fieldsObject.Count <= 0) ? src : src.DefaultView.ToTable(distinct: true, fieldsObject.ToArray()));
		List<DataRow> list = (from t in treeConfig.AsEnumerable()
			where ParseInt(t["FatherNum"]) == father && ToString(t["JsonType"]) == "F"
			select t).ToList();
		List<DataRow> list2 = (from t in treeConfig.AsEnumerable()
			where ParseInt(t["FatherNum"]) == father && ToString(t["JsonType"]) != "F"
			select t).ToList();
		if (list.Any())
		{
			List<string> list3 = (from t in list
				where !string.IsNullOrEmpty(ToString(t["Value"]))
				select ToString(t["Value"])).ToList();
			if (list3.Any())
			{
				DataTable dataTable2 = dataTable.DefaultView.ToTable(distinct: true, list3.ToArray());
				for (int num = 0; num < dataTable2.Rows.Count; num++)
				{
					text += (isArrayObject ? "{" : "");
					text += GetJsonMappingField(list, dataTable2.Rows[num]);
					if (list2.Any())
					{
						DataTable dtSource = dataTable.Select(GetSelectQuery(list3, dataTable2.Rows[num])).CopyToDataTable();
						text += GetJsonMappingChildObject(list2, dtSource, treeConfig, apiTokenKey);
					}
					if (text.EndsWith(","))
					{
						text = text.Substring(0, text.Length - 1);
					}
					text += (isArrayObject ? "}," : ",");
				}
			}
			else
			{
				text += (isArrayObject ? "{" : "");
				text += GetJsonMappingField(list, dataTable.Rows[0]);
				if (list2.Any())
				{
					text += GetJsonMappingChildObject(list2, dataTable, treeConfig, apiTokenKey);
				}
				if (text.EndsWith(","))
				{
					text = text.Substring(0, text.Length - 1);
				}
				text += (isArrayObject ? "}" : "");
			}
		}
		else
		{
			text += (isArrayObject ? "{" : "");
			if (list2.Any())
			{
				text += GetJsonMappingChildObject(list2, dataTable, treeConfig, apiTokenKey);
			}
			if (text.EndsWith(","))
			{
				text = text.Substring(0, text.Length - 1);
			}
			text += (isArrayObject ? "}" : "");
		}
		if (text.EndsWith(","))
		{
			text = text.Substring(0, text.Length - 1);
		}
		return text;
	}

	private static string GetSelectQuery(List<string> lstFieldMapping, DataRow row)
	{
		string[] array = new string[lstFieldMapping.Count];
		for (int i = 0; i < lstFieldMapping.Count; i++)
		{
			string text = lstFieldMapping[i];
			if (row[text] is string)
			{
				array[i] = "ISNULL([" + text + "],'') = '" + ToString(row[text]).Replace("'", "''") + "'";
			}
			else
			{
				array[i] = GetQueryField(text, row[text]);
			}
		}
		return string.Join(" AND ", array);
	}

	private static string GetQueryField(string fieldName, object value)
	{
		if (value == null || value == DBNull.Value)
		{
			return "[" + fieldName + "] IS NULL";
		}
		if (value is int || value is long || value is short)
		{
			return $"[{fieldName}] = {value}";
		}
		if (value is float num)
		{
			return $"[{fieldName}] = {num.ToString(CultureInfo.InvariantCulture)}";
		}
		if (value is double num2)
		{
			return $"[{fieldName}] = {num2.ToString(CultureInfo.InvariantCulture)}";
		}
		if (value is decimal num3)
		{
			return $"[{fieldName}] = {num3.ToString(CultureInfo.InvariantCulture)}";
		}
		if (value is DateTime dateTime)
		{
			return string.Format("[{0}] = '{1}'", fieldName, dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
		}
		return string.Format("[{0}] = '{1}'", fieldName, value.ToString().Replace("'", "''"));
	}

	private static string GetJsonMappingField(List<DataRow> lstMappingField, DataRow sourceRow)
	{
		string text = "";
		for (int i = 0; i < lstMappingField.Count; i++)
		{
			DataRow dataRow = lstMappingField[i];
			string text2 = ToString(dataRow["Name"]);
			string text3 = ToString(dataRow["Value"]);
			string text4 = ToString(dataRow["DfltValue"]);
			Apzon.Commons.Enums.Enums.DisplayFormatType displayFormatType = (Apzon.Commons.Enums.Enums.DisplayFormatType)ParseInt(dataRow["DataType"]);
			bool flag = ToString(dataRow["Required"]) == "Y";
			string text5 = "";
			object obj = null;
			if (!string.IsNullOrWhiteSpace(text3))
			{
				obj = sourceRow[text3];
				text5 = sourceRow.Table.Columns[text3].DataType.ToString().ToLower();
			}
			if (obj == null || obj == DBNull.Value)
			{
				obj = text4;
			}
			if (flag && (obj == null || obj == DBNull.Value || string.IsNullOrEmpty(ToString(obj))))
			{
				throw new Exception(text2 + " is required");
			}
			switch (displayFormatType)
			{
			case Apzon.Commons.Enums.Enums.DisplayFormatType.StringBase64:
			{
				string text6 = Convert.ToBase64String(Encoding.UTF8.GetBytes(ToString(obj)));
				text = text + "\"" + text2 + "\" :\"" + text6 + "\",";
				break;
			}
			case Apzon.Commons.Enums.Enums.DisplayFormatType.Qty:
			case Apzon.Commons.Enums.Enums.DisplayFormatType.Amount:
			case Apzon.Commons.Enums.Enums.DisplayFormatType.Rate:
			case Apzon.Commons.Enums.Enums.DisplayFormatType.Percent:
			case Apzon.Commons.Enums.Enums.DisplayFormatType.Unit:
			case Apzon.Commons.Enums.Enums.DisplayFormatType.Integer:
			case Apzon.Commons.Enums.Enums.DisplayFormatType.Weight:
			case Apzon.Commons.Enums.Enums.DisplayFormatType.LengthAndWidth:
				text = ((obj != null && obj != DBNull.Value && !string.IsNullOrWhiteSpace(ToString(obj))) ? (text + "\"" + text2 + "\" :" + $"{obj},") : (text + "\"" + text2 + "\" : null,"));
				break;
			default:
				text = text + "\"" + text2 + "\" :" + $"\"{obj}\",";
				break;
			case Apzon.Commons.Enums.Enums.DisplayFormatType.None:
				if (ToString(dataRow["DataType"]) == "0")
				{
					if (!string.IsNullOrWhiteSpace(text2))
					{
						text = text + "\"" + text2 + "\": ";
					}
					text += $"\"{obj}\",";
					break;
				}
				switch (text5)
				{
				case "system.double":
				case "system.decimal":
				case "system.float":
				case "system.single":
				case "system.int16":
				case "system.int32":
				case "system.int64":
					text = ((obj != null && obj != DBNull.Value && !string.IsNullOrWhiteSpace(ToString(obj))) ? (text + "\"" + text2 + "\" :" + $"{obj},") : (text + "\"" + text2 + "\" : null,"));
					break;
				default:
					text = text + "\"" + text2 + "\" :" + $"\"{obj}\",";
					break;
				}
				break;
			}
		}
		return text;
	}

	private static string GetJsonMappingChildObject(List<DataRow> lstMappingObject, DataTable dtSource, DataTable objectConfig, string apiTokenKey = "")
	{
		string text = "";
		for (int i = 0; i < lstMappingObject.Count; i++)
		{
			DataRow dataRow = lstMappingObject[i];
			string text2 = ToString(dataRow["JsonType"]);
			string text3 = ToString(dataRow["Name"]);
			int father = ParseInt(dataRow["ConfigID"]);
			switch (text2)
			{
			case "A":
				text = text + (string.IsNullOrEmpty(text3) ? "[" : ("\"" + text3 + "\":[")) + ConvertTableToJson(objectConfig, dtSource, father, isArrayObject: false, apiTokenKey) + "],";
				break;
			case "O":
			{
				int fatherNodeId = ParseInt(dataRow["FatherNum"]);
				bool flag = false;
				DataRow dataRow2 = objectConfig.AsEnumerable().FirstOrDefault((DataRow t) => ParseInt(t["ConfigID"]) == fatherNodeId);
				if (dataRow2 != null)
				{
					flag = ToString(dataRow2["JsonType"]).Equals("A");
				}
				string text5 = (flag ? "" : "{") + ConvertTableToJson(objectConfig, dtSource, father, flag, apiTokenKey) + (flag ? "," : "},");
				text = (string.IsNullOrEmpty(text3) ? (text + text5) : (text + "\"" + text3 + "\":" + text5));
				break;
			}
			case "EZ":
			{
				Convertor.Zip(ConvertTableToJson(objectConfig, dtSource, father, isArrayObject: false, apiTokenKey), out var zippedData);
				SecurityData.Encrypt_Aes(zippedData, apiTokenKey, out string Base64Result);
				string text4 = Base64Result;
				text = (string.IsNullOrEmpty(text3) ? (text + "\"" + text4 + "\"") : (text + "\"" + text3 + "\":\"" + text4 + "\""));
				break;
			}
			}
		}
		return text;
	}

	public static string ConvertTableToXml(DataTable treeConfig, DataTable data, int father, bool inArray = false)
	{
		string text = "";
		if (!treeConfig.IsNotNull())
		{
			return text;
		}
		string text2 = "";
		for (int i = 0; i < data.Rows.Count && (inArray || i <= 0); i++)
		{
			string text3 = "";
			for (int j = 0; j < treeConfig.Rows.Count; j++)
			{
				DataRow dataRow = treeConfig.Rows[j];
				if (ParseInt(dataRow["FatherNum"]) != father)
				{
					continue;
				}
				string text4 = ToString(dataRow["JsonType"]);
				string text5 = ToString(dataRow["Name"]);
				string value = ToString(dataRow["Value"]);
				string text6 = ToString(dataRow["DfltValue"]);
				Apzon.Commons.Enums.Enums.DisplayFormatType displayFormatType = (Apzon.Commons.Enums.Enums.DisplayFormatType)ParseInt(dataRow["DataType"]);
				string text7 = "";
				if (!data.Columns.Contains(value) && !string.IsNullOrWhiteSpace(value))
				{
					continue;
				}
				object obj = null;
				if (!string.IsNullOrWhiteSpace(value))
				{
					object obj2 = (from t in data.AsEnumerable()
						select t[value]).FirstOrDefault();
					obj = (inArray ? data.Rows[i][value] : obj2);
					text7 = data.Columns[value].DataType.ToString().ToLower();
				}
				if (obj == null || obj == DBNull.Value)
				{
					obj = text6;
				}
				int father2 = ParseInt(dataRow["ConfigID"]);
				switch (text4)
				{
				case "A":
					text3 = text3 + "<" + text5 + ">" + ConvertTableToXml(treeConfig, data, father2, inArray: true) + "</" + text5 + ">";
					break;
				case "O":
				{
					DataTable dataTable = data.Clone();
					dataTable.ImportRow(data.Rows[i]);
					text3 = ((!inArray) ? (text3 + "<" + text5 + ">" + ConvertTableToXml(treeConfig, data, father2) + "</" + text5 + ">") : (text3 + "<" + text5 + ">" + ConvertTableToXml(treeConfig, dataTable, father2) + "</" + text5 + ">"));
					break;
				}
				case "F":
					switch (displayFormatType)
					{
					case Apzon.Commons.Enums.Enums.DisplayFormatType.StringBase64:
					{
						string text8 = Convert.ToBase64String(Encoding.UTF8.GetBytes(ToString(obj)));
						text3 = text3 + "<" + text5 + ">" + text8 + "</" + text5 + ">";
						break;
					}
					case Apzon.Commons.Enums.Enums.DisplayFormatType.Qty:
					case Apzon.Commons.Enums.Enums.DisplayFormatType.Amount:
					case Apzon.Commons.Enums.Enums.DisplayFormatType.Rate:
					case Apzon.Commons.Enums.Enums.DisplayFormatType.Percent:
					case Apzon.Commons.Enums.Enums.DisplayFormatType.Unit:
					case Apzon.Commons.Enums.Enums.DisplayFormatType.Integer:
					case Apzon.Commons.Enums.Enums.DisplayFormatType.Weight:
					case Apzon.Commons.Enums.Enums.DisplayFormatType.LengthAndWidth:
						text3 = text3 + "<" + text5 + ">" + $"{obj}" + "</" + text5 + ">";
						break;
					default:
						text3 = text3 + "<" + text5 + ">" + $"{obj}" + "</" + text5 + ">";
						break;
					case Apzon.Commons.Enums.Enums.DisplayFormatType.None:
						if (ToString(dataRow["DataType"]) == "0")
						{
							text3 = text3 + "<" + text5 + ">" + $"{obj}" + "</" + text5 + ">";
							break;
						}
						switch (text7)
						{
						case "system.double":
						case "system.decimal":
						case "system.float":
						case "system.single":
						case "system.int16":
						case "system.int32":
						case "system.int64":
							text3 = text3 + "<" + text5 + ">" + $"{obj}" + "</" + text5 + ">";
							break;
						default:
							text3 = text3 + "<" + text5 + ">" + $"{obj}" + "</" + text5 + ">";
							break;
						}
						break;
					}
					break;
				}
			}
			text2 += text3;
		}
		return text + text2;
	}

	public static string GetJsonString(string fieldName, object value)
	{
		if (value == null || value == DBNull.Value)
		{
			return $"\"{fieldName}\":null,";
		}
		if (value is float num)
		{
			return $"\"{fieldName}\":{num.ToString(CultureInfo.InvariantCulture)},";
		}
		if (value is double num2)
		{
			return $"\"{fieldName}\":{num2.ToString(CultureInfo.InvariantCulture)},";
		}
		if (value is decimal num3)
		{
			return $"\"{fieldName}\":{num3.ToString(CultureInfo.InvariantCulture)},";
		}
		if (value is DateTime dateTime)
		{
			return string.Format("\"{0}\":\"{1}\",", fieldName, dateTime.ToString("yyyyMMdd"));
		}
		return $"\"{fieldName}\":{JsonConvert.ToString(ToString(value))},";
	}

	public static string GetStringSha256Hash(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		using SHA256Managed sHA256Managed = new SHA256Managed();
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		return BitConverter.ToString(sHA256Managed.ComputeHash(bytes)).Replace("-", string.Empty);
	}

	public static Bitmap ResizeBitmap(System.Drawing.Image bmp, int width, int height)
	{
		try
		{
			Bitmap bitmap = new Bitmap(width, height);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(bmp, 0, 0, width, height);
			}
			return bitmap;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return bmp as Bitmap;
	}

	public static System.Drawing.Image ConvertSVGtoPNG(string svgData, int width, int height, string colorHex = "")
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(svgData);
		SvgDocument svgDocument = SvgDocument.Open(xmlDocument);
		svgDocument.Width = width;
		svgDocument.Height = height;
		if (!string.IsNullOrEmpty(colorHex))
		{
			foreach (SvgElement child in svgDocument.Children)
			{
				ChangeFillSvg(child, ColorTranslator.FromHtml(colorHex));
			}
		}
		return svgDocument.Draw();
	}

	public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
	{
		double val = (double)maxWidth / (double)image.Width;
		double val2 = (double)maxHeight / (double)image.Height;
		double num = Math.Min(val, val2);
		int width = (int)((double)image.Width * num);
		int height = (int)((double)image.Height * num);
		if (image.Width <= maxWidth && image.Height <= maxHeight)
		{
			width = image.Width;
			height = image.Height;
		}
		Bitmap bitmap = new Bitmap(width, height);
		using Graphics graphics = Graphics.FromImage(bitmap);
		graphics.DrawImage(image, 0, 0, width, height);
		return bitmap;
	}

	public static void ChangeFillSvg(SvgElement element, Color replaceColor)
	{
		if (element is SvgPath)
		{
			(element as SvgPath).Fill = new SvgColourServer(replaceColor);
			(element as SvgPath).Stroke = new SvgColourServer(replaceColor);
		}
		if (element.Children.Count <= 0)
		{
			return;
		}
		foreach (SvgElement child in element.Children)
		{
			ChangeFillSvg(child, replaceColor);
		}
	}

	public static string ConvertIntToHex(int value)
	{
		Color color = ColorTranslator.FromHtml(ToString(value));
		Color color2 = Color.FromArgb(color.R, color.G, color.B);
		string text = color2.R.ToString("X2") + color2.G.ToString("X2") + color2.B.ToString("X2");
		return "#" + text;
	}

	public static string MergePDFFromStringBase64(List<string> lstBase64, string destFilePath)
	{
		try
		{
			string directoryName = Path.GetDirectoryName(destFilePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			using FileStream os = new FileStream(destFilePath, FileMode.Create);
			using Document document = new Document();
			using PdfCopy pdfCopy = new PdfCopy(document, os);
			document.Open();
			PdfReader pdfReader = null;
			PdfImportedPage pdfImportedPage = null;
			foreach (string item in lstBase64)
			{
				try
				{
					pdfReader = new PdfReader(Convert.FromBase64String(item));
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", "Merge PDF Reader", ex.Message);
					continue;
				}
				for (int i = 0; i < pdfReader.NumberOfPages; i++)
				{
					pdfImportedPage = pdfCopy.GetImportedPage(pdfReader, i + 1);
					pdfCopy.AddPage(pdfImportedPage);
				}
				pdfCopy.FreeReader(pdfReader);
				pdfReader.Close();
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", "Merge PDF", ex2.Message);
			return ex2.Message;
		}
		return "";
	}
}
