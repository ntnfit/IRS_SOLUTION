using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class BinLocMasterController : BaseApiController
{
	[HttpGet]
	public DataTable GetBinLocByCode(string binCode)
	{
		return UnitOfWork.BinLocMaster.GetBinLocByBinCode(binCode);
	}

	[HttpGet]
	public DataTable GetTableStructure()
	{
		return UnitOfWork.BinLocMaster.GetTableStructure();
	}

	[HttpGet]
	public DataTable GetWHSubDispName()
	{
		return UnitOfWork.BinLocMaster.GetWHSublevel();
	}

	[HttpGet]
	public DataTable GetDataBinLocation(int absEntry)
	{
		return UnitOfWork.BinLocMaster.GetDataBinLocation(absEntry);
	}

	[HttpGet]
	public DataTable GetDataBinLocation(string binCode, string whsCode)
	{
		return UnitOfWork.BinLocMaster.GetDataBinLocation(binCode, whsCode);
	}

	[HttpGet]
	public DataTable GetBatchBinAllowcateLineByDocEntry(string docEntry, ApzObjectTypes objectType)
	{
		return UnitOfWork.BinLocMaster.GetBatchBinAllowcateLineByDocEntry(docEntry, objectType);
	}

	[HttpGet]
	public DataTable GetDataCopyBinAllowcationLine(string lstDocentry, ApzObjectTypes objectType)
	{
		return UnitOfWork.BinLocMaster.GetDataCopyBinAllowcationLine(lstDocentry, objectType);
	}

	[HttpGet]
	public DataTable GetAllBinCode()
	{
		return UnitOfWork.BinLocMaster.GetAllBinCode();
	}

	[HttpGet]
	public DataTable GetBinLocProperties(int absEntry)
	{
		return UnitOfWork.BinLocMaster.GetBinLocProperties(absEntry);
	}

	[HttpGet]
	public string GetBinSeperatorOfWarehouse(string whsCode)
	{
		return UnitOfWork.BinLocMaster.GetBinSeperatorOfWarehouse(whsCode);
	}

	[HttpPost]
	public string Save([FromBody] Tuple<DataTable, string> model)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).BinLocMaster.Save(SessionUserInfo.SessionDiServer, model.Item1, model.Item2);
		}
		return "This function not support.";
	}

	[HttpPost]
	public string Remove([FromBody] int absEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).BinLocMaster.Delete(SessionUserInfo.SessionDiServer, absEntry);
		}
		return "This function not support";
	}

	[HttpGet]
	public DataTable GetBinDefault(string tranType, string whsCode, string itemCode, double qty)
	{
		return UnitOfWork.BinLocMaster.GetBinDefault(tranType, whsCode, itemCode, qty);
	}

	[HttpGet]
	public DataTable GetBinDefaultBatch(string tranType, string whsCode, string itemCode, int absEntry, double qty)
	{
		return UnitOfWork.BinLocMaster.GetBinDefaultBatch(tranType, whsCode, itemCode, absEntry, qty);
	}
}
