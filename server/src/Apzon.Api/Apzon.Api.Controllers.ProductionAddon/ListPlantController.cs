using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

public class ListPlantController : BaseApiController
{
	[HttpPost]
	public string SaveDocs(DataTable dt)
	{
		return UnitOfWork.ListPlant.SaveDocs(dt);
	}

	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.ListPlant.GetDocs();
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.ListPlant.DeleteDocs(ids);
	}

	[HttpGet]
	public DataTable GetListGroupControlKey()
	{
		return UnitOfWork.ListPlant.GetListGroupControlKey();
	}
}
