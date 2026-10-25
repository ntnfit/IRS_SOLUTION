using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class ScheduleRepository : BaseService<DataSet>, ISchedule
{
	private const string Doctemp = "#tempdoc";

	private const string Linetemp = "#templine";

	public ScheduleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SP_APZ_Search(string scheduleNo, DateTime fDate, DateTime tDate, string status)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@ID", Function.ToString(scheduleNo)),
				new SqlParameter("@fDate", fDate),
				new SqlParameter("@tDate", tDate),
				new SqlParameter("@Status", Function.ToString(status))
			};
			return ExecuteDataTable("SP_APZ_SearchSchedule", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SP_APZ_GetScheduling(string scheduleNo)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@PSCode", Function.ToString(scheduleNo))
			};
			return ExecuteDataTable("SP_APZ_GetScheduling", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_GetScheduling  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckPOInShift(string itemCode, DateTime date, string shift, string psCode)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("select ProductionOrder from apz_PRS1 where Item = '" + itemCode + "' and WorkDay = '" + date.ToString() + "' and Shift = '" + shift + "' and Type = 2 and PSCode = '" + psCode + "' ");
			return ExecuteDataTable(stringBuilder.ToString(), CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_GetScheduling  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SP_APZ_GetHoliDay(DateTime fDate, DateTime tDate)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@fDate", fDate),
				new SqlParameter("@tDate", tDate)
			};
			return ExecuteDataTable("SP_APZ_GetHoliDay", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_GetHoliDay  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxDocEntry(string tableName)
	{
		try
		{
			return GetUnitOfWork().MRP.GetMaxDocEntry(tableName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocEntry  FAILED ", ex.Message);
			return 1;
		}
	}

	public DataTable SP_APZ_RunSchedule(string mrpNo)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@mrpNo", Function.ToString(mrpNo)));
			return ExecuteDataTable("SP_APZ_RunSchedule", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_RunSchedule  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetScheduleGenPO(string scheduleNo)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("select * from apz_PRS1 where PSCode = '" + scheduleNo + "' and ISNULL(ProductionOrder, '') <> '' and Type = 2");
			return ExecuteDataTable(stringBuilder.ToString(), CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetScheduleGenPO  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet ds, Enums.TransactionType tranType, string objId)
	{
		try
		{
			return Process(ApzObjectTypes.oSchedule, -1, tranType, ds, objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return new HttpResult(Enums.MessageCode.Error, "Could not delete schedule");
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[1] == null || ds.Tables[1].Rows.Count == 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Data save could not null");
		}
		CreateTempTable(ds.Tables[0], "#tempdoc");
		BulkCopy(ds.Tables[0], "#tempdoc");
		CreateTempTable(ds.Tables[1], "#templine");
		BulkCopy(ds.Tables[1], "#templine");
		SqlParameter sqlParameter = new SqlParameter("@type", tranType.ToString());
		DataTable dataTable = ExecuteDataTable("APZ_Schedule_Save", CommandType.StoredProcedure, sqlParameter);
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
			{
				objId = Function.ToString(dataTable.Rows[0][0]);
				objNum = objId;
				return new HttpResult(Enums.MessageCode.Success, "", objId);
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error" : "Update error");
	}

	public string CheckInStock(string objType, int docEntry)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("SP_SYS_CheckInStock");
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@ObjType", Function.ToString(objType)),
				new SqlParameter("@DocEntry", Function.ParseInt(docEntry))
			};
			DataTable dataTable = ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0]["ErrInfo"]);
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckInStock  FAILED ", ex.Message);
			return null;
		}
	}

	public DataSet GetDataStructureGenProduction()
	{
		try
		{
			return ExecuteData("APZ_GenProductionFromSchedule_GetStructure", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetStructureGenPurchase()
	{
		try
		{
			return ExecuteDataTable("APZ_GenPurchaseFromSchedule_GetStructure", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStructureGenTransfer()
	{
		try
		{
			return ExecuteDataTable("APZ_GenTransferFromSchedule_GetStructure", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataGenProduction(string id, string type, string inputType)
	{
		try
		{
			return ExecuteDataTable("APZ_GenPOFromSchedule", CommandType.StoredProcedure, new SqlParameter("@scheduleID", id), new SqlParameter("@type", type), new SqlParameter("@inputType", inputType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_GenPOFromSchedule  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataProductionOrder(string itemCode, string version, string sourceType, int sourceId, int sourceLine)
	{
		try
		{
			return ExecuteDataTable("APZ_Scheduling_GetDataProductionOrder", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@version", version), new SqlParameter("@inputType", sourceType), new SqlParameter("@sourceId", sourceId), new SqlParameter("@line", sourceLine));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string UpdateStatus(ProductionOder_Impl_OWOR doc)
	{
		try
		{
			if (doc == null)
			{
				return "Data must not null!";
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<SqlParameter> list = new List<SqlParameter>();
			if (GetUnitOfWork().ProductionOrder.Exists(doc.DocEntry))
			{
				stringBuilder.Append("UPDATE dbo.OWOR SET ");
				if (doc.Status.Equals("C") || doc.Status.Equals("L"))
				{
					stringBuilder.Append(" CloseDate = @CloseDate, ");
					list.Add(new SqlParameter("@CloseDate", DateTime.Now));
				}
				stringBuilder.Append(" Status= @Status ");
				if (doc.UserFields != null && doc.UserFields.Count > 0)
				{
					for (int i = 0; i < doc.UserFields.Count; i++)
					{
						stringBuilder.Append("," + doc.UserFields[i].Name + "=@" + doc.UserFields[i].Name);
					}
				}
				stringBuilder.Append(" WHERE DocEntry = @DocEntry");
				list.Add(new SqlParameter("@Status", Function.ToString(doc.Status)));
				list.Add(new SqlParameter("@DocEntry", Function.ParseInt(doc.DocEntry)));
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray()) <= 0)
				{
					dbTransaction.Rollback();
					return "Update ERROR";
				}
				dbTransaction.Commit();
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable SP_APZ_ProductionGetCheck(string barcode)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("SP_APZ_ProductionGetCheck");
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@BarCode", barcode));
			return ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_ProductionGetCheck  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBarCodeManual(string barcode)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("APZ_Schedule_GetBarCodeManual");
			SqlParameter sqlParameter = new SqlParameter("@BarCode", barcode);
			return ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBarCodeManual  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet Run(string mrpCode, string scheduleType, string fdate, string tdate, string isDetailSchedule)
	{
		try
		{
			if (string.IsNullOrEmpty(mrpCode) || string.IsNullOrEmpty(fdate) || string.IsNullOrEmpty(tdate) || string.IsNullOrEmpty(scheduleType))
			{
				return new DataSet();
			}
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@MrpCode", mrpCode),
				new SqlParameter("@fdate", Function.ParseDateTime(fdate, "yyyyMMdd")),
				new SqlParameter("@tdate", Function.ParseDateTime(tdate, "yyyyMMdd")),
				new SqlParameter("@scheduleType", scheduleType),
				new SqlParameter("@isDetailSchedule", isDetailSchedule)
			};
			return ExecuteData("apz_mrp_schedule_run", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataLine(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@PrsId", id);
			return ExecuteData("APZ_Schedule_GetLine", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataDoc(int id, Enums.NavigateType type)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionScheduling_FindDataNextLast", CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@type", (int)type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetTablePO()
	{
		try
		{
			return ExecuteData("APZ_Schedule_GetTablePO", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataMrp(int mrpCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@mrpCode", mrpCode);
			return ExecuteDataTable("APZ_Schedule_GetDataMrp", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetWhs(string inputType, int inputId)
	{
		try
		{
			return ExecuteDataTable("APZ_Schehule_GetWhs", CommandType.StoredProcedure, new SqlParameter("@inputType", inputType), new SqlParameter("@inputId", inputId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataGenPurchase(string input, int code, string user)
	{
		try
		{
			if (input == "S")
			{
				return ExecuteDataTable("APZ_Schedule_GetDataPurchaseFormSchedule", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@user", user));
			}
			return ExecuteDataTable("APZ_Schedule_GetDataPurchaseFormMrp", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@user", user));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataInventoryTransferFormSchedule(string input, int code, string user)
	{
		try
		{
			try
			{
				if (input == "S")
				{
					return ExecuteDataTable("APZ_Schedule_GetDataInventoryTransferFormSchedule", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@user", user));
				}
				return ExecuteDataTable("APZ_Schedule_GetDataInventoryTransferFormMrp", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@user", user));
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				return new DataTable();
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new DataTable();
		}
	}

	public DataTable GetSeriesOrderAndRequest(string type)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@type", type);
			return ExecuteDataTable("select \"Series\" as \"Value\", \"SeriesName\" as \"Name\" from nnm1 where (@type = '22' and (\"ObjectCode\" = @type or  \"ObjectCode\" = '1470000113'))\r\n                                                                                               or (@type = '67' and (\"ObjectCode\" = @type or  \"ObjectCode\" = '1250000001'));", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataStructureAutoreport()
	{
		try
		{
			return ExecuteDataTable("APZ_GenProductionFromSchedule_GetDataStructureAutoreport", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult GetNewIdSchedule()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetNewIdScheduleQuery(), CommandType.Text);
			int num = Function.ParseInt(GetObjectId(ApzObjectTypes.oSchedule));
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) >= num)
			{
				return new HttpResult(Enums.MessageCode.Error, "Object schedule code had been changed");
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string GetNewIdScheduleQuery()
	{
		return "select isnull((select max(PSCode) from APZ_OPRS),0)";
	}

	public DataTable GetdataBpItem(string bpCode, string itemCode, string shipDate)
	{
		try
		{
			return ExecuteDataTable("APZ_Schedule_GetdataBpItem", CommandType.StoredProcedure, new SqlParameter("@bpCode", bpCode), new SqlParameter("@itemCode", itemCode), new SqlParameter("@shipDate", Function.ParseDateTime(shipDate, "yyyyMMdd")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocStructure()
	{
		try
		{
			return ExecuteDataTable("Select top 0 * from APZ_OPRS", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string CheckInputData(string type, int code, string genType)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_GenPurchseFromSchedule_CheckInputData", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@type", type), new SqlParameter("@genType", genType));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "can not check data input";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
