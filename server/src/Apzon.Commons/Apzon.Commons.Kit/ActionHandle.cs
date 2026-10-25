using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Apzon.Commons.Helper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer.DataDefModel;

namespace Apzon.Commons.Kit;

public class ActionHandle
{
	public MailImpl GetMailContent(System.Data.DataSet ds, DataTable source)
	{
		DataTable dataTable = ds.Tables[0];
		string s = Function.ToString(dataTable.Rows[0]["AddressFrom"]);
		string s2 = Function.ToString(dataTable.Rows[0]["AddressTo"]);
		string s3 = Function.ToString(dataTable.Rows[0]["AddressCc"]);
		string s4 = Function.ToString(dataTable.Rows[0]["AddressBcc"]);
		string s5 = Function.ToString(dataTable.Rows[0]["Subject"]);
		string s6 = Function.ToString(dataTable.Rows[0]["BodyText"]);
		Function.ToString(dataTable.Rows[0]["MsgSuccess"]);
		s2 = GetTextFromSource(s2, source, ";");
		s = GetTextFromSource(s, source, ";");
		s3 = GetTextFromSource(s3, source, ";");
		s4 = GetTextFromSource(s4, source, ";");
		s5 = GetTextFromSource(s5, source, " ");
		s6 = GetTextFromSource(s6, source, "", replaceHtml: true);
		Dictionary<string, Stream> attachStream = new Dictionary<string, Stream>();
		MailImpl mailImpl = new MailImpl();
		mailImpl.AddressBcc = s4.Split(';').ToList();
		mailImpl.AddressCc = s3.Split(';').ToList();
		mailImpl.AddressFrom = s;
		mailImpl.AddressTo = s2.Split(';').ToList();
		mailImpl.Subject = s5;
		mailImpl.BodyText = s6;
		mailImpl.AttachStream = attachStream;
		return mailImpl;
	}

	public ReportDocument GetCrystalReport(System.Data.DataSet ds, DataTable source, string attPath, out string commandText, out string fileName)
	{
		commandText = "";
		fileName = "Attachment";
		if (ds.Tables.Count > 3)
		{
			DataTable dataTable = ds.Tables[3];
			if (dataTable.IsNotNull())
			{
				ReportDocument reportDocument = new ReportDocument();
				string filename = string.Concat(attPath + "\\", Path.GetFileName(Function.ToString(dataTable.Rows[0]["FilePath"])));
				reportDocument.Load(filename);
				fileName = GetTextFromSource(Function.ToString(dataTable.Rows[0]["FileAlias"]), source, "");
				try
				{
					reportDocument.Refresh();
					foreach (object table in reportDocument.ReportClientDocument.DatabaseController.Database.Tables)
					{
						if (table is CommandTable)
						{
							CommandTable commandTable = (CommandTable)table;
							commandText = commandTable.CommandText;
							break;
						}
					}
					if (ds.Tables.Count > 4)
					{
						DataTable dataTable2 = ds.Tables[4];
						foreach (object parameterField in reportDocument.DataDefinition.ParameterFields)
						{
							if (parameterField is ParameterFieldDefinition)
							{
								ParameterFieldDefinition parameterFieldDefinition = (ParameterFieldDefinition)parameterField;
								commandText = commandText.Replace(parameterFieldDefinition.FormulaName, Function.ParseInt(source.Rows[0][Function.ToString(dataTable2.Rows[0]["SourceField"])]).ToString());
							}
						}
					}
					return reportDocument;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", ex);
				}
			}
		}
		return null;
	}

	private string GetTextFromSource(string s, DataTable source, string joinstring, bool replaceHtml = false)
	{
		try
		{
			string text = s;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != '$' || i + 1 >= text.Length || text[i + 1] != '[')
				{
					continue;
				}
				string item = "";
				text = text.Substring(i + 2, text.Length - i - 2);
				for (int j = 0; j < text.Length && text[j] != ']'; j++)
				{
					item += text[j];
				}
				if (source.Columns.Contains(item))
				{
					string text2 = string.Join(joinstring, (from t in source.AsEnumerable()
						select Function.ToString(t[item])).Distinct().ToArray());
					if (replaceHtml)
					{
						text2 = text2.Replace(Environment.NewLine, "<br/>");
					}
					s = s.Replace("$[" + item + "]", text2);
				}
				else
				{
					Logging.Write("ERROR", "Source data does not contain column - " + item);
				}
				i = item.Length;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return s;
	}
}
