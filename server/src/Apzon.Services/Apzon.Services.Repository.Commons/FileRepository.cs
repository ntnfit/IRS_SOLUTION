using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class FileRepository : BaseService<DataSet>, IFile
{
	public FileRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult DMSSaveAttachment(DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					CreateTempTable(dt, "#APZ_D_UUPIM");
					BulkCopy(dt, "#APZ_D_UUPIM");
					DataTable dataTable = ExecuteDataTable("APZ_D_Attachment_Save", CommandType.StoredProcedure);
					if (dataTable.IsNotNull())
					{
						if (Function.ParseInt(dataTable.Rows[0]["Code"]) != 200)
						{
							return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0]["Message"]));
						}
						dbTransaction.Commit();
						return new HttpResult(Enums.MessageCode.Success);
					}
					return new HttpResult(Enums.MessageCode.Error, "Save error");
				}
			}
			return new HttpResult(Enums.MessageCode.Error, "Notfound");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, Dictionary<string, object> code, string table)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				int num = GetAbsEntryByCode(code, table);
				if (lstFiles != null && lstFiles.Any())
				{
					if (num == 0)
					{
						num = Function.ParseInt(AddFile());
					}
					DataTable tableStruct = GetTableStruct("ATC1");
					tableStruct.Rows.Clear();
					foreach (AttachmentFile_Impl lstFile in lstFiles)
					{
						DataRow dataRow = tableStruct.NewRow();
						dataRow["Line"] = lstFile.Line;
						dataRow["srcPath"] = lstFile.srcPath;
						dataRow["trgtPath"] = lstFile.trgtPath;
						dataRow["FileName"] = lstFile.FileName;
						dataRow["FileExt"] = lstFile.FileExt;
						dataRow["Date"] = lstFile.Date;
						dataRow["UsrID"] = lstFile.UsrID;
						dataRow["Copied"] = lstFile.Copied;
						dataRow["Override"] = lstFile.Override;
						dataRow["subPath"] = lstFile.subPath;
						dataRow["FreeText"] = lstFile.FreeText;
						if (lstFile.UserFields != null && lstFile.UserFields.Any())
						{
							foreach (UserFieldsImpl userField in lstFile.UserFields)
							{
								try
								{
									dataRow[userField.Name] = userField.Value;
								}
								catch (Exception)
								{
								}
							}
						}
						tableStruct.Rows.Add(dataRow);
					}
					CreateTempTable(tableStruct, "#dtFile");
					BulkCopy(tableStruct, "#dtFile");
					ExecuteNonQuery("APZ_Attachment_Process", CommandType.StoredProcedure, new SqlParameter("@absEntry", num));
				}
				else if (num > 0)
				{
					ExecuteNonQuery("delete from OATC where \"AbsEntry\"=@absEntry;\r\n                                                    delete from ATC1 where \"AbsEntry\"=@absEntry;", CommandType.Text, new SqlParameter("@absEntry", num));
				}
				dbTransaction.Commit();
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	public virtual string AddFile()
	{
		try
		{
			return Function.ToString(ExecuteScalar("insert into OATC (absEntry)  (select isnull(max(absEntry),0)+1 From OATC);\r\n                       Update ONNM set AutoKey=(select isnull((select max(AbsEntry)+1 from OATC),0)) where ObjectCode='221';\r\n                       select isnull((select max(AbsEntry) from OATC),0)", CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private int GetAbsEntryByDocEntry(int docEntry, ApzObjectTypes objType)
	{
		try
		{
			string tableByApzObjectType = Function.GetTableByApzObjectType(objType);
			string columnKeyByApzObjectType = Function.GetColumnKeyByApzObjectType(objType);
			string query = string.Format("SELECT \"AtcEntry\" FROM {0}  WHERE  \"{2}\"= {1};", tableByApzObjectType, Function.ToString(docEntry), columnKeyByApzObjectType);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	private string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE \"AbsEntry\" = " + absEntry + ";";
			if (ExecuteNonQuery(query, CommandType.Text) >= 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private bool CheckLineFile(int absEntry, int line)
	{
		try
		{
			string query = "SELECT \"AbsEntry\" FROM atc1  WHERE\r\n                        \"AbsEntry\" = " + absEntry + " and  \"Line\" = " + line + ";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public string SaveLineFile(AttachmentFile_Impl line)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text;
			if (!CheckLineFile(line.AbsEntry, line.Line))
			{
				text = "INSERT INTO ATC1\r\n                               (\"AbsEntry\"\r\n                               ,\"Line\"\r\n                               ,\"srcPath\"\r\n                               ,\"trgtPath\"\r\n                               ,\"FileName\"\r\n                               ,\"FileExt\"\r\n                               ,\"Date\"\r\n                               ,\"UsrID\"\r\n                               ,\"Copied\"\r\n                               ,\"Override\"\r\n                               ,\"subPath\")\r\n                         VALUES (";
				text += "@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				text += ",@Line ";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
				text += ", @srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				text += ",@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				text += ",@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				text += ", @FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				text += ",@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				text += ",@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				text += ", @Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				text += ",@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				text += ", @subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				text += " );";
			}
			else
			{
				text = "update ATC1 set ";
				text += "\"srcPath\"=@srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				text += ",\"trgtPath\"=@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				text += ",\"FileName\"=@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				text += ", \"FileExt\"=@FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				text += ",\"Date\"=@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				text += ",\"UsrID\"=@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				text += ",\"Copied\"=@Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				text += ",\"Override\"=@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				text += ", \"subPath\"=@subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				text += " where \"AbsEntry\"=@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				text += " and \"Line\"=@Line; ";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
			}
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private int GetAbsEntryByCode(Dictionary<string, object> codes, string table)
	{
		try
		{
			string text = "SELECT \"AtcEntry\" FROM " + table + "  WHERE  1=1 ";
			SqlParameter[] array = new SqlParameter[codes.Count];
			int num = 0;
			foreach (KeyValuePair<string, object> code in codes)
			{
				string[] array2 = code.Key.Split(',');
				if (array2.Length <= 1)
				{
					continue;
				}
				string text2 = array2[1];
				if (!(text2 == "int"))
				{
					if (text2 == "string")
					{
						array[num] = new SqlParameter("@" + array2[0], Function.ToString(code.Value));
					}
				}
				else
				{
					array[num] = new SqlParameter("@" + array2[0], Function.ParseInt(code.Value));
				}
				text = text + " AND \"" + array2[0] + "\"=@" + array2[0];
				num++;
			}
			return Function.ParseInt(ExecuteScalar(text + ";", CommandType.Text, array));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString() + " " + table);
			return 0;
		}
	}

	public void UpdateTargetPath(List<AttachmentFile_Impl> lstFiles, int absEntry)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				foreach (AttachmentFile_Impl lstFile in lstFiles)
				{
					ExecuteNonQuery(GetQueryUpdateTrgPath(), CommandType.Text, new SqlParameter("@trgtPath", lstFile.trgtPath), new SqlParameter("@absEntry", absEntry), new SqlParameter("@srcPath", lstFile.srcPath), new SqlParameter("@fileName", lstFile.FileName), new SqlParameter("@fileExt", lstFile.FileExt));
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	protected virtual string GetQueryUpdateTrgPath()
	{
		return "update ATC1 set \"trgtPath\"=@trgtPath where \"AbsEntry\"=@absEntry and cast(\"srcPath\" as nvarchar(max))=@srcPath and \"FileName\"=@fileName and \"FileExt\"=@fileExt;";
	}
}
