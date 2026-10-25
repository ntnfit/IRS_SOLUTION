using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Financials;

public class DistributeRuleController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataOOCR()
	{
		return UnitOfWork.DistributeRule.GetOOCR();
	}

	[HttpGet]
	public DataTable GetDataOCR1(string key)
	{
		return UnitOfWork.DistributeRule.GetOCR1(key);
	}

	[HttpGet]
	public bool GetIsHasLinkedObject(string key)
	{
		return UnitOfWork.DistributeRule.GetIsHasLinkedObject(key);
	}

	[HttpGet]
	public DataTable LoadPrcCode()
	{
		return UnitOfWork.DistributeRule.LoadPrcCode();
	}

	[HttpPost]
	public async Task<string> Save([FromBody] DataSet ds)
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
				return await unitOfWork.DistributionRule.Save(SessionUserInfo.SessionDiServer, ds);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.DistributeRule.Save(ds);
	}

	[HttpPost]
	public async Task<string> DeleteDistributeRule([FromBody] string ocrCode)
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
				return await unitOfWork.DistributionRule.DeleteDistributeRule(SessionUserInfo.SessionDiServer, ocrCode);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.DistributeRule.DeleteDistributeRule(ocrCode);
	}

	[HttpPost]
	public DataTable Search([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.DistributeRule.Search(dtSearch);
	}

	[HttpGet]
	public DataSet LoadComBoBox()
	{
		return UnitOfWork.DistributeRule.LoadComBoBox();
	}

	[HttpGet]
	public DataTable GetStruct()
	{
		return UnitOfWork.DistributeRule.GetStruct();
	}

	[HttpGet]
	public DataTable GetStructRow()
	{
		return UnitOfWork.DistributeRule.GetStructRow();
	}
}
