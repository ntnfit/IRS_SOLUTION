using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

public class GroupControlKeyController : BaseApiController
{
	[HttpPost]
	public string SaveDocs(DataTable dt)
	{
		return UnitOfWork.GroupControlKey.SaveDocs(dt);
	}

	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.GroupControlKey.GetDocs();
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.GroupControlKey.DeleteDocs(ids);
	}

	[HttpGet]
	public DataTable GetListGroupControlKey()
	{
		return UnitOfWork.GroupControlKey.GetListGroupControlKey();
	}
}
