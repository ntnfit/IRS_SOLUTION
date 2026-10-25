using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Helper;

namespace Commons.Helper;

public class CopyFileShareHelper
{
	private static string _path = AppDomain.CurrentDomain.BaseDirectory + "CopyShareFolder.dat";

	public static void SetConfigData(CopyFileShareStruct _cd)
	{
		try
		{
			using ResourceWriter resourceWriter = new ResourceWriter(_path);
			resourceWriter.AddResource("Domain", EncodePassword.EncryptString(_cd.Domain, RSA.PasswordPhrase));
			resourceWriter.AddResource("User", EncodePassword.EncryptString(_cd.User, RSA.PasswordPhrase));
			resourceWriter.AddResource("Pass", EncodePassword.EncryptString(_cd.Pass, RSA.PasswordPhrase));
			resourceWriter.Generate();
			resourceWriter.Close();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static CopyFileShareStruct GetConfigData()
	{
		CopyFileShareStruct result = default;
		try
		{
			if (File.Exists(_path))
			{
				using ResourceSet resourceSet = new ResourceSet(_path);
				result.Domain = EncodePassword.DecryptString(resourceSet.GetString("Domain"), RSA.PasswordPhrase);
				result.User = EncodePassword.DecryptString(resourceSet.GetString("User"), RSA.PasswordPhrase);
				result.Pass = EncodePassword.DecryptString(resourceSet.GetString("Pass"), RSA.PasswordPhrase);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CopyFileShareHelper-->GetConfigData " + ex.Message);
		}
		return result;
	}

	public static string GetUpLoadPath(string fileExt)
	{
		try
		{
			if (fileExt[0] == '.')
			{
				fileExt = fileExt.Remove(0, 1);
			}
			fileExt = fileExt.ToLower();
			string text = "";
			if ("bmp,jpg,jpeg,gif,png,tiff".IndexOf(fileExt) > -1)
			{
				text = CopyFileSharingCfg.BitmapPath;
			}
			if ("xls,xlsx".IndexOf(fileExt) > -1)
			{
				text = CopyFileSharingCfg.ExcelPath;
			}
			if ("doc,docx".IndexOf(fileExt) > -1)
			{
				text = CopyFileSharingCfg.WordPath;
			}
			if ("xml".IndexOf(fileExt) > -1)
			{
				text = CopyFileSharingCfg.XmlPath;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return CopyFileSharingCfg.AttachPath;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CopyFileShareHelper-->GetUpLoadPath " + ex.Message);
			return CopyFileSharingCfg.AttachPath;
		}
	}
}
