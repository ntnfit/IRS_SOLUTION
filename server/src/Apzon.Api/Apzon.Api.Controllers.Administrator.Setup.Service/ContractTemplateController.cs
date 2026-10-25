using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup.Service;

public class ContractTemplateController : BaseApiController
{
	[HttpGet]
	public DataTable GetData(string tmpltName)
	{
		return UnitOfWork.ContractTemplate.GetData(tmpltName);
	}

	[HttpGet]
	public DataTable CheckDoc(string tmpltName)
	{
		return UnitOfWork.ContractTemplate.CheckDoc(tmpltName);
	}

	[HttpGet]
	public DataTable GetRecord(string position, string tmpltName)
	{
		return UnitOfWork.ContractTemplate.GetRecord(position, tmpltName);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ContractTemplate.SaveDoc(ds, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ContractTemplate.SaveDoc(ds);
	}

	[HttpPost]
	public DataSet Search([FromBody] DataTable dt)
	{
		return UnitOfWork.ContractTemplate.Search(dt);
	}
}
