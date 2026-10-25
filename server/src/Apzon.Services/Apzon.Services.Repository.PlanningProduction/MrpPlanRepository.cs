using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Services.Interfaces.PlanningProduction;

namespace Apzon.Services.Repository.PlanningProduction;

public class MrpPlanRepository : BaseService<Mps>, IMrpPlan
{
	public MrpPlanRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet RunMrpPlan(string bpCode, string timeType)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(bpCode))
			{
				list.Add(new SqlParameter("@bpcode", bpCode));
			}
			if (!string.IsNullOrEmpty(timeType))
			{
				list.Add(new SqlParameter("@timetype", timeType));
			}
			return ExecuteData("USP_MRP_MonthlyStatus_001", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult SaveMrpPlan(DataSet ds)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dt = ds.Tables[0];
				DataTable dt2 = ds.Tables[1];
				int num = SaveHeader(dt);
				if (num > 0 && SaveDetail(dt2, num) > 0)
				{
					dbTransaction.Commit();
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
				dbTransaction.Rollback();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Save Error!");
	}

	private int SaveHeader(DataTable dt)
	{
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt.IsNotNull())
			{
				int num = Function.ParseInt(dt.Rows[0]["DocEntry"]);
				if (num <= 0)
				{
					text = "INSERT INTO dbo.APZ_MRP_M2H (CreateDate, CreateUser, Employee, BPCode, TimeType) VALUES ( ";
					text += " @CreateDate ";
					list.Add(new SqlParameter("@CreateDate", Function.ParseDateTime(dt.Rows[0]["CreateDate"])));
					text += ", @CreateUser ";
					list.Add(new SqlParameter("@CreateUser", Function.ToString(dt.Rows[0]["CreateUser"])));
					text += ", @Employee ";
					list.Add(new SqlParameter("@Employee", Function.ToString(dt.Rows[0]["Employee"])));
					text += ", @BPCode ";
					list.Add(new SqlParameter("@BPCode", Function.ToString(dt.Rows[0]["BPCode"])));
					text += ", @TimeType ";
					list.Add(new SqlParameter("@TimeType", Function.ToString(dt.Rows[0]["TimeType"])));
					text += " ) \n";
					text += "SELECT SCOPE_IDENTITY()";
				}
				else
				{
					text = "UPDATE dbo.APZ_MRP_M2H SET ";
					text += " CreateDate = @CreateDate ";
					list.Add(new SqlParameter("@CreateDate", Function.ParseDateTime(dt.Rows[0]["CreateDate"])));
					text += ", CreateUser =  @CreateUser ";
					list.Add(new SqlParameter("@CreateUser", Function.ToString(dt.Rows[0]["CreateUser"])));
					text += ", Employee = @Employee ";
					list.Add(new SqlParameter("@Employee", Function.ToString(dt.Rows[0]["Employee"])));
					text += ", BPCode = @BPCode ";
					list.Add(new SqlParameter("@BPCode", Function.ToString(dt.Rows[0]["BPCode"])));
					text += ", TimeType = @TimeType ";
					list.Add(new SqlParameter("@TimeType", Function.ToString(dt.Rows[0]["TimeType"])));
					text += " OUTPUT Inserted.DocEntry";
					text += " WHERE DocEntry =@DocEntry";
					list.Add(new SqlParameter("@DocEntry", num));
				}
				DataTable dataTable = ExecuteDataTable(text, CommandType.Text, list.ToArray());
				if (dataTable.IsNotNull())
				{
					return Function.ParseInt(dataTable.Rows[0][0]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	private int SaveDetail(DataTable dt, int id)
	{
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt.IsNotNull())
			{
				int num = Function.ParseInt(dt.Rows[0]["DocEntry"]);
				if (num <= 0)
				{
					text = "INSERT INTO dbo.APZ_MRP_M2D (DocEntry, ItemCode, MinWidt, MinLeng, LeadTime, IOldCode, OBInStock, POQtySG1, POQtyHN1, InStock1, SOAvg1, SO1, SO2, SO3, ToDate1,\r\n                             InStock2, POQtySG2, POQtyHN2, SOMulti2, SOAvg2, ToDate2, InStock3, POQtySG3, POQtyHN3, SOMulti3, SOAvg3, ToDate3, InStock4,\r\n                             POQtySG4, POQtyHN4, SOMulti4, SOAvg4, ToDate4, EBInStock) OUTPUT Inserted.DocEntry VALUES ( ";
					text += "  @DocEntry ";
					list.Add(new SqlParameter("@DocEntry", id));
					text += ", @ItemCode ";
					list.Add(new SqlParameter("@ItemCode", Function.ToString(dt.Rows[0]["ItemCode"])));
					text += ", @MinWidt ";
					list.Add(new SqlParameter("@MinWidt", Function.ParseDecimal(dt.Rows[0]["MinWidt"])));
					text += ", @MinLeng ";
					list.Add(new SqlParameter("@MinLeng", Function.ParseDecimal(dt.Rows[0]["MinLeng"])));
					text += ", @LeadTime ";
					list.Add(new SqlParameter("@LeadTime", Function.ParseInt(dt.Rows[0]["LeadTime"])));
					text += ", @IOldCode ";
					list.Add(new SqlParameter("@IOldCode", Function.ToString(dt.Rows[0]["IOldCode"])));
					text += ", @OBInStock ";
					list.Add(new SqlParameter("@OBInStock", Function.ParseDecimal(dt.Rows[0]["OBInStock"])));
					text += ", @POQtySG1 ";
					list.Add(new SqlParameter("@POQtySG1", Function.ParseDecimal(dt.Rows[0]["POQtySG1"])));
					text += ", @POQtyHN1 ";
					list.Add(new SqlParameter("@POQtyHN1", Function.ParseDecimal(dt.Rows[0]["POQtyHN1"])));
					text += ", @InStock1 ";
					list.Add(new SqlParameter("@InStock1", Function.ParseDecimal(dt.Rows[0]["InStock1"])));
					text += ", @SOAvg1 ";
					list.Add(new SqlParameter("@SOAvg1", Function.ParseDecimal(dt.Rows[0]["SOAvg1"])));
					text += ", @SO1 ";
					list.Add(new SqlParameter("@SO1", Function.ParseDecimal(dt.Rows[0]["SO1"])));
					text += ", @SO2 ";
					list.Add(new SqlParameter("@SO2", Function.ParseDecimal(dt.Rows[0]["SO2"])));
					text += ", @SO3 ";
					list.Add(new SqlParameter("@SO3", Function.ParseDecimal(dt.Rows[0]["SO3"])));
					text += ", @ToDate1 ";
					list.Add(new SqlParameter("@ToDate1", Function.ParseDateTime(dt.Rows[0]["ToDate1"])));
					text += ", @InStock2 ";
					list.Add(new SqlParameter("@InStock2", Function.ParseDecimal(dt.Rows[0]["InStock2"])));
					text += ", @POQtySG2 ";
					list.Add(new SqlParameter("@POQtySG2", Function.ParseDecimal(dt.Rows[0]["POQtySG2"])));
					text += ", @POQtyHN2 ";
					list.Add(new SqlParameter("@POQtyHN2", Function.ParseDecimal(dt.Rows[0]["POQtyHN2"])));
					text += ", @SOMulti2 ";
					list.Add(new SqlParameter("@SOMulti2", Function.ParseDecimal(dt.Rows[0]["SOMulti2"])));
					text += ", @SOAvg2 ";
					list.Add(new SqlParameter("@SOAvg2", Function.ParseDecimal(dt.Rows[0]["SOAvg2"])));
					text += ", @ToDate2 ";
					list.Add(new SqlParameter("@ToDate2", Function.ParseDateTime(dt.Rows[0]["ToDate2"])));
					text += ", @InStock3 ";
					list.Add(new SqlParameter("@InStock3", Function.ParseDecimal(dt.Rows[0]["InStock3"])));
					text += ", @POQtySG3 ";
					list.Add(new SqlParameter("@POQtySG3", Function.ParseDecimal(dt.Rows[0]["POQtySG3"])));
					text += ", @POQtyHN3 ";
					list.Add(new SqlParameter("@POQtyHN3", Function.ParseDecimal(dt.Rows[0]["POQtyHN3"])));
					text += ", @SOMulti3 ";
					list.Add(new SqlParameter("@SOMulti3", Function.ParseDecimal(dt.Rows[0]["SOMulti3"])));
					text += ", @SOAvg3 ";
					list.Add(new SqlParameter("@SOAvg3", Function.ParseDecimal(dt.Rows[0]["SOAvg3"])));
					text += ", @ToDate3 ";
					list.Add(new SqlParameter("@ToDate3", Function.ParseDateTime(dt.Rows[0]["ToDate3"])));
					text += ", @InStock4 ";
					list.Add(new SqlParameter("@InStock4", Function.ParseDecimal(dt.Rows[0]["InStock4"])));
					text += ", @POQtySG4 ";
					list.Add(new SqlParameter("@POQtySG4", Function.ParseDecimal(dt.Rows[0]["POQtySG4"])));
					text += ", @POQtyHN4 ";
					list.Add(new SqlParameter("@POQtyHN4", Function.ParseDecimal(dt.Rows[0]["POQtyHN4"])));
					text += ", @SOMulti4 ";
					list.Add(new SqlParameter("@SOMulti4", Function.ParseDecimal(dt.Rows[0]["SOMulti4"])));
					text += ", @SOAvg4 ";
					list.Add(new SqlParameter("@SOAvg4", Function.ParseDecimal(dt.Rows[0]["SOAvg4"])));
					text += ", @ToDate4 ";
					list.Add(new SqlParameter("@ToDate4", Function.ParseDateTime(dt.Rows[0]["ToDate4"])));
					text += ", @EBInStock ";
					list.Add(new SqlParameter("@EBInStock", Function.ParseDecimal(dt.Rows[0]["EBInStock"])));
					text += " ) \n";
				}
				else
				{
					text = "UPDATE dbo.APZ_MRP_M2D SET ";
					text += " ItemCode = @ItemCode ";
					list.Add(new SqlParameter("@ItemCode", Function.ToString(dt.Rows[0]["ItemCode"])));
					text += ", MinWidt = @MinWidt ";
					list.Add(new SqlParameter("@MinWidt", Function.ParseDecimal(dt.Rows[0]["MinWidt"])));
					text += ", MinLeng = @MinLeng ";
					list.Add(new SqlParameter("@MinLeng", Function.ParseDecimal(dt.Rows[0]["MinLeng"])));
					text += ", LeadTime = @LeadTime ";
					list.Add(new SqlParameter("@LeadTime", Function.ParseInt(dt.Rows[0]["LeadTime"])));
					text += ", IOldCode = @IOldCode ";
					list.Add(new SqlParameter("@IOldCode", Function.ToString(dt.Rows[0]["IOldCode"])));
					text += ", OBInStock = @OBInStock ";
					list.Add(new SqlParameter("@OBInStock", Function.ParseDecimal(dt.Rows[0]["OBInStock"])));
					text += ", POQtySG1 = @POQtySG1 ";
					list.Add(new SqlParameter("@POQtySG1", Function.ParseDecimal(dt.Rows[0]["POQtySG1"])));
					text += ", POQtyHN1 = @POQtyHN1 ";
					list.Add(new SqlParameter("@POQtyHN1", Function.ParseDecimal(dt.Rows[0]["POQtyHN1"])));
					text += ", InStock1 = @InStock1 ";
					list.Add(new SqlParameter("@InStock1", Function.ParseDecimal(dt.Rows[0]["InStock1"])));
					text += ", SOAvg1 = @SOAvg1 ";
					list.Add(new SqlParameter("@SOAvg1", Function.ParseDecimal(dt.Rows[0]["SOAvg1"])));
					text += ", SO1 = @SO1 ";
					list.Add(new SqlParameter("@SO1", Function.ParseDecimal(dt.Rows[0]["SO1"])));
					text += ", SO2 = @SO2 ";
					list.Add(new SqlParameter("@SO2", Function.ParseDecimal(dt.Rows[0]["SO2"])));
					text += ", SO3 = @SO3 ";
					list.Add(new SqlParameter("@SO3", Function.ParseDecimal(dt.Rows[0]["SO3"])));
					text += ", ToDate1 = @ToDate1 ";
					list.Add(new SqlParameter("@ToDate1", Function.ParseDateTime(dt.Rows[0]["ToDate1"])));
					text += ", InStock2 = @InStock2 ";
					list.Add(new SqlParameter("@InStock2", Function.ParseDecimal(dt.Rows[0]["InStock2"])));
					text += ", POQtySG2 = @POQtySG2 ";
					list.Add(new SqlParameter("@POQtySG2", Function.ParseDecimal(dt.Rows[0]["POQtySG2"])));
					text += ", POQtyHN2 = @POQtyHN2 ";
					list.Add(new SqlParameter("@POQtyHN2", Function.ParseDecimal(dt.Rows[0]["POQtyHN2"])));
					text += ", SOMulti2 = @SOMulti2 ";
					list.Add(new SqlParameter("@SOMulti2", Function.ParseDecimal(dt.Rows[0]["SOMulti2"])));
					text += ", SOAvg2 = @SOAvg2 ";
					list.Add(new SqlParameter("@SOAvg2", Function.ParseDecimal(dt.Rows[0]["SOAvg2"])));
					text += ", ToDate2 = @ToDate2 ";
					list.Add(new SqlParameter("@ToDate2", Function.ParseDateTime(dt.Rows[0]["ToDate2"])));
					text += ", InStock3 = @InStock3 ";
					list.Add(new SqlParameter("@InStock3", Function.ParseDecimal(dt.Rows[0]["InStock3"])));
					text += ", POQtySG3 = @POQtySG3 ";
					list.Add(new SqlParameter("@POQtySG3", Function.ParseDecimal(dt.Rows[0]["POQtySG3"])));
					text += ", POQtyHN3 = @POQtyHN3 ";
					list.Add(new SqlParameter("@POQtyHN3", Function.ParseDecimal(dt.Rows[0]["POQtyHN3"])));
					text += ", SOMulti3 = @SOMulti3 ";
					list.Add(new SqlParameter("@SOMulti3", Function.ParseDecimal(dt.Rows[0]["SOMulti3"])));
					text += ", SOAvg3 = @SOAvg3 ";
					list.Add(new SqlParameter("@SOAvg3", Function.ParseDecimal(dt.Rows[0]["SOAvg3"])));
					text += ", ToDate3 = @ToDate3 ";
					list.Add(new SqlParameter("@ToDate3", Function.ParseDateTime(dt.Rows[0]["ToDate3"])));
					text += ", InStock4 = @InStock4 ";
					list.Add(new SqlParameter("@InStock4", Function.ParseDecimal(dt.Rows[0]["InStock4"])));
					text += ", POQtySG4 = @POQtySG4 ";
					list.Add(new SqlParameter("@POQtySG4", Function.ParseDecimal(dt.Rows[0]["POQtySG4"])));
					text += ", POQtyHN4 = @POQtyHN4 ";
					list.Add(new SqlParameter("@POQtyHN4", Function.ParseDecimal(dt.Rows[0]["POQtyHN4"])));
					text += ", SOMulti4 = @SOMulti4 ";
					list.Add(new SqlParameter("@SOMulti4", Function.ParseDecimal(dt.Rows[0]["SOMulti4"])));
					text += ", SOAvg4 = @SOAvg4 ";
					list.Add(new SqlParameter("@SOAvg4", Function.ParseDecimal(dt.Rows[0]["SOAvg4"])));
					text += ", ToDate4 = @ToDate4 ";
					list.Add(new SqlParameter("@ToDate4", Function.ParseDateTime(dt.Rows[0]["ToDate4"])));
					text += ", EBInStock = @EBInStock ";
					list.Add(new SqlParameter("@EBInStock", Function.ParseDecimal(dt.Rows[0]["EBInStock"])));
					text += " ) \n";
					text += " OUTPUT Inserted.DocEntry";
					text += " WHERE DocEntry =@DocEntry";
					list.Add(new SqlParameter("@DocEntry", num));
				}
				DataTable dataTable = ExecuteDataTable(text, CommandType.Text, list.ToArray());
				if (dataTable.IsNotNull())
				{
					return Function.ParseInt(dataTable.Rows[0][0]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public int GetPage(int current, int type, string username, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Current", current));
			list.Add(new SqlParameter("@Type", type));
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@Username", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			DataTable dataTable = ExecuteDataTable("Apz_MrpPlan_GetPage", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataTable GetHeader(int id)
	{
		try
		{
			return ExecuteDataTable("Apz_MrpPlan_GetHeader", CommandType.StoredProcedure, new SqlParameter("@DocEntry", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetDetail(int id)
	{
		try
		{
			return ExecuteData("Apz_MrpPlan_GetDetail", CommandType.StoredProcedure, new SqlParameter("@DocEntry", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}
}
