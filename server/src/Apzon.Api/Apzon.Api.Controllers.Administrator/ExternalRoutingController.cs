using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class ExternalRoutingController : BaseApiController
{
	[HttpPost]
	public string SaveDoc([FromBody] DataSet ds)
	{
		return UnitOfWork.ExternalRouting.SaveDoc(ds);
	}

	[HttpGet]
	public DataTable GetAllRoutings()
	{
		return UnitOfWork.ExternalRouting.GetAllRoutings();
	}

	[HttpGet]
	public DataSet GetLines(int absEntry)
	{
		return UnitOfWork.ExternalRouting.GetLines(absEntry);
	}

	[HttpGet]
	public string DeleteDoc(int absEntry)
	{
		return UnitOfWork.ExternalRouting.DeleteDoc(absEntry);
	}
}
