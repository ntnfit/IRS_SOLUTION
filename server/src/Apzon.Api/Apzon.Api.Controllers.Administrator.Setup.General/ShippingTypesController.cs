using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class ShippingTypesController : BaseApiController
{
	[HttpGet]
	public List<ShippingTypesImpl> Cfl()
	{
		return UnitOfWork.ShippingTypes.Cfl();
	}

	[HttpGet]
	public DataTable GetShippingTypes()
	{
		return UnitOfWork.ShippingTypes.GetShippingTypes();
	}

	[HttpGet]
	public DataTable ShowShippingTypes()
	{
		return UnitOfWork.ShippingTypes.ShowShipping();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.ShippingTypes.GetMaxCode();
	}

	[HttpPost]
	public HttpResult DeleteDoc([FromBody] int idTrnspCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			return new HttpResult(Enums.MessageCode.Success, unitOfWork.ShippingType.Delete(idTrnspCode, SessionUserInfo.SessionDiServer));
		}
		return UnitOfWork.ShippingTypes.DeleteDoc(idTrnspCode);
	}

	[HttpGet]
	public DataTable Search(string name)
	{
		return UnitOfWork.ShippingTypes.Search(name);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).ShippingType.Save(dt, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ShippingTypes.SaveDoc(dt);
	}

	[HttpGet]
	public DataTable CheckShippingOITM(int trnspCode)
	{
		return UnitOfWork.ShippingTypes.CheckShippingOITM(trnspCode);
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName, int codeMax)
	{
		return UnitOfWork.Commons.GetAddNewRow(tableName, codeMax);
	}
}
