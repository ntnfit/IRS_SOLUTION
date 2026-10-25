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
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class DMSDeliveryRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IDMSDelivery
{
	protected string Temp_ODLO = "#APZ_D_ODLO";

	protected string Temp_DLO1 = "#APZ_D_DLO1";

	protected string Temp_DLO4 = "#APZ_D_DLO4";

	protected string Temp_DLO5 = "#APZ_D_DLO5";

	protected string Temp_Confirm = "#Temp_Confirm";

	public DataTable GetListDeliveryHistory(string userCode, string lstCardCode, string lstConfirmStatus, string fromDate, string toDate, int pageIndex)
	{
		try
		{
			string[] array = Function.ToString(lstCardCode).Split(',');
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
			string[] array2 = Function.ToString(lstConfirmStatus).Split(',');
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Status", typeof(string));
			for (int j = 0; j < array2.Length; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(array2[j])))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["Status"] = Function.ToString(array2[j]);
					dataTable2.Rows.Add(dataRow2);
				}
			}
			dataTable2.AcceptChanges();
			DataTable result = new DataTable();
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(dataTable, "#tbl");
				BulkCopy(dataTable, "#tbl");
				CreateTempTable(dataTable2, "#tblStatus");
				BulkCopy(dataTable2, "#tblStatus");
				string query = "APZ_D_Delivery_GetListHistory";
				List<SqlParameter> list = new List<SqlParameter>();
				SqlParameter item = new SqlParameter("@fromDate", fromDate);
				SqlParameter item2 = new SqlParameter("@toDate", toDate);
				SqlParameter item3 = new SqlParameter("@userCode", userCode);
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

	public DataSet SearchDeliveryOrder(DataTable dt)
	{
		try
		{
			string query = "APZ_D_Delivery_SearchDeliveryOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@docEntry", dt.Rows[0]["docEntry"]));
				list.Add(new SqlParameter("@userSign", dt.Rows[0]["userSign"]));
				list.Add(new SqlParameter("@fromDate", dt.Rows[0]["fromDate"]));
				list.Add(new SqlParameter("@toDate", dt.Rows[0]["toDate"]));
				list.Add(new SqlParameter("@status", dt.Rows[0]["status"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetLines(int docentry)
	{
		try
		{
			string query = "APZ_D_Delivery_GetLines";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docentry));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult Save(DataSet ds, string action)
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
			return Process(ApzObjectTypes.oDMSDelivery, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		Enums.TransactionType transactionType = transtype;
		if ((uint)transactionType <= 1u)
		{
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			DataTable tableStruct = GetTableStruct("APZ_D_ODLO");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, Temp_ODLO);
			BulkCopy(dataTable, Temp_ODLO);
			DataTable tableStruct2 = GetTableStruct("APZ_D_DLO1");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, Temp_DLO1);
			BulkCopy(dataTable2, Temp_DLO1);
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable3 = ExecuteDataTable("APZ_D_Delivery_SaveProcess", CommandType.StoredProcedure, sqlParameter);
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
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetListDeliveryOrder(string userCode, DateTime date, string status)
	{
		try
		{
			string query = "APZ_D_Delivery_GetListDeliveryOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userCode", userCode));
			list.Add(new SqlParameter("@date", date));
			list.Add(new SqlParameter("@status", status));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDeliveryOrderDetails(string docentry, string cardcode)
	{
		try
		{
			string query = "APZ_D_Delivery_GetDeliveryOrderDetails";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docentry", docentry));
			list.Add(new SqlParameter("@cardcode", cardcode));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetLocationNote(string cardcode, string shipCode)
	{
		try
		{
			string query = "APZ_D_Delivery_GetLocationNote";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardcode));
			list.Add(new SqlParameter("@shipCode", shipCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBPNote(string cardCode)
	{
		try
		{
			string query = "APZ_D_Delivery_GetBPNote";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public ResponseResult SaveLocationNote(DataTable dt, Enums.TransactionType tranType = Enums.TransactionType.A)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				ResponseResult responseResult = new ResponseResult();
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (!dt.IsNotNull())
				{
					responseResult.MessageCode = "402";
					responseResult.Message = "No data to save";
				}
				else
				{
					CreateTempTable(dt, Temp_DLO4);
					BulkCopy(dt, Temp_DLO4);
					string value = Function.ToString(tranType);
					SqlParameter sqlParameter = new SqlParameter("@type", value);
					DataTable dataTable = ExecuteDataTable("Apz_D_Delivery_SaveLocationNote", CommandType.StoredProcedure, sqlParameter);
					if (dataTable.IsNotNull())
					{
						if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
						{
							dbTransaction.Commit();
							return new ResponseResult
							{
								Message = "",
								MessageCode = "200",
								Content = Function.ToString(dataTable.Rows[0][1])
							};
						}
						return new ResponseResult
						{
							Message = $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}",
							MessageCode = "403"
						};
					}
					dbTransaction.Rollback();
				}
				return new ResponseResult
				{
					Message = ((tranType == Enums.TransactionType.D) ? "Delete error!" : "Save error"),
					MessageCode = "405"
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	public ResponseResult SaveBPNote(DataTable dt, Enums.TransactionType tranType = Enums.TransactionType.A)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				ResponseResult responseResult = new ResponseResult();
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (!dt.IsNotNull())
				{
					responseResult.MessageCode = "402";
					responseResult.Message = "No data to save";
				}
				else
				{
					CreateTempTable(dt, Temp_DLO5);
					BulkCopy(dt, Temp_DLO5);
					string value = Function.ToString(tranType);
					SqlParameter sqlParameter = new SqlParameter("@type", value);
					DataTable dataTable = ExecuteDataTable("Apz_D_Delivery_SaveBPNote", CommandType.StoredProcedure, sqlParameter);
					if (dataTable.IsNotNull())
					{
						if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
						{
							dbTransaction.Commit();
							return new ResponseResult
							{
								Message = "",
								MessageCode = "200",
								Content = Function.ToString(dataTable.Rows[0][1])
							};
						}
						return new ResponseResult
						{
							Message = $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}",
							MessageCode = "403"
						};
					}
					dbTransaction.Rollback();
				}
				return new ResponseResult
				{
					Message = ((tranType == Enums.TransactionType.D) ? "Delete error!" : "Save error"),
					MessageCode = "405"
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	public ResponseResult ConfirmLateTime(string docentry, string cardcode, DateTime newTime, string reason)
	{
		try
		{
			string query = "APZ_D_Delivery_ConfirmLateTime";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docentry", docentry));
			list.Add(new SqlParameter("@cardcode", cardcode));
			list.Add(new SqlParameter("@newTime", newTime));
			list.Add(new SqlParameter("@reason", reason));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "200"
				};
			}
			return new ResponseResult
			{
				Message = "Save error",
				MessageCode = "405"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	public ResponseResult ConfirmDelivery(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				ResponseResult responseResult = new ResponseResult();
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (!dt.IsNotNull())
				{
					responseResult.MessageCode = "402";
					responseResult.Message = "No data to save";
				}
				else
				{
					CreateTempTable(dt, Temp_Confirm);
					BulkCopy(dt, Temp_Confirm);
					DataTable dataTable = ExecuteDataTable("Apz_D_Delivery_ConfirmDelivery", CommandType.StoredProcedure);
					if (dataTable.IsNotNull())
					{
						if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
						{
							dbTransaction.Commit();
							return new ResponseResult
							{
								Message = "",
								MessageCode = "200",
								Content = Function.ToString(dataTable.Rows[0][1])
							};
						}
						return new ResponseResult
						{
							Message = $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}",
							MessageCode = "403"
						};
					}
					dbTransaction.Rollback();
				}
				return new ResponseResult
				{
					Message = "Confirm error",
					MessageCode = "405"
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}
}
