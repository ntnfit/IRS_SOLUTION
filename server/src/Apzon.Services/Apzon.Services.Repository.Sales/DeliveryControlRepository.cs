using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Sales;
using Newtonsoft.Json;

namespace Apzon.Services.Repository.Sales;

public class DeliveryControlRepository : BaseService<DocumentsImpl>, IDeliveryControl
{
	public DeliveryControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet Get(DataTable dt)
	{
		try
		{
			string query = "apz_deliverycontrol_get";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@ToDate", dt.Rows[0]["todate"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@BatchNo", dt.Rows[0]["batchno"]));
				list.Add(new SqlParameter("@Status", dt.Rows[0]["status"]));
				list.Add(new SqlParameter("@UserName", dt.Rows[0]["username"]));
				list.Add(new SqlParameter("@Branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@Whs", dt.Rows[0]["whs"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetProduction(DataTable dt)
	{
		try
		{
			string query = "Apz_DeliveryControl_GetProduction";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@ToDate", dt.Rows[0]["todate"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@BatchNo", dt.Rows[0]["batchno"]));
				list.Add(new SqlParameter("@Status", dt.Rows[0]["status"]));
				list.Add(new SqlParameter("@UserName", dt.Rows[0]["username"]));
				list.Add(new SqlParameter("@Branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@Whs", dt.Rows[0]["whs"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet ListOfDelivery(DataTable dt)
	{
		try
		{
			string query = "Apz_DeliveryControl_ListOfDelivery";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@ToDate", dt.Rows[0]["todate"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@BatchNo", dt.Rows[0]["batchno"]));
				list.Add(new SqlParameter("@Status", dt.Rows[0]["status"]));
				list.Add(new SqlParameter("@UserName", dt.Rows[0]["username"]));
				list.Add(new SqlParameter("@Branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@Whs", dt.Rows[0]["whs"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet ListOfInvoice(DataTable dt)
	{
		try
		{
			string query = "Apz_DeliveryControl_ListOfInvoice";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@ToDate", dt.Rows[0]["todate"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@BatchNo", dt.Rows[0]["batchno"]));
				list.Add(new SqlParameter("@Status", dt.Rows[0]["status"]));
				list.Add(new SqlParameter("@UserName", dt.Rows[0]["username"]));
				list.Add(new SqlParameter("@Branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@Whs", dt.Rows[0]["whs"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet ListInvoiceForPayment(DataTable dt)
	{
		try
		{
			string query = "Apz_DeliveryControl_ListInvoiceForPayment";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@ToDate", dt.Rows[0]["todate"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@BatchNo", dt.Rows[0]["batchno"]));
				list.Add(new SqlParameter("@Status", dt.Rows[0]["status"]));
				list.Add(new SqlParameter("@UserName", dt.Rows[0]["username"]));
				list.Add(new SqlParameter("@Branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@Whs", dt.Rows[0]["whs"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetListShipers(string username, int branch)
	{
		try
		{
			string query = "Apz_DeliveryOrder_GetListShipers";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@Username", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListBranchs(string username, int branch)
	{
		try
		{
			string query = "Apz_DeliveryOrder_GetListBranchs";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@Username", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetBatch(int orderNo, int orderLine)
	{
		try
		{
			return ExecuteData("apz_delivery_control_getbatch", CommandType.StoredProcedure, new SqlParameter("@DocEntry", orderNo), new SqlParameter("@DocLine", orderLine));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadInvoices(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", id);
			return ExecuteData("apz_deliverycontrol_invoices", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadItems(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", id);
			return ExecuteData("apz_deliverycontrol_items", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadSchedule(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", id);
			return ExecuteData("Apz_Deliverycontrol_Schedule", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetById(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", id);
			return ExecuteDataTable("select * from [@ODLO] where DocEntry =@DocEntry", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int GetPage(int current, int type, string username, int branch)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_deliverycontrol_navigation", CommandType.StoredProcedure, new SqlParameter("@Current", current), new SqlParameter("@Type", type), new SqlParameter("@Username", username), new SqlParameter("@Branch", branch));
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataSet GetBatchDelivery(string docentry, string userName, int branch)
	{
		try
		{
			string query = "Apz_DeliveryOrder_GetBatchDelivery";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(docentry))
			{
				list.Add(new SqlParameter("@DocEntry", docentry));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@UserName", userName));
			}
			if (branch > -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetInvoice(int docentry)
	{
		try
		{
			string query = "Apz_DeliveryOrder_GetInvoice";
			List<SqlParameter> list = new List<SqlParameter>();
			if (docentry != -1)
			{
				list.Add(new SqlParameter("@DocEntry", docentry));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string ReturnDelRequestBatch(DataTable dtBatch)
	{
		try
		{
			string text = "";
			if (dtBatch.IsNotNull() && dtBatch.IsExistsColumn("U_BatchNo"))
			{
				for (int i = 0; i < dtBatch.Rows.Count; i++)
				{
					text = text + "," + Function.ToString(dtBatch.Rows[i]["U_BatchNo"]);
				}
			}
			if (!string.IsNullOrEmpty(text) && text.StartsWith(","))
			{
				text = text.Substring(1, text.Length - 1);
			}
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Batch", text));
			if (ExecuteDataTable("Apz_Productionforbatch_ReturnDelRequestBatch", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
			{
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Update False!";
	}

	public DataTable CheckDelivery(string docentry)
	{
		try
		{
			string query = "Apz_DeliveryOrder_CheckDelivery";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(docentry))
			{
				list.Add(new SqlParameter("@DocEntry", docentry));
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string UpdatePrintedInvoice(string docentry, string table, string type)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", docentry));
			list.Add(new SqlParameter("@Table", table));
			list.Add(new SqlParameter("@Type", type));
			if (ExecuteDataTable("Apz_DeliveryOrder_UpdatePrintedInvoice", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
			{
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Update Printed False!";
	}

	public DataSet SearchDeliveryOrder(DocumentsImpl doc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (doc.DocEntry > 0)
			{
				list.Add(new SqlParameter("@DocEntry", doc.DocEntry));
			}
			if (!string.IsNullOrEmpty(doc.CardCode))
			{
				list.Add(new SqlParameter("@EmpCode", doc.CardCode));
			}
			if (!string.IsNullOrEmpty(doc.CardName))
			{
				list.Add(new SqlParameter("@CarLicense", doc.CardName));
			}
			if (!string.IsNullOrEmpty(doc.DocStatus))
			{
				list.Add(new SqlParameter("@Status", doc.DocStatus));
			}
			if (!string.IsNullOrEmpty(doc.Ref1))
			{
				list.Add(new SqlParameter("@BatchNo", doc.Ref1));
			}
			if (!string.IsNullOrEmpty(doc.Ref2))
			{
				list.Add(new SqlParameter("@Barcode", doc.Ref2));
			}
			if (!string.IsNullOrEmpty(doc.UserName))
			{
				list.Add(new SqlParameter("@UserName", doc.UserName));
			}
			if (doc.Branch > 0)
			{
				list.Add(new SqlParameter("@Branch", doc.Branch));
			}
			if (doc.CreateDate > new DateTime(1900, 1, 1))
			{
				list.Add(new SqlParameter("@CreateDate", doc.CreateDate));
			}
			if (doc.DocDate > new DateTime(1900, 1, 1))
			{
				list.Add(new SqlParameter("@ConfirmDate", doc.DocDate));
			}
			return ExecuteData("Apz_DeliveryOrder_SearchDeliveryOrder", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSetResult GetDataByBarcode(DataSet model)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (model.Tables.Count < 2)
				{
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), JsonConvert.SerializeObject(model));
					return new DataSetResult("Dữ liệu gửi lên bị thiếu");
				}
				if (model.Tables[1].IsNotNull())
				{
					CreateTempTable(model.Tables[1], "#TempBatches");
					BulkCopy(model.Tables[1], "#TempBatches");
				}
				string value = Function.ToString(model.Tables[0].Rows[0]["Barcode"]);
				string value2 = Function.ToString(model.Tables[0].Rows[0]["DraftKey"]);
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter("@barcode", value),
					new SqlParameter("@draftKey", value2)
				};
				DataSet data = ExecuteData("Apz_Delivery_GetDataByBarcode", CommandType.StoredProcedure, list.ToArray());
				dbTransaction.Commit();
				return new DataSetResult("", data);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataSetResult GetDataByDraftKey(string barcode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode)
			};
			return new DataSetResult("", ExecuteData("Apz_Delivery_GetDataByDraftKey", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}
}
