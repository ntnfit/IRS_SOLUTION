using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class LocationController : BaseApiController
{
	[HttpGet]
	public DataTable GetLocation()
	{
		return UnitOfWork.Location.GetLocation();
	}

	[HttpGet]
	public DataTable ShowLocation()
	{
		return UnitOfWork.Location.ShowLocation();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.Location.GetMaxCode();
	}

	[HttpPost]
	public DataTable Search([FromBody] LocationImpl doc)
	{
		return UnitOfWork.Location.Search(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] int code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được xây dựng!";
		}
		return UnitOfWork.Location.DeleteDoc(code);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] LocationImpl doc)
	{
		return UnitOfWork.Location.SaveDoc(doc);
	}

	[HttpGet]
	public DataTable LoadLocation()
	{
		return UnitOfWork.Location.LoadLocation();
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName, int codeMax)
	{
		return UnitOfWork.Commons.GetAddNewRow(tableName, codeMax);
	}

	[HttpGet]
	public DataTable CheckExistLocationCode(int locCode)
	{
		return UnitOfWork.Location.CheckExistLocationCode(locCode);
	}

	[HttpPost]
	public HttpResult SaveOLCT([FromBody] DataTable dtOLCT)
	{
		if (Global.LoginSap)
		{
			if (Global.ServiceType == Enums.ServiceType.ServiceLayer)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "Invalid session");
				}
				return BrokerService.GetUnitOfWork(SessionId).Locations.Save(dtOLCT, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.Location.SaveOLCT(dtOLCT);
	}
}
