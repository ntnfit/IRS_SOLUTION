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

public class ManufacturersController : BaseApiController
{
	[HttpGet]
	public List<ManufacturersImpl> Cfl()
	{
		return UnitOfWork.Manufacturers.Cfl();
	}

	[HttpGet]
	public DataTable GetManufacturers()
	{
		return UnitOfWork.Manufacturers.GetManufacturers();
	}

	[HttpGet]
	public DataTable ShowManufacturers()
	{
		return UnitOfWork.Manufacturers.ShowManufacturers();
	}

	[HttpGet]
	public DataTable ShowCurrency()
	{
		return UnitOfWork.Manufacturers.ShowCurrency();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.Manufacturers.GetMaxCode();
	}

	[HttpGet]
	public DataTable Search(string firmName)
	{
		return UnitOfWork.Manufacturers.Search(firmName);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int idFirmCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Manufacturer.Delete(idFirmCode, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Manufacturers.DeleteDoc(idFirmCode);
	}

	[HttpPost]
	public HttpResult Delete([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.Manufacturer.Delete(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.Manufacturers.Remove(dt);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] DataTable doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).Manufacturer.Save(doc, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Manufacturers.SaveDoc(doc);
	}

	[HttpGet]
	public DataTable CheckFirmCodeOITM([FromBody] int firmCode)
	{
		return UnitOfWork.Manufacturers.CheckFirmCodeOITM(firmCode);
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName, int codeMax)
	{
		return UnitOfWork.Commons.GetAddNewRow(tableName, codeMax);
	}
}
