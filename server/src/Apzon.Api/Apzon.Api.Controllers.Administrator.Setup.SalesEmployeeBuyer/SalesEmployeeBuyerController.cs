using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Administrator.Setup.SalesEmployeeBuyer;

public class SalesEmployeeBuyerController : BaseApiController
{
	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.SalesPersons.Save(SessionUserInfo.SessionDiServer, dt);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.SalesEmployeeBuyer.Save(dt);
	}

	[HttpGet]
	public DataTable GetOSLP()
	{
		return UnitOfWork.SalesEmployeeBuyer.GetOSLP();
	}

	[HttpGet]
	public DataTable GetOCOG()
	{
		return UnitOfWork.SalesEmployeeBuyer.GetOCOG();
	}

	[HttpGet]
	public DataTable GetListEmployee()
	{
		return UnitOfWork.SalesEmployeeBuyer.GetListEmployee();
	}

	[HttpPost]
	public string DeleteRow([FromBody] int slpCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.SalesPersons.Remove(SessionUserInfo.SessionDiServer, slpCode);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.SalesEmployeeBuyer.DeleteRow(slpCode);
	}

	[HttpPost]
	public string SetDefaultCode([FromBody] int slpCode)
	{
		if (Global.LoginSap)
		{
			return "Function under constructions";
		}
		return UnitOfWork.SalesEmployeeBuyer.SetDefaultCode(slpCode);
	}

	[HttpGet]
	public int GetDefaultCode()
	{
		return UnitOfWork.SalesEmployeeBuyer.GetDefaultCode();
	}

	[HttpGet]
	public bool IsExistsSlpCode(int slpCode)
	{
		return UnitOfWork.SalesEmployeeBuyer.IsExistsSlpCode(slpCode);
	}

	[HttpGet]
	public DataTable GetStructure()
	{
		return UnitOfWork.SalesPersons.GetStructure();
	}
}
