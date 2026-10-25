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
using Apzon.Services.Interfaces.Retail;

namespace Apzon.Services.Repository.Retail;

public class RetailCmdConfigRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IRetailCmdConfig
{
	public string TempTable = "#APZ_RCMC";

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("Select * from APZ_RCMC Order by \"MnGroup\",\"VisOrder\";", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataTable dt)
	{
		return Process(ApzObjectTypes.oPosCmdConfig, -1, Enums.TransactionType.U, dt).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataTable dtS, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!dtS.IsNotNull())
			{
				ret.Message = "No data to save";
			}
			else
			{
				_ = transtype;
				int num = Function.ParseInt(dtS.AsEnumerable().Max((DataRow t) => t["CmdID"]));
				for (int num2 = 0; num2 < dtS.Rows.Count; num2++)
				{
					if (Function.ParseInt(dtS.Rows[num2]["CmdID"]) == 0)
					{
						dtS.Rows[num2]["CmdID"] = num + 1;
						num++;
					}
				}
				CreateTempTable(dtS, TempTable);
				BulkCopy(dtS, TempTable);
				DataTable dataTable = ExecuteDataTable("apz_retail_SaveCmdConfig", CommandType.StoredProcedure, (SqlParameter[])null);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
				else
				{
					ret.Message = "";
				}
			}
			if (!string.IsNullOrEmpty(ret.Message))
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public string Remove(int code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@Code", code));
				if (ExecuteNonQuery("", CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return "Delete error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}
}
