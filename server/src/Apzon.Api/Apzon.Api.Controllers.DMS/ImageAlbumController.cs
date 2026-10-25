using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class ImageAlbumController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData()
	{
		return UnitOfWork.ImageAlbum.LoadData();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.ImageAlbum.Saves(dt);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string id)
	{
		return UnitOfWork.ImageAlbum.DeleteDocs(id);
	}
}
