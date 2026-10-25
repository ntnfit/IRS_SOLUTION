using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Service;

namespace Apzon.Services.Repository.Service;

public class ServiceCallRepository : BaseService<IServiceCall>, IServiceCall
{
	public ServiceCallRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetSeries()
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ObjectCode", ApzObjectTypes.oServiceCalls);
			return ExecuteDataTable("APZ_ServiceCall_GetSeries", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetNewAbsEntry()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetNewAbsEntry", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAttachment(int callID)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@callID", callID);
			return ExecuteDataTable("APZ_ServiceCall_GetAttachment", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAdressID(string cardCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cardCode", cardCode);
			return ExecuteDataTable("APZ_ServiceCall_GetAdressID", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAddrType(string addressID)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@addressID", addressID);
			return ExecuteDataTable("APZ_ServiceCall_GetAddrType", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetContactPerson()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetContactPerson", CommandType.StoredProcedure, new SqlParameter("@cardCode", null));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetContactPerson(string cardCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cardCode", cardCode);
			return ExecuteDataTable("APZ_ServiceCall_GetContactPerson", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStreet(string cardCode, string adressID)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@cardCode", cardCode),
				new SqlParameter("@adressID", adressID)
			};
			return ExecuteDataTable("APZ_ServiceCall_LoadStreet", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCallStatus()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetCallStatus", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetOrigin()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetOrigin", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetProblemType()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetProblemType", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetProblemSubType()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetProblemSubType", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCallType()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetCallType", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAssignee()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetAssignee", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLocation()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetLocation", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCountries()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetCountries", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetActivities(int callID, int docNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@callID", callID),
				new SqlParameter("@docNum", docNum)
			};
			return ExecuteDataTable("APZ_ServiceCall_GetActivities", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetHistory(int callID)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@callID", callID);
			return ExecuteDataTable("APZ_ServiceCall_GetHistory", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetSolutions(int callID, int sltCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@callID", callID),
				new SqlParameter("@sltCode", sltCode)
			};
			return ExecuteDataTable("APZ_ServiceCall_GetSolutions", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetState(string countryCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Country", countryCode);
			return ExecuteDataTable("APZ_ServiceCall_GetState", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDoc(DataTable dts)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(Function.ToString(dts.Rows[0]["fromDate"])))
			{
				list.Add(new SqlParameter("@fromDate", dts.Rows[0]["fromDate"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dts.Rows[0]["fromDate"])))
			{
				list.Add(new SqlParameter("@toDate", dts.Rows[0]["toDate"]));
			}
			list.Add(new SqlParameter("@priority", Function.ToString(dts.Rows[0]["priority"])));
			list.Add(new SqlParameter("@customer", Function.ToString(dts.Rows[0]["customer"])));
			list.Add(new SqlParameter("@status", Function.ToString(dts.Rows[0]["status"])));
			return ExecuteDataTable("APZ_ServiceCall_GetDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLineGeneral(int callID)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@callID", callID);
			return ExecuteDataTable("APZ_ServiceCall_GetLineGeneral", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocNum(int series)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@series", series),
				new SqlParameter("@ObjectCode", ApzObjectTypes.oServiceCalls)
			};
			return ExecuteDataTable("APZ_ServiceCall_GetDocNum", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCallID()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetCallID", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListItemGroup()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceCall_GetListItemGroup", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemsInfo(string itemCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			return ExecuteDataTable("APZ_ItemMaster_GetOITM", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemsInfo()
	{
		try
		{
			return ExecuteDataTable("APZ_ItemMaster_GetOITM", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string Save(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment)
	{
		try
		{
			if (attachment.Count > 0)
			{
				string text = SaveAttachmentFiles(attachment, attachment[0].AbsEntry);
				if (!string.IsNullOrWhiteSpace(text))
				{
					return text;
				}
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_ServiceCall_SaveDoc";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@callId", Function.ParseInt(doc["callID"])));
				list.Add(new SqlParameter("@customer", Function.ToString(doc["customer"])));
				list.Add(new SqlParameter("@customerName", Function.ToString(doc["custmrName"])));
				list.Add(new SqlParameter("@contctCode", Function.ParseInt(doc["contctCode"])));
				list.Add(new SqlParameter("@NumAtCard", Function.ToString(doc["NumAtCard"])));
				list.Add(new SqlParameter("@manusSN", Function.ToString(doc["manufSN"])));
				list.Add(new SqlParameter("@internalSN", Function.ToString(doc["internalSN"])));
				list.Add(new SqlParameter("@itemCode", Function.ToString(doc["itemCode"])));
				list.Add(new SqlParameter("@itemName", Function.ToString(doc["itemName"])));
				list.Add(new SqlParameter("@itemGroup", Function.ParseInt(doc["itemGroup"])));
				list.Add(new SqlParameter("@series", Function.ParseInt(doc["Series"])));
				list.Add(new SqlParameter("@docNum", Function.ParseInt(doc["DocNum"])));
				list.Add(new SqlParameter("@status", Function.ParseInt(doc["status"])));
				list.Add(new SqlParameter("@priority", Function.ParseChar(doc["priority"])));
				list.Add(new SqlParameter("@createDate", Function.ParseDateTime(doc["createDate"])));
				list.Add(new SqlParameter("@createTime", Function.ParseInt(doc["createTime"])));
				list.Add(new SqlParameter("@closedate", Function.ParseDateTime(doc["closeDate"])));
				list.Add(new SqlParameter("@closetime", Function.ParseInt(doc["closeTime"])));
				list.Add(new SqlParameter("@contractID", Function.ParseInt(doc["contractID"])));
				list.Add(new SqlParameter("@cntrctDate", Function.ParseDateTime(doc["cntrctDate"])));
				list.Add(new SqlParameter("@subject", Function.ToString(doc["subject"])));
				list.Add(new SqlParameter("@origin", Function.ParseInt(line.Rows[0]["origin"])));
				list.Add(new SqlParameter("@problemtype", Function.ParseInt(line.Rows[0]["problemTyp"])));
				list.Add(new SqlParameter("@prosubtype", Function.ParseInt(line.Rows[0]["ProSubType"])));
				list.Add(new SqlParameter("@calltype", Function.ParseInt(line.Rows[0]["callType"])));
				list.Add(new SqlParameter("@technician", Function.ParseInt(line.Rows[0]["technician"])));
				list.Add(new SqlParameter("@isqueue", Function.ParseInt(line.Rows[0]["isQueue"])));
				list.Add(new SqlParameter("@assignee", Function.ParseInt(line.Rows[0]["assignee"])));
				list.Add(new SqlParameter("@queue", Function.ToString(line.Rows[0]["Queue"])));
				list.Add(new SqlParameter("@respByDate", Function.ParseDateTime(line.Rows[0]["respByDate"])));
				list.Add(new SqlParameter("@respBytime", Function.ParseInt(line.Rows[0]["respByTime"])));
				list.Add(new SqlParameter("@respOnDate", Function.ParseDateTime(line.Rows[0]["respOnDate"])));
				list.Add(new SqlParameter("@respOntime", Function.ParseInt(line.Rows[0]["respOnTime"])));
				list.Add(new SqlParameter("@resolDate", Function.ParseDateTime(line.Rows[0]["resolDate"])));
				list.Add(new SqlParameter("@resoltime", Function.ParseInt(line.Rows[0]["resolTime"])));
				list.Add(new SqlParameter("@resolOnDate", Function.ParseDateTime(line.Rows[0]["resolOnDat"])));
				list.Add(new SqlParameter("@resolOntime", Function.ParseInt(line.Rows[0]["resolOnTim"])));
				list.Add(new SqlParameter("@startDate", Function.ParseDateTime(line.Rows[0]["StartDate"])));
				list.Add(new SqlParameter("@starttime", Function.ParseInt(line.Rows[0]["StartTime"])));
				list.Add(new SqlParameter("@endDate", Function.ParseDateTime(line.Rows[0]["EndDate"])));
				list.Add(new SqlParameter("@endtime", Function.ParseInt(line.Rows[0]["EndTime"])));
				list.Add(new SqlParameter("@duration", Function.ParseInt(line.Rows[0]["Duration"])));
				list.Add(new SqlParameter("@remider", Function.ParseChar(line.Rows[0]["Reminder"])));
				list.Add(new SqlParameter("@remqty", Function.ParseInt(line.Rows[0]["RemQty"])));
				list.Add(new SqlParameter("@displlncal", Function.ParseChar(line.Rows[0]["DisplInCal"])));
				list.Add(new SqlParameter("@location", Function.ParseInt(line.Rows[0]["Location"])));
				list.Add(new SqlParameter("@street", Function.ToString(line.Rows[0]["Street"])));
				list.Add(new SqlParameter("@city", Function.ToString(line.Rows[0]["City"])));
				list.Add(new SqlParameter("@state", Function.ToString(line.Rows[0]["State"])));
				list.Add(new SqlParameter("@country", Function.ToString(line.Rows[0]["Country"])));
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
				if (dataTable == null || dataTable.Rows.Count == 0 || Function.ParseInt(doc["callID"]) <= 0)
				{
					dbTransaction.Rollback();
					return "error!";
				}
				doc["callID"] = dataTable.Rows[0]["id"];
				query = "APZ_SERVICECALL_DELETEACTIVITY";
				SqlParameter sqlParameter = new SqlParameter("@ServicecallID", Function.ParseInt(doc["callID"]));
				int num = ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameter);
				if (num <= 0)
				{
					dbTransaction.Rollback();
					return "error";
				}
				query = "APZ_ServiceCall_DeleteSolution";
				sqlParameter = new SqlParameter("@ServicecallID", Function.ParseInt(doc["callID"]));
				num = ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameter);
				if (num <= 0)
				{
					dbTransaction.Rollback();
					return "error";
				}
				num = 1;
				query = "APZ_SERVICECALL_SAVEACTIVITY";
				for (int i = 0; i < activiti.Rows.Count; i++)
				{
					list = new List<SqlParameter>
					{
						new SqlParameter("@SrvcCallId", Function.ParseInt(doc["callID"])),
						new SqlParameter("@Line", i),
						new SqlParameter("@clgID", Function.ParseInt(activiti.Rows[i]["ClgCode"])),
						new SqlParameter("@ObjectType", ApzObjectTypes.oServiceCalls),
						new SqlParameter("@CreateDate", Function.ParseDateTime(Function.ToString(activiti.Rows[i]["ActivityDate"]))),
						new SqlParameter("@UpdateDate", DateTime.Now)
					};
					num = ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray());
					if (num <= 0)
					{
						break;
					}
				}
				if (num <= 0)
				{
					return "error";
				}
				num = 1;
				query = "APZ_ServiceCall_SaveSolutions";
				for (int j = 0; j < solution.Rows.Count; j++)
				{
					list = new List<SqlParameter>
					{
						new SqlParameter("@srvcCallID", Function.ParseInt(doc["callID"])),
						new SqlParameter("@line", j),
						new SqlParameter("@solutionID", Function.ParseInt(solution.Rows[j]["SltCode"])),
						new SqlParameter("@objType", ApzObjectTypes.oServiceCalls),
						new SqlParameter("@createDate", Function.ParseDateTime(Function.ToString(solution.Rows[j]["DateCreate"]))),
						new SqlParameter("@updateDate", DateTime.Now)
					};
					num = ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray());
					if (num <= 0)
					{
						break;
					}
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
				return string.Empty;
			}
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
