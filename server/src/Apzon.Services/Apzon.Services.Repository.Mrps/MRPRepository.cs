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
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class MRPRepository : BaseService<DataRow>, IMRP
{
	public MRPRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SP_APZ_SearchMRP(string mrpNo, DateTime fDate, DateTime tDate, string status)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("SP_APZ_SearchMRP");
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", Function.ToString(mrpNo)));
			list.Add(new SqlParameter("@fDate", fDate));
			list.Add(new SqlParameter("@tDate", tDate));
			list.Add(new SqlParameter("@Status", Function.ToString(status)));
			return ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_SearchMRP  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPlant()
	{
		try
		{
			string query = "Select * from APZ_OPLA";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPlant  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SP_APZ_GetMRP(int idMrp)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("SP_APZ_GetMRP");
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@MRPID", Function.ToString(idMrp)));
			return ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SP_APZ_GetMRP  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Delete(int docEntry)
	{
		SqlParameter sqlParameter = new SqlParameter("@id", docEntry);
		if (ExecuteNonQuery("APZ_MRP_Delete", CommandType.StoredProcedure, sqlParameter) >= 0)
		{
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Delete error!");
	}

	public int GetMaxDocEntry(string tableName)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("SP_APZ_GetMaxID");
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tableName", Function.ToString(tableName)));
			DataTable dataTable = ExecuteDataTable(stringBuilder.ToString(), CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["IDNext"]);
			}
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocEntry  FAILED ", ex.Message);
			return 1;
		}
	}

	public HttpResult Save(DataRow dataRow, Enums.TransactionType tranType, string objId)
	{
		try
		{
			if (tranType == Enums.TransactionType.A)
			{
				return RunMrp(dataRow);
			}
			return Process(ApzObjectTypes.oMrp, -1, tranType, dataRow, objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataRow dsSave, ref string objId, ref string objNum)
	{
		return transtype switch
		{
			Enums.TransactionType.U => UpdateMrp(dsSave), 
			Enums.TransactionType.A => new HttpResult(Enums.MessageCode.Error, "Could not run Mrp"), 
			Enums.TransactionType.D => Delete(Function.ParseInt(objId)), 
			_ => new HttpResult(Enums.MessageCode.Error, "Could not execute function"), 
		};
	}

	public HttpResult RunMrp(DataRow row)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[10]
			{
				new SqlParameter("@fdate", Function.ParseDateTime(row["BeginDate"])),
				new SqlParameter("@tdate", Function.ParseDateTime(row["FCstDate"])),
				new SqlParameter("@MRPTYPE", Function.ParseInt(row["Type"])),
				new SqlParameter("@Forecast", Function.ToString(row["FCCode"])),
				new SqlParameter("@SalesOrderID", Function.ToString(row["SalesOrder"])),
				new SqlParameter("@PrdSchedule", Function.ToString(row["PRSId"])),
				new SqlParameter("@Whs", Function.ToString(row["Whs"])),
				new SqlParameter("@MrpId", Function.ParseInt(row["MrpCode"])),
				new SqlParameter("@Project", Function.ToString(row["PrjCode"])),
				new SqlParameter("@PeriodType", Function.ToString(row["PeriodType"]))
			};
			DataTable dataTable = ExecuteDataTable("APZ_MRP_Validate", CommandType.StoredProcedure, parameters);
			if (!dataTable.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not check input data");
			}
			if (Function.ParseInt(dataTable.Rows[0][0]) > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("ForeCast", typeof(string));
				dataTable2.Columns.Add("SalesOrder", typeof(string));
				DataRow dataRow = dataTable2.NewRow();
				dataRow["ForeCast"] = Function.ToString(row["FCCode"]);
				dataRow["SalesOrder"] = Function.ToString(row["SalesOrder"]);
				dataTable2.Rows.Add(dataRow);
				CreateTempTable(dataTable2, "#tempData");
				BulkCopy(dataTable2, "#tempData");
				HttpResult httpResult = NotifyProcedure("700002", Enums.TransactionType.R, -1, "", "");
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return httpResult;
				}
				DataTable dataTable3 = (DataTable)httpResult.Content;
				if (dataTable3 != null && dataTable3.Rows.Count > 0)
				{
					string text = Function.ToString(dataTable3.Rows[0][0]);
					string text2 = Function.ToString(dataTable3.Rows[0][1]);
					if (!string.IsNullOrEmpty(text2))
					{
						return new HttpResult(Enums.MessageCode.Error, text + "-" + text2);
					}
				}
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				DataTable dataTable4 = ExecuteDataTable("USP_APZ_MRP_IRS", CommandType.StoredProcedure, parameters);
				if (dataTable4.IsNotNull())
				{
					if (Function.ParseInt(dataTable4.Rows[0][0]) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "No data");
					}
					int num = Function.ParseInt(dataTable4.Rows[0][0]);
					row["MrpCode"] = num;
					HttpResult httpResult2 = UpdateMrp(row);
					if (!string.IsNullOrEmpty(httpResult2.Message))
					{
						return httpResult2;
					}
					dbTransaction.Commit();
					stopwatch.Stop();
					Logging.Write("Run Mrp", $"Time: {stopwatch.ElapsedMilliseconds} miliseconds");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
				return new HttpResult(Enums.MessageCode.Error, "No data");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult UpdateMrp(DataRow row)
	{
		ExecuteNonQuery("UPDATE apz_OMRP\r\n                           SET \"BeginDate\" =@BeginDate\r\n                              ,\"MrpName\" = @MrpName\r\n                              ,\"FCstDate\" = @EndDate\r\n                              ,\"Status\" = @Status\r\n                              ,\"CreateDate\" = @CreateDate\r\n                              ,\"FCCode\" = @FCCode\r\n                              ,\"Whs\" = @Whs\r\n                              ,\"Notes\" = @Notes\r\n                              ,\"FixDate\" = @FixDate\r\n                              ,\"Type\"=@Type\r\n                              ,\"BplID\" = @bplId \r\n                              ,\"PRSId\" = @PRSId\r\n                              ,\"SoId\" = @soId\r\n                              ,\"PrjCode\" = @prjCode\r\n                              , \"PeriodType\" = @periodType  \r\n                         WHERE \"MrpCode\"=@Id;", CommandType.Text, new SqlParameter("@BeginDate", Function.ParseDateTime(row["BeginDate"]) ?? DateTime.Today), new SqlParameter("@bplId", Function.ParseInt(row["BplID"])), new SqlParameter("@MrpName", Function.ToString(row["MrpName"])), new SqlParameter("@EndDate", Function.ParseDateTime(row["FCstDate"]) ?? DateTime.Today), new SqlParameter("@Status", Function.ToString(row["Status"])), new SqlParameter("@CreateDate", DateTime.Today), new SqlParameter("@FCCode", Function.ToString(row["FCCode"])), new SqlParameter("@Whs", Function.ToString(row["Whs"])), new SqlParameter("@Notes", Function.ToString(row["Notes"])), new SqlParameter("@FixDate", Function.ParseDateTime(row["FixDate"]) ?? DateTime.Today), new SqlParameter("@Id", Function.ParseInt(row["MrpCode"])), new SqlParameter("@Type", Function.ToString(row["Type"])), new SqlParameter("@PRSId", Function.ParseInt(row["PRSId"])), new SqlParameter("@soId", Function.ToString(row["SalesOrder"])), new SqlParameter("@prjCode", Function.ToString(row["PrjCode"])), new SqlParameter("@periodType", Function.ToString(row["PeriodType"])));
		return new HttpResult();
	}

	public DataTable GetWarehouse(int branch)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@branch", branch);
			return ExecuteDataTable("APZ_Mrp_GetWareHouse", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetById(int id)
	{
		try
		{
			return ExecuteDataTable("apz_mrp_getbyId", CommandType.StoredProcedure, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetEndateForecast(string forecastCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@code", forecastCode);
			DataTable dataTable = ExecuteDataTable("APZ_MRP_GetEndateForecast", CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseDateTimes(dataTable.Rows[0]["EndDate"]).ToString("yyyyMMdd");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetDataDoc(int id, Enums.NavigateType type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@id", id),
				new SqlParameter("@type", (int)type)
			};
			return ExecuteDataTable("APZ_MRP_GetDataDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult ValidateScheduleForMrp(string lstPsCode)
	{
		try
		{
			DataSet dataSet = ExecuteData(ValidateScheduleForMrpQuery(), CommandType.Text, new SqlParameter("@lstPsCode", lstPsCode));
			if (dataSet == null || dataSet.Tables.Count != 2 || !dataSet.Tables[0].IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Can not get data input");
			}
			if (Function.ParseInt(dataSet.Tables[0].Rows[0][0]) == 1)
			{
				return new HttpResult(Enums.MessageCode.Error, "You must choose approved schedule");
			}
			if (Function.ParseInt(dataSet.Tables[0].Rows[0][1]) == 1)
			{
				return new HttpResult(Enums.MessageCode.Error, "You must choose Production schedule or Transfer schedule");
			}
			return new HttpResult(Enums.MessageCode.Success, "", dataSet.Tables[1]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string ValidateScheduleForMrpQuery()
	{
		return "select (select 1 from apz_OPRS where PSCode in (select data from split(@lstPsCode, ',')) and \"Status\" <> 'A') Approval, \r\n                                        (select 1 from APZ_OPRS where PSCode in (select data  from split(@lstPsCode, ',')) and \"Type\" <> 'T' and \"Type\" <> 'O') ScheduleType\r\n                                        Select a.* from APZ_OMRP a inner join APZ_OPRS b on a.MrpCode = b.MrpId where (select count(1) \r\n                                                        from split(@lstPsCode,',')) = 1 and b.PSCode in (select data from split(@lstPsCode, ','))";
	}

	public HttpResult GetNewIdMrp()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetNewIdMrpQuery(), CommandType.Text);
			int num = Function.ParseInt(GetObjectId(ApzObjectTypes.oMrp));
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) >= num)
			{
				return new HttpResult(Enums.MessageCode.Error, "Object Mrp code had been changed");
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string GetNewIdMrpQuery()
	{
		return "select isnull((Select max(\"MrpCode\") from APZ_OMRP), 0);";
	}

	public DataTable GetSourceCellDataDocumentType()
	{
		try
		{
			return ExecuteDataTable($"Select \"TypeCode\" \"Code\", \"TypeName\" \"Name\" from APZ_OTCF where \"ObjectCode\" = '{700014}';", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadDataSourceCell(string demandType, int demandEntry, int demandLine)
	{
		try
		{
			return ExecuteDataTable("APZ_MRP_GetSourceMRP", CommandType.StoredProcedure, new SqlParameter("@DemandType", demandType), new SqlParameter("@DemandEntry", demandEntry), new SqlParameter("@DemandLine", demandLine));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
