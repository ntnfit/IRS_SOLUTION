using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class UnitOfMeasureController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.UnitOfMeasure.LoadDoc();
	}

	[HttpPost]
	public HttpResult Save(DataTable dt)
	{
		if (Global.LoginSap && Global.ServiceType == Enums.ServiceType.ServiceLayer)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.UnitOfMeasurements.SaveData(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return UnitOfWork.UnitOfMeasure.Save(dt);
	}

	[HttpGet]
	public DataTable CheckLengthUnit(string unitDisply)
	{
		return UnitOfWork.UnitOfMeasure.CheckLengthUnit(unitDisply);
	}

	[HttpGet]
	public DataTable CheckWidthUnit(string unitDisply)
	{
		return UnitOfWork.UnitOfMeasure.CheckWidthUnit(unitDisply);
	}

	[HttpGet]
	public DataTable GetAllLengthUnits()
	{
		return UnitOfWork.UnitOfMeasure.GetAllLengthUnits();
	}
}
