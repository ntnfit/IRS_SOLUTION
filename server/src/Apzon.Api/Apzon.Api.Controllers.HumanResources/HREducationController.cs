using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class HREducationController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.HREducation.LoadDoc();
	}

	[HttpGet]
	public DataTable CheckEducationType(int edType)
	{
		return UnitOfWork.HREducation.CheckEducationType(edType);
	}

	[HttpGet]
	public DataTable GetManagerName()
	{
		return UnitOfWork.HREducation.GetManagerName();
	}

	[HttpGet]
	public DataTable GetAutoKey(string ObjCode)
	{
		return UnitOfWork.HREducation.GetAutoKey(ObjCode);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dt)
	{
		return UnitOfWork.HREducation.Save(dt);
	}

	[HttpGet]
	public int GetMaxID()
	{
		return UnitOfWork.HREducation.GetMaxID();
	}
}
