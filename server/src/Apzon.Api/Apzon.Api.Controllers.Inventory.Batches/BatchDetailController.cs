using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory.Batches;

public class BatchDetailController : BaseApiController
{
	[HttpGet]
	public DataTable Search(string itemCode, string itemName, string batch)
	{
		return UnitOfWork.BatchDetail.Search(itemCode, itemName, batch);
	}

	[HttpGet]
	public DataTable GetDataBatchWhs(string itemCode, string code, string whs)
	{
		return UnitOfWork.BatchDetail.GetDataBatchWhs(itemCode, code, whs);
	}

	[HttpGet]
	public DataTable GetQuantities(string itemCode, int sysNumber)
	{
		return UnitOfWork.BatchDetail.GetQuantities(itemCode, sysNumber);
	}

	[HttpPost]
	[QueueModifiedAction]
	public string Save(DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).BatchNumberDetails.Save(ds, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.BatchDetail.Save(ds);
	}

	[HttpGet]
	public DataTable GetByAbsEntry(int absEntry)
	{
		return UnitOfWork.BatchDetail.GetByAbsEntry(absEntry);
	}

	[HttpGet]
	public DataTable LoadAnEntry(int absEntry)
	{
		return UnitOfWork.BatchDetail.LoadAnEntry(absEntry);
	}

	[HttpPost]
	public DataTable GetDataAutoSelect([FromBody] Tuple<string, string, string, string, double, DataTable> model)
	{
		return UnitOfWork.BatchDetail.GetDataAutoSelect(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5, model.Item6);
	}

	[HttpGet]
	public DataTable GetDataAllowcateBin(string itemCode, string whsCode, decimal qty)
	{
		return UnitOfWork.BatchDetail.GetDataAllowcateBin(itemCode, whsCode, qty);
	}

	[HttpGet]
	public DataSet LoadDataAutoSelectConfig()
	{
		return UnitOfWork.BatchDetail.LoadDataAutoSelectConfig();
	}

	[HttpPost]
	public string SaveConfig([FromBody] DataTable dt)
	{
		return UnitOfWork.BatchDetail.SaveConfig(dt, SessionUserInfo.UserName);
	}

	[HttpGet]
	public int GetPage(int currentId, int type, string username, int branch)
	{
		return UnitOfWork.BatchDetail.GetPage(currentId, type, username, branch);
	}

	[HttpGet]
	public DataSet GetBatchDetail(int absEntry, string itemCode, string status, string distNumber, string mnfSerial, string lotNumber)
	{
		return UnitOfWork.BatchDetail.GetBatchDetail(absEntry, itemCode, status, distNumber, mnfSerial, lotNumber);
	}

	[HttpGet]
	public DataTable GetDataBatchSerial(string serialNo, string mnfSerial, string lotNumber, string itemCode, string tranType)
	{
		return UnitOfWork.BatchDetail.GetDataBatchSerial(serialNo, mnfSerial, lotNumber, itemCode, tranType);
	}
}
