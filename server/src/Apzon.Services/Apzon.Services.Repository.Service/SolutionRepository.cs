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
using Apzon.Entities.Models.Service;
using Apzon.Services.Interfaces.Service;

namespace Apzon.Services.Repository.Service;

public class SolutionRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), Isolution
{
	private string TempDoc = "#OSLT";

	private const string Linetemp1 = "#ATC1";

	public DataTable Search(int sltCode, string itemCode, int status)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@SltCode", sltCode),
				new SqlParameter("@ItemCode", itemCode),
				new SqlParameter("@Status", status)
			};
			return ExecuteDataTable("apz_Service_Solution_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLine(int atcEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@AbsEntry", atcEntry)
			};
			return ExecuteDataTable("apz_Service_Solution_GetLine", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLine  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSolutionStatuses()
	{
		try
		{
			return ExecuteDataTable("SELECT \"Number\",\"Name\" FROM OSST;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSolutionStatuses  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetOwner()
	{
		try
		{
			return ExecuteDataTable("SELECT USERID,USER_CODE,U_NAME FROM ousr;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSolutionStatuses  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string Save(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oKnowledgeBaseSolutions, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(ref ret, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			DataTable dataTable = ds.Tables[0];
			DataTable tableStruct = GetTableStruct("OSLT");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, TempDoc);
			BulkCopy(dataTable, TempDoc);
			DataTable dataTable2 = ds.Tables[1];
			DataTable tableStruct2 = GetTableStruct("ATC1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#ATC1");
			BulkCopy(dataTable2, "#ATC1");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable3 = ExecuteDataTable("APZ_Service_Solution_SaveDoc", CommandType.StoredProcedure, sqlParameter);
			if (dataTable3 != null && dataTable3.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable3.Rows[0][1])))
				{
					objId = Function.ToString(dataTable3.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable3.Rows[0][0])}-{Function.ToString(dataTable3.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string DeleteDocs(int id)
	{
		try
		{
			return Process(ApzObjectTypes.oKnowledgeBaseSolutions, -1, Enums.TransactionType.D, new DataSet(), Function.ToString(id)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@id", objId);
			DataTable dataTable = ExecuteDataTable("APZ_Solution_DeleteDocs", CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new HttpResult(Enums.MessageCode.Success, "", "Delete success!");
			}
			return new HttpResult(Enums.MessageCode.Success, "", "Delete Error!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable FindDataNextLast(int id, int type)
	{
		try
		{
			string query = "APZ_Solution_FindNextLastFrist";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@type", type));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "FindDataNextLast  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxSltCode()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT MAX(\"SltCode\") FROM oslt;", CommandType.Text);
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]) + 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 1;
	}

	public string Save(KnowledgeBaseSolutionsImpl doc)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (doc.AttachmentFiles != null)
				{
					string text = SaveAttachmentFiles(doc.AttachmentFiles, doc.AtcEntry);
					if (!string.IsNullOrWhiteSpace(text))
					{
						return text;
					}
				}
				int num = 0;
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter("@sltCode", doc.SltCode),
					new SqlParameter("@itemCode", Function.RawText(doc.ItemCode)),
					new SqlParameter("@statusNum", doc.StatusNum),
					new SqlParameter("@subject", Function.RawText(doc.Subject)),
					new SqlParameter("@symptom", Function.RawText(doc.Symptom)),
					new SqlParameter("@Cause", Function.RawText(doc.Cause)),
					new SqlParameter("@descriptio", Function.RawText(doc.Descriptio)),
					new SqlParameter("@attachment", Function.RawText(doc.Attachment)),
					new SqlParameter("@atcEntry", Function.RawText(doc.AtcEntry))
				};
				DataTable dataTable = ExecuteDataTable("APZ_Service_Solution_SaveDoc", CommandType.StoredProcedure, list.ToArray());
				if (dataTable == null || dataTable.Rows.Count == 0 || Function.ParseInt(doc.SltCode) <= 0)
				{
					dbTransaction.Rollback();
					return "error!";
				}
				if (num > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "error";
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private string SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, int absEntry)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (lstFiles != null && lstFiles.Count > 0)
				{
					if (!CheckFile(absEntry))
					{
						string text = SaveFile(absEntry);
						if (!string.IsNullOrEmpty(text))
						{
							dbTransaction.Rollback();
							return text;
						}
						DeleteFile(absEntry);
						foreach (AttachmentFile_Impl lstFile in lstFiles)
						{
							string text2 = SaveLineFile(lstFile);
							if (!string.IsNullOrEmpty(text2))
							{
								dbTransaction.Rollback();
								return text2;
							}
						}
					}
					else
					{
						DeleteFile(absEntry);
						foreach (AttachmentFile_Impl lstFile2 in lstFiles)
						{
							string text3 = SaveLineFile(lstFile2);
							if (!string.IsNullOrEmpty(text3))
							{
								dbTransaction.Rollback();
								return text3;
							}
						}
					}
				}
				else
				{
					string text4 = DeleteFile(absEntry);
					if (!string.IsNullOrEmpty(text4))
					{
						dbTransaction.Rollback();
						return text4;
					}
				}
				dbTransaction.Commit();
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private bool CheckFile(int absEntry)
	{
		try
		{
			string query = "SELECT AbsEntry FROM oatc  WHERE\r\n                        AbsEntry = " + absEntry;
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

	public string SaveFile(int absEntry)
	{
		try
		{
			string query = "insert into oatc (absEntry) values (" + absEntry + ")";
			if (ExecuteNonQuery(query, CommandType.Text) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE absEntry = " + absEntry;
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

	public string SaveLineFile(AttachmentFile_Impl line)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text;
			if (!CheckLineFile(line.AbsEntry, line.Line))
			{
				text = "INSERT INTO [dbo].[ATC1]\r\n                               ([AbsEntry]\r\n                               ,[Line]\r\n                               ,[srcPath]\r\n                               ,[trgtPath]\r\n                               ,[FileName]\r\n                               ,[FileExt]\r\n                               ,[Date]\r\n                               ,[UsrID]\r\n                               ,[Copied]\r\n                               ,[Override]\r\n                               ,[subPath])\r\n                         VALUES (";
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
				text += " )";
			}
			else
			{
				text = "update [dbo].[ATC1] set ";
				text += "srcPath=@srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				text += ",trgtPath=@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				text += ",FileName=@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				text += ", FileExt=@FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				text += ",Date=@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				text += ",UsrID=@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				text += ",Copied=@Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				text += ",Override=@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				text += ", subPath=@subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				text += " where AbsEntry=@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				text += " and Line=@Line ";
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

	private bool CheckLineFile(int absEntry, int line)
	{
		try
		{
			string query = "SELECT AbsEntry FROM atc1  WHERE\r\n                        AbsEntry = " + absEntry + " and  Line = " + line;
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
}
