using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class WarehouseController : BaseApiController
{
	[HttpGet]
	public DataTable Cfl()
	{
		return UnitOfWork.Warehouse.Cfl();
	}

	[HttpGet]
	public DataTable CflDocument()
	{
		return UnitOfWork.Warehouse.CflDocument();
	}

	[HttpGet]
	public DataTable GetRecord(string position, string whsCode)
	{
		return UnitOfWork.Warehouse.GetRecord(position, whsCode);
	}

	[HttpGet]
	public List<WarehouseImpl> CflByWhseCode(string whseCode)
	{
		return UnitOfWork.Warehouse.CflByWhseCode(whseCode);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string vatCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được cập nhật!";
		}
		return UnitOfWork.Warehouse.DeleteDoc(vatCode);
	}

	[HttpGet]
	public DataTable Search(string whsCode, string whsName, int bPLid)
	{
		return UnitOfWork.Warehouse.Search(whsCode, whsName, bPLid);
	}

	[HttpGet]
	public DataTable ShowWarehouse()
	{
		return UnitOfWork.Warehouse.ShowWarehouse();
	}

	[HttpGet]
	public DataTable GetItemOnhandByWarehouseItemCode(string whsCode, string itemCode)
	{
		return UnitOfWork.Warehouse.GetItemOnhandByWarehouseItemCode(whsCode, itemCode);
	}

	[HttpGet]
	public DataTable GetDefaultTable()
	{
		return UnitOfWork.Warehouse.GetDefaultTable();
	}

	[HttpPost]
	public string Save([FromBody] WarehouseImpl obj)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Warehouses.Save(obj, SessionUserInfo.SessionDiServer, Enums.TransactionType.A);
		}
		return "[IRS-Controller] This function not supported.";
	}

	[HttpPost]
	public string Update([FromBody] WarehouseImpl obj)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Warehouses.Save(obj, SessionUserInfo.SessionDiServer, Enums.TransactionType.U);
		}
		return "[IRS-Controller] This function not supported.";
	}

	[HttpPost]
	public string SaveDataTable([FromBody] DataTable dt)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Warehouse.Save(dt);
		}
		return "[IRS-Controller] This function not supported.";
	}

	[HttpPost]
	public string DeleteWarehouse([FromBody] string code)
	{
		return UnitOfWork.Warehouse.DeleteWarehouse(code);
	}

	[HttpGet]
	public DataTable GetAllCode()
	{
		return UnitOfWork.Warehouse.GetAllCode();
	}

	[HttpGet]
	public Common.Table_Code WhsCheckFollowBin(string whsCode)
	{
		return UnitOfWork.Warehouse.WhsCheckFollowBin(whsCode);
	}

	[HttpGet]
	public Common.Table_Code WhsGetDefBinItem(string whsCode, string itemCode)
	{
		return UnitOfWork.Warehouse.WhsGetDefBinItem(whsCode, itemCode);
	}
}
