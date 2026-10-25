using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Service;

public class ServiceCallSettingController : BaseApiController
{
	[HttpGet]
	public string Save(string name, string email, string pass, int service, string userName)
	{
		return UnitOfWork.ServiceCallSetting.Save(name, email, pass, service, userName);
	}

	[HttpGet]
	public DataTable GetServiceCallSetting(string userName)
	{
		return UnitOfWork.ServiceCallSetting.GetSetting(userName);
	}
}
