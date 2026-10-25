using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class JVAnswerRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IJVAnswer
{
	private string TempTableQues = "#dtAPZ_OQLQ";

	private string TempTableAns = "#dtAPZ_OQLA";

	public DataTable LoadQuestion()
	{
		try
		{
			string query = "select * from APZ_OQLQ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadAnswer(int id)
	{
		try
		{
			string query = "select * from APZ_OQLA WHERE QuesCode=@id";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveData(DataSet dt)
	{
		return Process(ApzObjectTypes.oQuestions, -1, Enums.TransactionType.A, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			DataTable dataTable = document.Tables[0];
			DataTable tableStruct = GetTableStruct("APZ_OQLQ");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, TempTableQues);
			BulkCopy(dataTable, TempTableQues);
			DataTable dataTable2 = document.Tables[1];
			DataTable tableStruct2 = GetTableStruct("APZ_OQLA");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			if (dataTable2.Rows.Count > 0)
			{
				int num = Function.ParseInt((from row in dataTable2.AsEnumerable()
					where row["AnsCode"] != DBNull.Value && Function.ParseInt(row["AnsCode"]) != 0
					select row["AnsCode"]).Max());
				for (int num2 = 0; num2 < dataTable2.Rows.Count; num2++)
				{
					if (Function.ParseInt(dataTable2.Rows[num2]["AnsCode"]) == 0)
					{
						num++;
						dataTable2.Rows[num2]["AnsCode"] = num;
					}
				}
			}
			CreateTempTable(dataTable2, TempTableAns);
			BulkCopy(dataTable2, TempTableAns);
			object obj = ExecuteScalar("APZ_JVQuestion_Save", CommandType.StoredProcedure, (SqlParameter[])null);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
