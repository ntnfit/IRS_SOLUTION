using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Integrated;

namespace Apzon.Services.Repository.Integrated;

public class IntegratedUdoRepository : BaseService<DataTable>, IIntegratedUdo
{
	public IntegratedUdoRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(DataTable doc)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UdoCode", Function.ToString(doc.Rows[0]["UdoCode"])),
				new SqlParameter("@IsImported", Function.ToString(doc.Rows[0]["IsImported"]))
			};
			if (!string.IsNullOrWhiteSpace(Function.ToString(doc.Rows[0]["FromDate"])))
			{
				list.Add(new SqlParameter("@FromDate", Function.ToString(doc.Rows[0]["FromDate"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(doc.Rows[0]["ToDate"])))
			{
				list.Add(new SqlParameter("@ToDate", Function.ToString(doc.Rows[0]["ToDate"])));
			}
			result = ExecuteDataTable("APZ_IntegratedUdo_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet SearchLine(DataTable dt)
	{
		DataSet dataSet = new DataSet();
		try
		{
			GetAsyncDbName();
			string value = Function.ToString(dt.Rows[0]["UdoCode"]);
			string inputStr = Function.ToString(dt.Rows[0]["CustomID"]);
			DataTable dataTable = ExecuteDataTable("select * from OUDO where \"Code\"=@UdoCode;", CommandType.Text, new SqlParameter("@UdoCode", value));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string value2 = Function.ToString(dataTable.Rows[0]["TYPE"]);
				string value3 = Function.ToString(dataTable.Rows[0]["TableName"]);
				DataTable dataTable2 = ExecuteDataTable("select * from UDO1 where \"Code\"=@UdoCode;", CommandType.Text, new SqlParameter("@UdoCode", value));
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					DataTable dataTable3 = new DataTable();
					dataTable3.Columns.Add(new DataColumn("UdoName", typeof(string)));
					dataTable3.AcceptChanges();
					dataSet.Tables.Add(dataTable3);
					for (int i = 0; i < dataTable2.Rows.Count; i++)
					{
						DataRow dataRow = dataTable3.NewRow();
						dataRow[0] = Function.ToString(dataTable2.Rows[i]["SonName"]);
						dataTable3.Rows.Add(dataRow);
						dataTable3.AcceptChanges();
						string text = Function.ToString(dataTable2.Rows[i]["TableName"]);
						DataTable dataTable4 = ExecuteDataTable("APZ_IntegratedUdo_SearchChild", CommandType.StoredProcedure, new SqlParameter("@id", Function.ReplaceSQLInjectCharInColumnName(inputStr)), new SqlParameter("@tableName", text), new SqlParameter("@type", value2), new SqlParameter("@tableFather", value3));
						if (dataTable4 != null && dataTable4.Rows.Count > 0)
						{
							dataTable4.TableName = text;
							dataSet.Tables.Add(dataTable4);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataSet;
	}

	private string GetAsyncDbName()
	{
		try
		{
			return Function.ToString(ExecuteScalar("APZ_IntegratedUdo_GetAsyncDbName", CommandType.StoredProcedure));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public bool CheckExists(string id, string code, string type)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select * from OUDO where \"Code\"=@UdoCode;", CommandType.Text, new SqlParameter("@UdoCode", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = new DataTable();
				string text = Function.ToString(dataTable.Rows[0]["TableName"]);
				string text2 = "SELECT 1 FROM \"@" + text + "\" WHERE ";
				if (type == "1")
				{
					text2 += "\"Code\"=@id;";
					dataTable2 = ExecuteDataTable(text2, CommandType.Text, new SqlParameter("@id", id));
				}
				else
				{
					text2 += "\"U_DocEntry\"=@id;";
					dataTable2 = ExecuteDataTable(text2, CommandType.Text, new SqlParameter("@id", Function.ParseInt(id)));
				}
				return dataTable2 != null && dataTable2.Rows.Count > 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return true;
	}

	public DataSet GetDocGridStruct(string udoCode)
	{
		DataSet result = new DataSet();
		try
		{
			result = ExecuteData("APZ_IntegratedUdo_GetDocGridStruct", CommandType.StoredProcedure, new SqlParameter("@udoCode", udoCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetDocLineGridStruct(string udoCode)
	{
		DataSet result = new DataSet();
		try
		{
			result = ExecuteData("APZ_IntegratedUdo_GetDocLineGridStruct", CommandType.StoredProcedure, new SqlParameter("@udoCode", udoCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadUdoCombo()
	{
		try
		{
			return ExecuteDataTable("APZ_IntegratedUdo_LoadUdoCombo", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllChildCode(string udoCode)
	{
		try
		{
			return ExecuteDataTable("select \"SonName\" from udo1 where \"Code\"=@code;", CommandType.Text, new SqlParameter("@code", udoCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
