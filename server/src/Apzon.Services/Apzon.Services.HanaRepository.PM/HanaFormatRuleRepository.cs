using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Repository.PM;

namespace Apzon.Services.HanaRepository.PM;

public class HanaFormatRuleRepository : FormatRuleRepository
{
	public HanaFormatRuleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string DeleteQuery()
	{
		return "DELETE FROM APZ_FCRE where \"UserCode\" = @userCode and \"FormId\" = @formId and \"ItemId\" = @itemId;";
	}

	public override string Saves(DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return "Data must not null!";
			}
			string value = Function.ToString(dt.Rows[0]["UserCode"]);
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
			DataTable tableStruct = GetTableStruct("APZ_FCRE");
			if (!tableStruct.HasColumn())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			DataTable dataTable = dt.Copy();
			string[] array = (from DataColumn t in dt.Columns
				where t.DataType == typeof(bool) || t.DataType == typeof(byte)
				select t.ColumnName).ToArray();
			if (array.Any())
			{
				dataTable = dt.Clone();
				for (int num = 0; num < array.Length; num++)
				{
					dataTable.Columns[array[num]].DataType = typeof(short);
				}
				for (int num2 = 0; num2 < dt.Rows.Count; num2++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow.ItemArray = dt.Rows[num2].ItemArray;
					for (int num3 = 0; num3 < array.Length; num3++)
					{
						dataRow[array[num3]] = (Function.ParseBool(dt.Rows[num2][array[num3]]) ? 1 : 0);
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.AcceptChanges();
			dt = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, "#APZ_FCRE");
				BulkCopy(dt, "#APZ_FCRE");
				ExecuteNonQuery("APZ_FormatRule_Saves", CommandType.StoredProcedure, new SqlParameter("@userCode", value), new SqlParameter("@formId", value2), new SqlParameter("@itemId", value3));
				dbTransaction.Commit();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
