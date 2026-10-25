using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Api.Controllers.ProductionAddon;

public class ResourcesController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.Resources.GetDocs();
	}

	[HttpGet]
	public DataTable GetListResCode()
	{
		return UnitOfWork.Resources.GetListResCode();
	}

	[HttpPost]
	public DataTable GetMaxLineNumByResCode([FromBody] string resCode)
	{
		return UnitOfWork.Resources.GetMaxLineNumByResCode(resCode);
	}

	[HttpPost]
	public DataTable GetDate([FromBody] string resCode)
	{
		return UnitOfWork.Resources.GetDate(resCode);
	}

	[HttpPost]
	public DataTable GetCapacityChildren([FromBody] string rescode)
	{
		return UnitOfWork.Resources.GetCapacityChildren(rescode);
	}

	[HttpPost]
	public DataTable GetDocTabFix([FromBody] string rescode)
	{
		return UnitOfWork.Resources.GetDocTabFix(rescode);
	}

	[HttpPost]
	public DataTable GetDocTabCosting([FromBody] string rescode)
	{
		return UnitOfWork.Resources.GetDocTabCosting(rescode);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.Resources.Save(ds);
	}

	[HttpPost]
	public DataTable GetCapacityByRes([FromBody] Obj_Date obj)
	{
		return UnitOfWork.Resources.GetCapacityByRes(obj);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string resCode)
	{
		return UnitOfWork.Resources.DeleteDocs(resCode);
	}
}
