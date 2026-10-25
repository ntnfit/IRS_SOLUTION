using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class CycleCountDeterminationController : BaseApiController
{
	[HttpGet]
	public DataTable GetList()
	{
		return UnitOfWork.CycleCountDetermination.GetList();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.CycleCountDetermination.Save(dt, "U");
	}
}
