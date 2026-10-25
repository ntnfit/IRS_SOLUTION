using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class WHSubSetupController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllWHSub()
	{
		return UnitOfWork.WHSubSetup.GetAllWarehouseSublevel();
	}

	[HttpGet]
	public DataTable GetWHSubByAbs(int whSublevel)
	{
		return UnitOfWork.WHSubSetup.GetWarehouseSublevelByFldAbs(whSublevel);
	}

	[HttpGet]
	public string IsUsedInBinLocCodes(int fldAbs, string slCode)
	{
		return UnitOfWork.WHSubSetup.IsUsedInBinLocCodes(fldAbs, slCode);
	}

	[HttpPost]
	public DataTable IsUsedInBinLocCodes(DataTable data)
	{
		return UnitOfWork.WHSubSetup.IsUsedInBinLocCodes(data);
	}

	[HttpGet]
	public string CheckCodeExist(int fldAbs, string slCode)
	{
		return UnitOfWork.WHSubSetup.CheckCodeExist(fldAbs, slCode);
	}

	[HttpPost]
	public DataTable CheckCodeExist(DataTable data)
	{
		return UnitOfWork.WHSubSetup.CheckCodeExist(data);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).WarehouseSublevel.Save(SessionUserInfo.SessionDiServer, doc);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult SaveGen([FromBody] DataTable data)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).WarehouseSublevel.SaveGen(SessionUserInfo.SessionDiServer, data);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable GetRangeSlCode(int fldAbs, string slFrom, string slTo, string descr)
	{
		return UnitOfWork.WHSubSetup.GetRangeSlCode(fldAbs, slFrom, slTo, descr);
	}
}
