using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class UnitOfMeasureGroupsController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.UnitOfMeasureGroups.LoadDoc();
	}

	[HttpGet]
	public DataTable LoadDefinition(int ugpEntry)
	{
		return UnitOfWork.UnitOfMeasureGroups.LoadDefinition(ugpEntry);
	}

	[HttpPost]
	public HttpResult Save(DataTable dt)
	{
		return UnitOfWork.UnitOfMeasureGroups.Save(dt);
	}

	[HttpGet]
	public bool CheckUoMGroupExists(int ugpEntry)
	{
		return UnitOfWork.UnitOfMeasureGroups.CheckUoMGroupExists(ugpEntry);
	}

	[HttpPost]
	public HttpResult SaveSap(Tuple<DataTable, DataTable, Enums.HttpAction> tupData)
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
				return unitOfWork.UnitOfMeasureGroups.SaveData(tupData, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "Invalid session");
		}
		return UnitOfWork.UnitOfMeasureGroups.Save(tupData.Item1);
	}

	[HttpGet]
	public string CleanUoMGroup()
	{
		return UnitOfWork.UnitOfMeasureGroups.CleanUoMGroup();
	}

	[HttpGet]
	public DataTable CheckGroupInItems(int ugpEntry)
	{
		return UnitOfWork.UnitOfMeasureGroups.CheckGroupInItems(ugpEntry);
	}

	[HttpGet]
	public DataTable CheckUoMInGroups(int uomEntry)
	{
		return UnitOfWork.UnitOfMeasureGroups.CheckUoMInGroups(uomEntry);
	}

	[HttpGet]
	public DataTable GetDataUomInGroup(int ugpEntry, int uomEntry)
	{
		return UnitOfWork.UnitOfMeasureGroups.GetDataUomInGroup(ugpEntry, uomEntry);
	}

	[HttpGet]
	public DataTable GetUomItem(string itemCode)
	{
		return UnitOfWork.UnitOfMeasureGroups.GetUomItem(itemCode);
	}
}
