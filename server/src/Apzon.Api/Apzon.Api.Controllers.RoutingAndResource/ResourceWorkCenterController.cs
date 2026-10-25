using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.RoutingAndResource;

public class ResourceWorkCenterController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataResourceWorkCenter()
	{
		return UnitOfWork.ResourceWorkCenter.GetDataResourceWorkCenter();
	}

	[HttpGet]
	public DataSet GetDataOriginal()
	{
		return UnitOfWork.ResourceWorkCenter.GetDataOriginal();
	}

	public DataTable GetDataDoc(string resCode, Enums.NavigateType type)
	{
		return UnitOfWork.ResourceWorkCenter.GetDataDoc(resCode, type);
	}

	[HttpPost]
	public DataTable SearchResourceWorkCenter(DataTable dtSearch)
	{
		return UnitOfWork.ResourceWorkCenter.SearchResourceWorkCenter(dtSearch);
	}

	[HttpPost]
	public HttpResult SaveData(DataTable dt)
	{
		return UnitOfWork.ResourceWorkCenter.SaveData(dt);
	}

	[HttpPost]
	public HttpResult DeleteResource([FromBody] DataTable dt)
	{
		return UnitOfWork.ResourceWorkCenter.DeleteResource(dt);
	}

	[HttpGet]
	public DataTable GetDataWorkCenterByCode(string code)
	{
		return UnitOfWork.ResourceWorkCenter.GetDataWorkCenterByCode(code);
	}
}
