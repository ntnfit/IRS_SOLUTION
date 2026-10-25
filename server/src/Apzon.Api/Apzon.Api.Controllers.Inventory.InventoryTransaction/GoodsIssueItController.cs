using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

[ValidateLoginSapExceptionHandling]
public class GoodsIssueItController : BaseApiController
{
	internal readonly ApzObjectTypes FormObjectType = ApzObjectTypes.oInventoryGenExit;

	[HttpGet]
	public DataTable GetDataDoc(int baseType, int docNum, int docEntry, int priList, string fromDate, string toDate, Enums.NavigateType type, string userCode)
	{
		return UnitOfWork.GoodsIssueIt.GetDataDoc(baseType, docNum, docEntry, priList, fromDate, toDate, type, userCode);
	}

	[HttpGet]
	public DataSet SelectOne(int idDocEntry)
	{
		return UnitOfWork.GoodsIssueIt.SelectOne(idDocEntry);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.GoodsIssueIt.GetMaxDocEntry();
	}

	[HttpGet]
	public DataTable GetBranch()
	{
		return UnitOfWork.GoodsIssueIt.GetBranch(string.Empty);
	}

	[HttpGet]
	public DataTable GetDepartment()
	{
		return UnitOfWork.GoodsIssueIt.GetDepartment(string.Empty);
	}

	[HttpGet]
	public DataTable GetInStock(string itemCode)
	{
		return UnitOfWork.GoodsIssueIt.GetInStock(itemCode);
	}

	[HttpGet]
	public DataTable GetGroupNum()
	{
		return UnitOfWork.GoodsIssueIt.GetGroupNum();
	}

	[HttpGet]
	public DataTable GetListNum(string key)
	{
		return UnitOfWork.GoodsIssueIt.GetListNum(key);
	}

	[HttpGet]
	public DataTable LoadSeries(string usercode)
	{
		return UnitOfWork.Documents.LoadSeries(usercode, FormObjectType);
	}

	[HttpGet]
	public int GetNewDocNum(int series)
	{
		return UnitOfWork.GoodsIssueIt.GetNewDocNum(series);
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
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return await BrokerService.GetUnitOfWork(SessionId).Documents.Save(doc, SessionUserInfo.SessionDiServer, ApzObjectTypes.oInventoryGenExit);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
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
	public string DeleteDoc([FromBody] int idDocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.RemoveDoc(idDocEntry, SessionUserInfo.SessionDiServer, ApzObjectTypes.oInventoryGenExit);
		}
		return "This function is not supported";
	}

	[HttpGet]
	public DataTable GetItemProductionOrder(int docentry)
	{
		return UnitOfWork.GoodsIssueIt.GetItemProductionOrder(docentry);
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveAsDraft([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return await BrokerService.GetUnitOfWork(SessionId).Documents.Save(doc, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDrafts, ApzObjectTypes.oInventoryGenExit);
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
	public DataTable GetApprovalTemplates([FromBody] Tuple<DocumentsImpl, string, string> item)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new DataTable();
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.GetApprovalTemplate(item.Item1, SessionUserInfo.SessionDiServer, item.Item2, item.Item3, ApzObjectTypes.oInventoryGenExit);
		}
		return new DataTable();
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
}
