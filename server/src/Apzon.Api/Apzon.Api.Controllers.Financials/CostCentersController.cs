using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Financials;

public class CostCentersController : BaseApiController
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
				return unitOfWork.ProfitCenters.Save(SessionUserInfo.SessionDiServer, dt);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.CostCenters.Save(dt);
	}

	[HttpPost]
	public string DeleteCostCenters([FromBody] string prcCode)
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
				return unitOfWork.ProfitCenters.Remove(SessionUserInfo.SessionDiServer, prcCode);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.CostCenters.DeleteCostCenters(prcCode);
	}

	[HttpPost]
	public DataTable Search([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.CostCenters.Search(dtSearch);
	}

	[HttpGet]
	public DataSet LoadComboBox()
	{
		return UnitOfWork.CostCenters.LoadComboBox();
	}

	[HttpGet]
	public bool IsExistsCostCenters(string prcCode)
	{
		return UnitOfWork.CostCenters.IsExistsCostCenters(prcCode);
	}
}
