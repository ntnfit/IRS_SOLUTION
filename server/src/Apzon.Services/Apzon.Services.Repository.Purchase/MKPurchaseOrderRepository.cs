using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Purchase;

namespace Apzon.Services.Repository.Purchase;

public class MKPurchaseOrderRepository(IDatabaseService databaseService) : BaseService<PurchaseOrder>(databaseService), IMKPurchaseOrder
{
	private List<PurchaseOrderLine> PurchaseLine = new List<PurchaseOrderLine>();

	private DataSet dset = new DataSet();

	private string tempOPOR = "#MKS_OPOR";

	private string tempPOR1 = "#MKS_POR1";

	public DataSet GetDoc(int docEntry)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "SELECT * FROM MKS_OPOR WHERE DocEntry=@DocEntry \r\n                               SELECT * FROM MKS_POR1 WHERE DocEntry=@DocEntry";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", docEntry));
			result = ExecuteData(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Purchase - Line ", ex.Message.ToString());
		}
		return result;
	}

	public string Save(Tuple<PurchaseOrder, List<PurchaseOrderLine>> doc)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			PurchaseLine = doc.Item2;
			return Process(ApzObjectTypes.oPaymentRequest, -1, transtype, doc.Item1).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateStatus(DataSet ds)
	{
		try
		{
			if (ds.Tables.Count == 1)
			{
				DataTable dataTable = ds.Tables[0];
				if (dataTable.IsNotNull())
				{
					int num = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
					if (ExecuteDataTable("SELECT 1 FROM ODRF WHERE U_BaseReqEntry=@DocEntry", CommandType.Text, new SqlParameter("@DocEntry", num)).IsNotNull())
					{
						string text = Function.ToString(dataTable.Rows[0]["WddStatus"]);
						string text2 = Function.ToString(dataTable.Rows[0]["Note"]);
						if (text2.Length > 300)
						{
							text2 = text2.Substring(0, 300);
						}
						if (num == 0)
						{
							return "Invalid request Id";
						}
						if (string.IsNullOrEmpty(text) || text == "-")
						{
							return "Invalid approval status";
						}
						string query = "UPDATE ODRF Set WddStatus=@Status WHERE U_BaseReqEntry=@DocEntry AND U_BaseReqNote=@Note";
						ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@Status", text), new SqlParameter("@DocEntry", num), new SqlParameter("@Note", text2 ?? ""));
						return "##";
					}
				}
			}
			Enums.TransactionType transtype = Enums.TransactionType.U;
			dset = ds;
			return Process(ApzObjectTypes.oPaymentRequest, -1, transtype, new PurchaseOrder()).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, PurchaseOrder doc, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			switch (transtype)
			{
			case Enums.TransactionType.A:
			{
				DataTable dataTable2 = new DataTable();
				PropertyInfo[] properties = typeof(PurchaseOrder).GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
					if (!(propertyInfo.Name == "BranchName") && !(propertyInfo.Name == "DocTypeName"))
					{
						dataTable2.Columns.Add(propertyInfo.Name, type);
					}
				}
				DataRow dataRow = dataTable2.NewRow();
				dataRow["DocEntry"] = doc.DocEntry;
				dataRow["DocType"] = doc.DocType;
				dataRow["CardName"] = doc.CardName;
				dataRow["CardCode"] = doc.CardCode;
				dataRow["DocDate"] = doc.DocDate;
				dataRow["DocCur"] = doc.DocCur;
				dataRow["DocRate"] = doc.DocRate;
				dataRow["LicTradNum"] = doc.LicTradNum;
				dataRow["BPLId"] = doc.BPLId;
				dataRow["DocDueDate"] = doc.DocDueDate;
				dataRow["TaxDate"] = doc.TaxDate;
				dataRow["JVNumber"] = doc.JVNumber;
				dataRow["Address"] = doc.Address;
				dataRow["Address2"] = doc.Address2;
				dataRow["Comments"] = doc.Comments;
				dataRow["VatSum"] = doc.VatSum;
				dataRow["DocTotal"] = doc.DocTotal;
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
				DataTable dataTable3 = new DataTable();
				properties = typeof(PurchaseOrderLine).GetProperties();
				foreach (PropertyInfo propertyInfo2 in properties)
				{
					Type type2 = Nullable.GetUnderlyingType(propertyInfo2.PropertyType) ?? propertyInfo2.PropertyType;
					dataTable3.Columns.Add(propertyInfo2.Name, type2);
				}
				foreach (PurchaseOrderLine item in PurchaseLine)
				{
					DataRow dataRow2 = dataTable3.NewRow();
					dataRow2["DocEntry"] = item.DocEntry;
					dataRow2["LineNum"] = item.LineNum;
					dataRow2["ItemCode"] = item.ItemCode;
					dataRow2["Dscription"] = item.Dscription;
					dataRow2["Quantity"] = item.Quantity;
					dataRow2["PriceBefDi"] = item.PriceBefDi;
					dataRow2["NumInBuy"] = item.NumInBuy;
					dataRow2["VatGroup"] = item.VatGroup;
					dataRow2["VatPrcnt"] = item.VatPrcnt;
					dataRow2["ShipDate"] = item.ShipDate;
					dataRow2["FreeTxt"] = item.FreeTxt;
					dataRow2["BuyUnitMsr"] = item.BuyUnitMsr;
					dataRow2["InvntryUom"] = item.InvntryUom;
					dataTable3.Rows.Add(dataRow2);
				}
				dataTable3.AcceptChanges();
				CreateTempTable(dataTable2, tempOPOR);
				BulkCopy(dataTable2, tempOPOR);
				CreateTempTable(dataTable3, tempPOR1);
				BulkCopy(dataTable3, tempPOR1);
				string text3 = "";
				text3 = "MKS_PurchaseOrder_Save";
				DataTable dataTable4 = ExecuteDataTable(text3, CommandType.StoredProcedure, (SqlParameter[])null);
				if (dataTable4 != null && dataTable4.Rows.Count > 0)
				{
					httpResult.Message = Function.ToString(dataTable4.Rows[0]["ErrorMessage"]);
					string text4 = Function.ToString(dataTable4.Rows[0]["ErrorNumber"]);
					httpResult.Message = text4 + " - " + httpResult.Message;
				}
				else
				{
					httpResult.Message = "";
				}
				break;
			}
			case Enums.TransactionType.U:
				if (dset.Tables.Count > 0)
				{
					CreateTempTable(dset.Tables[0], tempOPOR);
					BulkCopy(dset.Tables[0], tempOPOR);
					string text = "";
					text = "MKS_PurchaseOrder_UpdateStatus";
					DataTable dataTable = ExecuteDataTable(text, CommandType.StoredProcedure, (SqlParameter[])null);
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						httpResult.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
						string text2 = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
						httpResult.Message = text2 + " - " + httpResult.Message;
					}
					else
					{
						httpResult.Message = "";
					}
				}
				break;
			}
			objId = "";
			objNum = "";
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string Approval(DataTable dt)
	{
		int num = Function.ParseInt(dt.Rows[0]["DocEntry"]);
		string text = Function.ToString(dt.Rows[0]["WddStatus"]);
		string text2 = Function.ToString(dt.Rows[0]["Note"]);
		if (text2.Length > 300)
		{
			text2 = text2.Substring(0, 300);
		}
		if (num == 0)
		{
			return "Invalid request Id";
		}
		if (string.IsNullOrEmpty(text) || text == "-")
		{
			return "Invalid approval status";
		}
		string query = "UPDATE ODRF Set WddStatus=@Status,U_BaseReqNote=@Note WHERE U_BaseReqEntry=@DocEntry ";
		ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@Status", text), new SqlParameter("@DocEntry", num), new SqlParameter("@Note", text2 ?? ""));
		return "";
	}
}
