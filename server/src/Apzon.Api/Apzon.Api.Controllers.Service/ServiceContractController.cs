using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Service;

public class ServiceContractController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataTable dts)
	{
		return UnitOfWork.ServiceContract.Search(dts);
	}

	[HttpGet]
	public DataSet GetDetail(int contractID)
	{
		return UnitOfWork.ServiceContract.GetDetail(contractID);
	}

	[HttpGet]
	public DataTable GetContractID()
	{
		return UnitOfWork.ServiceContract.GetContractID();
	}

	[HttpPost]
	public string Save(DataSet data)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "";
			}
			return BrokerService.GetUnitOfWork(SessionId).ServiceContract.Save(data, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ServiceContract.Save(data);
	}
}
