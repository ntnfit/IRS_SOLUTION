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
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Api.Controllers.Production;

public class IssueForProductionController : BaseApiController
{
	[HttpGet]
	public DataTable Search(int docEntry)
	{
		return UnitOfWork.IssueForProduction.Search(docEntry);
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.IssueForProduction.GetMaxCode();
	}

	[HttpGet]
	public int GetMaxFileEntry()
	{
		return UnitOfWork.IssueForProduction.GetMaxFileEntry();
	}

	[HttpGet]
	public DataTable LoadItemsByParentCode(int docEntry)
	{
		return UnitOfWork.IssueForProduction.LoadItemsByParentCode(docEntry);
	}

	[HttpGet]
	public DataTable LoadItemsByProdOrderEntry(int docEntry)
	{
		return UnitOfWork.IssueForProduction.LoadItemsByProdOrderEntry(docEntry);
	}

	[HttpGet]
	public DataTable LoadAttachFiles(int absEntry)
	{
		return UnitOfWork.IssueForProduction.LoadAttachFiles(absEntry);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] IssueForProduction_Impl_OIGE doc)
	{
		return "This function is not supported";
	}

	[HttpGet]
	public DataTable GetFirstRecord()
	{
		return UnitOfWork.IssueForProduction.GetFirstRecord();
	}

	[HttpGet]
	public DataTable GetLastRecord()
	{
		return UnitOfWork.IssueForProduction.GetLastRecord();
	}

	[HttpGet]
	public DataTable GetNextRecord(string value)
	{
		return UnitOfWork.IssueForProduction.GetNextRecord(value);
	}

	[HttpGet]
	public DataTable GetPreviousRecord(string value)
	{
		return UnitOfWork.IssueForProduction.GetPreviousRecord(value);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteIGE1(string docEntry, string lineNum)
	{
		return UnitOfWork.IssueForProduction.DeleteIGE1(docEntry, lineNum);
	}

	[HttpGet]
	public DataTable CopyFrom(string type, string entry, int bplId, string username)
	{
		return UnitOfWork.Documents.CopyFrom(type, entry, 60.ToString(), bplId, username);
	}

	[HttpGet]
	public DataTable CopyFromLine(string type, string entry)
	{
		return UnitOfWork.Documents.CopyFromLine(type, entry, 60.ToString());
	}

	[HttpGet]
	public DataTable CopyBatchSerial(string fromentry, string fromtype)
	{
		return UnitOfWork.Documents.CopyBatchSerial(fromentry, fromtype);
	}

	[HttpGet]
	public DataTableResult GetDataByPOEntry(string barcode)
	{
		try
		{
			return UnitOfWork.IssueForProduction.GetDataByPOEntry(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTableResult();
	}

	[HttpGet]
	public DataSetResult GetDataByBarcode(string barcode, int docEntry = 0)
	{
		try
		{
			return UnitOfWork.IssueForProduction.GetDataByBarcode(barcode, docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSetResult();
	}

	[HttpPost]
	public async Task<HttpResult> AddIssueForProduction([FromBody] DataTable data)
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
					DocumentsImpl documentsImpl = new DocumentsImpl();
					documentsImpl.ObjType = 60;
					documentsImpl.DocEntry = 0;
					documentsImpl.DocDate = DateTime.Today;
					documentsImpl.Comments = "Added by Mobile App";
					documentsImpl.Filler = Function.ToString(data.Rows[0]["Whs"]);
					documentsImpl.ToWhsCode = Function.ToString(data.Rows[0]["ToWhs"]);
					documentsImpl.Lines = new List<Document_LinesImpl>();
					documentsImpl.UserFields = new List<UserFieldsImpl>();
					new UserFieldsImpl();
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					int num = 0;
					int num2 = 0;
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
							document_LinesImpl.LineNum = num2;
							document_LinesImpl.ItemCode = Function.ToString(dataRow["ItemCode"]);
							document_LinesImpl.FromWhsCod = Function.ToString(dataRow["Whs"]);
							document_LinesImpl.Quantity = Function.ParseDouble(dataRow["LineQty"]);
							document_LinesImpl.BaseEntry = Function.ParseInt(dataRow["DocEntry"]);
							document_LinesImpl.BaseLine = Function.ParseInt(dataRow["LineNum"]);
							document_LinesImpl.BaseType = 202;
							document_LinesImpl.ItemType = Function.ParseInt(dataRow["ItemType"]);
							num = 0;
							document_LinesImpl.WhsCode = Function.ToString(dataRow["Whs"]);
							documentsImpl.Lines.Add(document_LinesImpl);
							num2++;
						}
						else if (Function.ParseDouble(dataRow["Quantity"]) > 0.0)
						{
							double num3 = Function.ParseDouble(dataRow["Quantity"]);
							double num4 = Function.ParseDouble(dataRow["Instock"]);
							if (num3 > num4)
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
							int num5 = Function.ParseInt(dataRow["BinId"]);
							int num6 = Function.ParseInt(dataRow["ToBinId"]);
							if (num5 > 0)
							{
								DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
								documentLinesBinAllocationsImpl.BinAbsEntry = num5;
								documentLinesBinAllocationsImpl.BaseLineNumber = document_LinesImpl.LineNum;
								documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
								documentLinesBinAllocationsImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
								documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num;
								document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
							}
							if (num6 > 0)
							{
								DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
								documentLinesBinAllocationsImpl2.BinAbsEntry = num6;
								documentLinesBinAllocationsImpl2.BaseLineNumber = document_LinesImpl.LineNum;
								documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
								documentLinesBinAllocationsImpl2.Quantity = Function.ParseDouble(dataRow["Quantity"]);
								documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num;
								document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
							}
							num++;
						}
					}
					return await unitOfWork.Documents.Save(documentsImpl, SessionUserInfo.SessionDiServer, ApzObjectTypes.oInventoryGenExit);
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
