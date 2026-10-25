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
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaUDRRepository : UDRRepository
{
	public HanaUDRRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || (ds.Tables.Count < 2 && tranType != Enums.TransactionType.D) || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
				return httpResult;
			}
			DataTable tableStruct = GetTableStruct("APZ_UDR1");
			if (!tableStruct.HasColumn())
			{
				throw new Exception("Could not get data structure");
			}
			DataTable dataTable = ds.Tables["dtUDR1"];
			if (dataTable.IsNotNull())
			{
				IEnumerable<string> source = from DataColumn t in tableStruct.Columns
					select t.ColumnName;
				dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
				DataTable dataTable2 = dataTable.Clone();
				List<string> list = (from DataColumn t in dataTable2.Columns
					where t.DataType == typeof(bool)
					select t.ColumnName).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					dataTable2.Columns[list[num]].DataType = typeof(byte);
				}
				for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
				{
					DataRow dataRow = dataTable2.NewRow();
					for (int num3 = 0; num3 < dataTable2.Columns.Count; num3++)
					{
						if (list.Contains(dataTable2.Columns[num3].ColumnName))
						{
							dataRow[num3] = (Function.ParseBool(dataTable.Rows[num2][num3]) ? 1 : 0);
						}
						else
						{
							dataRow[num3] = dataTable.Rows[num2][num3];
						}
					}
					dataTable2.Rows.Add(dataRow);
				}
				dataTable2.AcceptChanges();
				ds.Tables.Remove("dtUDR1");
				ds.Tables.Add(dataTable2);
			}
			DataTable dt = StandardizedTable(ds.Tables["dtOUDR"], "APZ_OUDR");
			CreateTempTable(dt, Temp_OUDR);
			BulkCopy(dt, Temp_OUDR);
			if (tranType != Enums.TransactionType.D)
			{
				if (ds.Tables["dtUDR1"].IsNotNull())
				{
					dt = StandardizedTable(ds.Tables["dtUDR1"], "APZ_UDR1");
					CreateTempTable(dt, Temp_UDR1);
					BulkCopy(dt, Temp_UDR1);
				}
				if (ds.Tables["dtUDR2"].IsNotNull())
				{
					dt = StandardizedTable(ds.Tables["dtUDR2"], "APZ_UDR2");
					CreateTempTable(dt, Temp_UDR2);
					BulkCopy(dt, Temp_UDR2);
				}
			}
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable3 = ExecuteDataTable("Apz_UDR_SaveProcess", CommandType.StoredProcedure, sqlParameter);
			if (dataTable3.IsNotNull())
			{
				if (Function.ParseInt(dataTable3.Rows[0][0]) == 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable3.Rows[0][0])}-{Function.ToString(dataTable3.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.D) ? "Delete error!" : "Save error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
