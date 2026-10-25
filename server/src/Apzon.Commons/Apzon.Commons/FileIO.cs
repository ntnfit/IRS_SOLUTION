using System;
using System.IO;
using System.Windows.Forms;

namespace Apzon.Commons;

public static class FileIO
{
	public static string PathSaveFile(string fileType, string fileExten)
	{
		string text = string.Empty;
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = string.Format("{0} Files (*.{1}*)|*.{1}", fileType, fileExten);
		saveFileDialog.FilterIndex = 1;
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			text = saveFileDialog.FileName;
		}
		if (FileExist(text))
		{
			DeleteFile(text);
		}
		return text;
	}

	public static string getExtenName(string strSourceFile)
	{
		return strSourceFile.Substring(strSourceFile.LastIndexOf(".") + 1, strSourceFile.Length - strSourceFile.LastIndexOf(".") - 1);
	}

	public static string getFileName(string strSourceFile)
	{
		return strSourceFile.Substring(strSourceFile.LastIndexOf("\\") + 1);
	}

	public static string getFileNameServer(string strSourceFile)
	{
		return strSourceFile.Substring(strSourceFile.LastIndexOf("/") + 1);
	}

	public static bool FileExist(string strFilePath)
	{
		bool flag = false;
		try
		{
			return File.Exists(strFilePath);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string getPath(string strSourceFile)
	{
		return strSourceFile.Substring(0, strSourceFile.LastIndexOf("\\")) + "\\";
	}

	public static bool CopyFile(string strSourceFile, string strDestinationFile, bool blnOverWrite)
	{
		if (!FileExist(strSourceFile))
		{
			return false;
		}
		if (!FolderExist(getPath(strDestinationFile)))
		{
			return false;
		}
		try
		{
			File.Copy(strSourceFile, strDestinationFile, blnOverWrite);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool DeleteFile(string strDeleteFile)
	{
		if (!FileExist(strDeleteFile))
		{
			return false;
		}
		try
		{
			File.Delete(strDeleteFile);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static string GetPathInFullName(string strFullName)
	{
		string result = "C:\\";
		try
		{
			if (strFullName.LastIndexOf("\\") > 0)
			{
				result = strFullName.Substring(0, strFullName.LastIndexOf("\\"));
			}
		}
		catch (Exception)
		{
			result = "C:\\";
		}
		return result;
	}

	public static string RenameFile(string strFilePath)
	{
		string empty = string.Empty;
		try
		{
			return DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + getFileName(strFilePath);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static bool MoveFile(string strSourceFile, string strDestinationFile)
	{
		try
		{
			File.Move(strSourceFile, strDestinationFile);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool MoveFolder(string strSourceFolder, string strDestinationFolder)
	{
		try
		{
			Directory.Move(strSourceFolder, strDestinationFolder);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool FolderExist(string strFolderPath)
	{
		bool flag = false;
		try
		{
			return Directory.Exists(strFolderPath);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool FolderDelete(string strFolderPath)
	{
		try
		{
			Directory.Delete(strFolderPath, recursive: true);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static void FolderCreate(string strFolderPath)
	{
		if (!FolderExist(strFolderPath))
		{
			Directory.CreateDirectory(strFolderPath);
		}
	}
}
