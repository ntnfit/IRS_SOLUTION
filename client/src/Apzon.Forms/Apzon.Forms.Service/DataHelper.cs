using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.Utils;

namespace Apzon.Forms.Service;

public class DataHelper
{
	private static List<Message> messages = null;

	internal static DateTime LastMailDate = DateTime.Now;

	internal static DataTable MailTable
	{
		get
		{
			DataTable dataTable = CreateDataTable("Messages");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				LastMailDate = (DateTime)dataTable.Rows[0]["Date"];
			}
			return dataTable;
		}
	}

	public static List<Message> Messages
	{
		get
		{
			try
			{
				if (messages == null)
				{
					messages = new List<Message>();
					DataTable mailTable = MailTable;
					if (mailTable != null)
					{
						foreach (DataRow row in mailTable.Rows)
						{
							messages.Add(new Message(row));
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				messages = new List<Message>();
			}
			return messages;
		}
	}

	private static DataTable CreateDataTable(string table)
	{
		DataSet dataSet = new DataSet();
		string text = FilesHelper.FindingFileName(Application.StartupPath, "Data\\Mail.xml");
		if (text != string.Empty)
		{
			FileInfo fileInfo = new FileInfo(text);
			dataSet.ReadXml(fileInfo.FullName);
			return dataSet.Tables[table];
		}
		return null;
	}
}
