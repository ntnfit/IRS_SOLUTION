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
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class QueriesRepository : BaseService<DataSet>, IQueries
{
	public QueriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetListQueries(string userName, string branch, string filter = "")
	{
		try
		{
			string query = "Apz_Query_GetListQueries";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet.IsHasStructure() && dataSet.Tables[1] != null && dataSet.Tables[1].Rows.Count > 0)
			{
				string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filter);
				DataRow[] source = dataSet.Tables[1].Select(filterText);
				if (source.Any())
				{
					DataTable dataTable = source.CopyToDataTable();
					dataTable.TableName = "Data";
					dataSet.Tables.RemoveAt(1);
					dataSet.Tables.Add(dataTable.Copy());
					return dataSet;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string DeleteLine(int cateId)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@categoryID", Function.ParseInt(cateId)));
				DataTable dataTable = ExecuteDataTable("Apz_QueriesCategory_Remove", CommandType.StoredProcedure, list.ToArray());
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					string text = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					return Function.ToString(dataTable.Rows[0]["ErrorNumber"]) + " - " + text;
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteQuery(string queryName, string catalog)
	{
		try
		{
			if (string.IsNullOrEmpty(queryName))
			{
				return "Query name could not null";
			}
			if (string.IsNullOrEmpty(catalog))
			{
				return "Catalog could not null";
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = $"Delete From OUQR where QCategory = (select CategoryId from OQCN where CatName = @category) and QName = @queryName";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@category", catalog), new SqlParameter("@queryName", queryName)) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Delete error";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetParameters(string qname)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT PARAMETER_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME=@qname ORDER BY ORDINAL_POSITION", CommandType.Text, new SqlParameter("@qname", qname));
			if (dataTable.IsNotNull())
			{
				DataTable tableStruct = GetTableStruct("APZ_EQR1");
				if (tableStruct != null)
				{
					tableStruct.Rows.Clear();
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = tableStruct.NewRow();
						dataRow["Name"] = Function.ToString(dataTable.Rows[i]["PARAMETER_NAME"]);
						dataRow["Caption"] = Function.SpaceBeforeCapital(dataTable.Rows[i]["PARAMETER_NAME"]);
						dataRow["DbType"] = Function.ToString(dataTable.Rows[i]["DATA_TYPE"]);
						dataRow["Size"] = Function.ParseDouble(dataTable.Rows[0]["CHARACTER_MAXIMUM_LENGTH"]);
						dataRow["SourceType"] = "M";
						dataRow["IsNullable"] = "N";
						dataRow["IsApply"] = "Y";
						dataRow["ObjectType"] = 0;
						tableStruct.Rows.Add(dataRow);
					}
				}
				return tableStruct;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetColumns(string qname, CommandType cmdType, DataTable prs)
	{
		try
		{
			SqlParameter[] parameters = (from DataRow pr in prs.Rows
				select new SqlParameter(Function.ToString(pr["Name"]), GetDefaultSqlValue(Function.ToString(pr["DbType"])))).ToArray();
			DataTable dataTable = ExecuteDataTable(qname, cmdType, parameters);
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("ColumnId", typeof(string));
				dataTable2.Columns.Add("Caption", typeof(string));
				dataTable2.Columns.Add("ObjectType", typeof(string));
				dataTable2.Columns.Add("DisplayFormat", typeof(string));
				dataTable2.Columns.Add("Visible", typeof(bool));
				dataTable2.Columns.Add("Fixed", typeof(string));
				foreach (DataColumn item in dataTable.Columns.Cast<DataColumn>())
				{
					DataRow dataRow = dataTable2.NewRow();
					dataRow["ColumnId"] = Function.ToString(item.ColumnName);
					dataRow["Caption"] = Function.SpaceBeforeCapital(item.ColumnName);
					dataRow["ObjectType"] = "0";
					dataRow["Visible"] = 1;
					dataRow["Fixed"] = "N";
					dataTable2.Rows.Add(dataRow);
				}
				return dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult SaveExtractQuery(DataSet ds)
	{
		try
		{
			return Process(ApzObjectTypes.oExtractQueryReport, -1, Enums.TransactionType.U, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		if (Exists(document, objtype))
		{
			transtype = Enums.TransactionType.U;
			return UpdateData(document, objtype, ref objId, ref objNum, ref transtype);
		}
		transtype = Enums.TransactionType.A;
		return AddNew(document, objtype, ref objId, ref objNum, ref transtype);
	}

	private HttpResult AddNew(DataSet document, ApzObjectTypes objtype, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (document.IsNotNull())
		{
			transtype = Enums.TransactionType.A;
			InitData(document, transtype, ref objId, ref objNum);
			string query = $"update ##APZ_OEQR set Id=@Id\r\n                                            update ##APZ_EQR1 set Id=@Id\r\n                                            update ##APZ_EQR2 set Id=@Id\r\n                                            update ##APZ_EQR3 set Id=@Id\r\n                                            update ##APZ_EQR4 set Id=@Id\r\n                                            insert into APZ_OEQR SELECT * FROM ##APZ_OEQR WHERE Id>0\r\n                                            insert into APZ_EQR1 SELECT * FROM ##APZ_EQR1 WHERE Id>0\r\n                                            insert into APZ_EQR2 SELECT * FROM ##APZ_EQR2 WHERE Id>0\r\n                                            insert into APZ_EQR3 SELECT * FROM ##APZ_EQR3 WHERE Id>0\r\n                                            insert into APZ_EQR4 SELECT * FROM ##APZ_EQR4 WHERE Id>0";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@Id", objId)) > 0)
			{
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "No record effect");
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	private void InitData(DataSet document, Enums.TransactionType transtype, ref string objId, ref string objNum)
	{
		DataTable dataTable = BulkToTempTable(document.Tables["APZ_OEQR"], "APZ_OEQR", "##APZ_OEQR");
		switch (transtype)
		{
		case Enums.TransactionType.A:
			objId = Function.ToString(ExecuteScalar("SELECT ISNULL((SELECT MAX(Id) FROM APZ_OEQR),0)+1", CommandType.Text));
			break;
		case Enums.TransactionType.U:
			objId = Function.ToString(dataTable.Rows[0]["Id"]);
			break;
		}
		BulkToTempTable(document.Tables["APZ_EQR1"], "APZ_EQR1", "##APZ_EQR1");
		BulkToTempTable(document.Tables["APZ_EQR2"], "APZ_EQR2", "##APZ_EQR2");
		BulkToTempTable(document.Tables["APZ_EQR3"], "APZ_EQR3", "##APZ_EQR3");
		BulkToTempTable(document.Tables["APZ_EQR4"], "APZ_EQR4", "##APZ_EQR4");
	}

	private HttpResult UpdateData(DataSet document, ApzObjectTypes objtype, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		throw new NotImplementedException();
	}

	private bool Exists(DataSet document, ApzObjectTypes objtype)
	{
		try
		{
			return Function.ParseInt(document.Tables[0].Rows[0]["Id"]) > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return true;
	}

	public DataSet LoadParameters(int reportId)
	{
		try
		{
			return ExecuteData("SELECT * FROM APZ_EQR1 WHERE Id=@Id SELECT * FROM APZ_EQR2 WHERE Id=@Id", CommandType.Text, new SqlParameter("@Id", reportId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable ListOfQueryReport(string username)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OEQR;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet SubmitQueryReport(DataTable dtPrms)
	{
		try
		{
			if (!dtPrms.IsNotNull())
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data is empty");
				return new DataSet();
			}
			int num = Function.ParseInt(dtPrms.Rows[0]["Id"]);
			string query = "SELECT * FROM APZ_OEQR WHERE Id=@Id";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Id", num));
			if (dataTable.IsNotNull())
			{
				string text = Function.ToString(dataTable.Rows[0]["Qtype"]);
				string query2 = "";
				CommandType commandType = CommandType.Text;
				if (!(text == "S"))
				{
					if (text == "T")
					{
						query2 = Function.ToString(dataTable.Rows[0]["Qstring"]);
					}
				}
				else
				{
					query2 = Function.ToString(dataTable.Rows[0]["Qname"]);
					commandType = CommandType.StoredProcedure;
				}
				List<SqlParameter> list = new List<SqlParameter>();
				DataRow[] array = (from t in dtPrms.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["Name"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					DataRow[] array2 = array;
					foreach (DataRow dataRow in array2)
					{
						string text2 = Function.ToString(dataRow["Type"]);
						object value = null;
						switch (text2)
						{
						case "datetime":
							value = Function.ParseDateTime(dataRow["Value"]);
							break;
						case "nvarchar":
						case "char":
						case "nchar":
						case "text":
						case "ntext":
							value = Function.ToString(dataRow["Value"]);
							break;
						}
						list.Add(new SqlParameter(Function.ToString(dataRow["Name"]), value));
					}
				}
				DataSet dataSet = new DataSet();
				DataTable dataTable2 = ExecuteDataTable(query2, commandType, list.ToArray());
				dataTable2.TableName = "data";
				dataSet.Tables.Add(dataTable2.Copy());
				query = "SELECT * FROM APZ_EQR3 WHERE Id=@Id SELECT * FROM APZ_EQR4 WHERE ID=@Id";
				foreach (DataTable table in ExecuteData(query, CommandType.Text, new SqlParameter("@Id", num)).Tables)
				{
					dataSet.Tables.Add(table.Copy());
				}
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string RemoveExtractQR(string id)
	{
		string empty = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "DELETE FROM dbo.APZ_OEQR WHERE Id = '" + id + "'   DELETE FROM dbo.APZ_EQR1 WHERE Id ='" + id + "'    DELETE FROM dbo.APZ_EQR4 WHERE Id ='" + id + "'      DELETE FROM dbo.APZ_EQR3 WHERE Id ='" + id + "'  DELETE FROM dbo.APZ_EQR4 WHERE Id ='" + id + "'  ";
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return Function.ToString("No record effected!");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetQueryById(int queryId)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM  OUQR WHERE \"IntrnalKey\"= @Id;", CommandType.Text, new SqlParameter("@Id", queryId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAuthGroup()
	{
		try
		{
			return ExecuteDataTable("select * from OQAG where \"Deleted\" = 'N';", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
