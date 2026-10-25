using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class MarkerController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData()
	{
		return UnitOfWork.Marker.LoadData();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.Marker.Saves(dt);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.Marker.DeleteDocs(ids);
	}
}
