using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class SalesEmployeeBuyerRepository : BaseService<ISalesEmployeeBuyer>, ISalesEmployeeBuyer
{
	public SalesEmployeeBuyerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Save(DataTable dtDoc)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (dtDoc != null && dtDoc.Rows.Count > 0)
				{
					string text = "";
					for (int i = 0; i < dtDoc.Rows.Count; i++)
					{
						text = SaveARow(dtDoc.Rows[i]);
						if (text != "")
						{
							break;
						}
					}
					if (text == "")
					{
						dbTransaction.Commit();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	public string DeleteRow(int slpCode)
	{
		try
		{
			string query = "APZ_SalesEmployeeBuyer_DeleteRow";
			SqlParameter sqlParameter = new SqlParameter("@SlpCode", slpCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, sqlParameter);
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private string SaveARow(DataRow row)
	{
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			int num = Function.ParseInt(row["SlpCode"]);
			bool flag = num != 0;
			if (flag)
			{
				text = "UPDATE [dbo].[OSLP]\r\n                           SET\r\n                              SlpName = @SlpName\r\n                              ,Memo = @Memo\r\n                              ,Commission = @Commission\r\n                              ,GroupCode = @GroupCode\r\n                              ,Locked = @Locked\r\n                              ,DataSource = 'I'\r\n                              ,UserSign = @UserSign\r\n                              ,EmpID = null\r\n                              ,Active = @Active\r\n                              ,Telephone = @Telephone\r\n                              ,Mobil = @Mobil\r\n                              ,Fax = @Fax\r\n                              ,Email = @Email" + GetUDFUpdateSQL(row) + " WHERE  SlpCode = @SlpCode";
			}
			else
			{
				text = "INSERT INTO [dbo].[OSLP]\r\n                               ([SlpCode]\r\n                               ,[SlpName]\r\n                               ,[Memo]\r\n                               ,[Commission]\r\n                               ,[GroupCode]\r\n                               ,[Locked]\r\n                               ,[DataSource]\r\n                               ,[UserSign]\r\n                               ,[EmpID]\r\n                               ,[Active]\r\n                               ,[Telephone]\r\n                               ,[Mobil]\r\n                               ,[Fax]\r\n                               ,[Email]" + GetUDFAddColumnsSQL(row) + ")\r\n                         VALUES\r\n                               (@SlpCode\r\n                               ,@SlpName\r\n                               ,@Memo\r\n                               ,@Commission\r\n                               ,@GroupCode\r\n                               ,@Locked\r\n                               ,'I'\r\n                               ,@UserSign\r\n                               ,null\r\n                               ,@Active\r\n                               ,@Telephone\r\n                               ,@Mobil\r\n                               ,@Fax\r\n                               ,@Email" + GetUDFAddParamsSQL(row) + ")";
				num = Function.ParseInt(GetObjectId(ApzObjectTypes.oSalesPersons));
			}
			string text2 = "";
			text2 = ((!flag) ? UpdateEmployee(num, Function.ParseInt(row["empID1"])) : UpdateEmployee(num, Function.ParseInt(row["empID1"]), GetOldEmpID(num)));
			if (!string.IsNullOrEmpty(text2))
			{
				return text2;
			}
			list.Add(new SqlParameter("@SlpName", Function.ToString(row["SlpName"])));
			list.Add(new SqlParameter("@Memo", Function.ToString(row["Memo"])));
			list.Add(new SqlParameter("@Commission", Function.ParseDouble(row["Commission"])));
			list.Add(new SqlParameter("@GroupCode", Function.ParseInt(row["GroupCode"])));
			list.Add(new SqlParameter("@Locked", Function.ToString(row["Locked"])));
			list.Add(new SqlParameter("@UserSign", Function.ParseInt(row["UserSign"])));
			list.Add(new SqlParameter("@Active", Function.ToString(row["Active"])));
			list.Add(new SqlParameter("@Telephone", Function.ToString(row["Telephone"])));
			list.Add(new SqlParameter("@Mobil", Function.ToString(row["Mobil"])));
			list.Add(new SqlParameter("@Fax", Function.ToString(row["Fax"])));
			list.Add(new SqlParameter("@Email", Function.ToString(row["Email"])));
			list.Add(new SqlParameter("@SlpCode", num));
			List<SqlParameter> uDFParamaters = GetUDFParamaters(row);
			list.AddRange(uDFParamaters.ToArray());
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No row updated";
			}
			if (!flag)
			{
				SetNextAutoKey(ApzObjectTypes.oSalesPersons);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private string UpdateEmployee(int slpCode, int empID, int oldEmpID = 0)
	{
		try
		{
			if (empID == 0)
			{
				if (oldEmpID > 0)
				{
					string query = "UPDATE OHEM SET salesPrson=null WHERE empID=@empID";
					List<SqlParameter> list = new List<SqlParameter>();
					list.Add(new SqlParameter("@empID", oldEmpID));
					if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) <= 0)
					{
						return "Update Employee - No row updated";
					}
				}
			}
			else
			{
				string query2 = "UPDATE OHEM SET salesPrson=@salesPrson WHERE empID=@empID";
				List<SqlParameter> list2 = new List<SqlParameter>();
				list2.Add(new SqlParameter("@empID", empID));
				list2.Add(new SqlParameter("@salesPrson", slpCode));
				if (ExecuteNonQuery(query2, CommandType.Text, list2.ToArray()) <= 0)
				{
					return "Update Employee - No row updated";
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool Exists(int slpCode)
	{
		try
		{
			string query = "Select 1 from OSLP Where SlpCode=@SlpCode";
			SqlParameter sqlParameter = new SqlParameter("@SlpCode", slpCode);
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

	public string SetDefaultCode(int slpCode)
	{
		try
		{
			string query = "Update OADM Set DfltSlp = @SlpCode";
			SqlParameter sqlParameter = new SqlParameter("@SlpCode", slpCode);
			if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) < 1)
			{
				return "No rows updated";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private int GetOldEmpID(int slpCode)
	{
		try
		{
			string query = "Select empID From OHEM Where salesPrson=@slpCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@slpCode", slpCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
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
					text += $",{dataColumn.ColumnName}";
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
					text += string.Format(",{0}=@{0}", dataColumn.ColumnName);
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

	public DataTable GetOSLP()
	{
		new DataTable();
		try
		{
			return ExecuteDataTable(GetOSLPQuery(), CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetOSLPQuery()
	{
		return "select \r\n\t                                a.*\r\n\t                                ,b.empID empID1\r\n\t                                ,\r\n\t                                case \r\n\t\t                                when b.lastName is null OR LTRIM(RTRIM(b.lastName)) = '' then isnull(b.firstName,'')\r\n\t                                else \r\n\t\t                                case \r\n\t\t\t                                when b.firstName is null OR LTRIM(RTRIM(b.firstName)) = '' then isnull(b.firstName,'')\r\n\t\t                                else isnull(b.lastName,'') + ', ' + isnull(b.firstName,'') \r\n\t\t                                end\r\n\t                                end\r\n\t                                empID2\r\n                            FROM OSLP a LEFT JOIN OHEM b ON a.SlpCode = b.salesPrson AND a.SlpCode<>-1";
	}

	public DataTable GetOCOG()
	{
		new DataTable();
		try
		{
			string query = "select \"GroupCode\" ,\"GroupName\" from OCOG;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListEmployee()
	{
		new DataTable();
		try
		{
			string query = "SELECT \r\n\t                                empID\r\n                                    ,lastName\r\n\t                                ,firstName\r\n\t                                ,middleName\r\n                                FROM ohem \r\n                                where active ='Y'";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public int GetDefaultCode()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetDefaultCodeQuery(), CommandType.Text);
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	protected virtual string GetDefaultCodeQuery()
	{
		return "Select isnull(DfltSlp,0) From OADM";
	}

	public bool IsExistsSlpCode(int slpCode)
	{
		try
		{
			string query = "Select 1 From OSLP Where \"SlpCode\" = @SlpCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@SlpCode", slpCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}
}
