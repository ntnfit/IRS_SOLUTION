using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class UDORepository : BaseService<DataSet>, IUDOs
{
	public UDORepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetListUdo()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_UDO_GetListUdo";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListUdo ", ex.Message);
		}
		return result;
	}

	public DataTable GetUdoByCode(Common.Table_Code item)
	{
		DataTable result = new DataTable();
		try
		{
			string query = $"SELECT * FROM \"{item.Name}\" WHERE \"Code\" ='{item.Code}';";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUdoByCode ", ex.Message);
		}
		return result;
	}

	public string GetTypeByCode(string code)
	{
		try
		{
			return Function.ToString(ExecuteScalar("select TYPE from OUDO where \"Code\"=@code;", CommandType.Text, new SqlParameter("@code", code)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			return string.Empty;
		}
	}

	public DataTable GetUdo1ByCode(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = $"SELECT a.*, b.\"Descr\" FROM UDO1 a LEFT OUTER JOIN OUTB b ON a.\"TableName\" = b.\"TableName\" WHERE \"Code\" = '{code}';";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUdo1ByCode ", ex.Message);
		}
		return result;
	}

	public DataSet GetData(DataTable dt)
	{
		try
		{
			string query = "Apz_UDO_GetData";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@udoCode", dt.Rows[0]["udoCode"]));
				list.Add(new SqlParameter("@tableName", dt.Rows[0]["tableName"]));
				list.Add(new SqlParameter("@docEntry", dt.Rows[0]["docentry"]));
				list.Add(new SqlParameter("@fromDate", dt.Rows[0]["fromDate"]));
				list.Add(new SqlParameter("@toDate", dt.Rows[0]["toDate"]));
				list.Add(new SqlParameter("@username", dt.Rows[0]["userName"]));
				list.Add(new SqlParameter("@branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@code", dt.Rows[0]["code"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult GetPage(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@UdoCode", dt.Rows[0]["udoCode"]));
				list.Add(new SqlParameter("@TableName", dt.Rows[0]["tableName"]));
				list.Add(new SqlParameter("@Current", dt.Rows[0]["docentry"]));
				list.Add(new SqlParameter("@Code", dt.Rows[0]["code"]));
				list.Add(new SqlParameter("@Type", dt.Rows[0]["type"]));
				list.Add(new SqlParameter("@Username", dt.Rows[0]["userName"]));
				list.Add(new SqlParameter("@Branch", dt.Rows[0]["branch"]));
			}
			DataTable dataTable = ExecuteDataTable("Apz_UDO_Navigation", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Success, "", dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public DataTable GetDataByDocEntry(Common.Table_Code item)
	{
		DataTable result = new DataTable();
		try
		{
			string query = $"SELECT * FROM \"{item.Name}\" WHERE \"DocEntry\" = {item.Code};";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataByDocEntry ", ex.Message, $"SELECT * FROM [{item.Name}] WHERE DocEntry = {item.Code}");
		}
		return result;
	}

	public HttpResult Save(DataSet ds)
	{
		try
		{
			string text = Function.ToString(ds.Tables[0].Rows[0]["Code"]);
			if (string.IsNullOrEmpty(text))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid UDO Code");
			}
			return Process(ApzObjectTypes.oUserDefinedObject, -1, Enums.TransactionType.U, ds, "", text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(DataSet ds)
	{
		if (ds.Tables.Count == 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Data is Empty");
		}
		if (!ds.Tables[0].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Could not get object code");
		}
		return Process(ApzObjectTypes.oUserDefinedObject, -1, Enums.TransactionType.D, ds, Function.ParseInt(ds.Tables[1].Rows[0]["DocEntry"]).ToString(), Function.ToString(ds.Tables[0].Rows[0]["Code"]));
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum, string objCode = "")
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (Exists(document, objCode))
			{
				transtype = Enums.TransactionType.U;
				return UpdateData(bplId, document, ref objId, ref objNum, objCode);
			}
			transtype = Enums.TransactionType.A;
			return AddNew(bplId, document, ref objId, ref objNum, objCode);
		case Enums.TransactionType.D:
		{
			if (string.IsNullOrEmpty(objCode))
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not get object code -empty code");
			}
			int num2 = Function.ParseInt(objId);
			if (num2 <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, Messages.D404);
			}
			DataTable dataTable = ExecuteDataTable("SELECT * FROM CUFD WHERE RelUDO =@udocode", CommandType.Text, new SqlParameter("@udocode", objCode));
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (ExecuteDataTable(string.Format("SELECT TOP 1 1 FROM [{0}] WHERE [U_{1}]=@DocEntry", Function.ToString(dataTable.Rows[i]["TableID"]), Function.ToString(dataTable.Rows[i]["AliasID"])), CommandType.Text, new SqlParameter("@DocEntry", num2)).IsNotNull())
					{
						return new HttpResult(Enums.MessageCode.Error, "Code cannot be modified or deleted because it is being referenced (object = " + Function.ToString(dataTable.Rows[i]["TableID"]) + ", field = " + Function.ToString(dataTable.Rows[i]["AliasID"]) + ")");
					}
				}
			}
			string text2 = Function.ToString(document.Tables[0].Rows[0]["TableName"]);
			if (string.IsNullOrEmpty(text2))
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not get data - Table Name");
			}
			if (ExecuteNonQuery($"DELETE [@{text2}] WHERE DocEntry=@DocEntry", CommandType.Text, new SqlParameter("@DocEntry", num2)) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No record affected - header");
			}
			DataTable dataTable2 = ExecuteDataTable("SELECT TableName FROM UDO1 WHERE Code=@Code", CommandType.Text, new SqlParameter("@Code", objCode));
			if (dataTable2.IsNotNull())
			{
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					string text3 = Function.ToString(dataTable2.Rows[j]["TableName"]);
					if (string.IsNullOrEmpty(text3))
					{
						return new HttpResult(Enums.MessageCode.Error, "Could not get son data - Table Name");
					}
					if (ExecuteNonQuery($"DELETE [@{text3}] WHERE DocEntry=@DocEntry", CommandType.Text, new SqlParameter("@DocEntry", num2)) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "No record affected - son:" + text3);
					}
				}
			}
			return new HttpResult(Enums.MessageCode.Success, "", num2);
		}
		case Enums.TransactionType.C:
		case Enums.TransactionType.L:
		{
			if (string.IsNullOrEmpty(objCode))
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not get object code -empty code");
			}
			int num = Function.ParseInt(objId);
			if (num <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, Messages.D404);
			}
			string text = Function.ToString(document.Tables[0].Rows[0]["TableName"]);
			if (string.IsNullOrEmpty(text))
			{
				return new HttpResult(Enums.MessageCode.Error, "Could not get data - Table Name");
			}
			if (ExecuteNonQuery(string.Format("UPDATE [@{0}] SET  {1} WHERE DocEntry=@DocEntry", text, (transtype == Enums.TransactionType.L) ? "Status='L'" : "Canceled='Y',Status='C'"), CommandType.Text, new SqlParameter("@DocEntry", num)) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No record affected - header");
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		default:
			return base.ProcessData(objtype, bplId, ref transtype, document, ref objId, ref objNum);
		}
	}

	private HttpResult AddNew(int bplId, DataSet doc, ref string objId, ref string objNum, string objCode)
	{
		if (doc.IsNotNull())
		{
			if (doc.Tables.Count < 2)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data is empty" + doc.Tables.Count);
			}
			string tableHeaderName = "";
			int series = -1;
			DataTable dtSons = new DataTable();
			InitData(doc, objCode, Enums.TransactionType.A, ref dtSons, ref objId, ref objNum, ref tableHeaderName, ref series);
			BeforeAction(doc, objCode, objId, objNum, Enums.TransactionType.A);
			ExecuteUdo(doc, objId, objCode, tableHeaderName, dtSons, Enums.TransactionType.A);
			AfterAction(doc, objCode, objId, objNum, Enums.TransactionType.A);
		}
		return new HttpResult();
	}

	private void ExecuteUdo(DataSet doc, string objId, string objCode, string tableHeaderName, DataTable dtSons, Enums.TransactionType transtype)
	{
		string[] source = (from DataColumn t in doc.Tables[tableHeaderName].Columns
			where !new string[19]
			{
				"DocEntry", "DocNum", "Period", "Instance", "Series", "Handwrtten", "Canceled", "Object", "LogInst", "UserSign",
				"Transfered", "Status", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime", "DataSource", "RequestStatus", "Creator"
			}.Contains(t.ColumnName)
			select t.ColumnName).ToArray();
		string text = (string.IsNullOrEmpty(string.Join(",", source.ToArray())) ? "" : ("," + string.Join(",", source.ToArray())));
		string query = string.Format("DELETE [@{0}] WHERE DocEntry=@DocEntry INSERT INTO [@{0}](DocEntry, DocNum, Period, Instance, Series, Handwrtten, Canceled, Object, LogInst, UserSign, Transfered, Status," + ((transtype == Enums.TransactionType.A) ? " CreateDate, CreateTime," : " UpdateDate, UpdateTime, ") + "DataSource, RequestStatus, Creator" + text + ") \r\n                    SELECT @DocEntry,@DocEntry,NULL,0,-1,'N','N',@UdoCode,0,NULL,'N','O',@Date,@Time ,'O',NULL,NULL" + text + " FROM #{0} ", tableHeaderName);
		ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(objId)), new SqlParameter("@UdoCode", Function.ToString(objCode)), new SqlParameter("@Date", DateTime.Today), new SqlParameter("@Time", Function.ParseInt(DateTime.Now.Hour.ToString() + DateTime.Now.Minute)));
		List<SqlParameter> list = new List<SqlParameter>();
		query = "";
		if (dtSons.IsNotNull())
		{
			bool flag = false;
			for (int num = 0; num < dtSons.Rows.Count; num++)
			{
				string text2 = Function.ToString(dtSons.Rows[num]["TableName"]);
				if (string.IsNullOrEmpty(text2))
				{
					throw new Exception("Could not get child data");
				}
				if (doc.Tables[text2] == null)
				{
					continue;
				}
				string[] source2 = (from DataColumn t in doc.Tables[text2].Columns
					where !new string[5] { "DocEntry", "LineId", "VisOrder", "Object", "LogInst" }.Contains(t.ColumnName)
					select t.ColumnName).ToArray();
				string text3 = (string.IsNullOrEmpty(string.Join(",", source2.ToArray())) ? "" : ("," + string.Join(",", source2.ToArray())));
				if (doc.Tables.Contains(text2))
				{
					query += string.Format("DELETE [@{0}] WHERE DocEntry=@DocEntry UPDATE #{0} SET DocEntry=@DocEntry INSERT INTO [@{0}](DocEntry,LineId,VisOrder,Object,LogInst" + text3 + ") \r\n                            SELECT @DocEntry,ROW_NUMBER() OVER (ORDER BY DocEntry,VisOrder),ROW_NUMBER() OVER (ORDER BY DocEntry,VisOrder),@ObjectCode,0" + text3 + " FROM #{0} ", text2);
					if (!flag)
					{
						list.Add(new SqlParameter("@DocEntry", Function.ParseInt(objId)));
						list.Add(new SqlParameter("@ObjectCode", Function.ToString(objCode)));
						flag = true;
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(query))
		{
			ExecuteNonQuery(query, CommandType.Text, list.ToArray());
		}
	}

	private void AfterAction(DataSet doc, string objCode, string objId, string objNum, Enums.TransactionType transactionType)
	{
		if (transactionType == Enums.TransactionType.A)
		{
			ExecuteNonQuery("Update ONNM SET AutoKey=@AutoKey WHERE ObjectCode=@Code", CommandType.Text, new SqlParameter("@AutoKey", Function.ParseInt(objId) + 1), new SqlParameter("@Code", objCode));
		}
	}

	private void BeforeAction(DataSet doc, string objCode, string objId, string objNum, Enums.TransactionType transactionType)
	{
	}

	private void InitData(DataSet doc, string objCode, Enums.TransactionType transtype, ref DataTable dtSons, ref string objId, ref string objNum, ref string tableHeaderName, ref int series)
	{
		tableHeaderName = Function.ToString(doc.Tables[0].Rows[0]["TableName"]);
		if (string.IsNullOrEmpty(tableHeaderName))
		{
			throw new Exception("Could not get header data- header table name");
		}
		DataTable dataTable = doc.Tables[tableHeaderName];
		CreateTempTable(dataTable, "#" + tableHeaderName);
		BulkCopy(dataTable, "#" + tableHeaderName);
		int num = 0;
		if (transtype == Enums.TransactionType.A)
		{
			num = Function.ParseInt(GetObjectId(objCode));
			if (num <= 0)
			{
				throw new Exception("Could not get next number");
			}
			dataTable.Rows[0]["DocEntry"] = num;
		}
		else
		{
			num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
		}
		dtSons = ExecuteDataTable("SELECT * FROM UDO1 WHERE Code=@Code", CommandType.Text, new SqlParameter("@Code", objCode));
		if (dtSons.IsNotNull())
		{
			for (int i = 0; i < dtSons.Rows.Count; i++)
			{
				string text = Function.ToString(dtSons.Rows[i]["TableName"]);
				if (string.IsNullOrEmpty(text))
				{
					throw new Exception("Could not get child data");
				}
				if (!doc.Tables.Contains(text))
				{
					continue;
				}
				if (!doc.Tables[text].ExistsColumn("VisOrder"))
				{
					doc.Tables[text].Columns.Add("VisOrder", typeof(int));
					if (doc.Tables[text].IsNotNull())
					{
						for (int j = 0; j < doc.Tables[text].Rows.Count; j++)
						{
							doc.Tables[text].Rows[j]["VisOrder"] = j;
						}
						doc.Tables[text].AcceptChanges();
					}
				}
				CreateTempTable(doc.Tables[text], "#" + text);
				BulkCopy(doc.Tables[text], "#" + text);
			}
		}
		objId = (objNum = num.ToString());
	}

	private HttpResult UpdateData(int bplId, DataSet doc, ref string objId, ref string objNum, string objCode)
	{
		if (doc.IsNotNull())
		{
			if (doc.Tables.Count < 2)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data is empty" + doc.Tables.Count);
			}
			string tableHeaderName = "";
			int series = -1;
			DataTable dtSons = new DataTable();
			InitData(doc, objCode, Enums.TransactionType.U, ref dtSons, ref objId, ref objNum, ref tableHeaderName, ref series);
			BeforeAction(doc, objCode, objId, objNum, Enums.TransactionType.U);
			ExecuteUdo(doc, objId, objCode, tableHeaderName, dtSons, Enums.TransactionType.U);
			AfterAction(doc, objCode, objId, objNum, Enums.TransactionType.U);
		}
		return new HttpResult();
	}

	private bool Exists(DataSet document, string objCode)
	{
		string name = Function.ToString(document.Tables[0].Rows[0]["TableName"]);
		if (Function.ParseInt(document.Tables[name].Rows[0]["DocEntry"]) > 0)
		{
			return true;
		}
		return false;
	}

	public int GetMaxId(string code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@code", code);
			DataTable dataTable = ExecuteDataTable("SELECT \"TableName\" FROM oudo WHERE \"Code\"=@code;", CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string tableName = Function.ToString(dataTable.Rows[0][0]);
				DataTable dataTable2 = ExecuteDataTable(GetMaxIdGetQuery(tableName), CommandType.Text);
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					return Function.ParseInt(dataTable2.Rows[0][0]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	protected virtual string GetMaxIdGetQuery(string tableName)
	{
		return $"SELECT ISNULL((SELECT MAX(DocEntry) FROM  [@{tableName}]),0) ID";
	}

	public HttpResult Cancel(DataSet ds)
	{
		if (ds.Tables.Count == 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Data is Empty");
		}
		if (!ds.Tables[0].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Could not get object code");
		}
		return Process(ApzObjectTypes.oUserDefinedObject, -1, Enums.TransactionType.C, ds, Function.ParseInt(ds.Tables[1].Rows[0]["DocEntry"]).ToString(), Function.ToString(ds.Tables[0].Rows[0]["Code"]));
	}

	public HttpResult Close(DataSet ds)
	{
		if (ds.Tables.Count == 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Data is Empty");
		}
		if (!ds.Tables[0].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Could not get object code");
		}
		return Process(ApzObjectTypes.oUserDefinedObject, -1, Enums.TransactionType.L, ds, Function.ParseInt(ds.Tables[1].Rows[0]["DocEntry"]).ToString(), Function.ToString(ds.Tables[0].Rows[0]["Code"]));
	}

	public DataTable GetSeriesByUser(string udoCode, string userCode)
	{
		try
		{
			return ExecuteDataTable("APZ_UDO_GetSeriesByUser", CommandType.StoredProcedure, new SqlParameter("@udoCode", udoCode), new SqlParameter("@userCode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetNextNumber(string udoCode, int series)
	{
		try
		{
			return Function.ToString(ExecuteScalar("APZ_UDO_GetNextNumber", CommandType.StoredProcedure, new SqlParameter("@udoCode", udoCode), new SqlParameter("@series", series)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}
}
