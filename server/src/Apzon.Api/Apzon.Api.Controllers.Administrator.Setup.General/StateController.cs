using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class StateController : BaseApiController
{
	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		return UnitOfWork.State.Save(dt);
	}

	[HttpGet]
	public DataTable GetCountry(int user)
	{
		return UnitOfWork.State.GetCountry(user);
	}

	[HttpGet]
	public DataTable GetListAllState()
	{
		return UnitOfWork.State.GetListAllState();
	}

	[HttpGet]
	public string Remove(string country)
	{
		return UnitOfWork.State.Remove(country);
	}
}
