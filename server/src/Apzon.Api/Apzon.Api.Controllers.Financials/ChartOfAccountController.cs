using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Financials;

public class ChartOfAccountController : BaseApiController
{
	[HttpGet]
	public List<ChartOfAccountsImpl> Cfl()
	{
		return UnitOfWork.ChartOfAccount.Cfl();
	}

	[HttpPost]
	public string Save([FromBody] List<ChartOfAccountsImpl> lstChartOfAccount)
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
				return unitOfWork.ChartOfAccount.Save(lstChartOfAccount, SessionUserInfo.SessionDiServer).Message;
			}
			return "INVALIDSESSION";
		}
		return "This function is not supported";
	}

	[HttpGet]
	public DataTable GetStruct()
	{
		return UnitOfWork.ChartOfAccount.GetStruct();
	}
}
