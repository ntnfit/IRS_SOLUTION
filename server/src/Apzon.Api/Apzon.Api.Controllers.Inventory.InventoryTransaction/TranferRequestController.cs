using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

[ValidateLoginSapExceptionHandling]
public class TranferRequestController : BaseApiController
{
	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.TranferRequest.GetMaxDocEntry();
	}

	[HttpPost]
	public DataTable Search([FromBody] DocumentsImpl doc)
	{
		return UnitOfWork.TranferRequest.Search(doc);
	}

	[HttpPost]
	public DataTable SearchLine([FromBody] Document_LinesImpl line)
	{
		return UnitOfWork.TranferRequest.SearchLine(line);
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveDoc([FromBody] DocumentsImpl doc)
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
				return await unitOfWork.StockTransferRequest.Save(doc, isDraft: false, SessionUserInfo.SessionDiServer);
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
			return new HttpResult(Enums.MessageCode.Success, UnitOfWork.TranferRequest.SaveDoc(model));
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
				return await unitOfWork.StockTransferRequest.Save(model, isDraft: true, SessionUserInfo.SessionDiServer);
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
			return BrokerService.GetUnitOfWork(SessionId).StockTransferRequest.Close(docEntry, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.TranferRequest.CloseDoc(docEntry);
	}

	[HttpPost]
	[QueueModifiedAction]
	public string CancelLine([FromBody] Tuple<string, string, string, string, string, string, string> model)
	{
		return UnitOfWork.TranferRequest.CancelLine(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6, model.Item7);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteLine(int docEntry, int lineNum)
	{
		return UnitOfWork.TranferRequest.DeleteLine(docEntry, lineNum);
	}

	[HttpPost]
	public DataTable GetCopyLines([FromBody] List<int> docEntries)
	{
		return UnitOfWork.TranferRequest.GetCopyLines(docEntries);
	}

	[HttpGet]
	public DataTable LoadSeries(string usercode)
	{
		return UnitOfWork.Documents.LoadSeries(usercode, ApzObjectTypes.oInventoryTransferRequest);
	}

	[HttpGet]
	public int GetNewDocNum()
	{
		return UnitOfWork.TranferRequest.GetNewDocNum();
	}

	[HttpGet]
	public DataSet GetDataLine(int docEntry)
	{
		return UnitOfWork.TranferRequest.GetDataLine(docEntry);
	}

	[HttpGet]
	public DataTable GetDataDoc(string cardCode, int docNum, int docEntry, string docStatus, int prilist, string fromDate, string toDate, Enums.NavigateType type)
	{
		return UnitOfWork.TranferRequest.GetDataDoc(cardCode, docNum, docEntry, docStatus, prilist, fromDate, toDate, type);
	}

	[HttpPost]
	public DataTable GetApprovalTemplates([FromBody] Tuple<DocumentsImpl, string, string> item)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new DataTable();
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.GetApprovalTemplate(item.Item1, SessionUserInfo.SessionDiServer, item.Item2, item.Item3, ApzObjectTypes.oInventoryTransferRequest);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataSet GetDataCopyFrom(string docEntry, ApzObjectTypes fromObj)
	{
		return UnitOfWork.TranferRequest.GetDataCopyFrom(docEntry, fromObj);
	}

	[HttpGet]
	public DataTableResult GetDataByBarcode(string barcode, string type)
	{
		try
		{
			return UnitOfWork.TranferRequest.GetDataByBarcode(barcode, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTableResult();
	}

	[HttpPost]
	public async Task<HttpResult> UpdateRequestByBarcode([FromBody] DataTable data)
	{
		try
		{
			if (!data.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Data is Empty!");
			}
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					DataTable dt = new DataTable
					{
						Columns = 
						{
							{
								"DocEntry",
								typeof(int)
							},
							{
								"LineNum",
								typeof(int)
							},
							{
								"ItemCode",
								typeof(string)
							},
							{
								"DistNumber",
								typeof(string)
							},
							{
								"BinId",
								typeof(int)
							},
							{
								"BinCode",
								typeof(string)
							},
							{
								"Quantity",
								typeof(double)
							}
						}
					};
					DocumentsImpl documentsImpl = new DocumentsImpl();
					documentsImpl.DocEntry = Function.ParseInt(data.Rows[0]["DocEntry"]);
					documentsImpl.DocDate = DateTime.Today;
					documentsImpl.TaxDate = DateTime.Today;
					documentsImpl.DocDueDate = DateTime.Today;
					documentsImpl.Comments = "Inventory transfer request out by Mobile App";
					documentsImpl.Filler = Function.ToString(data.Rows[0]["Whs"]);
					documentsImpl.ToWhsCode = Function.ToString(data.Rows[0]["ToWhs"]);
					documentsImpl.Lines = new List<Document_LinesImpl>();
					documentsImpl.UserFields = new List<UserFieldsImpl>();
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = "U_DOCTYPE";
					userFieldsImpl.Value = "TI";
					documentsImpl.UserFields.Add(userFieldsImpl);
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					int num = 0;
					for (int i = 0; i < data.Rows.Count; i++)
					{
						DataRow dataRow = data.Rows[i];
						string text = Function.ToString(dataRow["ObjType"]);
						if (text == "9999")
						{
							document_LinesImpl = new Document_LinesImpl();
							document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
							document_LinesImpl.SerialNumbers = new List<SerialNumbersImpl>();
							document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
							document_LinesImpl.LineNum = Function.ParseInt(dataRow["LineNum"]);
							document_LinesImpl.ItemCode = Function.ToString(dataRow["ItemCode"]);
							document_LinesImpl.FromWhsCod = Function.ToString(dataRow["Whs"]);
							document_LinesImpl.Quantity = Function.ParseDouble(dataRow["LineQty"]);
							num = 0;
							document_LinesImpl.WhsCode = Function.ToString(dataRow["ToWhs"]);
							documentsImpl.Lines.Add(document_LinesImpl);
						}
						else if (Function.ParseDouble(dataRow["Quantity"]) > 0.0)
						{
							double num2 = Function.ParseDouble(dataRow["Quantity"]);
							double num3 = Function.ParseDouble(dataRow["Instock"]);
							if (num2 > num3)
							{
								return new HttpResult(Enums.MessageCode.Error, string.Format("Số lượng tồn kho của batch {0} không đủ!", Function.ToString(dataRow["DistNumber"])));
							}
							if (text == "10000044")
							{
								BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
								string batchNumber = Function.ToString(dataRow["DistNumber"]);
								batchNumbersImpl.BatchNumber = batchNumber;
								batchNumbersImpl.BaseLineNumber = document_LinesImpl.LineNum;
								batchNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
								document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
							}
							else if (text == "10000045")
							{
								SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
								string batchID = Function.ToString(dataRow["DistNumber"]);
								serialNumbersImpl.BatchID = batchID;
								serialNumbersImpl.SystemSerialNumber = Function.ParseInt(dataRow["SysNumber"]);
								serialNumbersImpl.BaseLineNumber = document_LinesImpl.LineNum;
								serialNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
								document_LinesImpl.SerialNumbers.Add(serialNumbersImpl);
							}
							int num4 = Function.ParseInt(dataRow["BinId"]);
							int num5 = Function.ParseInt(dataRow["ToBinId"]);
							if (num4 > 0)
							{
								DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
								documentLinesBinAllocationsImpl.BinAbsEntry = num4;
								documentLinesBinAllocationsImpl.BaseLineNumber = document_LinesImpl.LineNum;
								documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
								documentLinesBinAllocationsImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
								documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num;
								document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
								DataRow dataRow2 = dt.NewRow();
								dataRow2["DocEntry"] = documentsImpl.DocEntry;
								dataRow2["LineNum"] = document_LinesImpl.LineNum;
								dataRow2["ItemCode"] = document_LinesImpl.ItemCode;
								dataRow2["DistNumber"] = Function.ToString(dataRow["DistNumber"]);
								dataRow2["BinId"] = num4;
								dataRow2["BinCode"] = Function.ToString(dataRow["BinCode"]);
								dataRow2["Quantity"] = Function.ParseDouble(dataRow["Quantity"]);
								dt.Rows.Add(dataRow2);
							}
							if (num5 > 0)
							{
								DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
								documentLinesBinAllocationsImpl2.BinAbsEntry = num5;
								documentLinesBinAllocationsImpl2.BaseLineNumber = document_LinesImpl.LineNum;
								documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
								documentLinesBinAllocationsImpl2.Quantity = Function.ParseDouble(dataRow["Quantity"]);
								documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num;
								document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
							}
							num++;
						}
					}
					HttpResult httpResult = await unitOfWork.StockTransferRequest.Save(documentsImpl, isDraft: false, SessionUserInfo.SessionDiServer);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						UnitOfWork.TranferRequest.LogBinAllocate(dt);
					}
					return httpResult;
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.Error, ex.Message);
		}
	}
}
