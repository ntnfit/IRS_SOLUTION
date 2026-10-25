using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class PackingRepository : BaseService<DataSet>, IPacking
{
	private const string TempDocument = "##APZ_OPAK";

	private const string TempDocumentLine = "##APZ_PAK1";

	private const string TableDoc = "APZ_OPAK";

	private const string TableLine = "APZ_PAK1";

	public PackingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDoc(DataTable pr)
	{
		try
		{
			return ExecuteDataTable("apz_packing_getdoc", CommandType.StoredProcedure, new SqlParameter("@FromDate", Function.ParseDateTime(pr.Rows[0]["Fromdate"])), new SqlParameter("@ToDate", Function.ParseDateTime(pr.Rows[0]["ToDate"])), new SqlParameter("@CardCode", Function.ToString(pr.Rows[0]["CardCode"])), new SqlParameter("@DocEntry", Function.ParseInt(pr.Rows[0]["DocEntry"])), new SqlParameter("@DocStatus", Function.ToString(pr.Rows[0]["DocStatus"])));
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
			return ExecuteDataTable("apz_packing_getlines", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry));
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
		return "SELECT ISNULL((SELECT Max(DocEntry) FROM APZ_OPAK),0)+1";
	}

	public DataTable GetLineByPickNo(int id)
	{
		return Choose(ApzObjectTypes.oPickLists, id);
	}

	public DataTable GetLineByPackNo(int id)
	{
		return Choose(ApzObjectTypes.oPacking, id);
	}

	public DataTable Choose(ApzObjectTypes type, int id, string distnumber = "", string cardcode = "")
	{
		try
		{
			SqlParameter[] array = new SqlParameter[4];
			int num = (int)type;
			array[0] = new SqlParameter("@ObjType", num.ToString());
			array[1] = new SqlParameter("@Id", id);
			array[2] = new SqlParameter("@DistNumber", distnumber);
			array[3] = new SqlParameter("@CardCode", cardcode);
			return ExecuteDataTable("apz_packing_choose", CommandType.StoredProcedure, array);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet ChoosePickList(string cardcode)
	{
		return new DataSet
		{
			Tables = { Choose(ApzObjectTypes.oPickLists, 0, "", cardcode).Copy() }
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
			return Process(ApzObjectTypes.oPacking, -1, Enums.TransactionType.U, ds);
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
		case Enums.TransactionType.L:
		{
			DataTable dataTable2 = ExecuteDataTable("apz_packing_close", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", Function.ToString((int)objtype)));
			if (dataTable2.IsNotNull())
			{
				int num2 = Function.ParseInt(dataTable2.Rows[0][0]);
				if (num2 > 0)
				{
					string text2 = Function.ToString(dataTable2.Rows[0][1]);
					throw new Exception(num2 + "-" + text2);
				}
			}
			objNum = objId;
			return new HttpResult();
		}
		case Enums.TransactionType.C:
		{
			DataTable dataTable = ExecuteDataTable("apz_packing_cancel", CommandType.StoredProcedure, new SqlParameter("@obj_id", Function.ParseInt(objId)), new SqlParameter("@objtype", Function.ToString((int)objtype)));
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
			string query = string.Format("update {1} set ObjType=@ObjCode\r\n                                            insert into {0} select * from {1} \r\n                                            update {2} set DocEntry=@DocEntry, LineStatus='O',ObjType=@ObjCode\r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity>0", "APZ_OPAK", "##APZ_OPAK", "##APZ_PAK1", "APZ_PAK1");
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", Function.ToString(700019)));
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
	}

	private void InitData(DataSet doc, ref string objId, ref string objNum, ref int series, Enums.TransactionType transtype, ref int cancelEntry)
	{
		DataTable dataTable = doc.Tables["APZ_OPAK"];
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
		BulkToTempTable(dataTable, "APZ_OPAK", "##APZ_OPAK");
		DataTable dataTable2 = doc.Tables["APZ_PAK1"];
		if (!dataTable2.IsNotNull())
		{
			throw new Exception("Content is empty");
		}
		BulkToTempTable(dataTable2, "APZ_PAK1", "##APZ_PAK1");
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
			string query = string.Format("delete {0} where DocEntry=@DocEntry\r\n                                                    delete {3} where DocEntry=@DocEntry\r\n                                                    update {1} set ObjType=@ObjCode\r\n                                                    update {2} set DocEntry=@DocEntry,ObjType=@ObjCode\r\n                                                    insert into {0} select * from {1} \r\n                                                    insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>'' and {2}.Quantity<>0", "APZ_OPAK", "##APZ_OPAK", "##APZ_PAK1", "APZ_PAK1");
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
			string query = "SELECT 1 FROM APZ_OPAK WHERE DocEntry=" + Function.ParseInt(doc.Tables["APZ_OPAK"].Rows[0]["DocEntry"]);
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
		return Process(ApzObjectTypes.oPacking, -1, Enums.TransactionType.L, new DataSet(), docEntry.ToString());
	}

	public HttpResult Cancel(int docEntry)
	{
		return Process(ApzObjectTypes.oPacking, -1, Enums.TransactionType.C, new DataSet(), docEntry.ToString());
	}
}
