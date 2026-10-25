using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class VehicleTypeController : BaseApiController
{
	[HttpGet]
	public DataTable GetList()
	{
		return UnitOfWork.VehicleType.GetList();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.VehicleType.Save(dt);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.VehicleType.DeleteDocs(ids);
	}
}
