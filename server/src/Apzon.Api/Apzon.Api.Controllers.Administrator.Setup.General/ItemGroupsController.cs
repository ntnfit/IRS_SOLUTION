using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class ItemGroupsController : BaseApiController
{
	[HttpGet]
	public List<ItemGroupsImpl> Cfl()
	{
		return UnitOfWork.ItemGroups.Cfl();
	}

	[HttpGet]
	public DataTable GetItemGroup()
	{
		return UnitOfWork.ItemGroups.GetItemGroup();
	}

	[HttpGet]
	public DataTable ShowItemGroup()
	{
		return UnitOfWork.ItemGroups.ShowItemGroup();
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ItemGroups.Delete(code, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ItemGroups.DeleteDoc(code);
	}

	[HttpPost]
	public DataTable Search([FromBody] ItemGroupsImpl doc)
	{
		return UnitOfWork.ItemGroups.Search(doc);
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.ItemGroups.GetMaxCode();
	}

	[HttpGet]
	public DataTable CheckExistItemGroupCode(int itgrCode)
	{
		return UnitOfWork.ItemGroups.CheckExistItemGroupCode(itgrCode);
	}

	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.ItemGroups.LoadDoc();
	}

	[HttpPost]
	public HttpResult SaveOITB([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).ItemGroups.Save(ds, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ItemGroups.SaveDoc(ds);
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName, int codeMax)
	{
		return UnitOfWork.Commons.GetAddNewRow(tableName, codeMax);
	}

	[HttpGet]
	public DataTable GetItemGroupWarehouse(int code)
	{
		return UnitOfWork.ItemGroups.GetItemGroupWarehouse(code);
	}
}
