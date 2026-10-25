using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources;

public class PenalTypeController : BaseApiController
{
	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.PenalType.GetData();
	}

	[HttpPost]
	public string Remove([FromBody] int code)
	{
		return UnitOfWork.PenalType.Remove(code);
	}

	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		return UnitOfWork.PenalType.Save(dt);
	}
}
