using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.Salary;

namespace Apzon.Services.Repository.HumanResources.Salary;

public class AllowanceTypeRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IAllowanceType
{
	private string processType = "";

	private string Temp_AllowanceType = "#APZ_OHAT";

	public DataTable GetAllowanceType()
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OHAT;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadAllowanceType  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string RemoveAllowanceType(int code)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = code;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			processType = "SaveAllowanceType";
			return Process(ApzObjectTypes.oAllowanceType, -1, Enums.TransactionType.D, dataTable).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "RemoveAllowanceType  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public HttpResult SaveAllowanceType(DataTable dt)
	{
		processType = "SaveAllowanceType";
		new DataSet().Tables.Add(dt);
		return Process(ApzObjectTypes.oAllowanceType, -1, Enums.TransactionType.A, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			if (processType == "SaveAllowanceType")
			{
				if (transtype == Enums.TransactionType.D)
				{
					text = "Delete from APZ_OHAT where  \"Code\" NOT IN(select \"AllowanceType\" from APZ_HSI1) and \"Code\" = @code;";
					string value = Function.ToString(document.Rows[0]["Code"]);
					List<SqlParameter> list = new List<SqlParameter>
					{
						new SqlParameter("@code", value)
					};
					if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "Cannot remove name because it has been used in Employee Salary Allowance");
					}
				}
				else
				{
					CreateTempTable(document, Temp_AllowanceType);
					BulkCopy(document, Temp_AllowanceType);
					text = "APZ_SaveAllowanceType";
					object obj = ExecuteScalar(text, CommandType.StoredProcedure);
					objId = Function.ToString(obj);
					objNum = objId;
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
