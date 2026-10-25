using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Service;

namespace Apzon.Services.Repository.Service;

public class ServiceContractRepository : BaseService<IServiceContract>, IServiceContract
{
	public ServiceContractRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(DataTable dts)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dts != null && dts.Rows.Count > 0)
			{
				if (Function.ParseDateTimes(dts.Rows[0]["fDate"]) != DateTime.MinValue)
				{
					list.Add(new SqlParameter("@StartDate", dts.Rows[0]["fDate"]));
				}
				if (Function.ParseDateTimes(dts.Rows[0]["tDate"]) != DateTime.MinValue)
				{
					list.Add(new SqlParameter("@EndDate", dts.Rows[0]["tDate"]));
				}
			}
			return ExecuteDataTable("APZ_ServiceContract_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDetail(int contractID)
	{
		try
		{
			string query = "APZ_ServiceContract_GetDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ContractID", contractID));
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public string Save(DataSet data)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dataTable = data.Tables[0];
				DataTable dataTable2 = data.Tables[1];
				int num;
				if (data.Tables.Count > 2 && data.Tables[2].Rows.Count > 0)
				{
					if (Function.ParseInt(data.Tables[0].Rows[0]["AtcEntry"]) == 0)
					{
						num = SaveFile();
						if (num > 0)
						{
							AddFile(data.Tables[2], num);
						}
					}
					else
					{
						num = Function.ParseInt(data.Tables[0].Rows[0]["AtcEntry"]);
					}
				}
				else
				{
					num = Function.ParseInt(data.Tables[0].Rows[0]["AtcEntry"]);
				}
				int num2 = Function.ParseInt((Function.ParseInt(dataTable.Rows[0]["ContractID"]) == 0) ? GetContractID().Rows[0]["ContractID"] : dataTable.Rows[0]["ContractID"]);
				string query = "APZ_ServiceContract_SaveDoc";
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter("@ContractID", num2),
					new SqlParameter("@CstmrCode", Function.RawText(dataTable.Rows[0]["CstmrCode"])),
					new SqlParameter("@CstmrName", Function.RawText(dataTable.Rows[0]["CstmrName"])),
					new SqlParameter("@CntctCode", Function.ParseInt(dataTable.Rows[0]["CntctCode"])),
					new SqlParameter("@Owner", Function.ParseInt(dataTable.Rows[0]["Owner"])),
					new SqlParameter("@Status", Function.RawText(dataTable.Rows[0]["Status"])),
					new SqlParameter("@CntrcTmplt", Function.RawText(dataTable.Rows[0]["CntrcTmplt"])),
					new SqlParameter("@CntrcType", Function.RawText(dataTable.Rows[0]["CntrcType"])),
					new SqlParameter("@Renewal", Function.RawText(dataTable.Rows[0]["Renewal"])),
					new SqlParameter("@RemindVal", Function.ParseInt(dataTable.Rows[0]["RemindVal"])),
					new SqlParameter("@RemindUnit", Function.RawText(dataTable.Rows[0]["RemindUnit"])),
					new SqlParameter("@StartDate", Function.ParseDateTimes(dataTable.Rows[0]["StartDate"])),
					new SqlParameter("@EndDate", Function.ParseDateTimes(dataTable.Rows[0]["EndDate"])),
					new SqlParameter("@ResponsVal", Function.ParseInt(dataTable.Rows[0]["ResponsVal"])),
					new SqlParameter("@AtcEntry", num),
					new SqlParameter("@ResponsUnt", Function.RawText(dataTable.Rows[0]["ResponsUnt"])),
					new SqlParameter("@Descriptio", Function.RawText(dataTable.Rows[0]["Descriptio"])),
					new SqlParameter("@MondayEnabled", Function.RawText(dataTable.Rows[0]["MonEnabled"])),
					new SqlParameter("@TuesdayEnabled", Function.RawText(dataTable.Rows[0]["TueEnabled"])),
					new SqlParameter("@WednesdayEnabled", Function.RawText(dataTable.Rows[0]["WedEnabled"])),
					new SqlParameter("@ThursdayEnabled", Function.RawText(dataTable.Rows[0]["ThuEnabled"])),
					new SqlParameter("@FridayEnabled", Function.RawText(dataTable.Rows[0]["FriEnabled"])),
					new SqlParameter("@SaturdayEnabled", Function.RawText(dataTable.Rows[0]["SatEnabled"])),
					new SqlParameter("@SundayEnabled", Function.RawText(dataTable.Rows[0]["SunEnabled"])),
					new SqlParameter("@MonStart", Function.RawText(dataTable.Rows[0]["MonStart"])),
					new SqlParameter("@MonEnd", Function.RawText(dataTable.Rows[0]["MonEnd"])),
					new SqlParameter("@TueStart", Function.RawText(dataTable.Rows[0]["TueStart"])),
					new SqlParameter("@TueEnd", Function.RawText(dataTable.Rows[0]["TueEnd"])),
					new SqlParameter("@assignee", Function.RawText(dataTable.Rows[0]["WedStart"])),
					new SqlParameter("@WedEnd", Function.RawText(dataTable.Rows[0]["WedEnd"])),
					new SqlParameter("@ThuStart", Function.RawText(dataTable.Rows[0]["ThuStart"])),
					new SqlParameter("@ThuEnd", Function.RawText(dataTable.Rows[0]["ThuEnd"])),
					new SqlParameter("@FriStart", Function.RawText(dataTable.Rows[0]["FriStart"])),
					new SqlParameter("@FriEnd", Function.RawText(dataTable.Rows[0]["FriEnd"])),
					new SqlParameter("@SatStart", Function.RawText(dataTable.Rows[0]["SatStart"])),
					new SqlParameter("@SatEnd", Function.RawText(dataTable.Rows[0]["SatEnd"])),
					new SqlParameter("@SunStrart", Function.RawText(dataTable.Rows[0]["SunStrart"])),
					new SqlParameter("@SunEnd", Function.RawText(dataTable.Rows[0]["SunEnd"])),
					new SqlParameter("@IncludeParts", Function.RawText(dataTable.Rows[0]["InclParts"])),
					new SqlParameter("@IncludeLabor", Function.RawText(dataTable.Rows[0]["InclWork"])),
					new SqlParameter("@IncludeTravel", Function.RawText(dataTable.Rows[0]["InclTravel"])),
					new SqlParameter("@Remarks1", Function.RawText(dataTable.Rows[0]["Remarks1"])),
					new SqlParameter("@Remarks2", Function.RawText(dataTable.Rows[0]["Remarks2"])),
					new SqlParameter("@IncludeHolidays", Function.RawText(dataTable.Rows[0]["InclHldays"])),
					new SqlParameter("@SrvcType", Function.RawText(dataTable.Rows[0]["SrvcType"])),
					new SqlParameter("@ResponseU", Function.RawText(dataTable.Rows[0]["ResponseU"])),
					new SqlParameter("@ResponseV", Function.ParseInt(dataTable.Rows[0]["ResponseV"])),
					new SqlParameter("@TermDate", Function.ParseDateTimes(dataTable.Rows[0]["TermDate"]))
				};
				DataTable dataTable3 = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
				if (dataTable3 == null || dataTable3.Rows.Count == 0 || Function.ParseInt(dataTable.Rows[0]["ContractID"]) <= 0)
				{
					dbTransaction.Rollback();
					return "error!";
				}
				dataTable.Rows[0]["ContractID"] = dataTable3.Rows[0]["id"];
				query = "APZ_ServiceCall_DeleteLines";
				SqlParameter sqlParameter = new SqlParameter("@ContractID", Function.ParseInt(dataTable.Rows[0]["ContractID"]));
				int num3 = ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameter);
				if (num3 <= 0)
				{
					dbTransaction.Rollback();
					return "error";
				}
				num3 = 1;
				query = "APZ_ServiceContract_SaveLines";
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					list = new List<SqlParameter>
					{
						new SqlParameter("@ContractID", Function.ParseInt(dataTable2.Rows[i]["ContractID"])),
						new SqlParameter("@ManufSN", Function.ParseInt(dataTable2.Rows[i]["ManufSN"])),
						new SqlParameter("@InternalSN", Function.ParseInt(dataTable2.Rows[i]["InternalSN"])),
						new SqlParameter("@ItemCode", Function.RawText(dataTable2.Rows[i]["ItemCode"])),
						new SqlParameter("@ItemName", Function.RawText(dataTable2.Rows[i]["ItemName"])),
						new SqlParameter("@ItemGroup", Function.RawText(dataTable2.Rows[i]["ItemGroup"])),
						new SqlParameter("@StartDate", Function.ParseDateTimes(dataTable2.Rows[i]["StartDate"])),
						new SqlParameter("@EndDate", Function.ParseDateTimes(dataTable2.Rows[i]["EndDate"])),
						new SqlParameter("@TermDate", Function.ParseDateTimes(dataTable2.Rows[i]["TermDate"]))
					};
					num3 = ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray());
					if (num3 <= 0)
					{
						break;
					}
				}
				if (num3 > 0)
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

	public DataTable GetContractID()
	{
		try
		{
			return ExecuteDataTable("APZ_ServiceContract_GetContractID", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	private int SaveFile()
	{
		try
		{
			string query = "select max(AbsEntry) from oatc";
			object obj = ExecuteScalar(query, CommandType.Text);
			query = "insert into oatc (absEntry) values (" + (Function.ParseInt(obj) + 1) + ") ";
			if (ExecuteNonQuery(query, CommandType.Text) > 0)
			{
				return Function.ParseInt(obj) + 1;
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	private string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE absEntry = @absEntry";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) >= 0)
			{
				query = "delete from OATC WHERE absEntry = @absEntry ";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) >= 0)
				{
					return "";
				}
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private string AddFile(DataTable dt, int absEntry)
	{
		using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
		try
		{
			string empty = string.Empty;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				empty = "INSERT INTO [dbo].[ATC1]\r\n                               ([AbsEntry]\r\n                               ,[Line]\r\n                               ,[srcPath]\r\n                               ,[trgtPath]\r\n                               ,[FileName]\r\n                               ,[FileExt]\r\n                               ,[Date])                             \r\n                         VALUES (";
				empty += "@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", absEntry));
				empty += ",@Line ";
				list.Add(new SqlParameter("@Line", i + 1));
				empty += ", @srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(dt.Rows[i]["srcPath"])));
				empty += ",@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(dt.Rows[i]["trgtPath"])));
				empty += ",@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(dt.Rows[i]["FileName"])));
				empty += ", @FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(dt.Rows[i]["FileExt"])));
				empty += ",@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(dt.Rows[i]["Date"])));
				empty += " )";
				if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) <= 0)
				{
					dbTransaction.Rollback();
					return "eror";
				}
			}
			dbTransaction.Commit();
			return string.Empty;
		}
		catch (Exception ex)
		{
			dbTransaction.Rollback();
			return ex.Message;
		}
	}
}
