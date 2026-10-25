using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class VehicleController : BaseApiController
{
	[HttpGet]
	public DataTable GetList()
	{
		return UnitOfWork.Vehicle.GetList();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.Vehicle.Save(dt);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.Vehicle.DeleteDocs(ids);
	}

	[HttpGet]
	public DataTable GetDataType()
	{
		return UnitOfWork.Vehicle.GetDataType();
	}

	[HttpGet]
	public DataTable GetDataBranch()
	{
		return UnitOfWork.Vehicle.GetDataBranch();
	}
}
