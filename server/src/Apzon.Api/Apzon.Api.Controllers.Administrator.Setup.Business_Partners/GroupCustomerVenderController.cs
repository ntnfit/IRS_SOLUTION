using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Business_Partners;

public class GroupCustomerVenderController : BaseApiController
{
	[HttpGet]
	public DataTable GetPriceList()
	{
		return UnitOfWork.GroupCustomerVender.GetPriceList();
	}

	[HttpGet]
	public DataTable LoadDoc(string catelogy)
	{
		return UnitOfWork.GroupCustomerVender.LoadDoc(catelogy);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] Tuple<DataTable, string> tup)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).GroupCustomerVender.Save(tup.Item1, SessionUserInfo.SessionDiServer, tup.Item2);
		}
		return UnitOfWork.GroupCustomerVender.SaveDoc(tup);
	}

	[HttpPost]
	public HttpResult Remove([FromBody] DataTable dt)
	{
		new HttpResult();
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.GroupCustomerVender.Remove(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.GroupCustomerVender.Delete(dt);
	}

	[HttpGet]
	public DataTable GetMaxCode()
	{
		return UnitOfWork.GroupCustomerVender.GetMaxCode();
	}

	[HttpGet]
	public DataTable CheckExistGroupCode(int grCode)
	{
		return UnitOfWork.GroupCustomerVender.CheckExistGroupCode(grCode);
	}

	[HttpGet]
	public string CheckGroupName(string groupName)
	{
		return UnitOfWork.GroupCustomerVender.CheckGroupName(groupName);
	}
}
