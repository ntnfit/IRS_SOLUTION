using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Financials;

namespace Apzon.Services.Repository.Financials;

public class CostCentersRepository : BaseService<DataSet>, ICostCenters
{
	public CostCentersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(DataTable dtSearch)
	{
		try
		{
			DataRow dataRow = dtSearch.Rows[0];
			string query = "APZ_CostCenters_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@PrcCode", Function.ToString(dataRow["PrcCode"])));
			list.Add(new SqlParameter("@PrcName", Function.ToString(dataRow["PrcName"])));
			list.Add(new SqlParameter("@GrpCode", Function.ToString(dataRow["GrpCode"])));
			list.Add(new SqlParameter("@DimCode", Function.ParseInt(dataRow["DimCode"])));
			list.Add(new SqlParameter("@CCTypeCode", Function.ToString(dataRow["CCTypeCode"])));
			string text = Function.ToString(dataRow["ValidFrom"]);
			DateTime dateTime = new DateTime(Function.ParseInt(text.Substring(0, 4)), Function.ParseInt(text.Substring(4, 2)), Function.ParseInt(text.Substring(6, 2)));
			list.Add(new SqlParameter("@ValidFrom", dateTime));
			string text2 = Function.ToString(dataRow["ValidTo"]);
			DateTime dateTime2 = new DateTime(Function.ParseInt(text2.Substring(0, 4)), Function.ParseInt(text2.Substring(4, 2)), Function.ParseInt(text2.Substring(6, 2)));
			list.Add(new SqlParameter("@ValidTo", dateTime2));
			list.Add(new SqlParameter("@Active", Function.ToString(dataRow["Active"])));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public bool IsExistsCostCenters(string prcCode)
	{
		return Exists(prcCode);
	}

	public DataSet LoadComboBox()
	{
		DataSet dataSet = new DataSet();
		try
		{
			DataTable table = LoadDimCode();
			DataTable table2 = LoadCostCenterType();
			dataSet.Tables.Add(table);
			dataSet.Tables.Add(table2);
			dataSet.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return dataSet;
	}

	private DataTable LoadDimCode()
	{
		try
		{
			string query = "Select \"DimCode\" \"Code\", \"DimDesc\" \"Name\" From ODIM Where \"DimActive\"='Y';";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	private DataTable LoadCostCenterType()
	{
		try
		{
			string query = "Select \"CctCode\" \"Code\", \"CctName\" \"Name\" From OCCT;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public string Save(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = "";
				DataRow row = dt.Rows[0];
				text = DoSave(row);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private string DoSave(DataRow row)
	{
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			string prcCode = Function.ToString(row["PrcCode"]);
			bool flag = Exists(prcCode);
			text = ((!flag) ? ("INSERT INTO OPRC (\"PrcCode\", \"PrcName\", \"GrpCode\", \"Locked\", \"DataSource\", \"UserSign\", \"DimCode\", \"CCTypeCode\", \"ValidFrom\", \"ValidTo\", \"Active\", \"LogInstanc\", \"UserSign2\", \"UpdateDate\"" + GetUDFAddColumnsSQL(row) + ")\r\n                         VALUES\r\n                               (@PrcCode\r\n                               ,@PrcName\r\n                               ,@GrpCode\r\n                               ,@Locked\r\n                               ,'I'\r\n                               ,@UserSign\r\n                               ,@DimCode\r\n                               ,@CCTypeCode\r\n                               ,@ValidFrom\r\n                               ,@ValidTo\r\n                               ,@Active\r\n                               ,@LogInstanc\r\n                               ,@UserSign2\r\n                               ,@UpdateDate" + GetUDFAddParamsSQL(row) + ");") : ("UPDATE OPRC SET \"PrcName\" = @PrcName, \"GrpCode\" = @GrpCode, \"Locked\" = @Locked, \"DataSource\" = 'I', \"UserSign\" = @UserSign, \"DimCode\" = @DimCode, \"CCTypeCode\" = @CCTypeCode, \"ValidFrom\" = @ValidFrom, \"ValidTo\" = @ValidTo, \"Active\" = @Active, \"LogInstanc\" = @LogInstanc, \"UserSign2\" = @UserSign2, \"UpdateDate\" = @UpdateDate " + GetUDFUpdateSQL(row) + " WHERE  \"PrcCode\" = @PrcCode;"));
			list.Add(new SqlParameter("@PrcCode", Function.ToString(row["PrcCode"])));
			list.Add(new SqlParameter("@PrcName", Function.ToString(row["PrcName"])));
			list.Add(new SqlParameter("@GrpCode", Function.ToString(row["GrpCode"])));
			list.Add(new SqlParameter("@Locked", Function.ToString(row["Locked"])));
			list.Add(new SqlParameter("@UserSign", Function.ParseInt(row["UserSign"])));
			list.Add(new SqlParameter("@DimCode", Function.ToString(row["DimCode"])));
			list.Add(new SqlParameter("@CCTypeCode", Function.ParseInt(row["CCTypeCode"])));
			DateTime? dateTime = Function.ParseDateTime(row["ValidFrom"]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				list.Add(new SqlParameter("@ValidFrom", dateTime.Value));
			}
			else
			{
				list.Add(new SqlParameter("@ValidFrom", DBNull.Value));
			}
			DateTime? dateTime2 = Function.ParseDateTime(row["ValidTo"]);
			if (dateTime2.HasValue)
			{
				_ = dateTime2.Value;
				list.Add(new SqlParameter("@ValidTo", dateTime2.Value));
			}
			else
			{
				list.Add(new SqlParameter("@ValidTo", DBNull.Value));
			}
			list.Add(new SqlParameter("@Active", Function.ToString(row["Active"])));
			list.Add(new SqlParameter("@LogInstanc", Function.ToString(row["LogInstanc"])));
			list.Add(new SqlParameter("@UserSign2", Function.ParseInt(row["UserSign2"])));
			if (flag)
			{
				list.Add(new SqlParameter("@UpdateDate", DateTime.Today));
			}
			else
			{
				list.Add(new SqlParameter("@UpdateDate", DBNull.Value));
			}
			List<SqlParameter> uDFParamaters = GetUDFParamaters(row);
			list.AddRange(uDFParamaters.ToArray());
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No row updated";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private bool Exists(string PrcCode)
	{
		try
		{
			string query = "Select 1 from OPRC Where \"PrcCode\"=@PrcCode;";
			SqlParameter sqlParameter = new SqlParameter("@PrcCode", PrcCode);
			if (ExecuteDataTable(query, CommandType.Text, sqlParameter).IsNotNull())
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public string DeleteCostCenters(string prcCode)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_CostCenters_DeleteCostCenters";
				SqlParameter sqlParameter = new SqlParameter("@PrcCode", prcCode);
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private string GetUDFAddParamsSQL(DataRow row)
	{
		string text = "";
		try
		{
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				DataColumn dataColumn = row.Table.Columns[i];
				if (dataColumn.ColumnName.ToUpper().StartsWith("U_"))
				{
					text += $",@{dataColumn.ColumnName}";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return text;
	}

	private string GetUDFAddColumnsSQL(DataRow row)
	{
		string text = "";
		try
		{
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				DataColumn dataColumn = row.Table.Columns[i];
				if (dataColumn.ColumnName.ToUpper().StartsWith("U_"))
				{
					text += $",\"{dataColumn.ColumnName}\"";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return text;
	}

	private string GetUDFUpdateSQL(DataRow row)
	{
		string text = "";
		try
		{
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				DataColumn dataColumn = row.Table.Columns[i];
				if (dataColumn.ColumnName.ToUpper().StartsWith("U_"))
				{
					text += string.Format(",\"{0}\"=@{0}", dataColumn.ColumnName);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return text;
	}

	private List<SqlParameter> GetUDFParamaters(DataRow row)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				DataColumn dataColumn = row.Table.Columns[i];
				if (!dataColumn.ColumnName.ToUpper().StartsWith("U_"))
				{
					continue;
				}
				switch (dataColumn.DataType.ToString().ToLower())
				{
				case "system.string":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ToString(row[dataColumn])));
					break;
				case "system.char":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ToString(row[dataColumn])));
					break;
				case "system.datetime":
				{
					DateTime? dateTime = Function.ParseDateTime(row[dataColumn]);
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						list.Add(new SqlParameter("@" + dataColumn.ColumnName, dateTime.Value));
					}
					else
					{
						list.Add(new SqlParameter("@" + dataColumn.ColumnName, DBNull.Value));
					}
					break;
				}
				case "system.double":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ParseDouble(row[dataColumn])));
					break;
				case "system.decimal":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ParseDouble(row[dataColumn])));
					break;
				case "system.single":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ParseDouble(row[dataColumn])));
					break;
				case "system.int16":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ParseShort(row[dataColumn])));
					break;
				case "system.int32":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ParseInt(row[dataColumn])));
					break;
				case "system.int64":
					list.Add(new SqlParameter("@" + dataColumn.ColumnName, Function.ParseLong(row[dataColumn])));
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return list;
	}
}
