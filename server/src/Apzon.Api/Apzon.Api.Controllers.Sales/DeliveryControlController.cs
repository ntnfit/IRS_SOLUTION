using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.Sales;

public class DeliveryControlController : BaseApiController
{
	[HttpPost]
	public DataSet Get([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.DeliveryControl.Get(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet GetProduction([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetProduction(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet ListOfDelivery([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.DeliveryControl.ListOfDelivery(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet ListOfInvoice([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.DeliveryControl.ListOfInvoice(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet ListInvoiceForPayment([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.DeliveryControl.ListInvoiceForPayment(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetListShipers(string username, int branch)
	{
		return UnitOfWork.DeliveryControl.GetListShipers(username, branch);
	}

	[HttpGet]
	public DataTable GetListBranchs(string username, int branch)
	{
		return UnitOfWork.DeliveryControl.GetListBranchs(username, branch);
	}

	[HttpGet]
	public DataSet GetBatch(int orderNo, int orderLine)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetBatch(orderNo, orderLine);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult CreateInvoices([FromBody] List<DocumentsImpl> lst)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.Invoices.CreateInvoices(lst, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult CreateDelivery([FromBody] List<DocumentsImpl> lst)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.Delivery.CreateDelivery(lst, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpPost]
	public string ReturnDelRequestBatch([FromBody] DataTable dtBatch)
	{
		try
		{
			return UnitOfWork.DeliveryControl.ReturnDelRequestBatch(dtBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataSet LoadInvoices(int id)
	{
		try
		{
			return UnitOfWork.DeliveryControl.LoadInvoices(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet LoadItems(int id)
	{
		try
		{
			return UnitOfWork.DeliveryControl.LoadItems(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet LoadSchedule(int id)
	{
		try
		{
			return UnitOfWork.DeliveryControl.LoadSchedule(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetDoc(int id)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public int GetPage(int currentId, int type, string username, int branch)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetPage(currentId, type, username, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	[HttpGet]
	public DataSet GetBatchDelivery(string docentry, string userName, int branch)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetBatchDelivery(docentry, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetInvoice(int docentry)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetInvoice(docentry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable CheckDelivery(string docentry)
	{
		try
		{
			return UnitOfWork.DeliveryControl.CheckDelivery(docentry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public string UpdatePrintedInvoice(string docentry, string table, string type)
	{
		try
		{
			return UnitOfWork.DeliveryControl.UpdatePrintedInvoice(docentry, table, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public DataSet SearchDeliveryOrder([FromBody] DocumentsImpl doc)
	{
		try
		{
			return UnitOfWork.DeliveryControl.SearchDeliveryOrder(doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSetResult GetDataByDraftKey(string barcode)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetDataByDraftKey(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	[HttpPost]
	public DataSetResult GetDataByBarcode([FromBody] DataSet model)
	{
		try
		{
			return UnitOfWork.DeliveryControl.GetDataByBarcode(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), JsonConvert.SerializeObject(model));
			return new DataSetResult(ex.Message);
		}
	}

	[HttpPost]
	public async Task<HttpResult> AddDeliveryFromDraft([FromBody] string draftkey)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				DataTable dataTable = UnitOfWork.Documents.CopyFrom(112.ToString(), draftkey, 15.ToString(), -1, "");
				DataTable dataTable2 = UnitOfWork.Documents.CopyFromLine(112.ToString(), draftkey, 15.ToString());
				new DocumentsImpl();
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					DocumentsImpl documentsImpl = dataTable.Rows[0].ToDocument();
					documentsImpl.DocEntry = 0;
					List<Document_LinesImpl> list = new List<Document_LinesImpl>();
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						list = dataTable2.ToDocumentLine();
						list = list.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
						if (list.Count > 0)
						{
							DataTable dataTable3 = UnitOfWork.Documents.CopyBatchSerial(draftkey, 112.ToString());
							if (dataTable3.IsNotNull())
							{
								if (!DocumentsHelper.BatchSerialNumbersStruct.IsNotNull())
								{
									DocumentsHelper.BatchSerialNumbersStruct = UnitOfWork.Documents.LoadBatchNumberStruct(ApzObjectTypes.oDrafts);
								}
								DataTable dataTable4 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
								string[] columnNames = (from DataColumn t in dataTable4.Columns
									select t.ColumnName).ToArray();
								dataTable3 = dataTable3.DefaultView.ToTable("Source", distinct: false, columnNames);
								DataRow dataRow = dataTable4.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
								if (dataRow != null)
								{
									dataTable4.Rows.Remove(dataRow);
									dataTable4.AcceptChanges();
								}
								foreach (DataRow row in dataTable3.Rows)
								{
									DataRow dataRow3 = dataTable4.NewRow();
									dataRow3.ItemArray = row.ItemArray;
									dataTable4.Rows.Add(dataRow3);
								}
							}
							foreach (Document_LinesImpl line in list)
							{
								DataRow[] array = (from t in dataTable3.AsEnumerable()
									where Function.ParseInt(t["BaseLineNumber"]) == line.BaseLine
									select t).ToArray();
								if (array.Length != 0)
								{
									DataRow[] array2 = array;
									for (int num = 0; num < array2.Length; num++)
									{
										array2[num]["BaseLineNumber"] = line.LineNum;
									}
									line.ToDocumentLineBatchSerial(array.CopyToDataTable(), ApzObjectTypes.oInventoryTransferRequest);
								}
							}
						}
					}
					if (documentsImpl != null)
					{
						documentsImpl.Lines = list;
						documentsImpl.DocObjectCode = ApzObjectTypes.oDeliveryNotes;
						documentsImpl.DocDate = DateTime.Today;
						return await unitOfWork.Documents.Save(documentsImpl, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDeliveryNotes);
					}
					return new HttpResult(Enums.MessageCode.Error, "Add delivery error!");
				}
				return new HttpResult(Enums.MessageCode.Error, "Add delivery error!");
			}
			return new HttpResult(Enums.MessageCode.Error, "Function not support!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public async Task<HttpResult> AddDeliveryFromDraftWithBatch([FromBody] DataSet model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (unitOfWork == null)
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				if (model.Tables.Count < 2 || !model.Tables[1].IsNotNull())
				{
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), JsonConvert.SerializeObject(model));
					return new HttpResult(Enums.MessageCode.Error, "Dữ liệu gửi lên bị thiếu");
				}
				string entry = Function.ToString(model.Tables[0].Rows[0]["DocEntry"]);
				DataTable dataTable = UnitOfWork.Documents.CopyFrom(112.ToString(), entry, 15.ToString(), -1, "");
				DataTable dataTable2 = UnitOfWork.Documents.CopyFromLine(112.ToString(), entry, 15.ToString());
				new DocumentsImpl();
				DocumentsImpl documentsImpl;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					documentsImpl = dataTable.Rows[0].ToDocument();
					documentsImpl.DocEntry = 0;
					List<string> list = (from DataColumn t in model.Tables[0].Columns
						where t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList();
					if (list.Count > 0)
					{
						foreach (string udfCol in list)
						{
							UserFieldsImpl userFieldsImpl = documentsImpl.UserFields.Where((UserFieldsImpl o) => o.Name.Equals(udfCol)).FirstOrDefault();
							if (userFieldsImpl != null)
							{
								userFieldsImpl.Value = model.Tables[0].Rows[0][udfCol];
							}
						}
					}
				}
				else
				{
					documentsImpl = null;
				}
				List<Document_LinesImpl> list2 = new List<Document_LinesImpl>();
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					list2 = dataTable2.ToDocumentLine();
					list2 = list2.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
					if (list2.Count > 0)
					{
						DataTable source = model.Tables[1];
						foreach (Document_LinesImpl line in list2)
						{
							DataRow[] array = (from t in source.AsEnumerable()
								where Function.ToString(t["ItemCode"]).Equals(line.ItemCode) && Function.ParseInt(t["LineNum"]) == line.LineNum
								select t).ToArray();
							if (array.Length != 0)
							{
								line.Quantity = 0.0;
								DataTable dataTable3 = array.CopyToDataTable();
								line.BatchNumbers = new List<BatchNumbersImpl>();
								for (int num = 0; num < dataTable3.Rows.Count; num++)
								{
									DataRow dataRow = dataTable3.Rows[num];
									BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
									batchNumbersImpl.BaseLineNumber = line.LineNum;
									batchNumbersImpl.BatchNumber = Function.ToString(dataRow["DistNumber"]);
									batchNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
									line.BatchNumbers.Add(batchNumbersImpl);
									line.Quantity += batchNumbersImpl.Quantity;
								}
							}
						}
					}
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = list2;
					documentsImpl.DocObjectCode = ApzObjectTypes.oDeliveryNotes;
					documentsImpl.DocDate = DateTime.Today;
					return await unitOfWork.Documents.Save(documentsImpl, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDeliveryNotes, ApzObjectTypes.oDeliveryNotes);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpPost]
	public async Task<HttpResult> UpdateDraftWithBatch([FromBody] DataSet model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (unitOfWork == null)
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				if (model.Tables.Count < 2 || !model.Tables[1].IsNotNull())
				{
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), JsonConvert.SerializeObject(model));
					return new HttpResult(Enums.MessageCode.Error, "Dữ liệu gửi lên bị thiếu");
				}
				int num = 0;
				if (model.Tables[0].Rows[0].IsExistsColumn("DraftKey"))
				{
					num = Function.ParseInt(model.Tables[0].Rows[0]["DraftKey"]);
				}
				else if (model.Tables[0].Rows[0].IsExistsColumn("DocEntry"))
				{
					num = Function.ParseInt(model.Tables[0].Rows[0]["DocEntry"]);
				}
				DataTable dataTable = UnitOfWork.Documents.Search(new DocumentsImpl
				{
					DocEntry = num
				}, ApzObjectTypes.oDrafts);
				DataTable dataTable2 = UnitOfWork.Documents.TableLine(num, ApzObjectTypes.oDrafts);
				new DocumentsImpl();
				DocumentsImpl documentsImpl;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					documentsImpl = dataTable.Rows[0].ToDocument();
					documentsImpl.draftKey = num;
					List<string> list = (from DataColumn t in model.Tables[0].Columns
						where t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList();
					if (list.Count > 0)
					{
						foreach (string udfCol in list)
						{
							UserFieldsImpl userFieldsImpl = documentsImpl.UserFields.Where((UserFieldsImpl o) => o.Name.Equals(udfCol)).FirstOrDefault();
							if (userFieldsImpl != null)
							{
								userFieldsImpl.Value = model.Tables[0].Rows[0][udfCol];
							}
						}
					}
				}
				else
				{
					documentsImpl = null;
				}
				List<Document_LinesImpl> list2 = new List<Document_LinesImpl>();
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					list2 = dataTable2.ToDocumentLine();
					list2 = list2.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
					if (list2.Count > 0)
					{
						DataTable source = model.Tables[1];
						foreach (Document_LinesImpl line in list2)
						{
							DataRow[] array = (from t in source.AsEnumerable()
								where Function.ToString(t["ItemCode"]).Equals(line.ItemCode) && Function.ParseInt(t["LineNum"]) == line.LineNum
								select t).ToArray();
							if (array.Length != 0)
							{
								line.Quantity = 0.0;
								DataTable dataTable3 = array.CopyToDataTable();
								line.BatchNumbers = new List<BatchNumbersImpl>();
								for (int num2 = 0; num2 < dataTable3.Rows.Count; num2++)
								{
									DataRow dataRow = dataTable3.Rows[num2];
									BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
									batchNumbersImpl.BaseLineNumber = line.LineNum;
									batchNumbersImpl.BatchNumber = Function.ToString(dataRow["DistNumber"]);
									batchNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
									line.BatchNumbers.Add(batchNumbersImpl);
									line.Quantity += batchNumbersImpl.Quantity;
								}
							}
						}
					}
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = list2;
					documentsImpl.DocObjectCode = ApzObjectTypes.oDeliveryNotes;
					documentsImpl.DocDate = DateTime.Today;
					return await unitOfWork.Documents.Save(documentsImpl, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDrafts, ApzObjectTypes.oDeliveryNotes);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpPost]
	public async Task<HttpResult> AddDeliveryFromOrder([FromBody] string orderKey)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.None, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				DataTable dataTable = UnitOfWork.Documents.CopyFrom(17.ToString(), orderKey, 15.ToString(), -1, "");
				DataTable dataTable2 = UnitOfWork.Documents.CopyFromLine(17.ToString(), orderKey, 15.ToString());
				new DocumentsImpl();
				DocumentsImpl documentsImpl;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					documentsImpl = dataTable.Rows[0].ToDocument();
					documentsImpl.DocEntry = 0;
				}
				else
				{
					documentsImpl = null;
				}
				List<Document_LinesImpl> list = new List<Document_LinesImpl>();
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					list = dataTable2.ToDocumentLine();
					list = list.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
					if (list.Count > 0)
					{
						DataTable dataTable3 = UnitOfWork.Documents.CopyBatchSerial(orderKey, 17.ToString());
						if (dataTable3.IsNotNull())
						{
							if (!DocumentsHelper.BatchSerialNumbersStruct.IsNotNull())
							{
								DocumentsHelper.BatchSerialNumbersStruct = UnitOfWork.Documents.LoadBatchNumberStruct(ApzObjectTypes.oOrders);
							}
							DataTable dataTable4 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
							string[] columnNames = (from DataColumn t in dataTable4.Columns
								select t.ColumnName).ToArray();
							dataTable3 = dataTable3.DefaultView.ToTable("Source", distinct: false, columnNames);
							DataRow dataRow = dataTable4.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
							if (dataRow != null)
							{
								dataTable4.Rows.Remove(dataRow);
								dataTable4.AcceptChanges();
							}
							foreach (DataRow row in dataTable3.Rows)
							{
								DataRow dataRow3 = dataTable4.NewRow();
								dataRow3.ItemArray = row.ItemArray;
								dataTable4.Rows.Add(dataRow3);
							}
						}
						foreach (Document_LinesImpl line in list)
						{
							DataRow[] array = (from t in dataTable3.AsEnumerable()
								where Function.ParseInt(t["BaseLineNumber"]) == line.BaseLine
								select t).ToArray();
							if (array.Length != 0)
							{
								DataRow[] array2 = array;
								for (int num = 0; num < array2.Length; num++)
								{
									array2[num]["BaseLineNumber"] = line.LineNum;
								}
								line.ToDocumentLineBatchSerial(array.CopyToDataTable(), ApzObjectTypes.oOrders);
							}
						}
					}
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = list;
					documentsImpl.DocObjectCode = ApzObjectTypes.oDeliveryNotes;
					documentsImpl.DocDate = DateTime.Today;
					return await unitOfWork.Documents.Save(documentsImpl, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDeliveryNotes);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpPost]
	public async Task<HttpResult> AddDeliveryFromOrderWithBatch([FromBody] DataSet model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (model.Tables.Count < 2 || !model.Tables[1].IsNotNull())
				{
					Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), JsonConvert.SerializeObject(model));
					return new HttpResult(Enums.MessageCode.Error, "Dữ liệu gửi lên bị thiếu");
				}
				string entry = Function.ToString(model.Tables[1].Rows[0]["DocEntry"]);
				DataTable dataTable = UnitOfWork.Documents.CopyFrom(17.ToString(), entry, 15.ToString(), -1, "");
				DataTable dataTable2 = UnitOfWork.Documents.CopyFromLine(17.ToString(), entry, 15.ToString());
				new DocumentsImpl();
				DocumentsImpl documentsImpl;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					documentsImpl = dataTable.Rows[0].ToDocument();
					documentsImpl.DocEntry = 0;
					List<string> list = (from DataColumn t in model.Tables[0].Columns
						where t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList();
					if (list.Count > 0)
					{
						foreach (string udfCol in list)
						{
							UserFieldsImpl userFieldsImpl = documentsImpl.UserFields.Where((UserFieldsImpl o) => o.Name.Equals(udfCol)).FirstOrDefault();
							if (userFieldsImpl != null)
							{
								userFieldsImpl.Value = model.Tables[0].Rows[0][udfCol];
							}
						}
					}
				}
				else
				{
					documentsImpl = null;
				}
				List<Document_LinesImpl> list2 = new List<Document_LinesImpl>();
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					list2 = dataTable2.ToDocumentLine();
					list2 = list2.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
					if (list2.Count > 0)
					{
						DataTable source = model.Tables[1];
						foreach (Document_LinesImpl line in list2)
						{
							DataRow[] array = (from t in source.AsEnumerable()
								where !string.IsNullOrEmpty(Function.ToString(t["DistNumber"])) && Function.ToString(t["ItemCode"]).Equals(line.ItemCode)
								select t).ToArray();
							if (array.Length != 0)
							{
								line.Quantity = 0.0;
								if (line.WhsCode != Function.ToString(array[0]["Whs"]))
								{
									line.WhsCode = Function.ToString(array[0]["Whs"]);
								}
								DataTable dataTable3 = array.CopyToDataTable();
								line.BatchNumbers = new List<BatchNumbersImpl>();
								for (int num = 0; num < dataTable3.Rows.Count; num++)
								{
									DataRow dataRow = dataTable3.Rows[num];
									BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
									batchNumbersImpl.BaseLineNumber = line.LineNum;
									batchNumbersImpl.BatchNumber = Function.ToString(dataRow["DistNumber"]);
									batchNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
									line.BatchNumbers.Add(batchNumbersImpl);
									line.Quantity += batchNumbersImpl.Quantity;
								}
							}
						}
					}
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = list2;
					documentsImpl.DocObjectCode = ApzObjectTypes.oDeliveryNotes;
					documentsImpl.DocDate = DateTime.Today;
					return await unitOfWork.Documents.Save(documentsImpl, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDeliveryNotes);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}
}
