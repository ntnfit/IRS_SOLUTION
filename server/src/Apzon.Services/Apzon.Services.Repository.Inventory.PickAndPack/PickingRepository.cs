using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Inventory.PickAndpack;

namespace Apzon.Services.Repository.Inventory.PickAndPack;

public class PickingRepository : BaseService<DataSet>, IPicking
{
	protected const string TempDocument = "#APZ_OPKL";

	protected const string TempDocumentLine = "#APZ_PKL1";

	protected const string TempDocumentBatch = "#APZ_PKL3";

	protected const string TempDocumentConfirmed = "#APZ_PKL2";

	protected const string TableDoc = "APZ_OPKL";

	protected const string TableLine = "APZ_PKL1";

	protected const string TableBatch = "APZ_PKL3";

	protected const string TableConfirmed = "APZ_PKL2";

	protected readonly string _keyEntry = "AbsEntry";

	private readonly IDatabaseService _databaseService;

	public PickingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
		_databaseService = databaseService;
	}

	public DataSet LoadBatchBarcode(string barcode, string itemcode, string whs, int docNo, int docRow, int line)
	{
		try
		{
			string query = "APZ_Picking_LoadBatchBarcode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@barcode", barcode));
			list.Add(new SqlParameter("@itemcode", itemcode));
			list.Add(new SqlParameter("@whs", whs));
			list.Add(new SqlParameter("@docNo", docNo));
			list.Add(new SqlParameter("@docRow", docRow));
			list.Add(new SqlParameter("@pickLine", line));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadSalesOrder(DataTable dt)
	{
		try
		{
			string query = "APZ_Picking_LoadSalesOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				int num = Function.ParseInt(dt.Rows[0]["docentry"]);
				string value = Function.ToString(dt.Rows[0]["whs"]);
				DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["fromDate"]);
				DateTime? dateTime2 = Function.ParseDateTime(dt.Rows[0]["toDate"]);
				string value2 = Function.ToString(dt.Rows[0]["docStatus"]);
				string value3 = Function.ToString(dt.Rows[0]["bpCode"]);
				string value4 = Function.ToString(dt.Rows[0]["userName"]);
				int num2 = Function.ParseInt(dt.Rows[0]["branch"]);
				new DateTime(1900, 1, 1);
				list.Add(new SqlParameter("@docEntry", num));
				list.Add(new SqlParameter("@fromDate", dateTime));
				list.Add(new SqlParameter("@toDate", dateTime2));
				list.Add(new SqlParameter("@docStatus", value2));
				list.Add(new SqlParameter("@bpCode", value3));
				list.Add(new SqlParameter("@userName", value4));
				list.Add(new SqlParameter("@branch", num2));
				list.Add(new SqlParameter("@warehouse", value));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadPickingList(DataTable dt)
	{
		try
		{
			string query = "APZ_Picking_LoadPickingList";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@docEntry", dt.Rows[0]["docentry"]));
				list.Add(new SqlParameter("@fromDate", dt.Rows[0]["fromDate"]));
				list.Add(new SqlParameter("@toDate", dt.Rows[0]["toDate"]));
				list.Add(new SqlParameter("@docStatus", dt.Rows[0]["docStatus"]));
				list.Add(new SqlParameter("@bpCode", dt.Rows[0]["bpCode"]));
				list.Add(new SqlParameter("@userName", dt.Rows[0]["userName"]));
				list.Add(new SqlParameter("@branch", dt.Rows[0]["branch"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadPicked(DataTable dt)
	{
		try
		{
			string query = "APZ_Picking_LoadPicked";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				int num = Function.ParseInt(dt.Rows[0]["docentry"]);
				string value = Function.ToString(dt.Rows[0]["whs"]);
				DateTime dateTime = Function.ParseDateTimes(dt.Rows[0]["fromDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(dt.Rows[0]["toDate"]);
				string value2 = Function.ToString(dt.Rows[0]["docStatus"]);
				string value3 = Function.ToString(dt.Rows[0]["bpCode"]);
				string value4 = Function.ToString(dt.Rows[0]["userName"]);
				int num2 = Function.ParseInt(dt.Rows[0]["branch"]);
				if (num > 0)
				{
					list.Add(new SqlParameter("@docEntry", num));
				}
				list.Add(new SqlParameter("@fromDate", dateTime));
				list.Add(new SqlParameter("@toDate", dateTime2));
				if (!string.IsNullOrEmpty(value2))
				{
					list.Add(new SqlParameter("@docStatus", value2));
				}
				if (!string.IsNullOrEmpty(value3))
				{
					list.Add(new SqlParameter("@bpCode", value3));
				}
				if (!string.IsNullOrEmpty(value4))
				{
					list.Add(new SqlParameter("@userName", value4));
				}
				if (num2 != -1)
				{
					list.Add(new SqlParameter("@branch", num2));
				}
				if (!string.IsNullOrEmpty(value))
				{
					list.Add(new SqlParameter("@warehouse", value));
				}
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadPickedLine(int docentry)
	{
		try
		{
			string query = "APZ_Picking_LoadPickedLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docentry));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetDoc(DataTable pr)
	{
		try
		{
			return ExecuteDataTable("apz_picking_getdoc", CommandType.StoredProcedure, new SqlParameter("@docentry", Function.ParseInt(pr.Rows[0]["docentry"])), new SqlParameter("@whs", Function.ToString(pr.Rows[0]["whs"])), new SqlParameter("@fromDate", Function.ParseDateTime(pr.Rows[0]["fromDate"])), new SqlParameter("@toDate", Function.ParseDateTime(pr.Rows[0]["toDate"])), new SqlParameter("@docStatus", Function.ToString(pr.Rows[0]["docStatus"])), new SqlParameter("@bpCode", Function.ToString(pr.Rows[0]["bpCode"])), new SqlParameter("@userName", Function.ToString(pr.Rows[0]["userName"])), new SqlParameter("@branch", Function.ParseInt(pr.Rows[0]["branch"])));
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
			return ExecuteDataTable("SELECT T0.*,CASE WHEN T1.\"ManBtchNum\"='Y' THEN 'B' WHEN T1.\"ManSerNum\"='Y' THEN 'S' ELSE 'N' END \"ManBatchSerial\" \r\n                                    FROM APZ_PKL1 T0 INNER JOIN OITM T1 ON T0.\"ItemCode\"=T1.\"ItemCode\" WHERE \"AbsEntry\"=@DocEntry;", CommandType.Text, new SqlParameter("@DocEntry", docEntry));
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
		return "SELECT ISNULL((SELECT Max(AbsEntry) FROM APZ_OPKL),0)+1";
	}

	public DataTable GetLineByPickNo(int id)
	{
		return Choose(ApzObjectTypes.oPickLists, id);
	}

	public DataTable GetLineByPackNo(int id)
	{
		return Choose(ApzObjectTypes.oPacking, id);
	}

	public DataSet ChooseList(ApzObjectTypes type)
	{
		try
		{
			SqlParameter[] array = new SqlParameter[1];
			int num = (int)type;
			array[0] = new SqlParameter("@ObjType", num.ToString());
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
			return Process(ApzObjectTypes.oPickLists, -1, Enums.TransactionType.U, ds);
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
			string query = ((transtype == Enums.TransactionType.C) ? "apz_picking_cancel" : "apz_picking_close");
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
		case Enums.TransactionType.R:
		case Enums.TransactionType.M:
			return AddConfirmed(document, ref objId, ref objNum, ref transtype);
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
			string addNewQuery = GetAddNewQuery();
			ExecuteNonQuery(addNewQuery, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", Function.ToString(156)));
			AfterAction(doc, objId, objNum, transtype, cancelEntry);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	protected virtual string GetAddNewQuery()
	{
		return string.Format("update {1} set ObjType=@ObjCode\r\n                                            insert into {0} select * from {1} \r\n                                            update {2} set AbsEntry=@DocEntry\r\n                                            update {5} set AbsEntry=@DocEntry\r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>''\r\n                                            insert into {4} select * from {5} where {5}.MdAbsEntry <>0", "APZ_OPKL", "#APZ_OPKL", "#APZ_PKL1", "APZ_PKL1", "APZ_PKL3", "#APZ_PKL3");
	}

	private void AfterAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
	}

	private void BeforeAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
		string query = $"apz_picking_beforeaction";
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
		DataTable dataTable = doc.Tables["APZ_OPKL"];
		int num = 0;
		switch (transtype)
		{
		case Enums.TransactionType.A:
			num = GetNextId();
			dataTable.Rows[0]["AbsEntry"] = num;
			break;
		case Enums.TransactionType.U:
			num = Function.ParseInt(dataTable.Rows[0]["AbsEntry"]);
			break;
		}
		objNum = (objId = num.ToString());
		BulkToTempTable(dataTable, "APZ_OPKL", "#APZ_OPKL");
		DataTable dataTable2 = doc.Tables["APZ_PKL1"];
		if (!dataTable2.IsNotNull())
		{
			throw new Exception("Content is empty");
		}
		BulkToTempTable(dataTable2, "APZ_PKL1", "#APZ_PKL1");
		DataTable source = doc.Tables["APZ_PKL3"];
		BulkToTempTable(source, "APZ_PKL3", "#APZ_PKL3");
		DataTable source2 = doc.Tables["APZ_PKL2"];
		BulkToTempTable(source2, "APZ_PKL2", "#APZ_PKL2");
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
			string updateQuery = GetUpdateQuery();
			ExecuteNonQuery(updateQuery, CommandType.Text, new SqlParameter("@DocEntry", objId), new SqlParameter("@ObjCode", Function.ToString(156)));
			AfterAction(doc, objId, objNum, transtype, cancelEntry);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	protected virtual string GetUpdateQuery()
	{
		return string.Format("delete {0} where AbsEntry=@DocEntry\r\n                                            delete {3} where AbsEntry=@DocEntry\r\n                                            delete {4} where AbsEntry=@DocEntry\r\n                                            \r\n                                            update {1} set ObjType=@ObjCode\r\n                                            insert into {0} select * from {1} \r\n                                            update {2} set AbsEntry=@DocEntry\r\n                                            update {5} set AbsEntry=@DocEntry\r\n                                             \r\n                                            insert into {3} select * from {2} where {2}.ItemCode is not null and {2}.ItemCode <>''\r\n                                            insert into {4} select * from {5} where {5}.MdAbsEntry <>0 \r\n                                           \r\n", "APZ_OPKL", "#APZ_OPKL", "#APZ_PKL1", "APZ_PKL1", "APZ_PKL3", "#APZ_PKL3");
	}

	private bool Exists(DataSet doc)
	{
		try
		{
			string query = "SELECT 1 FROM APZ_OPKL WHERE \"AbsEntry\"=" + Function.ParseInt(doc.Tables["APZ_OPKL"].Rows[0]["AbsEntry"]) + ";";
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
		return Process(ApzObjectTypes.oPickLists, -1, Enums.TransactionType.C, new DataSet(), docEntry.ToString());
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

	public DataTable GetBatch(int absEntry, int pickentry)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_PKL3 where \"AbsEntry\"=@AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetConfirmed(int absEntry, int pickentry)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_PKL2 where \"AbsEntry\"=@AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetListBatchSerial(string itemCode, string whsCode, int pickentry, int docEntry, int orderNum, int orderLine)
	{
		try
		{
			return ExecuteData("apz_picking_cfl_batchserial", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode), new SqlParameter("@LineNum", pickentry), new SqlParameter("@DocEntry", docEntry), new SqlParameter("@OrderNum", orderNum), new SqlParameter("@OrderLine", orderLine));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable AutoSelectBatch(string itemcode, string whsCode, double quantity, int linenum, int docentry, int orderNum, int orderLine)
	{
		try
		{
			return ExecuteDataTable("apz_picking_AutoSelect", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemcode), new SqlParameter("@WhsCode", whsCode), new SqlParameter("@Quantity", quantity), new SqlParameter("@LineNum", linenum), new SqlParameter("@DocEntry", docentry), new SqlParameter("@OrderNum", orderNum), new SqlParameter("@OrderLine", orderLine));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet CreateNewInstance()
	{
		try
		{
			DataTable tableStruct = GetTableStruct("APZ_OPKL");
			DataTable tableStruct2 = GetTableStruct("APZ_PKL1");
			DataTable tableStruct3 = GetTableStruct("APZ_PKL2");
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[3]
			{
				tableStruct.Copy(),
				tableStruct2.Copy(),
				tableStruct3.Copy()
			});
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public HttpResult SaveList(List<DataSet> model)
	{
		try
		{
			List<int> list = new List<int>();
			foreach (DataSet item in model)
			{
				item.Tables["APZ_OPKL"].Rows[0]["AbsEntry"] = 0;
				HttpResult httpResult = Save(item);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return httpResult;
				}
				list.Add(Function.ParseInt(httpResult.Content));
			}
			return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet LoadBatchLine(int docEntry, string barcode, string type)
	{
		try
		{
			string query = "APZ_Picking_LoadBatchLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AbsEntry", docEntry));
			list.Add(new SqlParameter("@Barcode", barcode));
			list.Add(new SqlParameter("@Type", type));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult PickedConfirm(DataTable dt)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			HttpResult result = Process(ApzObjectTypes.oPickLists, -1, Enums.TransactionType.R, dataSet);
			dataSet.DisposeDataSet();
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private HttpResult AddConfirmed(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			BulkToTempTable(doc.Tables[0], "APZ_PKL2", "#APZ_PKL2");
			string value = Function.ToString(transtype);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable = ExecuteDataTable("Apz_Picking_AddConfirmBatch", CommandType.StoredProcedure, sqlParameter);
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					objNum = $"{Function.ToString(dataTable.Rows[0][1])}";
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	public HttpResult UnpickedConfirm(DataTable dt)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			HttpResult result = Process(ApzObjectTypes.oPickLists, -1, Enums.TransactionType.M, dataSet);
			dataSet.DisposeDataSet();
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
