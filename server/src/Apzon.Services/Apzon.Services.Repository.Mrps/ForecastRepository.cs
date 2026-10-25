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
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class ForecastRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IForecast
{
	public string Temptable = "#APZ_OFCT";

	public string TemptableLine = "#APZ_FCT1";

	public DataTable GetData(string code, string name, DateTime? fromdate, DateTime? todate)
	{
		try
		{
			return ExecuteDataTable("apz_forecast_get", CommandType.StoredProcedure, new SqlParameter("@Code", code), new SqlParameter("@Name", name), new SqlParameter("@FromDate", fromdate), new SqlParameter("@ToDate", todate));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		return AddNew(objtype, bplId, document, ref objId, ref objNum, ref transtype);
	}

	private HttpResult AddNew(ApzObjectTypes objtype, int bplId, DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		objId = "";
		objNum = "";
		try
		{
			DataTable dt = doc.Tables[0];
			CreateTempTable(dt, Temptable);
			BulkCopy(dt, Temptable);
			DataTable dataTable = doc.Tables[1];
			CreateTempTable(dataTable, TemptableLine);
			BulkCopy(dataTable, TemptableLine);
			string query = "delete ofct where AbsId=" + objId + " insert into ofct (AbsId,Code,Name,UserSign,StartDate,EndDate,FormView) select AbsId,Code,Name,UserSign,StartDate,EndDate,FormView from #APZ_OFCT";
			string[] array = (from t in (from DataColumn t in dataTable.Columns
					select t.ColumnName).ToList()
				where Function.ParseDateTime(t.Replace('[', ' ').Replace(']', ' ').Trim()).HasValue
				select t).ToArray();
			List<string> values = array.Select((string col) => string.Format("ISNULL({0},0) {0}", col)).ToList();
			string query2 = " DELETE FCT1 WHERE AbsId=" + objId + " INSERT INTO FCT1(AbsId,ItemCode,Date,Quantity,LineID)\r\n\t\t\t\t\t                 SELECT " + objId + ", ItemCode,Date,Quantity,(ROW_NUMBER() OVER (ORDER BY AbsId))-1\r\n\t\t\t\t\t                 FROM \r\n\t\t\t\t\t                    (SELECT AbsId, ItemCode," + string.Join(",", values) + " FROM #APZ_FCT1) p\r\n\t\t\t\t\t\t                UNPIVOT\r\n\t\t\t\t\t                    ( Quantity FOR Date IN (" + string.Join(",", array) + ")) AS unpvt\r\n                                     DELETE FCT1 WHERE AbsId=" + objId + " AND Quantity=0";
			ExecuteNonQuery(query, CommandType.Text);
			ExecuteNonQuery(query2, CommandType.Text);
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetDataRow(int absId)
	{
		try
		{
			return ExecuteDataTable("apz_forecast_get_row", CommandType.StoredProcedure, new SqlParameter("@absId", absId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetForeCastFromOpportunity(int id)
	{
		try
		{
			return ExecuteDataTable("apz_forecast_from_opportunity", CommandType.StoredProcedure, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetMaxAbsId()
	{
		try
		{
			return ExecuteDataTable("SELECT MAX(AbsID) FROM dbo.OFCT", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetMaxLineIdByAbsId(int abs)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@AbsID", abs);
			return ExecuteDataTable("SELECT MAX(LineID) FROM dbo.FCT1 Where AbsID=@AbsID", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult SaveData(DataSet ds)
	{
		try
		{
			return Process(ApzObjectTypes.oSalesForecast, -1, Enums.TransactionType.U, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool Exists(DataSet ds)
	{
		try
		{
			return Exists(Function.ToString(ds.Tables[0].Rows[0]["Code"]), Function.ParseInt(ds.Tables[0].Rows[0]["AbsId"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public string DeleteDataLine(Tuple<int, string, DateTime> tup)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				SqlParameter item = new SqlParameter("@ItemCode", tup.Item2);
				list.Add(item);
				SqlParameter item2 = new SqlParameter("@AbsID", tup.Item1);
				list.Add(item2);
				SqlParameter item3 = new SqlParameter("@Date", tup.Item3.ToString("MM/dd/yyyy"));
				list.Add(item3);
				if (ExecuteNonQuery("DELETE FCT1 WHERE ItemCode=@ItemCode and AbsID=@AbsID and Date=@Date", CommandType.Text, list.ToArray()) <= 0)
				{
					return "No record effected";
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string InsertData(DataRow dataRow, DataTable dtStr, string tableName)
	{
		try
		{
			string text = " INSERT " + tableName + "  (";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2))
				{
					text = text + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ")";
			text += " VALUES (";
			for (int j = 0; j < dtStr.Rows.Count; j++)
			{
				string text3 = Function.ToString(dtStr.Rows[j]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text3))
				{
					text = text + "@" + text3 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += ")";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateData(DataRow dataRow, DataTable dtStr, string tableName)
	{
		try
		{
			string text = "UPDATE " + tableName + " SET ";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2) && text2 != "Code" && text2 != "AbsID")
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where Code= @Code and AbsID=@AbsID";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateData2(DataRow dataRow, DataTable dtStr, string tableName)
	{
		try
		{
			string text = "UPDATE " + tableName + " SET ";
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text2 = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				if (dataRow.IsExistsColumn(text2) && text2 != "AbsID" && text2 != "ItemCode" && text2 != "Date")
				{
					text = text + Environment.NewLine + text2 + "=@" + text2 + ",";
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where AbsID= @AbsID and ItemCode=@ItemCode and Date=@Date";
			List<SqlParameter> list = MappingSqlParameter(dataRow, dtStr);
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) <= 0)
			{
				return "No record effected";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteData(int abs)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "DELETE OFCT WHERE AbsID=@AbsID" + Environment.NewLine + "DELETE FCT1 WHERE AbsID=@AbsID";
				SqlParameter sqlParameter = new SqlParameter("@AbsID", abs);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) <= 0)
				{
					return "No record effected";
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private List<SqlParameter> MappingSqlParameter(DataRow dataRow, DataTable dtStr)
	{
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			for (int i = 0; i < dtStr.Rows.Count; i++)
			{
				string text = Function.ToString(dtStr.Rows[i]["COLUMN_NAME"]);
				string text2 = Function.ToString(dtStr.Rows[i]["DATA_TYPE"]);
				if (dataRow.IsExistsColumn(text))
				{
					switch (text2.ToUpper())
					{
					case "NVARCHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "VARCHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "DATETIME":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTime(dataRow[text])));
						break;
					case "CHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "NTEXT":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "SMALLINT":
						list.Add(new SqlParameter("@" + text, dataRow[text]));
						break;
					case "BIT":
						list.Add(new SqlParameter("@" + text, Function.ParseBool(dataRow[text])));
						break;
					case "DATE":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTimes(dataRow[text])));
						break;
					case "DATETIME2":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTimes(dataRow[text])));
						break;
					case "DECIMAL":
						list.Add(new SqlParameter("@" + text, Function.ParseDecimal(dataRow[text])));
						break;
					case "BINARY":
						list.Add(new SqlParameter("@" + text, dataRow[text]));
						break;
					case "NCHAR":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					case "INT":
						list.Add(new SqlParameter("@" + text, Function.ParseInt(dataRow[text])));
						break;
					case "NUMERIC":
						list.Add(new SqlParameter("@" + text, Function.ParseDecimal(dataRow[text])));
						break;
					case "SMALLDATETIME":
						list.Add(new SqlParameter("@" + text, Function.ParseDateTimes(dataRow[text])));
						break;
					case "TEXT":
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					default:
						list.Add(new SqlParameter("@" + text, Function.ToString(dataRow[text])));
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return list;
	}

	public bool Exists(string code, int abs)
	{
		try
		{
			return ExecuteDataTable("select Code from OFCT where Code=@Code and AbsID=@AbsID", CommandType.Text, new SqlParameter("@AbsId", abs), new SqlParameter("@Code", code)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public bool Exists2(int absId, string lindId, DateTime date)
	{
		try
		{
			string query = "select AbsID,ItemCode,Date from FCT1 where AbsID=@AbsID and ItemCode=@ItemCode and Date=@Date";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@AbsID", absId);
			list.Add(item);
			SqlParameter item2 = new SqlParameter("@ItemCode", lindId);
			list.Add(item2);
			SqlParameter item3 = new SqlParameter("@Date", date);
			list.Add(item3);
			if (ExecuteDataTable(query, CommandType.Text, list.ToArray()).IsNotNull())
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

	public DataTable GetTableColumns(string tableName)
	{
		try
		{
			if (!tableName.Contains(" "))
			{
				string query = "SELECT COLUMN_NAME,DATA_TYPE\r\n                FROM INFORMATION_SCHEMA.COLUMNS\r\n                WHERE TABLE_NAME = '" + tableName + "'";
				DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
				if (dataTable.IsNotNull())
				{
					return dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
