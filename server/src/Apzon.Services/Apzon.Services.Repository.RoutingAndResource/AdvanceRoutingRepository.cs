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
using Apzon.Services.Interfaces.RoutingAndResource;

namespace Apzon.Services.Repository.RoutingAndResource;

public class AdvanceRoutingRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IAdvanceRouting
{
	private const string _tempDoc = "#tempDoc";

	private const string _tempLine = "#tempLine";

	private string _typeDelete = "";

	public DataTable GetDataDoc(Tuple<DataTable, Enums.NavigateType> tuple)
	{
		try
		{
			DataRow dataRow = tuple.Item1.Rows[0];
			Enums.NavigateType item = tuple.Item2;
			return ExecuteDataTable("APZ_Routing_GetDataDoc", CommandType.StoredProcedure, new SqlParameter("@code", dataRow["RoutCode"]), new SqlParameter("@status", dataRow["Status"]), new SqlParameter("@fDate", Function.ParseDateTime(dataRow["FromDate"], "yyyyMMdd")), new SqlParameter("@tDate", Function.ParseDateTime(dataRow["ToDate"], "yyyyMMdd")), new SqlParameter("@type", (int)item));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataSet GetDataLine(string routCode)
	{
		try
		{
			return ExecuteData("select * from APZ_RTA1 where \"RoutCode\"=@routCode order by \"VisOrder\";\r\n                                    select * from APZ_RTA2 where \"RoutCode\" = @routCode;", CommandType.Text, new SqlParameter("@routCode", Function.ToString(routCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}

	public HttpResult SaveData(Tuple<DataSet, Enums.TransactionType> tuple)
	{
		try
		{
			return Process(ApzObjectTypes.oRouting, -1, tuple.Item2, tuple.Item1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult();
		}
	}

	public HttpResult DeleteDataDoc(string routCode)
	{
		try
		{
			return Process(ApzObjectTypes.oRouting, -1, Enums.TransactionType.D, null, routCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult();
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		string message = "";
		switch (transtype)
		{
		case Enums.TransactionType.A:
			message = Save(objtype, bplId, ds, ref objId, ref objNum, ref transtype).Message;
			break;
		case Enums.TransactionType.U:
			message = Save(objtype, bplId, ds, ref objId, ref objNum, ref transtype).Message;
			break;
		case Enums.TransactionType.D:
			message = Delete(objId).Message;
			break;
		}
		httpResult.Message = message;
		return httpResult;
	}

	public HttpResult Save(ApzObjectTypes objType, int bplId, DataSet ds, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		try
		{
			objNum = "";
			DataTable dataTable = ds.Tables[0];
			DataTable dataTable2 = ds.Tables[1];
			DataTable dataTable3 = ds.Tables[2];
			DataTable tableStruct = GetTableStruct("APZ_ORTA");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			DataTable tableStruct2 = GetTableStruct("APZ_RTA1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			DataTable tableStruct3 = GetTableStruct("APZ_RTA2");
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable, "#tempDoc");
			BulkCopy(dataTable, "#tempDoc");
			CreateTempTable(dataTable2, "#tempLine");
			BulkCopy(dataTable2, "#tempLine");
			CreateTempTable(dataTable3, "#tempEffic");
			BulkCopy(dataTable3, "#tempEffic");
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@trantype", Function.ToString(transtype)));
			DataTable dataTable4 = ExecuteDataTable("APZ_AdvanceRouting_Save", CommandType.StoredProcedure, list.ToArray());
			objId = Function.ToString(dataTable.Rows[0]["RoutCode"]);
			return new HttpResult(Enums.MessageCode.Success, Function.ToString(dataTable4.Rows[0][1]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Delete(string routCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_AdvanceRouting_DeleteData", CommandType.StoredProcedure, new SqlParameter("@routCode", routCode));
			if (dataTable.IsNotNull())
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Can not delete this routing.");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult ImportFromFile(DataSet dtSet)
	{
		try
		{
			if (dtSet == null || dtSet.Tables.Count != 3 || !dtSet.Tables[0].IsNotNull() || !dtSet.Tables[1].IsNotNull() || dtSet.Tables[2] == null || dtSet.Tables[2].Columns.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Not enough data to import");
			}
			DataTable dataTable = dtSet.Tables[0];
			DataTable dataTable2 = dtSet.Tables[1];
			DataTable dataTable3 = dtSet.Tables[2];
			DataTable tableStruct = GetTableStruct("APZ_ORTA");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			DataTable tableStruct2 = GetTableStruct("APZ_RTA1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			DataTable tableStruct3 = GetTableStruct("APZ_RTA2");
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dataTable, "#tempDocRouting");
				BulkCopy(dataTable, "#tempDocRouting");
				CreateTempTable(dataTable2, "#tempLineRouting");
				BulkCopy(dataTable2, "#tempLineRouting");
				CreateTempTable(dataTable3, "#tempEfficRouting");
				BulkCopy(dataTable3, "#tempEfficRouting");
				DataTable dataTable4 = ExecuteDataTable("APZ_AdvanceRouting_ImportData", CommandType.Text);
				if (!dataTable4.IsNotNull())
				{
					dbTransaction.Rollback();
					return new HttpResult(Enums.MessageCode.Error, "Can not import data.");
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable4.Rows[0][1])))
				{
					dbTransaction.Rollback();
					return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable4.Rows[0][0])}-{Function.ToString(dataTable4.Rows[0][1])}");
				}
				dbTransaction.Commit();
				return new HttpResult();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
