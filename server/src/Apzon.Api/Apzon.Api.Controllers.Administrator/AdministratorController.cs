using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class AdministratorController : BaseApiController
{
	public DataTable GetCodeOcyc()
	{
		return UnitOfWork.Administrator.GetCodeOcyc();
	}

	public DataTable GetCodeOcycItemGroups()
	{
		return UnitOfWork.Administrator.GetCodeOcycItemGroups();
	}

	public DataTable GetListCountry()
	{
		return UnitOfWork.Administrator.GetListCountry();
	}

	[HttpGet]
	public DataTable GetListState(string country)
	{
		return UnitOfWork.Administrator.GetListState(country);
	}
}
