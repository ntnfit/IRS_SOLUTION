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
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class GoodReceiptItController : BaseApiController
{
	internal readonly ApzObjectTypes FormObjectType = ApzObjectTypes.oInventoryGenEntry;

	[HttpGet]
	public DataTable GetDataDoc(int baseType, int docNum, int docEntry, int priList, string fromDate, string toDate, Enums.NavigateType type, string userCode)
	{
		return UnitOfWork.GoodsReceipt.GetDataDoc(baseType, docNum, docEntry, priList, fromDate, toDate, type, userCode);
	}

	[HttpGet]
	public DataSet SelectOne(int idDocEntry)
	{
		return UnitOfWork.GoodsReceipt.SelectOne(idDocEntry);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.GoodsReceipt.GetMaxDocEntry();
	}

	[HttpGet]
	public DataTable GetBranch()
	{
		return UnitOfWork.GoodsReceipt.GetBranch(string.Empty);
	}

	[HttpGet]
	public DataTable GetDepartment()
	{
		return UnitOfWork.GoodsReceipt.GetDepartment(string.Empty);
	}

	[HttpGet]
	public DataTable GetInStock(string itemCode)
	{
		return UnitOfWork.GoodsReceipt.GetInStock(itemCode);
	}

	[HttpGet]
	public DataTable GetGroupNum()
	{
		return UnitOfWork.GoodsReceipt.GetGroupNum();
	}

	[HttpGet]
	public DataTable GetListNum(string key)
	{
		return UnitOfWork.GoodsReceipt.GetListNum(key);
	}

	[HttpGet]
	public DataTable LoadSeries(string usercode)
	{
		return UnitOfWork.Documents.LoadSeries(usercode, FormObjectType);
	}

	[HttpGet]
	public int GetNewDocNum(int series)
	{
		return UnitOfWork.GoodsReceipt.GetNewDocNum(series);
	}

	[HttpPost]
	public DataTable GetItemInStockAWarehouse([FromBody] List<string> itAndWh)
	{
		return UnitOfWork.GoodsReceipt.GetItemInStockAWarehouse(itAndWh);
	}

	[HttpGet]
	public DataTable GetItemInStockAllWarehouse(string itemCode)
	{
		return UnitOfWork.GoodsReceipt.GetItemInStockAllWarehouse(itemCode);
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveDoc([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return await BrokerService.GetUnitOfWork(SessionId).Documents.Save(doc, SessionUserInfo.SessionDiServer, FormObjectType);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> AddNew(DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return await BrokerService.GetUnitOfWork(SessionId).Documents.AddNew(doc, SessionUserInfo.SessionDiServer, FormObjectType);
		}
		return new HttpResult(Enums.MessageCode.Error, "Unknown error");
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveDataset([FromBody] DataSet doc)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Documents.SaveDataSet(doc, FormObjectType);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveAsDraftDataset([FromBody] DataSet doc)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Documents.SaveAsDraftDataSet(doc);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult ReportComplete([FromBody] DataSet dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			return new HttpResult(Enums.MessageCode.Success, unitOfWork.InventoryGenEntry.AutoReportComplete(dt, SessionUserInfo.SessionDiServer));
		}
		return UnitOfWork.GoodsReceipt.ReportComplete(dt);
	}

	[HttpPost]
	[QueueModifiedAction]
	public string DeleteDoc([FromBody] int idDocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.RemoveDoc(idDocEntry, SessionUserInfo.SessionDiServer, FormObjectType);
		}
		return "This function is not supported";
	}

	[HttpGet]
	public DataTable GetItemProductionOrder(int docentry)
	{
		return UnitOfWork.GoodsReceipt.GetItemProductionOrder(docentry);
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
			return BrokerService.GetUnitOfWork(SessionId).Documents.GetApprovalTemplate(item.Item1, SessionUserInfo.SessionDiServer, item.Item2, item.Item3, ApzObjectTypes.oInventoryGenEntry);
		}
		return new DataTable();
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveAsDraft([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return await BrokerService.GetUnitOfWork(SessionId).Documents.Save(doc, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDrafts, FormObjectType);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable CopyFrom(string type, string entry, int bplId, string username)
	{
		return UnitOfWork.Documents.CopyFrom(type, entry, ((int)FormObjectType).ToString(), bplId, username);
	}

	[HttpGet]
	public DataTable CopyFromLine(string type, string entry)
	{
		return UnitOfWork.Documents.CopyFromLine(type, entry, ((int)FormObjectType).ToString());
	}

	[HttpGet]
	public DataTable CopyBatchSerial(string fromentry, string fromtype)
	{
		return UnitOfWork.Documents.CopyBatchSerial(fromentry, fromtype);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveDocWithUdo([FromBody] Tuple<DocumentsImpl, DataSet> model)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			DocumentsImpl item = model.Item1;
			if (item.Comments.Length > 250)
			{
				item.Comments = item.Comments.Substring(0, 250);
			}
			return unitOfWork.Documents.AddDocWithUDO(item, model.Item2, SessionUserInfo.SessionDiServer, FormObjectType);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_FunctionNotSupported);
	}

	[HttpPost]
	public HttpResult SaveDocWithUdoDynamic(DocInput model)
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
				DocumentsImpl doc = model.Doc;
				if (doc.Comments.Length > 250)
				{
					doc.Comments = doc.Comments.Substring(0, 251);
				}
				return unitOfWork.Documents.AddDocWithUDO(doc, model.DataUdo, SessionUserInfo.SessionDiServer, FormObjectType);
			}
			return new HttpResult(Enums.MessageCode.Error, Resources.Msg_FunctionNotSupported);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
