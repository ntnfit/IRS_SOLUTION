using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

[ValidateLoginSapExceptionHandling]
public class InventoryTranferController : BaseApiController
{
	[HttpGet]
	public DataSet GetDataCopyFrom(string lstDocEntry, ApzObjectTypes fromObj)
	{
		return UnitOfWork.InventoryTranfer.GetDataCopyFrom(lstDocEntry, fromObj);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.InventoryTranfer.GetMaxDocEntry();
	}

	[HttpPost]
	public DataTable Search([FromBody] DocumentsImpl doc)
	{
		return UnitOfWork.InventoryTranfer.Search(doc);
	}

	[HttpPost]
	public DataTable SearchLine([FromBody] Document_LinesImpl line)
	{
		return UnitOfWork.InventoryTranfer.SearchLine(line);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveDoc([FromBody] DocumentsImpl model)
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
				return unitOfWork.StockTransfer.Save(model, SessionUserInfo.SessionDiServer, isDraft: false);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveDataSet([FromBody] DataSet model)
	{
		if (!Global.LoginSap)
		{
			return new HttpResult(Enums.MessageCode.Success, UnitOfWork.InventoryTranfer.SaveDoc(model));
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveAsDraft([FromBody] DocumentsImpl model)
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
				return unitOfWork.StockTransfer.Save(model, SessionUserInfo.SessionDiServer, isDraft: true);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveAsDraftDataSet([FromBody] DataSet model)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Documents.SaveAsDraftDataSet(model);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable LoadSeries(string usercode)
	{
		return UnitOfWork.Documents.LoadSeries(usercode, ApzObjectTypes.oStockTransfer);
	}

	[HttpGet]
	public int GetNewDocNum()
	{
		return UnitOfWork.InventoryTranfer.GetNewDocNum();
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveTransferBin([FromBody] Tuple<DocumentsImpl, DataSet> model)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.StockTransfer.SaveTransferBin(model, SessionUserInfo.SessionDiServer);
			}
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_FunctionNotSupported);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	[QueueModifiedAction]
	public string DeleteLine(int docEntry, int lineNum)
	{
		return UnitOfWork.InventoryTranfer.DeleteLine(docEntry, lineNum);
	}

	[HttpPost]
	[QueueModifiedAction]
	public string CloseDoc([FromBody] int docEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).StockTransfer.Close(docEntry, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.InventoryTranfer.CloseDoc(docEntry);
	}

	[HttpPost]
	[QueueModifiedAction]
	public string CancelDoc([FromBody] int docEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).StockTransfer.Cancel(docEntry, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.InventoryTranfer.CancelDoc(docEntry);
	}

	[HttpGet]
	public string SaveInventoryTransferRequestDetail(string docs)
	{
		return UnitOfWork.InventoryTranfer.SaveInventoryTransferRequestDetail(docs);
	}

	[HttpGet]
	public DataTable GetDataDoc(string cardCode, int docNum, int docEntry, string docStatus, int prilist, string fromDate, string toDate, Enums.NavigateType type)
	{
		return UnitOfWork.InventoryTranfer.GetDataDoc(cardCode, docNum, docEntry, docStatus, prilist, fromDate, toDate, type);
	}

	[HttpGet]
	public DataSet GetDataLine(int docEntry)
	{
		return UnitOfWork.InventoryTranfer.GetDataLine(docEntry);
	}

	[HttpPost]
	public string CancelLine([FromBody] Tuple<string, string, string, string, string, string, string> model)
	{
		return UnitOfWork.InventoryTranfer.CancelLine(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
	}

	[HttpGet]
	public int GetDocNumByDocEntry(int docEntry)
	{
		return UnitOfWork.InventoryTranfer.GetDocNumByDocEntry(docEntry);
	}

	[HttpPost]
	[QueueModifiedAction]
	public DataTable GetApprovalTemplates([FromBody] Tuple<DocumentsImpl, string, string> item)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new DataTable();
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.GetApprovalTemplate(item.Item1, SessionUserInfo.SessionDiServer, item.Item2, item.Item3, ApzObjectTypes.oStockTransfer);
		}
		return new DataTable();
	}

	[HttpPost]
	public HttpResult SaveDraftToDocument([FromBody] int docEntry)
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
					return unitOfWork.StockTransfer.SaveDraftToDocument(docEntry, SessionUserInfo.SessionDiServer);
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpGet]
	public DataSet GetItemByBarcode(string barcode, string type = "N")
	{
		return UnitOfWork.InventoryTranfer.GetItemByBarcode(barcode, type);
	}

	[HttpGet]
	public DataTable GetWhsByBin(string binCode)
	{
		return UnitOfWork.InventoryTranfer.GetWhsByBin(binCode);
	}

	[HttpGet]
	public string CreatePalletNo(int binId)
	{
		return UnitOfWork.InventoryTranfer.CreatePalletNo(binId);
	}

	[HttpPost]
	public HttpResult AddTransferMobileApp([FromBody] DataTable dt)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
		if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
		{
			return unitOfWork.StockTransfer.AddTransferMobileApp(dt, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
	}

	[HttpPost]
	public HttpResult AddTransferFromRequest([FromBody] string requestKey)
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
				int docEntryByDocNumType = UnitOfWork.Documents.GetDocEntryByDocNumType(Function.ParseInt(requestKey), 1250000001);
				DataTable dataTable = UnitOfWork.Documents.CopyFrom(1250000001.ToString(), docEntryByDocNumType.ToString(), 67.ToString(), -1, "");
				DataTable dataTable2 = UnitOfWork.Documents.CopyFromLine(1250000001.ToString(), docEntryByDocNumType.ToString(), 67.ToString());
				DocumentsImpl documentsImpl = new DocumentsImpl();
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
						DataTable dataTable3 = UnitOfWork.Documents.CopyBatchSerial(docEntryByDocNumType.ToString(), 1250000001.ToString());
						if (dataTable3.IsNotNull())
						{
							if (!DocumentsHelper.BatchSerialNumbersStruct.IsNotNull())
							{
								DocumentsHelper.BatchSerialNumbersStruct = UnitOfWork.Documents.LoadBatchNumberStruct(ApzObjectTypes.oInventoryTransferRequest);
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
					documentsImpl.DocObjectCode = ApzObjectTypes.oStockTransfer;
					documentsImpl.DocDate = DateTime.Today;
					return unitOfWork.StockTransfer.Save(documentsImpl, SessionUserInfo.SessionDiServer, isDraft: false);
				}
				return new HttpResult(Enums.MessageCode.None, "Data not found!");
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
