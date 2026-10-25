using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class VisitStoreRepository : BaseService<FinishVisitStoreImpl>, IVisitStore
{
	private const string tempDoc = "#tempDoc";

	private const string tempLine = "#tempLine";

	private const string tempUpdateInventory = "#tempUpdateInventory";

	private const string tempDocCompetitor = "#tempDocCompetitor";

	private const string tempLineCompetitor = "#tempLineCompetitor";

	public VisitStoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetBusinessTypeDataByCardCode(string cardCode)
	{
		try
		{
			new DataTable();
			string query = "APZ_D_Store_GetBusinessTypeByCardCode";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public ResponseResult GetListPotentialCustomers(string userCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			string query = "APZ_D_Customers_GetListPotential";
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@userCode", userCode));
			return new ResponseResult
			{
				Message = "",
				Content = dataTable,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	public DataTable DMSHomeGetData(string userCode)
	{
		try
		{
			new DataTable();
			string query = "APZ_D_Home_GetData";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@userCode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetVisitStoreStep()
	{
		try
		{
			new DataTable();
			string query = "select * from APZ_D_VSTEP where \"Active\" = 'Y' order by \"OrderId\" asc;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataSet GetVisitHistoryDetail(string visitId)
	{
		try
		{
			new DataSet();
			string query = "APZ_D_HistoryVisitDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@visitId", visitId);
			SqlParameter item2 = new SqlParameter("@objectType", Function.ToString(1470000171));
			list.Add(item);
			list.Add(item2);
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetTop10VisitHistory(string storeCode)
	{
		try
		{
			string query = "APZ_D_HistoryVisit_GetTop10";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@storeCode", storeCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListVisitHistory(string fromDate, string toDate, int userSign, string listCardCode, int pageIndex)
	{
		try
		{
			string[] array = Function.ToString(listCardCode).Split(',');
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("CardCode", typeof(string));
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(array[i])))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["CardCode"] = Function.ToString(array[i]);
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.AcceptChanges();
			DataTable result = new DataTable();
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(dataTable, "#tbl");
				BulkCopy(dataTable, "#tbl");
				string query = "APZ_D_HistoryVisit";
				List<SqlParameter> list = new List<SqlParameter>();
				SqlParameter item = new SqlParameter("@fromDate", fromDate);
				SqlParameter item2 = new SqlParameter("@toDate", toDate);
				SqlParameter item3 = new SqlParameter("@userSign", userSign);
				SqlParameter item4 = new SqlParameter("@pageIndex", pageIndex);
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				list.Add(item4);
				result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public ResponseResult SyncDataDetailStore(DataTable listBP, string userName)
	{
		try
		{
			DataSet dataSet = new DataSet();
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(listBP.Copy(), "#temp");
				BulkCopy(listBP.Copy(), "#temp");
				string query = "APZ_D_Sync_UpdateInventory_SearchData";
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
				dataTable.TableName = "dtInventory";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@userName", userName));
				list.Add(new SqlParameter("@relatedTyp", "oBusinessPartners"));
				list.Add(new SqlParameter("@objectType", Function.ToString(1470000171)));
				string query2 = "APZ_D_Sync_GetCommentCustomer";
				DataTable dataTable2 = ExecuteDataTable(query2, CommandType.StoredProcedure, list.ToArray());
				dataTable2.TableName = "dtComment";
				dataSet.Tables.Add(dataTable);
				dataSet.Tables.Add(dataTable2);
			}
			return new ResponseResult
			{
				MessageCode = "200",
				Message = "",
				Content = dataSet
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				MessageCode = "405",
				Message = ex.Message
			};
		}
	}

	public DataTable SearchDataUpdateInventory(string cardCode, string searchKey)
	{
		try
		{
			new DataTable();
			string query = "APZ_D_UpdateInventory_SearchData";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@cardCode", cardCode);
			SqlParameter item2 = new SqlParameter("@searchKey", (!string.IsNullOrEmpty(searchKey)) ? searchKey : string.Empty);
			list.Add(item);
			list.Add(item2);
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetListCompetitor(string cardCode)
	{
		try
		{
			new DataTable();
			string query = "select *,(select Count(\"Code\") from APZ_D_CPA1 t1 where t0.\"code\" = t1.\"Code\") as \"NumOfItem\", ROW_NUMBER() OVER(ORDER BY \"Code\") as \"LineNum\" from APZ_D_OCPA t0 where \"StoreCode\" = @cardCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@cardCode", cardCode);
			list.Add(item);
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetOrderByVisitId(string orderType, string visitId)
	{
		try
		{
			new DataTable();
			string query = "select * from ORDR where \"U_GUID\" = @visitId;";
			if (orderType == "V")
			{
				query = "select * from OINV where \"U_GUID\"= @visitId;";
			}
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@visitId", visitId);
			list.Add(item);
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetVisitSchedule(string userCode, string date)
	{
		try
		{
			new DataTable();
			DateTime? dateTime = Function.ParseDateTime(date, "ddMMyyyy");
			string query = "APZ_D_VisitSchedule_GetData";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@userName", userCode);
			SqlParameter item2 = new SqlParameter("@date", dateTime);
			list.Add(item);
			list.Add(item2);
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetListCompetitorProductAvailable(string competitorCode)
	{
		try
		{
			new DataTable();
			string query = "select * from APZ_D_CPA1 where \"Code\"=@code;";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@code", competitorCode);
			list.Add(item);
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public HttpResult FinishVisitStore(FinishVisitStoreImpl data)
	{
		try
		{
			return Process(ApzObjectTypes.oVisitStore, -1, Enums.TransactionType.U, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult
			{
				Message = ex.ToString()
			};
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, FinishVisitStoreImpl data, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		string message = "";
		switch (transtype)
		{
		case Enums.TransactionType.U:
			message = Upadate(objtype, bplId, ref transtype, data, ref objId, ref objNum).Message;
			break;
		}
		httpResult.Message = message;
		return httpResult;
	}

	public HttpResult Upadate(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, FinishVisitStoreImpl data, ref string objId, ref string objNum)
	{
		try
		{
			objId = "";
			objNum = "";
			HttpResult httpResult = new HttpResult();
			DataTable dataHeader = data.DataHeader;
			DataTable dataLine = data.DataLine;
			DataTable dataTable = data.DataUpdateInventory;
			List<CompetitorProductAvailableImpl> dataCompetitor = data.DataCompetitor;
			if (dataTable.Rows.Count <= 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("ItemCode", typeof(string));
				dataTable.Columns.Add("ItemName", typeof(string));
				dataTable.Columns.Add("LineNum", typeof(int));
				dataTable.Columns.Add("Quantity", typeof(int));
				dataTable.AcceptChanges();
			}
			DataTable dataTable2 = GetTableStruct("APZ_D_OCPA").Clone();
			dataTable2.Columns.Add("NumOfItem", typeof(int));
			DataTable dataTable3 = GetTableStruct("APZ_D_CPA1").Clone();
			for (int i = 0; i < dataCompetitor.Count; i++)
			{
				dataTable2.Rows.Add(dataCompetitor[i].DataHeader.Rows[0].ItemArray);
				for (int j = 0; j < dataCompetitor[i].DataLine.Rows.Count; j++)
				{
					dataTable3.Rows.Add(dataCompetitor[i].DataLine.Rows[j].ItemArray);
				}
			}
			dataTable2.AcceptChanges();
			dataTable3.AcceptChanges();
			DataTable dataTable4 = GetTableStruct("APZ_D_VISIT1").Clone();
			for (int k = 0; k < dataLine.Rows.Count; k++)
			{
				DataRow dataRow = dataTable4.NewRow();
				dataRow["LineNum"] = k;
				if (dataLine.IsExistsColumn("stepCode"))
				{
					dataRow["StepCode"] = Function.ToString(dataLine.Rows[k]["stepCode"]);
				}
				else
				{
					dataRow["StepCode"] = k;
				}
				dataRow["Status"] = Function.ToString(dataLine.Rows[k]["status"]);
				dataTable4.Rows.Add(dataRow.ItemArray);
				dataTable4.AcceptChanges();
			}
			DataRow dataRow2 = dataTable4.NewRow();
			dataRow2["LineNum"] = dataTable4.Rows.Count;
			dataRow2["StepCode"] = -1;
			dataRow2["Status"] = "F";
			dataRow2["GeoLocation"] = Function.ToString(dataHeader.Rows[0]["GeoLocationFinish"]);
			dataTable4.Rows.Add(dataRow2.ItemArray);
			dataTable4.AcceptChanges();
			string query = "APZ_VisitStore_FinishVisitStore";
			CreateTempTable(dataHeader, "#tempDoc");
			BulkCopy(dataHeader, "#tempDoc");
			CreateTempTable(dataTable4.Copy(), "#tempLine");
			BulkCopy(dataTable4.Copy(), "#tempLine");
			CreateTempTable(dataTable2.Copy(), "#tempDocCompetitor");
			BulkCopy(dataTable2.Copy(), "#tempDocCompetitor");
			CreateTempTable(dataTable3.Copy(), "#tempLineCompetitor");
			BulkCopy(dataTable3.Copy(), "#tempLineCompetitor");
			CreateTempTable(dataTable, "#tempUpdateInventory");
			BulkCopy(dataTable, "#tempUpdateInventory");
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure);
			httpResult.Message = "";
			for (int l = 0; l < dataSet.Tables.Count; l++)
			{
				if (Function.ToString(dataSet.Tables[l].Rows[0]["Message"]) != "")
				{
					httpResult.Message = Function.ToString(dataSet.Tables[l].Rows[0]["Message"]);
					break;
				}
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
