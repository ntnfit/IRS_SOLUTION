using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaFormSettingRepository : FormSettingRepository
{
	public HanaFormSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string SaveData(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return "Data save is empty";
			}
			string value = Function.ToString(dt.Rows[0]["UserSign"]);
			string value2 = Function.ToString(dt.Rows[0]["FormId"]);
			string value3 = Function.ToString(dt.Rows[0]["ItemId"]);
			if (string.IsNullOrEmpty(value))
			{
				return "User save setting can not empty";
			}
			if (string.IsNullOrEmpty(value2))
			{
				return "Form save setting can not empty";
			}
			if (string.IsNullOrEmpty(value3))
			{
				return "Item save setting can not empty";
			}
			DataTable tableStruct = GetTableStruct("APZ_FORMSETTING");
			if (!tableStruct.HasColumn())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
			DataTable dataTable = dt.Clone();
			List<string> list = (from DataColumn t in dataTable.Columns
				where t.DataType == typeof(bool)
				select t.ColumnName).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				dataTable.Columns[list[num]].DataType = typeof(byte);
			}
			for (int num2 = 0; num2 < dt.Rows.Count; num2++)
			{
				DataRow dataRow = dataTable.NewRow();
				for (int num3 = 0; num3 < dataTable.Columns.Count; num3++)
				{
					if (list.Contains(dataTable.Columns[num3].ColumnName))
					{
						dataRow[num3] = (Function.ParseBool(dt.Rows[num2][num3]) ? 1 : 0);
					}
					else
					{
						dataRow[num3] = dt.Rows[num2][num3];
					}
				}
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dataTable, "#tempSetting");
				BulkCopy(dataTable, "#tempSetting");
				ExecuteNonQuery("apz_formsetting_save", CommandType.StoredProcedure, new SqlParameter("@userSign", value), new SqlParameter("@formId", value2), new SqlParameter("@itemId", value3));
				dbTransaction.Commit();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
