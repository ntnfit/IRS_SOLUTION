using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class VatGroupsRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IVatGroups, IService<VatGroupsImpl>
{
	private string TempTableDoc = "#dtOVTG";

	private string TempTableLine = "#dtVTG1";

	public string Insert(VatGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(VatGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(VatGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public VatGroupsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public List<VatGroupsImpl> Cfl()
	{
		List<VatGroupsImpl> list = new List<VatGroupsImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"Code\", \"Name\", CASE WHEN \"Category\" = 'I' THEN 'Input Tax' ELSE 'Output Tax' END AS \"Category\", \"Rate\", \"Account\" FROM OVTG;", CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new VatGroupsImpl
				{
					Code = Function.ToString(dataTable.Rows[i]["Code"]),
					Name = Function.ToString(dataTable.Rows[i]["Name"]),
					Category = Function.ToString(dataTable.Rows[i]["Category"]),
					TaxAccount = Function.ToString(dataTable.Rows[i]["Account"]),
					Rate = Function.ParseDouble(dataTable.Rows[i]["Rate"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public List<VatGroupsImpl> Cfl(string category)
	{
		List<VatGroupsImpl> list = new List<VatGroupsImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"Code\", \"Name\", CASE WHEN \"Category\" = 'I' THEN 'Input Tax' ELSE 'Output Tax' END AS \"Category\", \"Rate\", \"Account\" FROM OVTG WHERE \"Category\"=@Category;", CommandType.Text, new SqlParameter("@Category", Function.ToString(category)));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new VatGroupsImpl
				{
					Code = Function.ToString(dataTable.Rows[i]["Code"]),
					Name = Function.ToString(dataTable.Rows[i]["Name"]),
					Category = Function.ToString(dataTable.Rows[i]["Category"]),
					TaxAccount = Function.ToString(dataTable.Rows[i]["Account"]),
					Rate = Function.ParseDouble(dataTable.Rows[i]["Rate"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public DataTable GetVatGroupI()
	{
		try
		{
			string vatGroupIQuery = GetVatGroupIQuery();
			return ExecuteDataTable(vatGroupIQuery, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetVatGroupIQuery()
	{
		return "SELECT Code,Name,Rate FROM OVTG where Category='I' And isnull(Locked,'N') ='N' And isnull(Inactive,'N')='N' order by rate;";
	}

	public DataTable GetVatGroupO()
	{
		try
		{
			string vatGroupOQuery = GetVatGroupOQuery();
			return ExecuteDataTable(vatGroupOQuery, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetVatGroupOQuery()
	{
		return "SELECT Code,Name,Rate FROM OVTG where Category='O' And isnull(Locked,'N') ='N' And isnull(Inactive,'N')='N' order by rate;";
	}

	public DataTable GetRate(VatGroupsImpl doc)
	{
		try
		{
			string text = "SELECT \"Rate\" FROM  OVTG  WHERE 1=1\r\n                               ";
			if (!string.IsNullOrEmpty(doc.Name))
			{
				text = text + "AND \"Name\" like N'%" + doc.Name + "%' ";
			}
			if (!string.IsNullOrEmpty(doc.Code))
			{
				text = text + "AND \"Code\" = N'%" + doc.Code + "%' ";
			}
			return ExecuteData(text + ";", CommandType.Text).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Manufacturers Search - Doc ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ShowVatGroupI()
	{
		try
		{
			string query = ShowVatGroupIQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string ShowVatGroupIQuery()
	{
		return "SELECT Code,Name,Rate,ISNULL( EffecDate,getDate()) as EffecDate,\r\n                                    CASE \r\n                                        WHEN Category= 'I' then'Input Taxt' \r\n                                        WHEN Category='O' then 'Output Taxt' \r\n                                    END Category\r\n                                   ,CASE\r\n                                        WHEN Category='N' then 'No'\r\n                                        WHEN Category='Y' then 'Yes'\r\n                                    END Category\r\n                                   ,ISNULL (Account,'') as Account \r\n                               FROM OVTG where Category='I' order by rate ";
	}

	public DataTable ShowVatGroupO()
	{
		try
		{
			string query = ShowVatGroupOQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string ShowVatGroupOQuery()
	{
		return "SELECT Code,Name,Rate,ISNULL( EffecDate,getDate()) as EffecDate,\r\n                                    CASE \r\n                                        WHEN Category= 'I' then'Input Taxt' \r\n                                        WHEN Category='O' then 'Output Taxt' \r\n                                    END Category\r\n                                   ,CASE\r\n                                        WHEN Category='N' then 'No'\r\n                                        WHEN Category='Y' then 'Yes'\r\n                                    END Category\r\n                                   ,ISNULL (Account,'') as Account \r\n                               FROM OVTG where Category='O' order by rate ";
	}

	public DataTable SearchVatGroupI(VatGroupsImpl doc)
	{
		try
		{
			string text = "SELECT \"Code\", \"Name\", \"Rate\", \"EffecDate\", \"Category\", \"Category\", \"Account\" FROM OVTG WHERE \"Category\" = 'I' ";
			if (!string.IsNullOrEmpty(doc.Name))
			{
				text = text + "AND \"Name\" like N'%" + doc.Name + "%' ";
			}
			text = text + "AND \"Rate\" = '" + doc.Rate + "'";
			return ExecuteData(text + ";", CommandType.Text).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Manufacturers Search - Doc ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchVatGroupO(VatGroupsImpl doc)
	{
		try
		{
			string text = "SELECT \"Code\", \"Name\", \"Rate\", \"EffecDate\", \"Category\", \"Category\", \"Account\" FROM OVTG WHERE \"Category\" ='O' ";
			if (!string.IsNullOrEmpty(doc.Name))
			{
				text = text + "AND \"Name\" like N'%" + doc.Name + "%' ";
			}
			text = text + "AND \"Rate\" = '" + doc.Rate + "'";
			return ExecuteData(text + ";", CommandType.Text).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Manufacturers Search - Doc ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ShowRateByCodeVatGroup(string codeVatGroup)
	{
		try
		{
			string query = "SELECT \"Rate\" FROM  OVTG  WHERE  \"Code\" = N'" + codeVatGroup + "'; ";
			return ExecuteData(query, CommandType.Text).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Manufacturers Search - Doc ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetSql(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT ";
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NTEXT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text7 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text7 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",GETDATE()) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						text = text + Environment.NewLine + "ISNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + tableName;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	public DataTable GetData()
	{
		try
		{
			DataTable structureTable = GetStructureTable("OVTG");
			string sql = GetSql(structureTable, "OVTG");
			DataTable dataTable = ExecuteDataTable(sql, CommandType.Text);
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public string CheckCode(string code)
	{
		string result = "";
		try
		{
			string query = "APZ_VatGroup_ExistInDocument";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", code));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetAllLine_VTG1()
	{
		try
		{
			string query = "SELECT  *  FROM VTG1; ";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetStructureTable(string tableName)
	{
		try
		{
			string structureTableQuery = GetStructureTableQuery(tableName);
			return ExecuteDataTable(structureTableQuery, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetStructureTableQuery(string tableName)
	{
		return "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT,IS_NULLABLE\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = '" + tableName + "'";
	}

	public HttpResult SaveVat(DataSet ds)
	{
		return Process(ApzObjectTypes.oVatGroups, -1, Enums.TransactionType.U, ds);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			string docTableByObjectType = GetDocTableByObjectType(ApzObjectTypes.oVatGroups);
			DataTable tableStruct = GetTableStruct(docTableByObjectType);
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			DataTable dt = ds.Tables[0].DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dt, TempTableDoc);
			BulkCopy(dt, TempTableDoc);
			CreateTempTable(ds.Tables[1], TempTableLine);
			BulkCopy(ds.Tables[1], TempTableLine);
			if (ExecuteNonQuery("DELETE dbo.OVTG\r\n                                     INSERT INTO dbo.OVTG\r\n                                     SELECT * FROM #dtOVTG", CommandType.Text) > 0)
			{
				object obj = ExecuteScalar("DELETE dbo.VTG1\r\n                                     INSERT INTO dbo.VTG1\r\n                                     SELECT * FROM #dtVTG1", CommandType.Text);
				objId = Function.ToString(obj);
				objNum = objId;
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "Error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetDataOVTG(string vatType)
	{
		try
		{
			string query;
			if (string.IsNullOrWhiteSpace(vatType))
			{
				query = "SELECT *, 'Y' as \"IsExist\" FROM OVTG;";
				return ExecuteDataTable(query, CommandType.Text);
			}
			query = "SELECT *, 'Y' as \"IsExist\" FROM OVTG WHERE \"Category\"=@vatType;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@vatType", vatType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public bool Exists(string code)
	{
		try
		{
			if (ExecuteDataTable("Select top 1 1 from OVTG where \"Code\"=@code;", CommandType.Text, new SqlParameter("@code", code)).IsNotNull())
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return false;
	}

	public string Delete(string code)
	{
		try
		{
			string query = "Delete FROM OVTG where \"Code\"=@code;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@code", code));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > -1)
			{
				return "";
			}
			return "Delete failed";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable GetStructHeader()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM OVTG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetStructLines()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM VTG1;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
