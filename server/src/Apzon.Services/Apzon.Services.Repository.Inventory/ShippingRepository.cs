using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class ShippingRepository : BaseService<DataSet>, IShipping
{
	private const string TempDocument = "#APZ_SHIP";

	private const string TempDocumentLine = "#APZ_SHIP1";

	private const string TableDoc = "APZ_SHIP";

	private const string TableLine = "APZ_SHIP1";

	public ShippingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDoc(DataTable pr)
	{
		try
		{
			return ExecuteDataTable("apz_shipping_getdoc", CommandType.StoredProcedure, new SqlParameter("@FromDate", Function.ParseDateTime(pr.Rows[0]["Fromdate"])), new SqlParameter("@ToDate", Function.ParseDateTime(pr.Rows[0]["ToDate"])), new SqlParameter("@CardCode", Function.ToString(pr.Rows[0]["CardCode"])), new SqlParameter("@DocEntry", Function.ParseInt(pr.Rows[0]["DocEntry"])), new SqlParameter("@DocStatus", Function.ToString(pr.Rows[0]["DocStatus"])));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetLines(int docEntry)
	{
		try
		{
			return ExecuteDataTable("apz_shipping_getlines", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int GetNextId()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetNextIdQuery(), CommandType.Text);
			if (dataTable.IsNotNull())
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

	protected virtual string GetNextIdQuery()
	{
		return "SELECT ISNULL((SELECT Max(DocEntry) FROM APZ_SHIP),0)+1";
	}

	public DataTable GetLineByPickNo(int id)
	{
		return Choose(ApzObjectTypes.oPickLists, id);
	}

	public DataTable GetLineByPackNo(int id)
	{
		return Choose(ApzObjectTypes.oPacking, id);
	}

	public DataSet ChooseList(ApzObjectTypes type, string cardcode)
	{
		try
		{
			SqlParameter[] array = new SqlParameter[2];
			int num = (int)type;
			array[0] = new SqlParameter("@ObjType", num.ToString());
			array[1] = new SqlParameter("@CardCode", cardcode);
			return ExecuteData("apz_packing_chooselist", CommandType.StoredProcedure, array);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable Choose(ApzObjectTypes type, int id, string distnumber = "")
	{
		try
		{
			SqlParameter[] array = new SqlParameter[3];
			int num = (int)type;
			array[0] = new SqlParameter("@ObjType", num.ToString());
			array[1] = new SqlParameter("@Id", id);
			array[2] = new SqlParameter("@DistNumber", distnumber);
			return ExecuteDataTable("apz_packing_choose", CommandType.StoredProcedure, array);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet ChoosePickList()
	{
		return new DataSet
		{
			Tables = { Choose(ApzObjectTypes.oPickLists, 0).Copy() }
		};
	}

	public DataSet ChoosePacking()
	{
		return new DataSet
		{
			Tables = { Choose(ApzObjectTypes.oPacking, 0).Copy() }
		};
	}

	public DataSet ChooseBatch()
	{
		return new DataSet
		{
			Tables = { Choose(ApzObjectTypes.oPickLists, 0).Copy() }
		};
	}

	public HttpResult Save(DataSet ds)
	{
		try
		{
			return Process(ApzObjectTypes.oShipping, -1, Enums.TransactionType.U, ds, "", "", lockNumbering: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (Exists(document))
			{
				transtype = Enums.TransactionType.U;
				return UpdateData(document, ref objId, ref objNum, ref transtype);
			}
			transtype = Enums.TransactionType.A;
			return AddNew(document, ref objId, ref objNum, ref transtype);
		case Enums.TransactionType.C:
		case Enums.TransactionType.L:
		{
			string query = ((transtype == Enums.TransactionType.C) ? "apz_shipping_cancel" : "apz_shipping_close");
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", Function.ToString((int)objtype)));
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(dataTable.Rows[0][0]);
				if (num > 0)
				{
					string text = Function.ToString(dataTable.Rows[0][1]);
					throw new Exception(num + "-" + text);
				}
			}
			objNum = objId;
			return new HttpResult();
		}
		default:
			return new HttpResult(Enums.MessageCode.Error, "Unknown error");
		}
	}

	private HttpResult AddNew(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.A;
			int series = -1;
			int cancelEntry = 0;
			InitData(doc, ref objId, ref objNum, ref series, transtype, ref cancelEntry);
			BeforeAction(doc, objId, objNum, transtype, cancelEntry);
			string query = string.Format("update {1} set ObjType=@ObjCode\r\n                                            insert into {0} select * from {1} \r\n                                            update {2} set DocEntry=@DocEntry, LineStatus='O',ObjType=@ObjCode\r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity>0", "APZ_SHIP", "#APZ_SHIP", "#APZ_SHIP1", "APZ_SHIP1");
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", Function.ToString(700021)));
			AfterAction(doc, objId, objNum, transtype, cancelEntry);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	private void AfterAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
	}

	private void BeforeAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
		string query = $"apz_shipping_beforeaction";
		DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@obj_id", objId), new SqlParameter("@transtype", transtype.ToString()));
		if (dataTable.IsNotNull())
		{
			int num = Function.ParseInt(dataTable.Rows[0][0]);
			if (num > 0)
			{
				string text = Function.ToString(dataTable.Rows[0][1]);
				throw new Exception(num + "-" + text);
			}
		}
	}

	private void InitData(DataSet doc, ref string objId, ref string objNum, ref int series, Enums.TransactionType transtype, ref int cancelEntry)
	{
		DataTable dataTable = doc.Tables["APZ_SHIP"];
		int num = 0;
		switch (transtype)
		{
		case Enums.TransactionType.A:
			num = GetNextId();
			dataTable.Rows[0]["DocEntry"] = num;
			break;
		case Enums.TransactionType.U:
			num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			break;
		}
		objNum = (objId = num.ToString());
		BulkToTempTable(dataTable, "APZ_SHIP", "#APZ_SHIP");
		DataTable dataTable2 = doc.Tables["APZ_SHIP1"];
		if (!dataTable2.IsNotNull())
		{
			throw new Exception("Content is empty");
		}
		BulkToTempTable(dataTable2, "APZ_SHIP1", "#APZ_SHIP1");
	}

	private HttpResult UpdateData(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.U;
			int series = -1;
			int cancelEntry = 0;
			InitData(doc, ref objId, ref objNum, ref series, transtype, ref cancelEntry);
			BeforeAction(doc, objId, objNum, transtype, cancelEntry);
			string query = string.Format("delete {0} where DocEntry=@DocEntry\r\n                                                    delete {3} where DocEntry=@DocEntry\r\n                                                    update {1} set ObjType=@ObjCode\r\n                                                    update {2} set DocEntry=@DocEntry,ObjType=@ObjCode\r\n                                                    insert into {0} select * from {1} \r\n                                                    insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity<>0", "APZ_SHIP", "#APZ_SHIP", "#APZ_SHIP1", "APZ_SHIP1");
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", 700019.ToString()));
			AfterAction(doc, objId, objNum, transtype, cancelEntry);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	private bool Exists(DataSet doc)
	{
		try
		{
			string query = "SELECT 1 FROM APZ_SHIP WHERE DocEntry=" + Function.ParseInt(doc.Tables["APZ_SHIP"].Rows[0]["DocEntry"]);
			return ExecuteDataTable(query, CommandType.Text).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public HttpResult Close(int docEntry)
	{
		return Process(ApzObjectTypes.oPacking, -1, Enums.TransactionType.L, new DataSet(), docEntry.ToString(), "", lockNumbering: false);
	}

	public HttpResult Cancel(int docEntry)
	{
		return Process(ApzObjectTypes.oPacking, -1, Enums.TransactionType.C, new DataSet(), docEntry.ToString(), "", lockNumbering: false);
	}

	public DataTable Copy(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lstEntry)
	{
		try
		{
			SqlParameter[] array = new SqlParameter[3];
			int num = (int)fromtype;
			array[0] = new SqlParameter("@fromtype", num.ToString());
			num = (int)totype;
			array[1] = new SqlParameter("@totype", num.ToString());
			array[2] = new SqlParameter("@entry", string.Join(",", lstEntry.ToArray()));
			return ExecuteDataTable("apz_shipping_copy", CommandType.StoredProcedure, array);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable CopyHeader(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lstEntry)
	{
		try
		{
			string query = "";
			if (fromtype == ApzObjectTypes.oPickLists)
			{
				query = "SELECT * FROM APZ_OPKL WHERE AbsEntry IN (SELECT data FROM dbo.Split(@Entry,','))";
			}
			if (fromtype == ApzObjectTypes.oPacking)
			{
				query = "SELECT * FROM APZ_OPAK WHERE DocEntry IN (SELECT data FROM dbo.Split(@Entry,','))";
			}
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Entry", string.Join(",", lstEntry.ToArray())));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int GetOrderEntry(ApzObjectTypes type, int entry)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select top 1 DocEntry from ORDR WHERE DocEntry in (SELECT Top 1 TransNo FROM APZ_SHIP1 WHERE DocEntry=@DocEntry)", CommandType.Text, new SqlParameter("@DocEntry", entry));
			if (dataTable.IsNotNull())
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
}
